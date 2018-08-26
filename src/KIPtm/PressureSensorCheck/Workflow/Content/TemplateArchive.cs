using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using KipTM.Interfaces;
using Newtonsoft.Json;

namespace PressureSensorCheck.Workflow.Content
{
    /// <summary>
    /// Хранилище настроек по типу
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TemplateArchive<T>:ITamplateArchive<T> where T:class 
    {
        private readonly string _path;
        private readonly string _pathLast;

        public TemplateArchive()
        {
            //var storedType = typeof(T);
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), @"KTM\KipTM\ConfArchive\DictConf.json");
            _pathLast = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), @"KTM\KipTM\ConfArchive\LastConf.json");
        }

        public Dictionary<string, T> GetArchive()
        {
            if(!File.Exists(_path))
                return new Dictionary<string, T>();
            var file = File.ReadAllText(_path);
            Dictionary<string, T> data;
            try
            {
                data = JsonConvert.DeserializeObject<Dictionary<string, T>>(file);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"OnDeserializeError: {e.ToString()}");
                data = new Dictionary<string, T>();
            }
            return data;
        }

        private void SaveArchive(Dictionary<string, T> data)
        {
            if (File.Exists(_path))
                File.Delete(_path);
            try
            {
                var strData = JsonConvert.SerializeObject(data);
                var dir = Path.GetDirectoryName(_path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(_path, strData, Encoding.Unicode);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"OnSerializeError: {e.ToString()}");
            }
        }

        public T GetLast()
        {
            if(!File.Exists(_pathLast))
                return null;
            var file = File.ReadAllText(_pathLast);
            T data;
            try
            {
                data = JsonConvert.DeserializeObject<T>(file);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"OnDeserializeError: {e.ToString()}");
                data = null;
            }
            return data;
        }

        public void SetLast(T data)
        {
            if (File.Exists(_pathLast))
                File.Delete(_pathLast);
            try
            {
                var strData = JsonConvert.SerializeObject(data);
                var dir = Path.GetDirectoryName(_pathLast);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(_pathLast, strData, Encoding.Unicode);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"OnSerializeError: {e.ToString()}");
            }
        }

        public void AddTemplate(string name, T conf)
        {
            var data = GetArchive();
            data.Add(name, conf);
            SaveArchive(data);
        }

        public void Update(string name, T conf)
        {
            var data = GetArchive();
            data[name] = conf;
            SaveArchive(data);
        }

        public void Remove(string name)
        {
            var data = GetArchive();
            data.Remove(name);
            SaveArchive(data);
        }
    }
}