using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ArchiveData.DTO;
using CheckFrame.ViewModel.Archive;
using GalaSoft.MvvmLight;
using KipTM.Model.Devices;
using KipTM.ViewModel.ResultFiller;
using MarkerService.Filler;
using System.Windows.Input;
using KipTM.Archive;
using KipTM.Checks;
using KipTM.Model;
using MarkerService;
using Tools.View;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TestResultViewModel : ViewModelBase, ITestResultViewModel
    {
        private string _user;
        private DateTime _time;
        private IDeviceViewModel _device;
        private ObservableCollection<IParameterResultViewModel> _parameters;
        private string _testType;
        private ObservableCollection<IDeviceViewModel> _etalons;
        private IDataAccessor _save;

        private readonly TestResult _result;

        /// <summary>
        /// Initializes a new instance of the TestsViewModel class.
        /// </summary>
        public TestResultViewModel(TestResult result, IEnumerable<IParameterResultViewModel> expectedResuls, IFillerFabrik<IParameterResultViewModel> _filler, IDataAccessor accessor)
        {
            _result = result;
            if (IsInDesignMode)
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
                        PointMeashuring = "1000",
                        Tolerance = "0.1",
                        Error = "0.01"
                    },
                    new ParameterResultViewModel()
                    {
                        NameParameter = "Давление",
                        Unit = "мБар",
                        PointMeashuring = "1100",
                        Tolerance = "0.1",
                        Error = "0.01"
                    },
                    new ParameterResultViewModel()
                    {
                        NameParameter = "Давление",
                        Unit = "мБар",
                        PointMeashuring = "1200",
                        Tolerance = "0.1",
                        Error = "0.01"
                    },
                });
                #endregion
            }
            else
            {
                _testType = _result.Note;
                _user = _result.User;
                _time = _result.Timestamp;
                _device = new DeviceViewModel(_result.TargetDevice);
                _etalons = new ObservableCollection<IDeviceViewModel>(_result.Etalon.Select(el => new DeviceViewModel(el)));
                //Parameters = new ObservableCollection<IParameterResultViewModel>(_result.Results.Select(el=>new ParameterResultViewModel(){NameParameter = el.StepKey, PointMeashuring = el.Result.ToString()}));

                var results = new List<IParameterResultViewModel>(expectedResuls);
                foreach (var stepResult in result.Results)
                {
                    var filledResult = _filler.FillMarker(stepResult.Result.GetType(), new Tuple<string, string>(stepResult.CheckKey, stepResult.StepKey), stepResult.Result);
                    if (filledResult == null)
                        continue;
                    var index = results.FindIndex((el) => el.PointMeashuring == filledResult.PointMeashuring);
                    if (index >= 0)
                    {
                        results.RemoveAt(index);
                        results.Insert(index, filledResult);
                    }
                }
                Parameters = new ObservableCollection<IParameterResultViewModel>(results);
                _save = accessor;
            }
        }

        /// <summary>
        /// Тип проводимого теста (Поверка/Калибровка)
        /// </summary>
        public string TestType
        {
            get { return _testType; }
            set { Set(ref _testType, value); }
        }

        /// <summary>
        /// Пользователь проводивший тест
        /// </summary>
        public string User
        {
            get { return _user; }
            set { Set(ref _user, value); }
        }

        /// <summary>
        /// Дата и время теста
        /// </summary>
        public DateTime Time
        {
            get { return _time; }
            set { Set(ref _time, value); }
        }

        /// <summary>
        /// Проверяемое устройство
        /// </summary>
        public IDeviceViewModel Device
        {
            get { return _device; }
            set { Set(ref _device, value); }
        }

        /// <summary>
        /// Список проверяемых параметров
        /// </summary>
        public ObservableCollection<IParameterResultViewModel> Parameters
        {
            get { return _parameters; }
            set { Set(ref _parameters, value); }
        }

        /// <summary>
        /// Список используемых для проверки эталонов
        /// </summary>
        public ObservableCollection<IDeviceViewModel> Etalons
        {
            get { return _etalons; }
            set { Set(ref _etalons, value); }
        }

        public ICommand Save { get { return new CommandWrapper(DoSave); } }

        private void DoSave()
        {
            _save.Save(_result);
        }
    }
}