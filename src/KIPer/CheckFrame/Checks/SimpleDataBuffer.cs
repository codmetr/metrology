using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CheckFrame.Checks 
{
    /// <summary>
    /// Хранилище результата (базовая версия - использовать для теста)
    /// </summary>
    public class SimpleDataBuffer : IDataBuffer 
    {
        private readonly IDictionary<Type, object> _dicByType = new Dictionary<Type, object>();
        private readonly IDictionary<Type, IDictionary<string, object>> _dicByTypeKey = new Dictionary<Type, IDictionary<string, object>>();
        
        /// <summary>
        /// Добавить данные в архив
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <param name="data">Данные</param>
        public void Append<T>(T data) where T : class 
        {
            if (!_dicByType.ContainsKey(typeof (T)))
                _dicByType.Add(typeof (T), data);
            else
                _dicByType[typeof (T)] = data;
        }

        /// <summary>
        /// Добавить данные в архив по ключу
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <param name="data">Данные</param>
        /// <param name="key">Ключ</param>
        public void Append<T>(T data, string key) 
        {
            if (!_dicByTypeKey.ContainsKey(typeof (T)))
            {
                _dicByTypeKey.Add(typeof (T), new Dictionary<string, object>() { {key, (object) data}});
            }
            else
            {
                var subDir = _dicByTypeKey[typeof (T)];
                if (!subDir.ContainsKey(key))
                    subDir.Add(key, data);
                else
                    subDir[key] = data;
            }
        }

        /// <summary>
        /// Получить данные из архива
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <returns>Результат</returns>
        public T Resolve<T>() where T : class 
        {
            if (!_dicByType.ContainsKey(typeof(T)))
                throw new IndexOutOfRangeException(string.Format("index [{0}] not found in dicByType", typeof(T)));
            
            return _dicByType[typeof(T)] as T;
        }

        /// <summary>
        /// Получить данные из архива
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <returns>Результат</returns>
        public bool TryResolve<T>(out T res) where T : class
        {
            res = null;
            try
            {
                if (!_dicByType.ContainsKey(typeof(T)))
                    return false;

                res = _dicByType[typeof(T)] as T;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// получить данные из архива по ключу
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <param name="key">Ключ</param>
        /// <returns>Результат</returns>
        public T Resolve<T>(string key) 
        {
            if (!_dicByTypeKey.ContainsKey(typeof(T)))
                throw new IndexOutOfRangeException(string.Format("index [{0}] not found in dicByTypeKey", typeof(T)));
            
            var subDir = _dicByTypeKey[typeof(T)];
            if (!subDir.ContainsKey(key))
                throw new IndexOutOfRangeException(string.Format("index [{0}][{1}] not found in dicByTypeKey", typeof(T), key));

            return (T)subDir[key];
        }

        /// <summary>
        /// получить данные из архива по ключу
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <param name="key">Ключ</param>
        /// <returns>Результат</returns>
        public bool TryResolve<T>(string key, out T res) where T : class
        {
            res = null;
            try
            {
                if (!_dicByTypeKey.ContainsKey(typeof(T)))
                    return false;

                var subDir = _dicByTypeKey[typeof(T)];
                if (!subDir.ContainsKey(key))
                    return false;

                res = (T)subDir[key];
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Отчистка всего справочника
        /// </summary>
        public void Clear()
        {
            _dicByType.Clear();
            _dicByTypeKey.Clear();
        }
    }
} 