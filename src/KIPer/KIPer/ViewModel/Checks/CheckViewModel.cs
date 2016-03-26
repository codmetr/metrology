using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using KipTM.Archive;
using KipTM.Archive.DTO;
using KipTM.Interfaces;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Settings;
using KipTM.ViewModel.Checks;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CheckViewModel : ViewModelBase
    {
        private DictionariesPool _dictionaries;
        private MainSettings _settings;
        private readonly IDictionary<string, ICheckMethodic> _checks;
        private readonly IPropertyPool _propertyPool;
        private object _selectedCheck;
        private ICheckMethodic _selectedCheckType;
        private TestResult _result;
        private DeviceTypeDescriptor _deviceTypeDescriptor;

        /// <summary>
        /// Initializes a new instance of the CheckViewModel class.
        /// </summary>
        public CheckViewModel(MainSettings settings, IDictionary<string, ICheckMethodic> checks, IPropertyPool propertyPool, DictionariesPool dictionaries)
        {
            _result = new TestResult();
            _settings = settings;
            _checks = checks;
            _propertyPool = propertyPool;
            _dictionaries = dictionaries;
            var firstType =  _dictionaries.DeviceTypes.First();
            var settingsDevice = _settings.LastDevices.First(el=>el.)
            _result.TargetDevice = new DeviceDescriptor(new DeviceTypeDescriptor(el.Model, el.DeviceCommonType, el.DeviceManufacturer)));
            if (_checks.Count > 0)
            {
                var selected = _checks.First();
                CheckTypes = _checks.Values;
            }
        }
        #region Перечисления
        /// <summary>
        /// Доступные типы устройства
        /// </summary>
        public IEnumerable<string> DeviceTypes { get { return _dictionaries.DeviceTypes; } }

        /// <summary>
        /// Каналы устройства
        /// </summary>
        public IEnumerable<string> Channels { get; set; } 

        /// <summary>
        /// Дострупные для выбранного типа устройства методики
        /// </summary>
        public IEnumerable<ICheckMethodic> CheckTypes { get; set; }
        #endregion

        #region Фактические настройки
        
        #region Условия проверки
        /// <summary>
        /// Дата проверки
        /// </summary>
        public DateTime CheckDateTime { get; set; }

        /// <summary>
        /// Лаборатория
        /// </summary>
        public string Laboratory { get; set; }

        /// <summary>
        /// Температура
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        public string Humidity { get; set; }
        #endregion

        #region Настройки проверяемого устройства
        /// <summary>
        /// Тип устройства
        /// </summary>
        public string SelectedDeviceType { get; set; }

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string InventarNumber { get; set; }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Дата предыдущей поверки/калибровки
        /// </summary>
        public DateTime PreviousCheckTime { get; set; }

        /// <summary>
        /// Выбранная методика
        /// </summary>
        public ICheckMethodic SelectedCheckType
        {
            get { return _selectedCheckType; }
            set
            {
                _selectedCheckType = value;
                Check = GetViewModelFor(_selectedCheckType);
            }
        }
 
        /// <summary>
        /// Выбранный канал
        /// </summary>
        public string SelectedChannel { get; set; }

        /// <summary>
        /// Выбранный порт
        /// </summary>
        public string SelectedPort { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }

        #endregion

        #region Настройки эталона


        #endregion
        #endregion

        public ICommand Save { get; set; }

        public ICommand Report { get; set; }

        public object Check
        {
            get { return _selectedCheck; }
            set { Set(ref _selectedCheck, value); }
        }

        public object GetViewModelFor(ICheckMethodic methodic)
        {
            if (methodic is ADTSCheckMethodic)
                return new ADTSCalibrationViewModel(methodic as ADTSCheckMethodic, _settings, _propertyPool);
            return null;
        }
    }
}