using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ArchiveData.DTO;
using CheckFrame.ViewModel.Archive;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Checks;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Settings;
using KipTM.View;
using KipTM.ViewModel.Checks;
using KipTM.ViewModel.Checks.Config;
using KipTM.ViewModel.DeviceTypes;
using KipTM.ViewModel.Report;
using KipTM.ViewModel.Workflow;
using KipTM.ViewModel.Workflow.States;
using KipTM.Workflow.States;
using MarkerService;
using MarkerService.Filler;
using ReportService;
using SQLiteArchive;
using Tools;
using KipTM.Checks.ViewModel.Config;
using KipTM.EventAggregator;
using KipTM.ViewModel.Events;
using KipTM.Workflow.States.Events;
using Tools.View;
using ViewViewmodelMatcher = Tools.ViewViewmodelMatcher;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, ISubscriber<EventCheckState>, ISubscriber<ErrorMessageEventArg>, ISubscriber<HelpMessageEventArg>
    {
        #region Переменные

        private readonly NLog.Logger _logger = null;
        private readonly IDataService _dataService;
        private IMethodsService _methodicService;
        private IMainSettings _settings;
        private IPropertiesLibrary _propertiesLibrary;
        private IArchive _archive;

        private readonly ServiceViewModel _services;
        private readonly IEventAggregator _eventAggregator;

        private IArchivesViewModel _tests;
        private DeviceTypeCollectionViewModel _deviceTypes;
        private DeviceTypeCollectionViewModel _etalonTypes;
        private Workflow.Workflow _workflow;
        private List<IWorkflowStep> _steps;

        private string _helpMessage;
        private bool _isError;
        private object _selectedAction;
        private IMarkerFabrik<IParameterResultViewModel> _resulMaker;
        private IFillerFabrik<IParameterResultViewModel> _filler;
        private IReportFabrik _reportFabric;
        private bool _isActiveCheck;
        private bool _isActiveService;
        private bool _isActiveSwitchServices = true;

        #endregion

        #region Инициализация загрузка

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(
            IEventAggregator eventAggregator, IDataService dataService, IMethodsService methodicService,
            IMainSettings settings, IPropertiesLibrary propertiesLibrary, IArchive archive,
            IMarkerFabrik<IParameterResultViewModel> resulMaker, IFillerFabrik<IParameterResultViewModel> filler,
            IReportFabrik reportFabric, IEnumerable<IService> services, IFeaturesDescriptor faetures)
        {
            try
            {
                _logger = NLog.LogManager.GetCurrentClassLogger();
            }
            catch (Exception)
            {
                _logger = null;
            }
            _eventAggregator = eventAggregator;
            _dataService = dataService;
            _methodicService = methodicService;
            _settings = settings;
            _propertiesLibrary = propertiesLibrary;
            _archive = archive;

            _dataService.LoadResults();
            _dataService.InitDevices(faetures);
            _resulMaker = resulMaker;
            _filler = filler;
            _reportFabric = reportFabric;
            _services = new ServiceViewModel(services);
                //new List<IService>()
                //                                 {
                //                                     new Pace1000ViewModel(_dataService.DeviceManager),
                //                                     new ADTSViewModel(_dataService.DeviceManager.GetModel<ADTSModel>())
                //                                 });
        }

        /// <summary>
        /// загрузка всех состояний
        /// </summary>
        public void Load()
        {
            try
            {
                _tests = new ArchivesViewModel();
                _tests.LoadTests(_dataService.ResultsArchive);

                _etalonTypes = new DeviceTypeCollectionViewModel();
                _etalonTypes.LoadTypes(_dataService.EtalonTypes);

                _deviceTypes = new DeviceTypeCollectionViewModel();
                _deviceTypes.LoadTypes(_dataService.DeviceTypes);

                var checkFabrik = new CheckFabrik(_dataService.DeviceManager, _propertiesLibrary.PropertyPool);
                var result = new TestResult();
                var checkConfig = new CheckConfig(_settings, _methodicService, _propertiesLibrary.PropertyPool,
                    _propertiesLibrary.DictionariesPool, result);
                var channelTargetDevice = new SelectChannelViewModel();
                var channelEthalonDevice = new SelectChannelViewModel();
                var configViewModelFabrik = new CustomConfigFabrik();
                var checkConfigViewModel = new CheckConfigViewModel(checkConfig, channelTargetDevice, channelEthalonDevice,
                    configViewModelFabrik);

                _eventAggregator.Subscribe(this);

                _steps = new List<IWorkflowStep>()
                {
                    new ConfigCheckState(checkConfigViewModel),
                    new CheckState(() => checkFabrik.GetViewModelFor(checkConfig.SelectedMethod, checkConfig.Data, checkConfig.CustomSettings, result,
                        channelTargetDevice.SelectedChannel, channelEthalonDevice.SelectedChannel), _eventAggregator),
                    new ResultState(() => new TestResultViewModel(result,
                        _resulMaker.GetMarkers(checkConfig.SelectedMethod.GetType(), checkConfig.SelectedMethod), _filler,
                        (res) =>{/*TODO make save*/})),
                    new ReportState(() => new ReportViewModel(_reportFabric, result)),
                };
                _workflow = new Workflow.Workflow(_steps);

                SelectChecks.Execute(null);
            }
            catch (Exception e)
            {
                _logger.With(l => l.Error(string.Format("Load error: {0}", e.ToString())));
            }
        }

        #endregion

        /// <summary>
        /// Поясняющее сообщение 
        /// </summary>
        public string HelpMessage
        {
            get { return _helpMessage; }
            set { Set(ref _helpMessage, value); }
        }

        /// <summary>
        /// Сообшение - описание ошибки
        /// </summary>
        public bool IsError
        {
            get { return _isError; }
            set { Set(ref _isError, value); }
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

        /// <summary>
        /// Доступность переключения сервисов
        /// </summary>
        public bool IsActiveSwitchServices
        {
            get { return _isActiveSwitchServices; }
            set { Set(ref _isActiveSwitchServices, value); }
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
        /// Закрытие окна
        /// </summary>
        public ICommand Close
        {   get{return new RelayCommand(() =>{Application.Current.MainWindow.Close();});}}

        /// <summary>
        /// Закрытие окна
        /// </summary>
        public ICommand GoToUrl
        {
            get
            {
                return new RelayCommand<object>(
                    (url) =>
                    {
                        var strUrl = url as string;
                        if (strUrl != null)
                            System.Diagnostics.Process.Start(strUrl);
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
                    SetHelpMessage("Выполнение поверки");
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
                    SetHelpMessage("Архив Проверок: список пойденных поверок");
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
                    SetHelpMessage("Список поддерживаемых типов проверяемых приборов");
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
                    SetHelpMessage("Список поддерживаемых типов эталонных приборов");
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
                    SetHelpMessage("Настройки приложения");
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
                return new RelayCommand<object>((arg) =>
                {
                    var serveseKey = arg as string;
                    if (serveseKey == null)
                        return;
                    IsActiveCheck = false;
                    IsActiveService = true;
                    _services.SelectedService = _services.Services.FirstOrDefault(el => el.Title == serveseKey);
                    SelectedAction = _services;
                    SetHelpMessage("Сервисная вкладка для отладки различных механизмов");
                });
            }
        }

        public IArchivesViewModel Tests { get { return _tests; } }

        public DeviceTypeCollectionViewModel DeviceTypes { get { return _deviceTypes; } }

        public DeviceTypeCollectionViewModel EtalonTypes { get { return _etalonTypes; } }

        public Workflow.Workflow Checks { get { return _workflow; } }

        public override void Cleanup()
        {
            _eventAggregator.Unsubscribe(this);
            base.Cleanup();
            foreach (var step in _steps)
            {
                var dispStep = step.ViewModel as IDisposable;
                if(dispStep!=null)
                    dispStep.Dispose();
            }
        }

        private void SetHelpMessage(string msg)
        {
            HelpMessage = msg;
            IsError = false;
        }
        #region Events

        public void OnEvent(EventCheckState message)
        {
            IsActiveSwitchServices = !message.Runned;
        }

        public void OnEvent(ErrorMessageEventArg message)
        {
            HelpMessage = message.Error;
            IsError = true;
        }

        public void OnEvent(HelpMessageEventArg message)
        {
            HelpMessage = message.Message;
            IsError = false;
        }

        #endregion

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed
        ////    base.Cleanup();
        ////}
    }
}