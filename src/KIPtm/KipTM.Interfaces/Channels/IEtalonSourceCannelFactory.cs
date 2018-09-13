using System;
using KipTM.Model.Channels;

namespace KipTM.Interfaces.Channels
{
    /// <summary>
    /// Фабрика эталонного канала источника
    /// </summary>
    public interface IEtalonSourceCannelFactory
    {
        /// <summary>
        /// Получить эталонный канал
        /// </summary>
        /// <returns></returns>
        IEtalonSourceChannel GetChanel();

        /// <summary>
        /// Получить визуальную модель заданного эталонного канала
        /// </summary>
        /// <returns></returns>
        object ConfigViewModel { get; }
    }

    /// <summary>
    /// Фабрика эталонного канала источника
    /// </summary>
    public interface IEtalonSourceCannelFactory<in T>
    {
        /// <summary>
        /// Получить эталонный канал
        /// </summary>
        /// <returns></returns>
        IEtalonSourceChannel<T> GetChanel();

        /// <summary>
        /// Получить визуальную модель заданного эталонного канала
        /// </summary>
        /// <returns></returns>
        object ConfigViewModel { get; }
    }
}