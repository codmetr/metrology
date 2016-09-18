using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SQLiteArchive
{
    public class ArchiveXML : IArchive
    {
        public string _archiveFileFormat = "KTM\\KipTM\\Archive\\{0}.xml";
        const string ExtraTypesKeyFormat = "{0}_extraTypes";

        public ArchiveXML()
        {
            
        }

        /// <summary>
        /// Save entity by key
        /// </summary>
        /// <typeparam name="T">entity Type</typeparam>
        /// <param name="key">key entity</param>
        /// <param name="entity">value entity</param>
        public void Save<T>(string key, T entity)
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDocuments), string.Format(_archiveFileFormat, key));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var archiveType = entity == null ? typeof(T) : entity.GetType();
            var realTypes = (new List<Type>()).FillNoObjectTypes(typeof (T));
            var allTypes = (new List<Type>()).FillNoObjectTypes(entity, typeof(T));
            var arrSubTypes = allTypes.Where(el => !realTypes.Contains(el)).ToList();
            
            // Save value
            var xmlSerializer = new XmlSerializer(archiveType, arrSubTypes.ToArray());
            var dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                XmlWriter writer = XmlWriter.Create(fs);
                xmlSerializer.Serialize(writer, entity);
            }

            if (!arrSubTypes.Any())
                return;

            SaveExtraTypes(key, arrSubTypes);
        }

        /// <summary>
        /// Load by key
        /// </summary>
        /// <typeparam name="T">entity Type</typeparam>
        /// <param name="key">key entity</param>
        /// <param name="def">default value entity</param>
        /// <returns>value entity</returns>
        public T Load<T>(string key, T def = default (T))
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDocuments), string.Format(_archiveFileFormat, key));

            var result = def;
            if (!File.Exists(path))
            {
                return result;
            }

            // Load extra Types
            List<Type> arrSubTypes = new List<Type>();
                arrSubTypes = LoadExtraTypes(key);

            var archiveType = typeof(T);
            var xmlSerializer = new XmlSerializer(archiveType, arrSubTypes.ToArray());

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                XmlReader reader = XmlReader.Create(fs);
                result = (T)xmlSerializer.Deserialize(reader);
            }
            return result;
        }

        /// <summary>
        /// Save extra types
        /// </summary>
        /// <param name="key">key entity</param>
        /// <param name="arrSubTypes">extra types</param>
        private void SaveExtraTypes(string key, IEnumerable<Type> arrSubTypes)
        {
            var pathExtTypes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDocuments), string.Format(_archiveFileFormat, string.Format(ExtraTypesKeyFormat, key)));
            if (File.Exists(pathExtTypes))
            {
                File.Delete(pathExtTypes);
            }
            
            // Save extraTypes
            var arrSubTypesStr = arrSubTypes.Select(TypeToString).ToList();
            var xmlSerializer = new XmlSerializer(arrSubTypesStr.GetType());
            using (FileStream fs = new FileStream(pathExtTypes, FileMode.Create))
            {
                XmlWriter writer = XmlWriter.Create(fs);
                xmlSerializer.Serialize(writer, arrSubTypesStr);
            }
        }

        /// <summary>
        /// Load extra types
        /// </summary>
        /// <param name="key">key entity</param>
        /// <returns>loaded extra types</returns>
        private List<Type> LoadExtraTypes(string key)
        {
            var pathExtTypes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDocuments), string.Format(_archiveFileFormat, string.Format(ExtraTypesKeyFormat, key)));
            if(!File.Exists(pathExtTypes))
                return new List<Type>();
            List<Type> arrSubTypes = new List<Type>();
            var xmlSerializerExtTypes = new XmlSerializer(typeof(List<string>));

            using (FileStream fs = new FileStream(pathExtTypes, FileMode.Open))
            {

                XmlReader reader = XmlReader.Create(fs);
                var arrSubTypesStr = (List<string>)xmlSerializerExtTypes.Deserialize(reader);
                foreach (var typeName in arrSubTypesStr)
                {
                    var type = GetType(typeName);
                    arrSubTypes.Add(type);
                }
            }
            return arrSubTypes;
        }

        /// <summary>
        /// get type string identificator
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string TypeToString(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Get type by name
        /// </summary>
        /// <param name="typeNeme"></param>
        /// <returns></returns>
        private static Type GetType(string typeNeme)
        {
            Type result = Type.GetType(typeNeme);

            if (result != null)
                return result;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                result = assembly.GetType(typeNeme);
                if(result != null)
                    break;
            }
            return result;
        }
    }
}
