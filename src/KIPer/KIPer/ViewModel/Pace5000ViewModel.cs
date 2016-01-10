using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using KIPer.Interfaces;
using KIPer.Model;

namespace KIPer.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class Pace5000ViewModel : ViewModelBase, IService
    {
        private readonly PACE5000Model _model;
        private double _manometerValue;
        private bool _isAutoRead;
        private TimeSpan _autoreadPeriod;
        private ObservableCollection<ParameterValuePair> _deviceParameters = new ObservableCollection<ParameterValuePair>();

        /// <summary>
        /// Initializes a new instance of the Pace5000ViewModel class.
        /// </summary>
        public Pace5000ViewModel(PACE5000Model model)
        {
            _model = model;
        }

        public string Title { get { return _model.Title; } }

        public double ManometerValue
        {
            get { return _manometerValue; }
            private set
            {
                Set(ref _manometerValue, value);
            }
        }

        public bool IsAutoRead
        {
            get { return _isAutoRead; }
            set { Set(ref _isAutoRead, value); }
        }

        public TimeSpan AutoreadPeriod
        {
            get { return _autoreadPeriod; }
            set { Set(ref _autoreadPeriod, value); }
        }

        public ObservableCollection<ParameterValuePair> DeviceParameters
        {
            get { return _deviceParameters; }
            private set { Set(ref _deviceParameters, value); }
        }

        public ICommand UpdateDeviceParameters { get{return new RelayCommand(() =>
        {
            //TODO: realise update parameters
            _deviceParameters.Add(new ParameterValuePair() {Parameter = "paremeter 1", Vlaue = "value1"});
            if (ManometerValue < 90)
                ManometerValue += 10;
            else
                ManometerValue -= 1;
        });} }

    }
}