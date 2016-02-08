using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SQLiteArchive
{
    public class Archive : IArchive
    {
        public string _archiveFileFormat = "./Archive/{0}.xml";

        public void Save<T>(string key, T entity)
        {
            var path = string.Format(_archiveFileFormat, key);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var archiveType = typeof(T);
            var xmlSerializer = new XmlSerializer(archiveType);
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                XmlWriter writer = XmlWriter.Create(fs);
                xmlSerializer.Serialize(writer, entity);
            }
        }

        public T Load<T>(string key, T def = default (T))
        {
            var path = string.Format(_archiveFileFormat, key);
            var result = def;
            if (!File.Exists(path))
            {
                return result;
            }
            var archiveType = typeof(T);
            var xmlSerializer = new XmlSerializer(archiveType);

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                XmlReader reader = XmlReader.Create(fs);
                result = (T)xmlSerializer.Deserialize(reader);
            }
            return result;
        }
    }
}
