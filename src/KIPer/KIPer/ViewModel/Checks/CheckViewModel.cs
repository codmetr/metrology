using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Archive;
using KipTM.Archive.DTO;
using KipTM.Interfaces;
using KipTM.Model;
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
        private readonly IDeviceManager _deviceManager;
        private DictionariesPool _dictionaries;
        private MainSettings _settings;
        private readonly IMethodicsService _methodics;
        private IDictionary<string, ICheckMethodic> _check;
        private readonly IPropertyPool _propertyPool;
        private object _selectedCheck;
        private KeyValuePair<string, ICheckMethodic> _selectedCheckType;
        private TestResult _result;
        private readonly IDictionary<string, DeviceTypeDescriptor> _avalableDeviceTypes; 
        private string _devTypeKey;
        private KeyValuePair<string, DeviceTypeDescriptor> _selectedType;
        private Dictionary<Type, Type> _viewDict;

        /// <summary>
        /// For disiner
        /// </summary>
        public CheckViewModel()
        {}
        /// <summary>
        /// Initializes a new instance of the CheckViewModel class.
        /// </summary>
        public CheckViewModel(MainSettings settings, IMethodicsService methodics, IPropertyPool propertyPool, DictionariesPool dictionaries, IDeviceManager deviceManager, Dictionary<Type, Type> viewDict)
        {
            _result = new TestResult();
            _settings = settings;
            _methodics = methodics;
            _propertyPool = propertyPool;
            _dictionaries = dictionaries;
            _deviceManager = deviceManager;
            _viewDict = new Dictionary<Type, Type>();

            // заполнение списка поддерживаемых устройств и выбор первого элемента
            var avalableDeviceTypes = new Dictionary<string, DeviceTypeDescriptor>();
            _devTypeKey = null;
            foreach (var deviceType in _dictionaries.DeviceTypes)
            {
                var setDevice = _settings.Devices.First(el => el.Key == deviceType);
                if (setDevice == null)
                    continue;
                if (_devTypeKey == null)
                    _devTypeKey = deviceType;
                avalableDeviceTypes.Add(deviceType, new DeviceTypeDescriptor(setDevice.Model, setDevice.DeviceCommonType, setDevice.DeviceManufacturer));
            }
            _avalableDeviceTypes = avalableDeviceTypes;
            _selectedType = _avalableDeviceTypes.First();
            if (_devTypeKey != null)
            {
                _result.TargetDevice = new DeviceDescriptor(_selectedType.Value);
                _check = _methodics.MethodicsForType(_devTypeKey);
                SelectedCheckType = _check.First();
                Channels = _dictionaries.CheckTypes[_devTypeKey];
                _result.Channel = Channels.First();
            }
            
        }

        /// <summary>
        /// Действия при загрузке окна
        /// </summary>
        public ICommand LoadView
        {
            get
            {
                return new RelayCommand<object>(
                    (mainView) =>
                    {
                        var view = mainView as Window;
                        if (view == null)
                            return;

                        try
                        {
                            foreach (var mod in _viewDict)
                            {
                                var typeModel = mod.Key;
                                var typeView = mod.Value;
                                var template = new DataTemplate
                                {
                                    DataType = typeModel,
                                    VisualTree = new FrameworkElementFactory(typeView)
                                };
                                view.Resources.Add(new DataTemplateKey(typeModel), template);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    });
            }
        }

        #region Перечисления
        /// <summary>
        /// Доступные типы устройства
        /// </summary>
        public IDictionary<string, DeviceTypeDescriptor> DeviceTypes { get { return _avalableDeviceTypes; } }

        /// <summary>
        /// Каналы устройства
        /// </summary>
        public IEnumerable<string> Channels { get; set; }

        /// <summary>
        /// Дострупные для выбранного типа устройства методики
        /// </summary>
        public IDictionary<string, ICheckMethodic> CheckTypes
        {
            get { return _check; }
            set { Set(ref _check, value); }
        }
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
        public KeyValuePair<string, DeviceTypeDescriptor> SelectedDeviceType
        {
            get { return _selectedType; }
            set
            {
                if (_selectedType.Key == value.Key)
                    return;
                _result.TargetDevice.DeviceType = value.Value;
                CheckTypes = _methodics.MethodicsForType(value.Key);
                RaisePropertyChanged("Manufacturer");
            }
        }


        public TestResult Result
        {
            get { return _result; }
            set { Set(ref _result, value); }
        }
        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string Manufacturer { get { return _result.TargetDevice.DeviceType.DeviceManufacturer; } }

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string InventarNumber
        {
            get { return _result.TargetDevice.InventarNumber; }
            set { _result.TargetDevice.InventarNumber = value; }
        }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber
        {
            get { return _result.TargetDevice.SerialNumber; }
            set { _result.TargetDevice.SerialNumber = value; }
        }

        /// <summary>
        /// Дата предыдущей поверки/калибровки
        /// </summary>
        public DateTime PreviousCheckTime
        {
            get { return _result.TargetDevice.PreviousCheckTime; }
            set { _result.TargetDevice.PreviousCheckTime = value; }
        }

        /// <summary>
        /// Выбранная методика
        /// </summary>
        public KeyValuePair<string, ICheckMethodic> SelectedCheckType
        {
            get { return _selectedCheckType; }
            set
            {
                _selectedCheckType = value;
                _result.CheckType = _selectedCheckType.Key;
                Check = GetViewModelFor(_selectedCheckType.Value);
            }
        }
 
        /// <summary>
        /// Выбранный канал
        /// </summary>
        public string SelectedChannel
        {
            get { return _result.Channel; }
            set { _result.Channel = value; }
        }

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
            {
                var adtsMethodic = methodic as ADTSCheckMethodic;
                adtsMethodic.SetADTS(_deviceManager.ADTS);
                return new ADTSCalibrationViewModel(adtsMethodic, _settings, _propertyPool);
            }
            return null;
        }
    }
}