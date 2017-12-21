using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// Load by key
        /// </summary>
        /// <typeparam name="T">entity Type</typeparam>
        /// <param name="key">key entity</param>
        /// <param name="feautures"></param>
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
