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
        private readonly ICheckFabrik _checkFabrik;
        private IMethodViewModel _selectedCheck;
        private Action<TestResult> _saver;
        
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
        public CheckViewModel(CheckConfig checkConfig, Action<TestResult> saver, ICheckFabrik checkFabrik, SelectChannelViewModel checkDeviceChanel, SelectChannelViewModel ethalonChanel)
        {
            _checkConfig = checkConfig;
            _checkConfigViewModel = new CheckConfigViewModel(_checkConfig, checkDeviceChanel, ethalonChanel);

            _checkConfig.SelectedCheckTypeChanged += _checkConfig_SelectedCheckTypeChanged;
            _checkConfig.SelectedChannelChanged += _checkConfig_SelectedChannelChanged;
            _checkConfig.SelectedEthalonTypeChanged += _checkConfig_SelectedEthalonTypeChanged;

            _checkConfigViewModel.CheckedDeviseChannelChanged += _checkConfigViewModel_CheckedDeviseChannelChanged;
            _checkConfigViewModel.EthalonDeviseChannelChanged += _checkConfigViewModel_EthalonDeviseChannelChanged;
            _saver = saver;
            _checkFabrik = checkFabrik;
            Check = _checkFabrik.GetViewModelFor(_checkConfig, _checkConfigViewModel.GetCheckedDeviseChannel(), _checkConfigViewModel.GetEthalonDeviseChannel());
            
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

        /// <summary>
        /// Изменен канал подключения целевого устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _checkConfigViewModel_CheckedDeviseChannelChanged(object sender, EventArgs e)
        {
            if (Check != null)
            {
                Check.SetConnection(_checkConfigViewModel.GetCheckedDeviseChannel());
            }
        }

        /// <summary>
        /// Изменен канал подключения эталона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _checkConfigViewModel_EthalonDeviseChannelChanged(object sender, EventArgs e)
        {
            UpdateEthalon();
        }

        /// <summary>
        /// Изменен тип эталона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _checkConfig_SelectedEthalonTypeChanged(object sender, EventArgs e)
        {
            UpdateEthalon();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _checkConfig_SelectedChannelChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _checkConfig_SelectedCheckTypeChanged(object sender, EventArgs e)
        {
            Check = _checkFabrik.GetViewModelFor(_checkConfig, _checkConfigViewModel.GetCheckedDeviseChannel(), _checkConfigViewModel.GetEthalonDeviseChannel());
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
            }
            else
            {
                Check.SetEthalonChannel(null, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DoSave()
        {
            if(_saver!=null)
                _saver(Check.CurrentResult);
            else
                throw new NullReferenceException("Saver is null");
        }

        /// <summary>
        /// 
        /// </summary>
        private void DoShowReport()
        {
            throw new NotImplementedException("DoShowReport");
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