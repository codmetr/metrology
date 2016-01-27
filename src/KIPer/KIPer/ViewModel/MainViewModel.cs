using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KIPer.Interfaces;
using KIPer.View;

namespace KIPer.ViewModel
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

        private readonly ServiceViewModel _services;

        private readonly Dictionary<Type, Type> ViewModelViewDic;


        private IArchivesViewModel _tests;
        private DeviceTypesViewModel _deviceTypes;
        private DeviceTypesViewModel _etalonTypes;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.LoadSettings();
            _dataService.InitDevices();

            _services = new ServiceViewModel(new List<IService>()
            {
                new Pace5000ViewModel(_dataService.Pace5000)
            });
            ViewModelViewDic = new Dictionary<Type, Type>()
            {
                //Вкладка Сервис
                {typeof(ServiceViewModel), typeof(ServicesView)},
                {typeof(Pace5000ViewModel), typeof(PACE5000View)},

                //Вкладка Архив
                {typeof(ArchivesViewModel), typeof(ArchivesView)},
                {typeof(TestResultViewModel), typeof(ResultView)},
                
                //Вкладка Приборы
                {typeof(DeviceTypesViewModel), typeof(DeviceTypesView)},
                {typeof(DeviceTypeViewModel), typeof(DeviceTypeView)},

                //Вкладка Эталоны
                {typeof(EtalonTypeViewModel), typeof(EtalonTypeView)},

            };
        }

        private string _helpMessage;
        public string HelpMessage
        {
            get { return _helpMessage; }
            set { Set(ref _helpMessage, value); }
        }

        private object _selectedAction;

        public object SelectedAction
        {
            get { return _selectedAction; }
            set { Set(ref _selectedAction, value); }
        }

        public ICommand LoadView
        {
            get
            {
                return new RelayCommand<object>(
                    (mainView) =>
                    {
                        Loadsettings();

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
                    SelectedAction = "Проверки";//todo установить выбор соответсвующего ViewModel
                    HelpMessage = "Список Проверок";
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
                    HelpMessage = "Архив Проверок";
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
                    HelpMessage = "Список проверяемых приборов";
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
                    HelpMessage = "Список эталонных приборов";
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
                    SelectedAction = "Настройки";//todo установить выбор соответсвующего ViewModel
                    HelpMessage = "Список настроек";
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

        public void Loadsettings()
        {
            var etalonTypes = new[]
            {
                new EtalonTypeViewModel()
                {
                    Device = new DeviceTypeDescriptor("PACE5000", "Датчик давления", "GE Druk"),
                    TypesEtalonParameters = new[] {"давление", "авиационная высота", "авиационная скорость"}
                },
                new EtalonTypeViewModel()
                {
                    Device = new DeviceTypeDescriptor("DPI 620", "Многофункциональный калибратор", "GE Druk"),
                    TypesEtalonParameters = new[] {"напряжение", "ток", "относительное сопротивление"}
                }
            };
            _tests = new ArchivesViewModel();
            _tests.LoadTests(new List<ITestResultViewModel>
            {
                new TestResultViewModel()
                {
                    TestType = "поверка",
                    User = "Иван Иванович Иванов",
                    Time = DateTime.Parse("11/11/11"),
                    Device = new DeviceViewModel()
                    {
                        DeviceType = new DeviceTypeDescriptor("UNIK 5000","Датчик давления","GE"),
                        SerialNumber = "111",
                    },
                    Etalons = new ObservableCollection<IDeviceViewModel>(new IDeviceViewModel[]
                    {
                        new DeviceViewModel()
                        {
                            DeviceType = etalonTypes[0].Device,
                            SerialNumber = "222",
                        },
                        new DeviceViewModel()
                        {
                            DeviceType = etalonTypes[1].Device,
                            SerialNumber = "333",
                        }
                    }),
                    Parameters = new ObservableCollection<IParameterResultViewModel>(new IParameterResultViewModel[]
                    {
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1000", Tolerance = "0.1", Error = "0.01"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1100", Tolerance = "0.1", Error = "0.01"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1200", Tolerance = "0.1", Error = "0.01"},
                    })
                }
            });
            _etalonTypes = new DeviceTypesViewModel();
            _etalonTypes.LoadTypes(etalonTypes);

            _deviceTypes = new DeviceTypesViewModel();
            var methodics = new IMethodicViewModel[]
            {
                new MethodicViewModel()
                {
                    Name = "Поверка",
                    TypesEtalonParameters = new[] {"напряжение", "давление"},
                    CalculatedParameters = new Dictionary<IParameterViewModel, FunctionDescriptor>()
                    {
                        {
                            new ParameterViewModel()
                            {
                                NameParameter = "основная погрешность",
                                Tolerance = "0.1",
                                Unit = "давление"
                            },
                            new FunctionDescriptor() {Name = "абсолютная разница"}
                        }
                    },
                    Points = new[] {"100 мм.рт.ст", "200 мм.рт.ст", "300 мм.рт.ст", "400 мм.рт.ст"}
                }
            };
            _deviceTypes.LoadTypes(new []
            {
                new DeviceTypeViewModel()
                {
                    Device = new DeviceTypeDescriptor("UNIK 5000","Датчик давления","GE"),
                    SelectedMethodic = methodics[0],
                    Methodics = methodics
                }
            });
        }

        public IArchivesViewModel Tests { get { return _tests; } }

        public DeviceTypesViewModel DeviceTypes { get { return _deviceTypes; } }

        public DeviceTypesViewModel EtalonTypes { get { return _etalonTypes; } }
    }
}