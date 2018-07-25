using System.Collections.Generic;
using ArchiveData.DTO;

namespace ArchiveData
{
    /// <summary>
    /// Доступ к данным
    /// </summary>
    public interface IDataPool
    {
        /// <summary>
        /// Справочник всех результатов проверок по ключу
        /// </summary>
        IDictionary<TestResultID, object> Repairs { get; }

        /// <summary>
        /// Справочник всех конфигураций проверок по ключу
        /// </summary>
        IDictionary<TestResultID, object> Configs { get; }

        /// <summary>
        /// Добавить результат
        /// </summary>
        /// <param name="check"></param>
        /// <param name="res"></param>
        /// <param name="conf"></param>
        void Add(TestResultID check, object res, object conf);

        /// <summary>
        /// Обновить описание проверки
        /// </summary>
        /// <param name="check"></param>
        void UpdateRepair(TestResultID check);

        /// <summary>
        /// Обновить описание проверки и результат
        /// </summary>
        /// <param name="check"></param>
        /// <param name="res"></param>
        void UpdateResult(TestResultID check, object res);

        /// <summary>
        /// Обновить описание проверки и конфигурацию
        /// </summary>
        /// <param name="check"></param>
        /// <param name="conf"></param>
        void UpdateConfig(TestResultID check, object conf);
    }
}