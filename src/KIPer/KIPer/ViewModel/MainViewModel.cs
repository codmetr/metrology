using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ArchiveData.DTO;
using CheckFrame;
using CheckFrame.ViewModel.Archive;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Archive;
using KipTM.Archive.ViewModel;
using KipTM.Checks;
using KipTM.Checks.ViewModel;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Model.TransportChannels;
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
using KipTM.Manuals.ViewModel;
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
        private IDeviceManager _deviceManager;
        private IMethodsService _methodicService;
        private IMainSettings _settings;
        private IPropertiesLibrary _propertiesLibrary;
        private IArchive _archive;
        private readonly IEventAggregator _eventAggregator;
        private IArchivesViewModel _tests;
        private DeviceTypeCollectionViewModel _deviceTypes;
        private DeviceTypeCollectionViewModel _etalonTypes;
        private Workflow.Workflow _workflow;
        private List<IWorkflowStep> _steps;
        private CustomConfigFactory _customFactory;

        private readonly ServiceViewModel _services;
        private DocsViewModel _lib;
        private IEnumerable<OneBtnDescripto> _checkBtns;



        private string _helpMessage;
        private bool _isError;
        private object _selectedAction;
        private IMarkerFactory<IParameterResultViewModel> _resultMaker;
        private IFillerFactory<IParameterResultViewModel> _filler;
        private IEnumerable<ICheckViewModelFactory> _factoriesViewModels;
        private IChannelsFactory _channelFactory;
        private IReportFactory _reportFactory;
        private bool _isActiveCheck;
        private bool _isActiveService;
        private bool _isActiveSwitchServices = true;
        private IDataAccessor _accessor;
        #endregion

        #region Инициализация загрузка

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <param name="eventAggregator">Сборщик событий</param>
        /// <param name="dataService">Источник данных</param>
        /// <param name="methodicService">Источник методик</param>
        /// <param name="settings">Настройки</param>
        /// <param name="propertiesLibrary">Свойства</param>
        /// <param name="archive">Архив</param>
        /// <param name="filler">Заполнение результата</param>
        /// <param name="reportFactory">Фабрика отчетов</param>
        /// <param name="services">Сервисы</param>
        /// <param name="features">Забор возмодностей модулей</param>
        /// <param name="customFatories">Фабрики специализированных настроек</param>
        /// <param name="deviceManager">Пулл устройств</param>
        /// <param name="factoriesViewModels">Преобразователи в визуальные модели</param>
        public MainViewModel(
            IEventAggregator eventAggregator, IDataService dataService, IMethodsService methodicService,
            IMainSettings settings, IPropertiesLibrary propertiesLibrary, IArchive archive, IFillerFactory<IParameterResultViewModel> filler,
            IReportFactory reportFactory, IEnumerable<IService> services, FeatureDescriptorsCombiner features,
            IDictionary<Type, ICustomConfigFactory> customFatories, IDeviceManager deviceManager,
            IEnumerable<ICheckViewModelFactory> factoriesViewModels, IMarkerFactory<IParameterResultViewModel> resultMaker)
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
            _deviceManager = deviceManager;
            _methodicService = methodicService;
            _factoriesViewModels = factoriesViewModels;
            _resultMaker = resultMaker;
            _settings = settings;
            _propertiesLibrary = propertiesLibrary;
            _archive = archive;
            _customFactory = new CustomConfigFactory(customFatories);
            _dataService.LoadResults();
            _dataService.FillDeviceList(features.DeviceTypes, features.EthalonTypes);
            _filler = filler;
            _reportFactory = reportFactory;
            _channelFactory = features.ChannelFactories;
            _services = new ServiceViewModel(services, new SelectChannelViewModel(_channelFactory.GetChannels()));
            _accessor = new DataAccessor();
            var checkBtns = new List<OneBtnDescripto>();
            foreach (var keyCheck in _methodicService.GetKeys())
            {
                checkBtns.Add(new OneBtnDescripto(keyCheck, _methodicService.GetTitle(keyCheck),
                    BitmapToImage(_methodicService.GetBigImage(keyCheck)),
                    BitmapToImage(_methodicService.GetSmallImage(keyCheck)), SelectChecks));
            }
            _checkBtns = checkBtns;
            var books = new List<BookViewModel>();
            var basePath = Path.Combine(Path.GetFullPath(Environment.CurrentDirectory), "Manuals");
            var path = Path.Combine(basePath, @"PACE\pace5000_pace6000_user_manual_k0443_ru.pdf");
            books.Add(new BookViewModel() { Title = "PACE Руководство пользователя(RU)", Path = path});
            path = Path.Combine(basePath, @"PACE\pace5000_pace6000_user_manual_k0443_en.pdf");
            books.Add(new BookViewModel() { Title = "PACE Руководство пользователя(EN)", Path = path });
            _lib = new DocsViewModel(books);
        }

        /// <summary>
        /// Загрузка всех состояний
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

                var channelTargetDevice = new SelectChannelViewModel(_channelFactory.GetChannels());
                var channelEthalonDevice = new SelectChannelViewModel(_channelFactory.GetChannels());

                var result = new TestResult();
                var checkConfig = new CheckConfig(_settings, _methodicService, _propertiesLibrary.PropertyPool,
                    _propertiesLibrary.DictionariesPool, result);
                var checkConfigViewModel = new CheckConfigViewModel(checkConfig, channelTargetDevice, channelEthalonDevice,
                    _customFactory);
                var resFactory = new TestResultViewModelFactory(result, checkConfig, _resultMaker, _propertiesLibrary,
                    _filler, _accessor);
                var checkPool = new NewCheckPool(_deviceManager, _propertiesLibrary.PropertyPool, _factoriesViewModels);
                var checkFactory = new CheckFactory(checkPool, checkConfig, result, channelTargetDevice, channelEthalonDevice, _eventAggregator);
                _eventAggregator.Subscribe(this);

                _steps = new List<IWorkflowStep>()
                {
                    new ConfigCheckState(checkConfigViewModel),
                    new CheckState(checkFactory, _eventAggregator),
                    new ResultState(resFactory),
                    new ReportState(() => new ReportViewModel(_reportFactory, result)),
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

        #region Разрушение

        public override void Cleanup()
        {
            _eventAggregator.Unsubscribe(this);
            base.Cleanup();
            if(_steps!=null)
                foreach (var step in _steps)
                {
                    var dispStep = step.ViewModel as IDisposable;
                    if (dispStep != null)
                        dispStep.Dispose();
                }
            var disp = _deviceManager as IDisposable;
            if (disp != null)
                disp.Dispose();

        }

        #endregion

        #region Свойства

        /// <summary>
        /// Набор кнопок проверок
        /// </summary>
        public IEnumerable<OneBtnDescripto> CheckBtns
        {
            get { return _checkBtns; }
            set { Set(ref _checkBtns, value); }
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
                        ViewViewmodelMatcher.AddMatch(view.Resources, ViewAttribute.CheckView,
                            ViewAttribute.CheckViewModelCashOnly);
                    });
            }
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        public ICommand Close
        {
            get { return new RelayCommand(() => { Application.Current.MainWindow.Close(); }); }
        }

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
                            Process.Start(strUrl);
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
                return new RelayCommand<string>((string opt) =>
                {
                    IsActiveCheck = true;
                    IsActiveService = false;
                    SelectedAction = Checks;
                    SetHelpMessage("Выполнение поверки");
                });
            }
        }


        /// <summary>
        /// Выбрана вкладка Документация
        /// </summary>
        public ICommand SelectLib
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedAction = _lib;
                    SetHelpMessage("Документация: список доступной документации по приборам");
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
                    SelectedAction = Tests; //todo установить выбор соответсвующего ViewModel
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
                    SelectedAction = _deviceTypes; //todo установить выбор соответсвующего ViewModel
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
                    SelectedAction = _etalonTypes; //todo установить выбор соответсвующего ViewModel
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
                    SelectedAction = "Здесь будут элементы управления настройками приложения";
                        //todo установить выбор соответсвующего ViewModel
                    SetHelpMessage("Настройки приложения");
                });
            }
        }

        /// <summary>
        /// Выбрана вкладка Сервис
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

        public IArchivesViewModel Tests
        {
            get { return _tests; }
        }

        /// <summary>
        /// Проверки
        /// </summary>
        public Workflow.Workflow Checks
        {
            get { return _workflow; }
        }

        #endregion

        #region События(Events)

        public void OnEvent(EventCheckState message)
        {
            IsActiveSwitchServices = !message.Runned;
        }

        public void OnEvent(ErrorMessageEventArg message)
        {
            HelpMessage = string.Format("[{0}] {1}", DateTime.Now.ToString("hh:mm:ss"), message.Error);
            IsError = true;
        }

        public void OnEvent(HelpMessageEventArg message)
        {
            HelpMessage = message.Message;
            IsError = false;
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Установить подсказку
        /// </summary>
        /// <param name="msg"></param>
        private void SetHelpMessage(string msg)
        {
            HelpMessage = msg;
            IsError = false;
        }


        private static BitmapImage BitmapToImage(Bitmap bitm)
        {
            var img = new BitmapImage();
            MemoryStream ms = new MemoryStream();
            bitm.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        #endregion
    }
}