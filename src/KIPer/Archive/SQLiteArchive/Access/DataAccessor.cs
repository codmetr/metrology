using System;
using System.Linq;
using ArchiveData.DTO;
using Core.Archive.DataTypes;

namespace SQLiteArchive
{
    /// <summary>
    /// Механизм работы с хранилищем результатов в SQLite
    /// </summary>
    /// <remarks>
    /// Сохраняет только результаты проверки и запись о существовании проверки
    /// Справочники:
    /// - релевантный набор условий
    /// Результат:
    /// - срез выборов пользователей
    /// - дерево результата
    /// </remarks>
    public class DataAccessorSqLite : IDataAccessor
    {
        /// <summary>
        /// работа с БД
        /// </summary>
        private readonly SQLiteArchive.DataPool _dataPool;

        public DataAccessorSqLite(SQLiteArchive.DataPool dataPool)
        {
            _dataPool = dataPool;
        }

        /// <summary>
        /// Добавить новую проверку
        /// </summary>
        /// <param name="check">описатель новой проверки</param>
        /// <param name="res">результат</param>
        /// <param name="conf">конфигурация</param>
        public void Add(TestResultID check, object res, object conf)
        {
            if (check.Id!=null)
                throw new Exception(string.Format("Try add new check, but RepeirId is filled ({0})", check.Id));
            _dataPool.Add(check, res, conf);
            _dataPool.Repairs.Add(check, res);
            _dataPool.Configs.Add(check, conf);
        }

        /// <summary>
        /// Обновить характеристики и метаданные проверки
        /// </summary>
        /// <param name="check"></param>
        public void Update(TestResultID check)
        {
            var repair = _dataPool.Repairs.Keys.FirstOrDefault(el => el.Id == check.Id);
            if (repair == null)
                throw new IndexOutOfRangeException(string.Format("Try save result for not added RepeirId({0})", check.Id));
            _dataPool.UpdateRepair(check);

        }

        /// <summary>
        /// Обновить результат проверки
        /// </summary>
        /// <param name="check"></param>
        /// <param name="result"></param>
        public void UpdateResult(TestResultID check, object result)
        {
            var repair = _dataPool.Repairs.Keys.FirstOrDefault(el => el.Id == check.Id);
            if (repair == null)
                throw new IndexOutOfRangeException(string.Format("Try save result for not added RepeirId({0})", check.Id));
            _dataPool.UpdateResult(check, result);

        }

        /// <summary>
        /// Обновить конфигурацию проверки
        /// </summary>
        /// <param name="check"></param>
        /// <param name="config"></param>
        public void UpdateConfig(TestResultID check, object config)
        {
            var repair = _dataPool.Configs.Keys.FirstOrDefault(el => el.Id == check.Id);
            if (repair == null)
                throw new IndexOutOfRangeException(string.Format("Try save result for not added RepeirId({0})", check.Id));
            _dataPool.UpdateConfig(check, config);

        }

        /// <summary>
        /// Сохранить тип результата в контейнер результатов проверки
        /// </summary>
        /// <param name="check"></param>
        /// <param name="data"></param>
        public void Save<T>(TestResultID check, T data)
        {
            if (!_dataPool.Repairs.ContainsKey(check))
                throw new IndexOutOfRangeException(string.Format("Try save result for not added RepeirId({0})", check.Id));
            _dataPool.Repairs[check] = data;
            _dataPool.UpdateResult(check, data);
        }

        /// <summary>
        /// Сохранить конфигурацию в контейнер результатов проверки типизировано
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="check"></param>
        /// <param name="data"></param>
        public void SaveConfig<T>(TestResultID check, T data)
        {
            if (!_dataPool.Configs.ContainsKey(check))
                throw new IndexOutOfRangeException(string.Format("Try save result for not added RepeirId({0})", check.Id));
            _dataPool.Repairs[check] = data;
            _dataPool.UpdateConfig(check, data);
        }

        /// <summary>
        /// Загрузить результат по проверке
        /// </summary>
        /// <returns></returns>
        public object Load(TestResultID check)
        {
            return _dataPool.Repairs[check];
        }

        /// <summary>
        /// Загрузить конфигурацию по проверке
        /// </summary>
        /// <returns></returns>
        public object LoadConfig(TestResultID check)
        {
            return _dataPool.Configs[check];
        }
    }
}
