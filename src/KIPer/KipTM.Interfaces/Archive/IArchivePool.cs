namespace KipTM.Archive
{
    public interface IArchivePool : IPropertyPool
    {
        /// <summary>
        /// Дополнить архив значением
        /// </summary>
        /// <typeparam name="T">Тип устанавливаемого параметра</typeparam>
        /// <param name="key">Ключ параметра</param>
        /// <param name="value">Значение параметра</param>
        void AddOrUpdateProperty<T>(string key, T value);
    }
}
