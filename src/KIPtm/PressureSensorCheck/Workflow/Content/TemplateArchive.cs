using System;
using System.Collections.Generic;
using System.IO;
using KipTM.Interfaces;

namespace PressureSensorCheck.Workflow.Content
{
    /// <summary>
    /// Хранилище настроек по типу
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TemplateArchive<T>:ITamplateArchive<T>
    {
        private readonly Type _storedType;
        private string _path;

        public TemplateArchive()
        {
            _storedType = typeof(T);
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), @"KipTM\CongArchive", _storedType.Name);
        }

        public Dictionary<string, T> GetArchive()
        {
            throw new NotImplementedException();
        }

        public void AddTemplate(string name, T conf)
        {
            throw new NotImplementedException();
        }

        public void Update(string name, T conf)
        {
            throw new NotImplementedException();
        }

        public void Remove(string name)
        {
            throw new NotImplementedException();
        }
    }
}