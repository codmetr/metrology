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
    public class TestViewModel : ViewModelBase
    {
        private string _user;
        private DateTime _time;
        private DeviceViewModel _device;
        private ObservableCollection<ParameterViewModel> _parameters;
        private string _testType;

        /// <summary>
        /// Initializes a new instance of the TestsViewModel class.
        /// </summary>
        public TestViewModel()
        {
        }

        public string TestType
        {
            get { return _testType; }
            set { Set(ref _testType, value); }
        }

        public string User
        {
            get { return _user; }
            set { Set(ref _user, value); }
        }

        public DateTime Time
        {
            get { return _time; }
            set { Set(ref _time, value); }
        }

        public DeviceViewModel Device
        {
            get { return _device; }
            set { Set(ref _device, value); }
        }

        public ObservableCollection<ParameterViewModel> Parameters
        {
            get { return _parameters; }
            set { Set(ref _parameters, value); }
        }
    }
}