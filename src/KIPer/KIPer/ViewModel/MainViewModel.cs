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
        private readonly IEventAggregator _eventAggregator;
        private IArchivesViewModel _tests;
        private DeviceTypeCollectionViewModel _etalonTypes;
        private IWorkflow _workflow;
        private List<IWorkflowStep> _steps;
        
        private readonly ServiceViewModel _services;
        private DocsViewModel _lib;
        private IEnumerable<OneBtnDescripto> _checkBtns;

        private CheckWorkflowFactory _checkFactory;



        private string _helpMessage;
        private bool _isError;
        private object _selectedAction;
        private bool _isActiveSwitchServices = true;
        #endregion

        #region Инициализация загрузка

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <param name="eventAggregator">Сборщик событий</param>
        /// <param name="dataService">Источник данных</param>
        /// <param name="services">Сервисы</param>
        /// <param name="features">Забор возмодностей модулей</param>
        /// <param name="deviceManager">Пулл устройств</param>
        /// <param name="lib"></param>
        /// <param name="checkFactory"></param>
        public MainViewModel(IEventAggregator eventAggregator, IDataService dataService, IEnumerable<IService> services,
            FeatureDescriptorsCombiner features, IDeviceManager deviceManager, DocsViewModel lib, CheckWorkflowFactory checkFactory)
        {
            try
            {
                _logger = NLog.LogManager.GetCurrentClassLogger();
            }
            catch (Exception)
            {
                _logger = null;
            }

            _services = new ServiceViewModel(services, new SelectChannelViewModel(features.ChannelFactories.GetChannels()));
            _lib = lib;
            _checkFactory = checkFactory;

            _eventAggregator = eventAggregator;
            _dataService = dataService;
            _deviceManager = deviceManager;
            _dataService.LoadResults();
            _dataService.FillDeviceList(features.DeviceTypes, features.EthalonTypes);

            var checkBtns = new List<OneBtnDescripto>();
            foreach (var keyCheck in _checkFactory.GetAvailableKeys())
            {
                checkBtns.Add(new OneBtnDescripto(keyCheck.Key, keyCheck.Title,
                    BitmapToImage(keyCheck.BigImg),
                    BitmapToImage(keyCheck.SmallImg), SelectChecks));
            }
            _checkBtns = checkBtns;

            _tests = new ArchivesViewModel();
            _tests.LoadTests(_dataService.ResultsArchive);

            _eventAggregator.Subscribe(this);
            _workflow = _checkFactory.GetNew();
        }

        /// <summary>
        /// Загрузка всех состояний
        /// </summary>
        public void Load()
        {
            try
            {
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
            if (_workflow != null)
            {
                var dispCheck = _workflow as IDisposable;
                if(dispCheck!=null)
                    dispCheck.Dispose();
            }
            var disp = _deviceManager as IDisposable;
            if (disp != null)
                disp.Dispose();

        }

        #endregion

        #region Свойства

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
        /// Выбранная вкладка
        /// </summary>
        public object SelectedAction
        {
            get { return _selectedAction; }
            set { Set(ref _selectedAction, value); }
        }

        /// <summary>
        /// Набор кнопок проверок
        /// </summary>
        public IEnumerable<OneBtnDescripto> CheckBtns
        {
            get { return _checkBtns; }
            set { Set(ref _checkBtns, value); }
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
                    SelectedAction = _workflow;
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
                    SelectedAction = _tests; //todo установить выбор соответсвующего ViewModel
                    SetHelpMessage("Архив Проверок: список пойденных поверок");
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
                    SelectedAction = "Здесь будут элементы управления настройками приложения"; //todo установить выбор соответсвующего ViewModel
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
                    _services.SelectedService = _services.Services.FirstOrDefault(el => el.Title == serveseKey);
                    SelectedAction = _services;
                    SetHelpMessage("Сервисная вкладка для отладки различных механизмов");
                });
            }
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
        /// Доступность переключения сервисов
        /// </summary>
        public bool IsActiveSwitchServices
        {
            get { return _isActiveSwitchServices; }
            set { Set(ref _isActiveSwitchServices, value); }
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