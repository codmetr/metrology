using System;
using KipTM.Model.Channels;

namespace KipTM.Interfaces.Channels
{
    /// <summary>
    /// Фабрика эталонного канала
    /// </summary>
    public interface IEtalonCannelFactory
    {
        /// <summary>
        /// Получить эталонный канал по модели заданного в свойстве <see cref="ModelType"/> типа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IEtalonChannel GetChanel(object model);

        /// <summary>
        /// Получить визуальную модель заданного эталонного канала полученного из <see cref="GetChanel"/>
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        object GetChanelViewModel(IEtalonChannel channel);

        /// <summary>
        /// Тип необходимой модели
        /// </summary>
        Type ModelType { get;}
    }
}