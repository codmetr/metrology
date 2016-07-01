using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SQLiteArchive
{
    public class Archive : IArchive
    {
        public string _archiveFileFormat = "KTM\\KipTM\\Archive\\{0}.xml";

        public Archive()
        {
            //var conn = new SQLiteConnection()
        }

        public void Save<T>(string key, T entity)
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDocuments), string.Format(_archiveFileFormat, key));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var archiveType = typeof(T);
            var realTypes = (new List<Type>()).FillNoObjectTypes(typeof (T));
            var allTypes = (new List<Type>()).FillNoObjectTypes(entity, typeof(T));
            var arrSubTypes = allTypes.Where(el => !realTypes.Contains(el));
            foreach (var subType in arrSubTypes)
            {
                subType.GetCustomAttributesData().Add(new XmlElementAttribute("link", Namespace="http://www.w3.org/2005/Atom"));]);
            }
            var xmlSerializer = new XmlSerializer(archiveType, arrSubTypes.ToArray());
            var dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir))
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
