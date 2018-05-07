using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ADTSChecks.Checks.Data;
using CheckFrame.ViewModel.Checks;
using Tools.View;

namespace ADTSChecks.Checks.ViewModel
{
    public class AdtsCheckConfVm : INotifyPropertyChanged, ICustomSettingsViewModel
    {
        private ADTSParameters _customConf;
        private ObservableCollection<ADTSPoint> _points;
        private double _newPressure;
        private double _newTolerance = 0.1;
        private ICommand _up;
        private ICommand _down;
        private ICommand _addPoint;
        private ADTSPoint _selectedPoint;


        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public AdtsCheckConfVm(ADTSParameters customConf)
        {
            _customConf = customConf;
            _points = new ObservableCollection<ADTSPoint>(_customConf.Points);
            
            _up = new CommandWrapper((arg) => DoUp((int?)arg));

            _down = new CommandWrapper((arg) => DoDown((int?)arg));

            _addPoint = new CommandWrapper(DoAddPoint);
        }

        /// <summary>
        /// Коллекция точек
        /// </summary>
        public ObservableCollection<ADTSPoint> Points { get { return _points; } }

        /// <summary>
        /// Выделенная точка
        /// </summary>
        public ADTSPoint SelectedPoint
        {
            get { return _selectedPoint; }
            set { _selectedPoint = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Контрольное давление
        /// </summary>
        public double NewPressure
        {
            get { return _newPressure; }
            set { _newPressure = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Допустимая погрешность на контрольном давлении
        /// </summary>
        public double NewTolerance
        {
            get { return _newTolerance; }
            set { _newTolerance = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddPoint { get { return _addPoint; } }

        public ICommand Up { get { return _up; } }

        public ICommand Down { get { return _down; } }

        private void DoUp(int? index)
        {
            if (index == null)
                return;

            if (_points.Count == 0 || index.Value >= _points.Count)
                return;

            if (index.Value < 1)
                return;

            var val = _points[index.Value];
            _points.RemoveAt(index.Value);
            _points.Insert(index.Value - 1, val);
            _customConf.Points.RemoveAt(index.Value);
            _customConf.Points.Insert(index.Value - 1, val);
            SelectedPoint = val;
        }

        private void DoDown(int? index)
        {
            if (index == null)
                return;

            if (_points.Count == 0 || index.Value >= _points.Count)
                return;

            if (index.Value >= _points.Count - 1)
                return;

            var val = _points[index.Value];
            _points.RemoveAt(index.Value);
            _points.Insert(index.Value + 1, val);
            _customConf.Points.RemoveAt(index.Value);
            _customConf.Points.Insert(index.Value + 1, val);
            SelectedPoint = val;
        }

        private void DoAddPoint()
        {
            if (_points == null)
                return;
            var point = new ADTSPoint()
            {
                IsAvailable = true,
                Pressure = NewPressure,
                Tolerance = NewTolerance
            };

            if (_points.Count == 0 || point.Pressure < _points.Last().Pressure)
            {
                _points.Add(point);
                _customConf.Points.Add(point);
                return;
            }

            int index = _points.Count - 1;
            for (int i = _points.Count - 1; i >= 0; i--)
            {
                if (point.Pressure < _points[i].Pressure)
                    break;
                index = i;
            }
            _points.Insert(index, point);
            _customConf.Points.Insert(index, point);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}