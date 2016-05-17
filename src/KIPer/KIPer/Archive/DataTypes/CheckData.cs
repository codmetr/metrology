using System.Collections.Generic;
using System.Data;
using System.Linq;
using ArchiveData.DTO.Params;

namespace KipTM.Archive.DataTypes
{
    public class CheckData
    {
        public CheckData(CheckKey key)
        {
            Key = key;
            Metadata = new CheckMetadata();
            Results = new Dictionary<ParameterDescriptor, ParameterResult>();
        }

        public static Dictionary<ParameterDescriptor, ParameterResult> LoadResults(IArchivePool archivePool)
        {
            var results = new Dictionary<ParameterDescriptor, ParameterResult>();
            foreach (var key in archivePool.GetAllKeys())
            {
                var pair = archivePool.GetProperty<SimplyPair<ParameterDescriptor, ParameterResult>>(key);

                if(pair == null)
                    continue;

                var descriptor = pair.Item1;
                var value = pair.Item2;

                if (results.Any(el =>
                            el.Key.Name == descriptor.Name &&
                            el.Key.Point == descriptor.Point &&
                            el.Key.PType == descriptor.PType))
                    throw new DuplicateNameException(
                        string.Format("Find duplicate descriptor [Name = {0}][Point = {1}][PType ={2}] in results",
                            descriptor.Name, descriptor.Point, descriptor.PType));
                results.Add(descriptor, value);
            }
            return results;
        }

        public void SaveResult(IArchivePool archivePool)
        {
            foreach (var parameterResult in Results)
            {
                var key = GetResultKey(parameterResult.Key);
                var data = new SimplyPair<ParameterDescriptor, ParameterResult>()
                {
                    Item1 = parameterResult.Key,
                    Item2 = parameterResult.Value
                };
                archivePool.AddOrUpdateProperty(key, data);
            }
        }

        public CheckKey Key { get; private set; }

        public CheckMetadata Metadata { get; set; }

        public Dictionary<ParameterDescriptor, ParameterResult> Results { get; set; }

        private string GetResultKey(ParameterDescriptor resDescriptor)
        {
            return resDescriptor.ToString();
        }


    }
}
