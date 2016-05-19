using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using ADTS;
using ArchiveData.DTO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Archive;
using KipTM.Archive.DTO;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Channels;
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
        #region Внутренние переменные
        private readonly IDeviceManager _deviceManager;
        private readonly IMethodsService _methodics;
        private IDictionary<string, ICheckMethod> _check;
        private readonly IPropertyPool _propertyPool;
        private IMethodViewModel _selectedCheck;
        private KeyValuePair<string, ICheckMethod> _selectedCheckType;
        private TestResult _result;
        private IDictionary<string, DeviceTypeDescriptor> _avalableDeviceTypes; 
        private string _devTypeKey;
        private KeyValuePair<string, DeviceTypeDescriptor> _selectedType;

        private IDictionary<string, DeviceTypeDescriptor> _avalableEthalonTypes; 
        private string _ethalonTypeKey;
        private KeyValuePair<string, DeviceTypeDescriptor> _selectedEthalonType;

        private DeviceDescriptor _ethalon;
        private bool _isAnalogEthalon;
        private SelectChannelViewModel _checkDeviceChanel;
        private SelectChannelViewModel _ethalonChanel;

        private CheckConfig _checkConfig;
        private CheckConfigViewModel _checkConfigViewModel;
        #endregion

        #region Конструкторы и инициализация
        /// <summary>
        /// For disiner
        /// </summary>
        public CheckViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CheckViewModel class.
        /// </summary>
        public CheckViewModel(MainSettings settings, IMethodsService methodics, IPropertyPool propertyPool, DictionariesPool dictionaries, IDeviceManager deviceManager, TestResult result)
        {
            _checkConfig = new CheckConfig(settings, methodics, propertyPool, dictionaries, result);
            _checkConfigViewModel = new CheckConfigViewModel(_checkConfig);

            _checkConfig.SelectedCheckTypeChanged += _checkConfig_SelectedCheckTypeChanged;
            _checkConfig.SelectedEthalonTypeChanged += _checkConfig_SelectedEthalonTypeChanged;

            _checkConfigViewModel.CheckedDeviseChannelChanged += _checkConfigViewModel_CheckedDeviseChannelChanged;
            _checkConfigViewModel.EthalonDeviseChannelChanged += _checkConfigViewModel_EthalonDeviseChannelChanged;
            _result = result;
            _methodics = methodics;
            _propertyPool = propertyPool;
            _deviceManager = deviceManager;
        }

        void _checkConfigViewModel_CheckedDeviseChannelChanged(object sender, EventArgs e)
        {
            var ch = Check as ADTSTestViewModel;
            if (ch != null)
            {
                ch.SetConnection(_checkConfigViewModel.GetCheckedDeviseChannel());
            }
        }

        void _checkConfigViewModel_EthalonDeviseChannelChanged(object sender, EventArgs e)
        {
            UpdateEthalon();
        }

        void _checkConfig_SelectedEthalonTypeChanged(object sender, EventArgs e)
        {
            UpdateEthalon();
        }

        void _checkConfig_SelectedCheckTypeChanged(object sender, EventArgs e)
        {
            Check = GetViewModelFor(_checkConfig.SelectedCheckType);
            var ch = Check as ADTSTestViewModel;
            if (ch != null)
            {
                ch.SetConnection(_checkConfigViewModel.GetCheckedDeviseChannel());
            }
            UpdateEthalon();
        }
        #endregion

        #region Условия проверки

        public CheckConfigViewModel CheckConfig
        {
            get { return _checkConfigViewModel; }
        }
        #endregion

        #region Команды
        public ICommand Save { get { return new CommandWrapper(DoSave); } }

        public ICommand Report { get { return new CommandWrapper(DoShowReport); } }
        #endregion

        public IMethodViewModel Check
        {
            get { return _selectedCheck; }
            set { Set(ref _selectedCheck, value); }
        }

        public IMethodViewModel GetViewModelFor(ICheckMethod methodic)
        {
            if (methodic is ADTSCheckMethod)
            {
                var adtsMethodic = methodic as ADTSCheckMethod;
                adtsMethodic.SetADTS(_deviceManager.GetModel<ADTSModel>());
                return new ADTSCalibrationViewModel(adtsMethodic, _propertyPool.ByKey(_devTypeKey), _deviceManager, _result);
            }
            else if (methodic is ADTSTestMethod)
            {
                var adtsMethodic = methodic as ADTSTestMethod;
                adtsMethodic.SetADTS(_deviceManager.GetModel<ADTSModel>());
                return new ADTSTestViewModel(adtsMethodic, _propertyPool.ByKey(_devTypeKey), _deviceManager, _result);
            }
            return null;
        }

        #region Сервисные методы
        private void UpdateEthalon()
        {
            //IsAnalogEthalon = _selectedEthalonType.Key == UserEthalonChannel.Key;
            //if (!IsAnalogEthalon)
            if (!_checkConfig.IsAnalogEthalon)
            {
                //Ethalon.DeviceType = _selectedEthalonType.Value;
                //Check.SetEthalonChannel(_selectedEthalonType.Key, EthalonChanel.SelectedChannel); //ToDo добавить настройки подключения
                Check.SetEthalonChannel(_checkConfig.SelectedEthalonTypeKey, _checkConfigViewModel.GetEthalonDeviseChannel()); //ToDo добавить настройки подключения
                return;
            }
            else
            {
                Check.SlectUserEthalonChannel();
            }
        }

        private void DoSave()
        {
            
        }

        private void DoShowReport()
        {

        }
        #endregion
    }
}