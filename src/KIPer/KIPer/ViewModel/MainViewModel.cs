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
using KipTM.View;
using KipTM.ViewModel;

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
        private CheckViewModel _check;


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

                //Вкладка Проверка
                {typeof(CheckViewModel), typeof(CheckView)},

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
                    SelectedAction = _check;
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
                    TestType = "Поверка",
                    User = "Иван Иванович Иванов",
                    Time = DateTime.Parse("11/10/2015"),
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
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1000", Tolerance = "0.1", Error = "0.06"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1100", Tolerance = "0.1", Error = "0.01"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1200", Tolerance = "0.1", Error = "0.05"},
                    })
                },
                new TestResultViewModel()
                {
                    TestType = "Аттестация",
                    User = "Иван Иванович Иванов",
                    Time = DateTime.Parse("11/11/2015"),
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
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "800", Tolerance = "0.2", Error = "0.01"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "900", Tolerance = "0.2", Error = "0.02"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1000", Tolerance = "0.2", Error = "0.05"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1100", Tolerance = "0.2", Error = "0.06"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1200", Tolerance = "0.2", Error = "0.04"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1300", Tolerance = "0.2", Error = "0.02"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1400", Tolerance = "0.2", Error = "0.06"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1500", Tolerance = "0.2", Error = "0.02"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1600", Tolerance = "0.2", Error = "0.03"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1700", Tolerance = "0.2", Error = "0.04"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1800", Tolerance = "0.2", Error = "0.05"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1900", Tolerance = "0.2", Error = "0.02"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2000", Tolerance = "0.2", Error = "0.03"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2100", Tolerance = "0.2", Error = "0.04"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2200", Tolerance = "0.2", Error = "0.05"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2300", Tolerance = "0.2", Error = "0.02"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2400", Tolerance = "0.2", Error = "0.01"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2500", Tolerance = "0.2", Error = "0.06"},
                    })
                },
                new TestResultViewModel()
                {
                    TestType = "Поверка",
                    User = "Иван Иванович Иванов",
                    Time = DateTime.Parse("11/12/2015"),
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
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1000", Tolerance = "0.1", Error = "0.05"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1100", Tolerance = "0.1", Error = "0.02"},
                        new ParameterResultViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1200", Tolerance = "0.1", Error = "0.01"},
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
                            new ParameterResultViewModel()
                            {
                                NameParameter = "основная погрешность",
                                Tolerance = "0.1",
                                Unit = "давление"
                            },
                            new FunctionDescriptor() {Name = "абсолютная разница"}
                        }
                    },
                    Points = new[]
                    {
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1000", Tolerance = "0.1"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1100", Tolerance = "0.1"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1200", Tolerance = "0.1"},
                    }
                },
                new MethodicViewModel()
                {
                    Name = "Аттестация",
                    TypesEtalonParameters = new[] {"напряжение", "давление"},
                    CalculatedParameters = new Dictionary<IParameterViewModel, FunctionDescriptor>()
                    {
                        {
                            new ParameterResultViewModel()
                            {
                                NameParameter = "основная погрешность",
                                Tolerance = "0.2",
                                Unit = "давление"
                            },
                            new FunctionDescriptor() {Name = "абсолютная разница"}
                        }
                    },
                    Points = new[]
                    {
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "800", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "900", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1000", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1100", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1200", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1300", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1400", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1500", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1600", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1700", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1800", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "1900", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2000", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2100", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2200", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2300", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2400", Tolerance = "0.2"},
                        new ParameterViewModel(){NameParameter = "Давление", Unit = "мБар", PointMeashuring = "2500", Tolerance = "0.2"},
                    }
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

            _check = new CheckViewModel(_dataService);
        }

        public IArchivesViewModel Tests { get { return _tests; } }

        public DeviceTypesViewModel DeviceTypes { get { return _deviceTypes; } }

        public DeviceTypesViewModel EtalonTypes { get { return _etalonTypes; } }

        public CheckViewModel Check { get { return _check; } }
    }
}