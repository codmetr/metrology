using ArchiveData.DTO;

namespace Core.Archive.DataTypes
{
    /// <summary>
    /// Сохранение и загрузка универсального результата
    /// </summary>
    public interface IDataAccessor
    {
        /// <summary>
        /// Обновить 
        /// </summary>
        /// <param name="check"></param>
        void Update(TestResultID check);

        /// <summary>
        /// Обновить результат проверки
        /// </summary>
        /// <param name="check"></param>
        /// <param name="result"></param>
        void UpdateResult(TestResultID check, object result);

        /// <summary>
        /// Обновить конфигурацию проверки
        /// </summary>
        /// <param name="check"></param>
        /// <param name="result"></param>
        void UpdateConfig(TestResultID check, object result);

        /// <summary>
        /// Сохранить результат в контейнер результатов проверки типизировано
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="check"></param>
        /// <param name="data"></param>
        void Save<T>(TestResultID check, T data);

        /// <summary>
        /// Сохранить конфигурацию в контейнер результатов проверки типизировано
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="check"></param>
        /// <param name="data"></param>
        void SaveConfig<T>(TestResultID check, T data);

        /// <summary>
        /// Загрузить результат по проверке
        /// </summary>
        /// <returns></returns>
        object Load(TestResultID check);

        /// <summary>
        /// Загрузить конфигурацию по проверке
        /// </summary>
        /// <returns></returns>
        object LoadConfig(TestResultID check);
    }

    /// <summary>
    /// Сохранение и загрузка универсального результата типа Т
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataAccessor<T>
    {
        /// <summary>
        /// Сохранить тип результата в контейнер результатов проверки
        /// </summary>
        /// <param name="data"></param>
        void Save(T data);

        /// <summary>
        /// Загрузить тип результата из контейнера результатов проверки
        /// </summary>
        /// <returns></returns>
        T Load();
    }
}
