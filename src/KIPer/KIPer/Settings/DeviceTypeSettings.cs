using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Settings
{
    public class DeviceTypeSettings
    {
        /// <summary>
        /// Ключь типа
        /// </summary>
        public string Key;

        /// <summary>
        /// Модель прибора
        /// </summary>
        public string Model;

        /// <summary>
        /// Класс устройств
        /// </summary>
        public string DeviceCommonType;

        /// <summary>
        /// Изготовитель
        /// </summary>
        public string DeviceManufacturer;

        /// <summary>
        /// Типы параметров
        /// </summary>
        public List<string> TypesEtalonParameters;


        public List<string> AvilableEthalonTypes;
    }
}
