using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
        private ObservableCollection<ADTSPionViewModel> _points;
        private double _newPressure;
        private double _newTolerance = 0.1;
        private ICommand _up;
        private ICommand _down;
        private ICommand _addPoint;
        private ADTSPionViewModel _selectedPoint;


        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSCheckConfigViewModel(ADTSMethodParameters customConf)
        {
            _customConf = customConf;
            _points = new ObservableCollection<ADTSPionViewModel>(_customConf.Points.Select(el => new ADTSPionViewModel(el, Up, Down)));
            UpdatePoints(_points);

            _up = new CommandWrapper((arg) => DoUp((int?)arg));

            _down = new CommandWrapper((arg) => DoDown((int?)arg));

            _addPoint = new CommandWrapper(DoAddPoint);
        }

        /// <summary>
        /// Коллекция точек
        /// </summary>
        public ObservableCollection<ADTSPionViewModel> Points { get { return _points; } }

        /// <summary>
        /// Выделенная точка
        /// </summary>
        public ADTSPionViewModel SelectedPoint
        {
            get { return _selectedPoint; }
            set { Set(ref _selectedPoint, value); }
        }

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

        public ICommand AddPoint { get { return _addPoint; } }

        public ICommand Up { get { return _up; } }

        public ICommand Down { get { return _down; } }

        private void UpdatePoints(ObservableCollection<ADTSPionViewModel> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].Index = i;
            }
        }

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
            SelectedPoint = val;
            UpdatePoints(_points);
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
            SelectedPoint = val;
            UpdatePoints(_points);
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

            if (_points.Count == 0 || point.Pressure < _points.Last().Value.Pressure)
            {
                _points.Add(new ADTSPionViewModel(point, Up, Down));
                _customConf.Points.Add(point);
                UpdatePoints(_points);
                return;
            }

            int index = _points.Count - 1;
            for (int i = _points.Count - 1; i >= 0; i--)
            {
                if (point.Pressure < _points[i].Value.Pressure)
                    break;
                index = i;
            }
            _points.Insert(index, new ADTSPionViewModel(point, Up, Down));
            _customConf.Points.Insert(index, point);
            UpdatePoints(_points);
        }

    }

    public class ADTSPionViewModel
    {
        private readonly ADTSPoint _value;
        private readonly ICommand _up;
        private readonly ICommand _down;

        public ADTSPionViewModel(ADTSPoint value, ICommand up, ICommand down)
        {
            _value = value;
            _up = up;
            _down = down;
        }

        public ADTSPoint Value
        {
            get { return _value; }
        }

        public int Index { get; set; }

        public ICommand Up
        {
            get { return _up; }
        }

        public ICommand Down
        {
            get { return _down; }
        }
    }
}