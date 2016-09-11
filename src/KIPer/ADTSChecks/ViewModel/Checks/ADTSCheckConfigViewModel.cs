using System.Collections.ObjectModel;
using System.Windows.Input;
using ArchiveData.DTO;
using GalaSoft.MvvmLight;
using KipTM.Archive;
using KipTM.Model;
using KipTM.Model.Checks;

namespace KipTM.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ADTSCheckConfigViewModel : ViewModelBase, ICustomSettingsViewModel
    {
        private ADTSMethodParameters _customConf;
        private ObservableCollection<ADTSPoint> _points;
        private double _newPressure;
        private double _newTolerance = 0.1;

        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSCheckConfigViewModel(ADTSMethodParameters customConf)
        {
            _customConf = customConf;
            _points = new ObservableCollection<ADTSPoint>(_customConf.Points);
        }

        public ObservableCollection<ADTSPoint> Points { get { return _points; } }

        /// <summary>
        /// Контрольное давление
        /// </summary>
        public double NewPressure
        {
            get { return _newPressure; }
            set { Set(ref _newPressure, value); }
        }

        /// <summary>
        /// Допустимая погрешность на контрольном давлении
        /// </summary>
        public double NewTolerance
        {
            get { return _newTolerance; }
            set { Set(ref _newTolerance, value); }
        }

        public ICommand AddPoint
        {
            get
            {
                return new CommandWrapper(() =>
                    {
                        if (_points == null)
                            return;
                        var point = new ADTSPoint()
                        {
                            IsAvailable = true,
                            Pressure = NewPressure,
                            Tolerance = NewTolerance
                        };
                        _points.Add(point);
                        _customConf.Points.Add(point);
                    });
            }
        }
    }
}