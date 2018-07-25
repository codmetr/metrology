using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ArchiveData.DTO;
using CheckFrame.ViewModel.Archive;
using System.Windows.Input;
using ArchiveData;
using CheckFrame.Checks;
using Tools.View;

namespace KipTM.ViewModel
{
    /// <summary>
    /// Описатель результата проверок конкретного прибора
    /// </summary>
    public class TestResultViewModel : INotifyPropertyChanged, ITestResultViewModel
    {
        private string _user;
        private DateTime _time;
        private IDeviceViewModel _device;
        private ObservableCollection<IParameterResultViewModel> _parameters;
        private string _testType;
        private ObservableCollection<IDeviceViewModel> _etalons;
        private IDataAccessor _save;

        private readonly TestResultID _result;

        /// <summary>
        /// Описатель результата проверок конкретного прибора
        /// </summary>
        public TestResultViewModel(TestResultID result, CheckConfigData data, IEnumerable<IParameterResultViewModel> parameters, IDataAccessor accessor)
        {
            _result = result;
            if (false)
            {
                #region Design
                TestType = "поверка";
                User = "Иван Иванович Иванов";
                Time = DateTime.Parse("11/11/11");
                Device = new DeviceViewModel(new DeviceDescriptor(new DeviceTypeDescriptor("UNIK 5000", "Датчик давления", "GE")) { SerialNumber = "111" });
                Etalons = new ObservableCollection<IDeviceViewModel>(new IDeviceViewModel[]
                {
                    new DeviceViewModel(new DeviceDescriptor(new DeviceTypeDescriptor("PACE5000", "Датчик давления", "GE Druk")){SerialNumber = "222"}),
                    new DeviceViewModel(new DeviceDescriptor(new DeviceTypeDescriptor("DPI 620", "Многофункциональный калибратор", "GE Druk")){SerialNumber = "333"}),
                });
                Parameters = new ObservableCollection<IParameterResultViewModel>(new IParameterResultViewModel[]
                {
                    new ParameterResultViewModel()
                    {
                        NameParameter = "Давление",
                        Unit = "мБар",
                        PointMeasuring = "1000",
                        Tolerance = "0.1",
                        Error = "0.01"
                    },
                    new ParameterResultViewModel()
                    {
                        NameParameter = "Давление",
                        Unit = "мБар",
                        PointMeasuring = "1100",
                        Tolerance = "0.1",
                        Error = "0.01"
                    },
                    new ParameterResultViewModel()
                    {
                        NameParameter = "Давление",
                        Unit = "мБар",
                        PointMeasuring = "1200",
                        Tolerance = "0.1",
                        Error = "0.01"
                    },
                });
                #endregion
            }
            else
            {
                _testType = result.DeviceType;
                _user = data.User;
                _time = _result.Timestamp;
                _device = new DeviceViewModel(data.TargetDevice.Device);
                _etalons = new ObservableCollection<IDeviceViewModel>(data.Ethalons.Values.Select(el => new DeviceViewModel(el.Device)));
                Parameters = new ObservableCollection<IParameterResultViewModel>(parameters);
                _save = accessor;
            }
        }

        /// <summary>
        /// Тип проводимого теста (Поверка/Калибровка)
        /// </summary>
        public string TestType
        {
            get { return _testType; }
            set { _testType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Пользователь проводивший тест
        /// </summary>
        public string User
        {
            get { return _user; }
            set { _user = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата и время теста
        /// </summary>
        public DateTime Time
        {
            get { return _time; }
            set { _time = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Проверяемое устройство
        /// </summary>
        public IDeviceViewModel Device
        {
            get { return _device; }
            set { _device = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список проверяемых параметров
        /// </summary>
        public ObservableCollection<IParameterResultViewModel> Parameters
        {
            get { return _parameters; }
            set { _parameters = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список используемых для проверки эталонов
        /// </summary>
        public ObservableCollection<IDeviceViewModel> Etalons
        {
            get { return _etalons; }
            set { _etalons = value;
                OnPropertyChanged();
            }
        }

        public ICommand Save { get { return new CommandWrapper(DoSave); } }

        private void DoSave()
        {
            _save.Save(_result, _parameters);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}