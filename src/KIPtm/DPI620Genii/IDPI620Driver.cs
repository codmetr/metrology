using System.Collections.Generic;

namespace DPI620Genii
{
    /// <summary>
    /// Драйвер DPI620
    /// </summary>
    public interface IDPI620Driver
    {
        /// <summary>
        /// Получить набор доступных слотов
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> TestSlots();

        /// <summary>
        /// Открыть сессию
        /// </summary>
        void Open();

        /// <summary>
        /// Получить данные по каналу
        /// </summary>
        /// <param name="slotId">номер слота</param>
        /// <returns></returns>
        double GetValue(int slotId);

        /// <summary>
        /// Закрыть сессию
        /// </summary>
        void Close();
    }
}