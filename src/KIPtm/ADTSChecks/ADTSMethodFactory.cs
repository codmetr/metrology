﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Devices;
using ADTSChecks.Model.Checks;
using ArchiveData.DTO;
using KipTM.Interfaces;
using KipTM.Interfaces.Checks;
using KipTM.Model.Checks;

namespace ADTSChecks
{
    /// <summary>
    /// Фабрика методик
    /// </summary>
    public class ADTSMethodFactory : IMethodFactory
    {
        private Dictionary<string, ICheckMethod> _methods;
        /// <summary>
        /// Методы формирования новых проверок
        /// </summary>
        private Dictionary<string, Func<ICheckMethod>> createMetods = new Dictionary<string, Func<ICheckMethod>>()
        {
                {Test.key, ()=> new Calibration(NLog.LogManager.GetLogger("ADTSCheckMethod"))},
                {Calibration.key, ()=> new Test(NLog.LogManager.GetLogger("ADTSTestMethod"))},
        }; 

        public ADTSMethodFactory()
        {
            _methods = createMetods.ToDictionary(el => el.Key, el => el.Value());
        }

        /// <summary>
        /// Получить ключ устройства для типа оборудования
        /// </summary>
        public DeviceTypeDescriptor GetKey()
        {
            return ADTSModel.Descriptor;
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
            return ADTSChecks.Properties.Resources.adts405;
        }

        /// <summary>
        /// Получить малое изображение ADTS
        /// </summary>
        /// <returns></returns>
        public Bitmap GetSmallImage()
        {
            return ADTSChecks.Properties.Resources.adts405;
        }

        /// <summary>
        /// Получить заголовок проверок ADTS
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return ADTSChecks.Properties.Resources.NameADTS;
        }

    }
}
