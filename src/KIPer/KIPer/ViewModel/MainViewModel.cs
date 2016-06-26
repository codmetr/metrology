using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ADTSData;
using ArchiveData.DTO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using KipTM.Settings;
using KipTM.View;
using KipTM.View.Checks;
using KipTM.View.Checks.Steps;
using KipTM.View.Services;
using KipTM.ViewModel.Channels;
using KipTM.ViewModel.Checks;
using KipTM.ViewModel.Services;

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
        private MainSettings _settings;
        private ArchiveService _archive;

        private readonly ServiceViewModel _services;

        private readonly Dictionary<Type, Type> ViewModelViewDic;

        private IArchivesViewModel _tests;
        private DeviceTypesViewModel _deviceTypes;
        private DeviceTypesViewModel _etalonTypes;
        private CheckViewModel _checks;

        private string _helpMessage;
        private object _selectedAction;


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService, IMethodsService methodicService, MainSettings settings, ArchiveService archive)
        {
            _dataService = dataService;
            _methodicService = methodicService;
            _settings = settings;
            _archive = archive;

            _dataService.LoadResults();
            _dataService.InitDevices();

            _services = new ServiceViewModel(new List<IService>()
            {
                new Pace1000ViewModel(_dataService.DeviceManager),
                new ADTSViewModel(_dataService.DeviceManager)
            });
            ViewModelViewDic = new Dictionary<Type, Type>()
            {
                //Вкладка Сервис
                {typeof(ServiceViewModel), typeof(ServicesView)},
                {typeof(Pace1000ViewModel), typeof(PACE1000View)},
                {typeof(ADTSViewModel), typeof(ADTSView)},
                {typeof(SelectChannelViewModel), typeof(SelectChannelView)},
                {typeof(VisaSettings), typeof(VisaSettingsView)},

                //Вкладка Архив
                {typeof(ArchivesViewModel), typeof(ArchivesView)},
                {typeof(TestResultViewModel), typeof(ResultView)},
                
                //Вкладка Приборы
                {typeof(DeviceTypesViewModel), typeof(DeviceTypesView)},
                {typeof(DeviceTypeViewModel), typeof(DeviceTypeView)},

                //Вкладка Эталоны
                {typeof(EtalonTypeViewModel), typeof(EtalonTypeView)},
                {typeof(PACEEchalonChannelViewModel), typeof(PACE1000EthalonChannelView)},

                //Вкладка Проверка
                {typeof(CheckViewModel), typeof(CheckView)},
                {typeof(CheckConfigViewModel), typeof(CheckConfigView)},
                {typeof(MechanicalManometerViewModel), typeof(MechanicalManometerView)},
                {typeof(ADTSCalibrationViewModel), typeof(AdtsCheckView)},
                {typeof(ADTSTestViewModel), typeof(AdtsCheckView)},
                {typeof(AdtsPointResult), typeof(AdtsPointResultView)},
            };
        }

        public void Load()
        {
            _tests = new ArchivesViewModel();
            _tests.LoadTests(_dataService.ResultsArchive);

            _etalonTypes = new DeviceTypesViewModel();
            _etalonTypes.LoadTypes(_dataService.EtalonTypes);

            _deviceTypes = new DeviceTypesViewModel();
            _deviceTypes.LoadTypes(_dataService.DeviceTypes);

            _checks = new CheckViewModel(_settings, _methodicService, _archive.PropertyPool, _archive.DictionariesPool, _dataService.DeviceManager, new TestResult());
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

                        try
                        {
                            foreach (var mod in ViewModelViewDic)
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

        /// <summary>
        /// Выбрана вкладка Проверки
        /// </summary>
        public ICommand SelectChecks
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedAction = _checks;
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
                    SelectedAction = _services;
                    HelpMessage = "Сервисная вкладка для отладки различных механизмов";
                });
            }
        }


        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        public IArchivesViewModel Tests { get { return _tests; } }

        public DeviceTypesViewModel DeviceTypes { get { return _deviceTypes; } }

        public DeviceTypesViewModel EtalonTypes { get { return _etalonTypes; } }

        public CheckViewModel Checks { get { return _checks; } }
    }
}