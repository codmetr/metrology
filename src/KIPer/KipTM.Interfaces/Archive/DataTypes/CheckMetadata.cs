using System.Collections.Generic;
using System.Data;

namespace KipTM.Archive.DataTypes
{
    public class CheckMetadata
    {
        public static string KeyString { get { return "MetadataKey"; } }

        private readonly Dictionary<string, object> _properties;

        public static CheckMetadata Load(IArchivePool archivePool)
        {
            var properties = new Dictionary<string, object>();
            foreach (var key in archivePool.GetAllKeys())
            {
                if(properties.ContainsKey(key))
                    throw new DuplicateNameException(string.Format("Find duplicate key [{0}] in metadata", key));
                properties.Add(key, archivePool.GetProperty<object>(key));
            }
            return new CheckMetadata(properties);
        }

        public CheckMetadata()
        {
            _properties = new Dictionary<string, object>();
        }

        private CheckMetadata(Dictionary<string, object> properties)
        {
            _properties = properties;
        }

        public void Save(IArchivePool archivePool)
        {
            foreach (var property in Properties)
            {
                archivePool.AddOrUpdateProperty(property.Key, property.Value);
            }
        }

        public Dictionary<string, object> Properties
        {
            get { return _properties; }
        }
    }
}
