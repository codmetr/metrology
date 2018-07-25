using System;

namespace MarkerService.Filler
{
    public interface IFillerFactory<T>
    {
        /// <summary>
        /// Заполнить маркер
        /// </summary>
        /// <typeparam name="Ttarget">Тип объекта по которому заполняется маркер</typeparam>
        /// <param name="Key">Ключ маркера</param>
        /// <param name="result">Объект по которому формируется маркер</param>
        /// <returns></returns>
        T FillMarker<Ttarget>(object Key, Ttarget result);
        /// <summary>
        /// Заполнить маркер
        /// </summary>
        /// <param name="ttarget">Тип объекта по которому заполняется маркер</param>
        /// <param name="Key">Ключ маркера</param>
        /// <param name="item">Объект по которому формируется маркер</param>
        /// <returns></returns>
        T FillMarker(Type ttarget, object Key, object item);
    }
}