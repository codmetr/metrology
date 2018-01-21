using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ArchiveData;
using ArchiveData.DTO;
using CheckFrame;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Workflow.States;
using Tools;
using KipTM.EventAggregator;
using KipTM.Manuals.ViewModel;
using KipTM.ViewModel.Events;
using KipTM.Workflow;
using KipTM.Workflow.States.Events;
using PressureSensorCheck.Check;
using PressureSensorCheck.Workflow;
using PressureSensorData;
using ReportService;
using SQLiteArchive;
using Tools.View;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ISubscriber<EventCheckState>, ISubscriber<ErrorMessageEventArg>, ISubscriber<HelpMessageEventArg>, INotifyPropertyChanged
    {

        #region Переменные

        private readonly NLog.Logger _logger = null;
        private readonly IDataService _dataService;
        private readonly IDeviceManager _deviceManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IArchivesViewModel _store;
        private IDictionary<string, IWorkflow> _workflows;
        
        private readonly ServiceViewModel _services;
        private readonly DocsViewModel _lib;

        private readonly CheckWorkflowFactory _checkFactory;


        private object _selectedAction;

        #endregion

        #region Инициализация загрузка

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <param name="eventAggregator">Сборщик событий</param>
        /// <param name="dataService">Источник данных</param>
        /// <param name="services">Сервисы</param>
        /// <param name="dataPool"></param>
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

            var reportFactories = new Dictionary<string, IReportFactory>()
            {{ PresSensorCheck.CheckKey, new PressureSensorCheck.Report.ReportFactory()}};

            var resTypes = new Dictionary<string, Type>() { { PresSensorCheck.CheckKey, typeof(PressureSensorResult) } };
            var confTypes = new Dictionary<string, Type>() { { PresSensorCheck.CheckKey, typeof(PressureSensorConfig) } };
            var dataPool = DataPool.Load(null, resTypes, confTypes, GetDbPath(Properties.Settings.Default.DbName), (msg)=>_logger.With(l=>l.Debug(msg)));

            _store = new ArchivesViewModel(dataPool, reportFactories);
            //_store.LoadTests(_dataService.ResultsArchive);
            _workflows = new Dictionary<string, IWorkflow>();
            var checkBtns = new List<OneBtnDescriptor>();
            //foreach (var keyCheck in _checkFactory.GetAvailableKeys())
            //{
            //    checkBtns.Add(new OneBtnDescriptor(keyCheck.Key.TypeKey, keyCheck.Name,
            //        BitmapToImage(keyCheck.BigImg),
            //        BitmapToImage(keyCheck.SmallImg), SelectChecks));
            //    _workflows.Add(keyCheck.Key.TypeKey, _checkFactory.GetNew(keyCheck.Key));
            //}
            checkBtns.Add(new OneBtnDescriptor(PresSensorCheck.CheckKey, "Датчик давления", BitmapToImage(Resources.EHCerabarM),
                BitmapToImage(Resources.EHCerabarM), SelectChecks));
            _workflows.Add(PresSensorCheck.CheckKey, new PressureSensorWorkflow().Make(_logger, new DataAccessor(dataPool)));
            CheckBtns = checkBtns;
            _eventAggregator.Subscribe(this);
        }

        /// <summary>
        /// Загрузка всех состояний
        /// </summary>
        public void Load()
        {
            try
            {
                SelectChecks.Execute(_workflows.Keys.FirstOrDefault());
                Checks = _workflows.Values.FirstOrDefault();
                OnPropertyChanged("Checks");
                OnPropertyChanged("Checks.ViewModel");
            }
            catch (Exception e)
            {
                _logger.With(l => l.Error(string.Format("Load error: {0}", e.ToString())));
            }
        }

        #endregion

        #region Разрушение

        public void Cleanup()
        {
            _eventAggregator.Unsubscribe(this);
            //base.Cleanup();
            if (_workflows != null)
            {
                foreach (var workflow in _workflows.Values)
                {
                    if (workflow != null)
                    {
                        var dispCheck = workflow as IDisposable;
                        if (dispCheck != null)
                            dispCheck.Dispose();
                    }
                }
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
                return new CommandWrapper(
                    (mainView) =>
                    {
                        var view = mainView as Window;
                        if (view == null)
                            return;
                        Tools.ViewViewmodelMatcher.AddMatch(view.Resources, ViewAttribute.CheckView,
                            ViewAttribute.CheckViewModelCashOnly);

                        Load();
                    });
            }
        }

        public DocsViewModel Libs { get { return _lib; } }

        public IArchivesViewModel Store { get { return _store; } }

        public IEnumerable<IWorkflowStep> Checks { get; set; }

        /// <summary>
        /// Выбранная вкладка
        /// </summary>
        public object SelectedAction
        {
            get { return _selectedAction; }
            set
            {
                _selectedAction = value;
                OnPropertyChanged("SelectedAction");
                if (value is IWorkflow)
                    return;
                foreach (var btn in CheckBtns)
                {
                    btn.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Набор кнопок проверок
        /// </summary>
        public IEnumerable<OneBtnDescriptor> CheckBtns { get; set; }

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
                    if(SelectedAction == _workflows[opt])
                        return;
                    SelectedAction = _workflows[opt];
                    foreach (var btnDescripto in CheckBtns)
                    {
                        btnDescripto.IsSelected = btnDescripto.Key == opt;
                    }
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
                    SelectedAction = _store; //todo установить выбор соответсвующего ViewModel
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
        public string HelpMessage { get; set; }

        /// <summary>
        /// Сообшение - описание ошибки
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Доступность переключения сервисов
        /// </summary>
        public bool IsActiveSwitchServices { get; set; } = true;

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
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private string GetDbPath(string dbName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "KipTM\\Db", dbName);
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}