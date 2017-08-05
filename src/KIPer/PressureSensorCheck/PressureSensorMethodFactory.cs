using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KipTM.Interfaces;
using KipTM.Interfaces.Checks;

namespace ADTSChecks
{
    /// <summary>
    /// Фабрика методик
    /// </summary>
    public class PressureSensorMethodFactory : IMethodFactory
    {
        private Dictionary<string, ICheckMethod> _methods;
        /// <summary>
        /// Методы формирования новых проверок
        /// </summary>
        private Dictionary<string, Func<ICheckMethod>> createMetods = new Dictionary<string, Func<ICheckMethod>>()
        {
        //        {Test.key, ()=> new Calibration(NLog.LogManager.GetLogger("ADTSCheckMethod"))},
        //        {Calibration.key, ()=> new Test(NLog.LogManager.GetLogger("ADTSTestMethod"))},
        }; 

        public PressureSensorMethodFactory()
        {
            _methods = createMetods.ToDictionary(el => el.Key, el => el.Value());
        }

        /// <summary>
        /// Получить ключ устройства для типа оборудования
        /// </summary>
        public string GetKey()
        {
            throw new NotImplementedException();
            //    return ADTSModel.Key;
        }

        public ICheckMethod GetNewMethod(string key)
        {
            if(!_methods.ContainsKey(key))
                throw new KeyNotFoundException(string.Format("For {0} not found method {1}", GetKey(), key));
            _methods[key] = createMetods[key]();
            return _methods[key];
        }

        /// <summary>
        /// Получить набор методик для конкретного типа оборудования
        /// </summary>
        /// <returns>
        /// Tuple("ключ устройства", Dictionary("ключ методики", "методика"))
        /// </returns>
        public Dictionary<string, ICheckMethod> GetMethods()
        {
            return _methods;
        }

        /// <summary>
        /// Получить большое изображение ADTS
        /// </summary>
        /// <returns></returns>
        public Bitmap GetBigImage()
        {
            return PressureSensorCheck.Properties.Resources.pressureSensor;
        }

        /// <summary>
        /// Получить малое изображение ADTS
        /// </summary>
        /// <returns></returns>
        public Bitmap GetSmallImage()
        {
            return PressureSensorCheck.Properties.Resources.pressureSensor;
        }

        /// <summary>
        /// Получить заголовок проверок ADTS
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return PressureSensorCheck.Properties.Resources.NamePressureSensor;
        }

    }
}
