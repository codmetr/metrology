using System.Collections.Generic;
using System.Linq;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    public class DataPool : IArchivePool
    {
        private readonly ArchiveBase _archive;

        public DataPool(ArchiveBase archive)
        {
            _archive = archive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DataPool CreatePool(string key)
        {
            return new DataPool(_archive.CreateArchive(key));
        }

        public DataPool GetPool(string key)
        {
            return new DataPool(_archive.GetArchive(key));
        }

        #region Implementation of IPropertyPool

        /// <summary>
        /// Получить список всех ключей
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllKeys()
        {
            return _archive.Data.Select(el => el.Key);
        }

        /// <summary>
        /// Получить свойство
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetProperty<T>(string key)
        {
            var keyValue = GetTypedKeyValue<T>(key);
            if(keyValue == null)
                throw new KeyNotFoundException(string.Format("Not found value for key[{0}] and type[{1}]", key, typeof(T)));
            return (T)keyValue.Value;
        }

        /// <summary>
        /// Получить хранилище свойств по ключу
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IPropertyPool ByKey(string key)
        {
            return GetPool(key);
        }

        /// <summary>
        /// Дополнить архив значением
        /// </summary>
        /// <typeparam name="T">Тип устанавливаемого параметра</typeparam>
        /// <param name="key">Ключ параметра</param>
        /// <param name="value">Значение параметра</param>
        public void AddOrUpdateProperty<T>(string key, T value)
        {
            var keyValue = GetKeyValue(key, false);
            if (keyValue == null)
                _archive.Data.Add(new ArchivedKeyValuePair(key, value));
            else
                keyValue.Value = value;
        }
        #endregion

        #region Services

        public bool KeyExist(string key)
        {
            return _archive.Data.Any(el => el.Key == key);
        }

        public ArchivedKeyValuePair GetKeyValue(string key, bool byAssignable = true)
        {
            return _archive.Data.FirstOrDefault(keyValuePair => keyValuePair.Key == key);
        }

        public ArchivedKeyValuePair GetTypedKeyValue<T>(string key, bool byAssignable = true)
        {
            foreach (var keyValuePair in _archive.Data)
            {
                if (keyValuePair.Key != key)
                    continue;
                var valueType = keyValuePair.Value.GetType();
                var targetType = typeof(T);
                if (!byAssignable)
                {
                    if(valueType!=targetType)
                        continue;
                }
                else if (!targetType.IsAssignableFrom(valueType))
                    continue;
                return keyValuePair;
            }
            return null;
        }

        #endregion
    }
}
