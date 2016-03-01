using System;

namespace KipTM.Model.Checks
{
    public interface ICheckMethodic
    {
        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        bool Init();

        /// <summary>
        /// Запуск калибровки
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="GetRealValue"></param>
        /// <param name="GetAccept"></param>
        /// <returns></returns>
        bool Start(string channels, Func<double> GetRealValue, Func<bool> GetAccept );

        /// <summary>
        /// Отмена
        /// </summary>
        void Cancel();
    }
}