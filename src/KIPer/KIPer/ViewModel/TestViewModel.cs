using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace KIPer.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TestViewModel : ViewModelBase, ITestViewModel
    {
        private string _user;
        private DateTime _time;
        private IDeviceViewModel _device;
        private ObservableCollection<IParameterViewModel> _parameters;
        private string _testType;
        private ObservableCollection<IDeviceViewModel> _etalons;

        /// <summary>
        /// Initializes a new instance of the TestsViewModel class.
        /// </summary>
        public TestViewModel()
        {
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
        public ObservableCollection<IParameterViewModel> Parameters
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
    }
}