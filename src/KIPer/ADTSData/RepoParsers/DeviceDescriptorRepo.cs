using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ADTSData.Keys;
using ArchiveData.DTO;
using ArchiveData.Repo;

namespace ADTSData.RepoParsers
{
    public class DeviceDescriptorRepo : IRepo<DeviceDescriptor>
    {
        /// <summary>
        /// Загрузка Описателя устройства
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public DeviceDescriptor Load(ITreeEntity node)
        {
            return new DeviceDescriptor()
            {
                DeviceType = RepoFabrik.Get<DeviceTypeDescriptor>().Load(node["DeviceType"]),
                PreviousCheckTime = DateTime.Parse(node["PreviousCheckTime"].Value),
                SerialNumber = node["SerialNumber"].Value,
            };
        }

        /// <summary>
        /// Сохранение Описателя устройства
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ITreeEntity Save(DeviceDescriptor entity)
        {
            var result = new TreeEntity();
            result["PreviousCheckTime"] = TreeEntity.Make(result.Id, entity.PreviousCheckTime.ToString(CultureInfo.InvariantCulture));
            result["SerialNumber"] = TreeEntity.Make(result.Id, entity.SerialNumber);
            result["DeviceType"] = RepoFabrik.Get<DeviceTypeDescriptor>().Save(entity.DeviceType);
            result.Key = entity.GetKey();
            return result;
        }

        /// <summary>
        /// Обновить состояние описателя в соответствие с описателем устройства
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public ITreeEntity Update(ITreeEntity node, DeviceDescriptor entity)
        {
            node.Values["PreviousCheckTime"] = entity.PreviousCheckTime.ToString(CultureInfo.InvariantCulture);
            node.Values["SerialNumber"] = entity.SerialNumber;
            if (node.Childs.Any(el => el.Key == "DeviceType"))
                RepoFabrik.Get<DeviceTypeDescriptor>().Update(node["DeviceType"], entity.DeviceType);
            else
                node["DeviceType"] = RepoFabrik.Get<DeviceTypeDescriptor>().Save(entity.DeviceType);
            return node;
        }
    }
}
