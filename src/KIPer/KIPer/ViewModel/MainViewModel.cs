using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ADTSChecks.ViewModel.Services;
using ArchiveData.DTO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Checks;
using KipTM.Settings;
using KipTM.View;
using KipTM.ViewModel.Checks;
using KipTM.ViewModel.Checks.States;
using KipTM.ViewModel.Master;
using KipTM.ViewModel.Report;
using MarkerService;
using MarkerService.Filler;
using ReportService;
using SQLiteArchive;
using Tools;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private IMethodsService _methodicService;
        private IMainSettings _settings;
        private IPropertiesLibrary _propertiesLibrary;
        private IArchive _archive;

        private readonly ServiceViewModel _services;

        private IArchivesViewModel _tests;
        private DeviceTypesViewModel _deviceTypes;
        private DeviceTypesViewModel _etalonTypes;
        private Workflow _workflow;
        private List<IWorkflowStep> _steps;

        private string _helpMessage;
        private object _selectedAction;
        private IMarkerFabrik<IParameterResultViewModel> _resulMaker;
        private IFillerFabrik<IParameterResultViewModel> _filler;
        private IReportFabrik _reportFabric;
        private bool _isActiveCheck;
        private bool _isActiveService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(
            IDataService dataService, IMethodsService methodicService, IMainSettings settings,
            IPropertiesLibrary propertiesLibrary, IArchive archive,
            IMarkerFabrik<IParameterResultViewModel> resulMaker, IFillerFabrik<IParameterResultViewModel> filler,
            IReportFabrik reportFabric)
        {
            _dataService = dataService;
            _methodicService = methodicService;
            _settings = settings;
            _propertiesLibrary = propertiesLibrary;
            _archive = archive;
            
            _dataService.LoadResults();
            _dataService.InitDevices();
            _resulMaker = resulMaker;
            _filler = filler;
            _reportFabric = reportFabric;
            _services = new ServiceViewModel(new List<IService>()
            {
                new Pace1000ViewModel(_dataService.DeviceManager),
                new ADTSViewModel(_dataService.DeviceManager)
            });
        }

        /// <summary>
        /// загрузка всех состояний
        /// </summary>
        public void Load()
        {
            _tests = new ArchivesViewModel();
            _tests.LoadTests(_dataService.ResultsArchive);

            _etalonTypes = new DeviceTypesViewModel();
            _etalonTypes.LoadTypes(_dataService.EtalonTypes);

            _deviceTypes = new DeviceTypesViewModel();
            _deviceTypes.LoadTypes(_dataService.DeviceTypes);

            var checkFabrik = new CheckFabrik(_dataService.DeviceManager, _propertiesLibrary.PropertyPool);
            var result = new TestResult();
            var checkConfig = new CheckConfig(_settings, _methodicService, _propertiesLibrary.PropertyPool, _propertiesLibrary.DictionariesPool, result);
            var channelTargetDevice = new SelectChannelViewModel();
            var channelEthalonDevice = new SelectChannelViewModel();
            var configViewModelFabrik = new CustomConfigFabrik();
            var checkConfigViewModel = new CheckConfigViewModel(checkConfig, channelTargetDevice, channelEthalonDevice, configViewModelFabrik);

            _steps = new List<IWorkflowStep>()
            {
                new ConfigCheckState(checkConfigViewModel),
                new ADTSCheckState(() => checkFabrik.GetViewModelFor(checkConfig, channelTargetDevice.SelectedChannel, channelEthalonDevice.SelectedChannel)),
                new ResultState(() => new TestResultViewModel(result, _resulMaker.GetMarkers(checkConfig.SelectedMethod.GetType(), checkConfig.SelectedMethod), _filler)),
                new ReportState(() => new ReportViewModel(_reportFabric, result)),
            };
            _workflow = new Workflow(_steps);

            SelectChecks.Execute(null);
        }

        /// <summary>
        /// Поясняющее сообщение 
        /// </summary>
        public string HelpMessage
        {
            get { return _helpMessage; }
            set { Set(ref _helpMessage, value); }
        }

        /// <summary>
        /// Выбранная вкладка
        /// </summary>
        public object SelectedAction
        {
            get { return _selectedAction; }
            set { Set(ref _selectedAction, value); }
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
                        Load();

                        var view = mainView as Window;
                        if (view == null)
                            return;
                        ViewViewmodelMatcher.AddMatch(view.Resources, ViewAttribute.CheckView, ViewAttribute.CheckViewModelCashOnly);
                    });
            }
        }

        /// <summary>
        /// Выбрана вкладка Проверки
        /// </summary>
        public ICommand SelectChecks
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsActiveCheck = true;
                    IsActiveService = false;
                    SelectedAction = Checks;
                    HelpMessage = "Выполнение поверки";
                });
            }
        }

        /// <summary>
        /// Выбрана вкладка Архив
        /// </summary>
        public ICommand SelectArchive
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedAction = _tests;//todo установить выбор соответсвующего ViewModel
                    HelpMessage = "Архив Проверок:\nсписок пойденных поверок";
                });
            }
        }

        /// <summary>
        /// Выбрана вкладка Приборы
        /// </summary>
        public ICommand SelectTargetDevices
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedAction = _deviceTypes;//todo установить выбор соответсвующего ViewModel
                    HelpMessage = "Список поддерживаемых типов проверяемых приборов";
                });
            }
        }

        /// <summary>
        /// Выбрана вкладка Эталоны
        /// </summary>
        public ICommand SelectEtalonDevices
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedAction = _etalonTypes;//todo установить выбор соответсвующего ViewModel
                    HelpMessage = "Список поддерживаемых типов эталонных приборов";
                });
            }
        }

        /// <summary>
        /// Выбрана вкладка Настройки
        /// </summary>
        public ICommand SelectSettings
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedAction = "Здесь будут элементы управления настройками приложения";//todo установить выбор соответсвующего ViewModel
                    HelpMessage = "Настройки приложения";
                });
            }
        }

        /// <summary>
        /// Выбрана вкладка Настройки
        /// </summary>
        public ICommand SelectService
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsActiveCheck = false;
                    IsActiveService = true;
                    SelectedAction = _services;
                    HelpMessage = "Сервисная вкладка для отладки различных механизмов";
                });
            }
        }

        /// <summary>
        /// Активна вкладка Проверки
        /// </summary>
        public bool IsActiveCheck
        {
            get { return _isActiveCheck; }
            set { Set(ref _isActiveCheck, value); }
        }

        /// <summary>
        /// Активна вкладка Проверки
        /// </summary>
        public bool IsActiveService
        {
            get { return _isActiveService; }
            set { Set(ref _isActiveService, value); }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        public IArchivesViewModel Tests { get { return _tests; } }

        public DeviceTypesViewModel DeviceTypes { get { return _deviceTypes; } }

        public DeviceTypesViewModel EtalonTypes { get { return _etalonTypes; } }

        //public CheckViewModel Checks { get { return _checks; } }
        public Workflow Checks { get { return _workflow; } }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var step in _steps)
            {
                var dispStep = step.ViewModel as IDisposable;
                if(dispStep!=null)
                    dispStep.Dispose();
            }
        }
    }
}