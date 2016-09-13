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

        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSCheckConfigViewModel(ADTSMethodParameters customConf)
        {
            _customConf = customConf;
            _points = new ObservableCollection<ADTSPionViewModel>(_customConf.Points.Select(el=>new ADTSPionViewModel(el, Up, Down)));
            UpdatePoints(_points);

            _up = new CommandWrapper((arg) =>
            {
                var index = (int?)arg;
                if (index == null)
                    return;

                if (_points.Count == 0 || index.Value >= _points.Count)
                    return;

                if (index.Value < 1)
                    return;

                var val = _points[index.Value];
                _points.RemoveAt(index.Value);
                _points.Insert(index.Value - 1, val);
                UpdatePoints(_points);
            });

            _down = new CommandWrapper((arg) =>
            {
                var index = (int?)arg;
                if (index == null)
                    return;

                if (_points.Count == 0 || index.Value >= _points.Count)
                    return;

                if (index.Value < 1)
                    return;

                var val = _points[index.Value];
                _points.RemoveAt(index.Value);
                _points.Insert(index.Value - 1, val);
                UpdatePoints(_points);
            });
        }

        public ObservableCollection<ADTSPionViewModel> Points { get { return _points; } }

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

        private ICommand _up;
        private ICommand _down;

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

                        if (_points.Count == 0 || point.Pressure < _points.Last().Value.Pressure)
                        {
                            _points.Add(new ADTSPionViewModel(point, Up, Down));
                            _customConf.Points.Add(point);
                            UpdatePoints(_points);
                            return;
                        }

                        int index = _points.Count - 1;
                        for (int i = _points.Count-1; i >=0 ; i--)
                        {
                            if (point.Pressure < _points[i].Value.Pressure)
                                break;
                            index = i;
                        }
                        _points.Insert(index, new ADTSPionViewModel(point, Up, Down));
                        _customConf.Points.Insert(index, point);
                        UpdatePoints(_points);
                    });
            }
        }

        public ICommand Up{get{return _up;}}

        public ICommand Down{get{return _down;}}

        private void UpdatePoints(ObservableCollection<ADTSPionViewModel> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].Index = i;
            }
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