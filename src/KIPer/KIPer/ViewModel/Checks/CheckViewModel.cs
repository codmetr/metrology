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
        private readonly IPropertyPool _propertyPool;
        private IMethodViewModel _selectedCheck;
        
        private SelectChannelViewModel _checkDeviceChanel;
        private SelectChannelViewModel _ethalonChanel;

        private CheckConfig _checkConfig;
        private CheckConfigViewModel _checkConfigViewModel;
        private bool _isChecConfigAvailable;

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
            _checkConfig.SelectedChannelChanged += _checkConfig_SelectedChannelChanged;
            _checkConfig.SelectedEthalonTypeChanged += _checkConfig_SelectedEthalonTypeChanged;

            _checkConfigViewModel.CheckedDeviseChannelChanged += _checkConfigViewModel_CheckedDeviseChannelChanged;
            _checkConfigViewModel.EthalonDeviseChannelChanged += _checkConfigViewModel_EthalonDeviseChannelChanged;
            _propertyPool = propertyPool;
            _deviceManager = deviceManager;
            Check = GetViewModelFor(_checkConfig.SelectedCheckType);
            
            if (Check != null)
            {
                Check.SetConnection(_checkConfigViewModel.GetCheckedDeviseChannel());
                UpdateEthalon();
            }
        }
        #endregion

        #region Условия проверки
        /// <summary>
        /// Доступность конфигурации проверки
        /// </summary>
        public bool IsCheckConfigAvailable
        {
            get { return _isChecConfigAvailable; }
            set { Set(ref _isChecConfigAvailable, value); }
        }

        /// <summary>
        /// Представление конфигурации проверки
        /// </summary>
        public CheckConfigViewModel CheckConfig
        {
            get { return _checkConfigViewModel; }
        }
        #endregion

        #region Команды
        public ICommand Save { get { return new CommandWrapper(DoSave); } }

        public ICommand Report { get { return new CommandWrapper(DoShowReport); } }
        #endregion

        #region Модель самой проверки
        public IMethodViewModel Check
        {
            get { return _selectedCheck; }
            set
            {
                if (_selectedCheck == value)
                    return;
                DetachMethod(_selectedCheck);
                Set(ref _selectedCheck, value);
                AttachMethod(_selectedCheck);
            }
        }
        #endregion

        #region Сервисные методы
        /// <summary>
        /// Прикрепить обработчики к методике
        /// </summary>
        /// <param name="method"></param>
        private void AttachMethod(IMethodViewModel method)
        {
            if(method==null)
                return;
            method.Started += method_Started;
            method.Stoped += method_Stoped;
        }

        /// <summary>
        /// Открепить обработчики от методики
        /// </summary>
        /// <param name="method"></param>
        private void DetachMethod(IMethodViewModel method)
        {
            if(method==null)
                return;
            method.Started -= method_Started;
            method.Stoped -= method_Stoped;
        }

        private IMethodViewModel GetViewModelFor(ICheckMethod methodic)
        {
            if (methodic is ADTSCheckMethod)
            {
                var adtsMethodic = methodic as ADTSCheckMethod;
                adtsMethodic.SetADTS(_deviceManager.GetModel<ADTSModel>());
                return new ADTSCalibrationViewModel(adtsMethodic, _propertyPool.ByKey(_checkConfig.SelectedDeviceTypeKey), _deviceManager, _checkConfig.Result);
            }
            else if (methodic is ADTSTestMethod)
            {
                var adtsMethodic = methodic as ADTSTestMethod;
                adtsMethodic.SetADTS(_deviceManager.GetModel<ADTSModel>());
                return new ADTSTestViewModel(adtsMethodic, _propertyPool.ByKey(_checkConfig.SelectedDeviceTypeKey), _deviceManager, _checkConfig.Result);
            }
            return null;
        }

        /// <summary>
        /// Выполнение методики завершено
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void method_Stoped(object sender, EventArgs e)
        {
            IsCheckConfigAvailable = true;
        }

        /// <summary>
        /// Методика запущена на выполнение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void method_Started(object sender, EventArgs e)
        {
            IsCheckConfigAvailable = false;
        }

        void _checkConfigViewModel_CheckedDeviseChannelChanged(object sender, EventArgs e)
        {
            if (Check != null)
            {
                Check.SetConnection(_checkConfigViewModel.GetCheckedDeviseChannel());
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

        void _checkConfig_SelectedChannelChanged(object sender, EventArgs e)
        {
        }

        void _checkConfig_SelectedCheckTypeChanged(object sender, EventArgs e)
        {
            Check = GetViewModelFor(_checkConfig.SelectedCheckType);
            if (Check != null)
            {
                Check.SetConnection(_checkConfigViewModel.GetCheckedDeviseChannel());
            }
            UpdateEthalon();
        }

        private void UpdateEthalon()
        {
            if (!_checkConfig.IsAnalogEthalon)
            {
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

        public override void Cleanup()
        {
            _checkConfig.SelectedCheckTypeChanged -= _checkConfig_SelectedCheckTypeChanged;
            _checkConfig.SelectedChannelChanged -= _checkConfig_SelectedChannelChanged;
            _checkConfig.SelectedEthalonTypeChanged -= _checkConfig_SelectedEthalonTypeChanged;

            _checkConfigViewModel.CheckedDeviseChannelChanged -= _checkConfigViewModel_CheckedDeviseChannelChanged;
            _checkConfigViewModel.EthalonDeviseChannelChanged -= _checkConfigViewModel_EthalonDeviseChannelChanged;
            base.Cleanup();
        }
    }
}