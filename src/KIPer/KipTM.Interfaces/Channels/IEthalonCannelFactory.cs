using System;
using KipTM.Model.Channels;

namespace KipTM.Interfaces.Channels
{
    /// <summary>
    /// Фабрика эталонного канала
    /// </summary>
    public interface IEthalonCannelFactory
    {
        /// <summary>
        /// Получить эталонный канал по модели заданного в свойстве <see cref="ModelType"/> типа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IEthalonChannel GetChanel(object model);

        /// <summary>
        /// Тип необходимой модели
        /// </summary>
        Type ModelType { get;}
    }
}