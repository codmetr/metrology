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
    class DataAccessorSqLite : IDataAccessor
    {
        /// <summary>
        /// работа с БД
        /// </summary>
        private readonly SQLiteArchive.DataPool _dataPool;
        private readonly TestResultID _check;

        public DataAccessorSqLite(TestResultID check, SQLiteArchive.DataPool dataPool)
        {
            _check = check;
            _dataPool = dataPool;
            if (!_dataPool.Repairs.ContainsKey(_check))
            {
                _dataPool.AddRepair(check, null);
            }
        }

        /// <summary>
        /// Обновить характеристики и метаданные проверки
        /// </summary>
        /// <param name="check"></param>
        public void Update(TestResultID check)
        {
            var repair = _dataPool.Repairs.Keys.FirstOrDefault(el => el.Id == _check.Id);
            if (repair == null)
                throw new IndexOutOfRangeException(string.Format("Try save result for not added RepeirId({0})", _check.Id));
            _dataPool.UpdateRepair(check);

        }

        /// <summary>
        /// Обновить результат проверки
        /// </summary>
        /// <param name="check"></param>
        public void UpdateResult(TestResultID check, object result)
        {
            var repair = _dataPool.Repairs.Keys.FirstOrDefault(el => el.Id == _check.Id);
            if (repair == null)
                throw new IndexOutOfRangeException(string.Format("Try save result for not added RepeirId({0})", _check.Id));
            _dataPool.UpdateResult(check, result);

        }

        /// <summary>
        /// Сохранить тип результата в контейнер результатов проверки
        /// </summary>
        /// <param name="data"></param>
        public void Save<T>(T data)
        {
            if (!_dataPool.Repairs.ContainsKey(_check))
                throw new IndexOutOfRangeException(string.Format("Try save result for not added RepeirId({0})", _check.Id));
            _dataPool.Repairs[_check] = data;
            _dataPool.UpdateResult(_check, data);
        }

        /// <summary>
        /// Загрузить результат по проверке
        /// </summary>
        /// <returns></returns>
        public object Load(TestResultID check)
        {
            return _dataPool.Repairs[check];
        }
    }
}
