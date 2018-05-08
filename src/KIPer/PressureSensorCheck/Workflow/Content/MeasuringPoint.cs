using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PressureSensorCheck.Workflow
{
    public class MeasuringPoint : INotifyPropertyChanged
    {
        private double _pressure;
        private double _I;
        private double _in;
        private double _dI;
        private double _dIn;
        private double _qI;
        private double _qIn;
        private TimeSpan _timeStamp;

        /// <summary>
        /// Текущее давление
        /// </summary>
        public double Pressure
        {
            get { return _pressure; }
            set { _pressure = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущее напряжение
        /// </summary>
        public double I
        {
            get { return _I; }
            set { _I = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Нормативный выходной сигнал соответствующий заданному давлению
        /// </summary>
        public double In
        {
            get { return _in; }
            set { _in = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Отклонение от нормативного выходного сигнала на заданном давлении
        /// </summary>
        public double dI
        {
            get { return _dI; }
            set { _dI = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Допустимое отклонение от нормативного выходного сигнала на заданном давлении
        /// </summary>
        public double dIn
        {
            get { return _dIn; }
            set { _dIn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Относительное отклонение от нормативного выходного сигнала на заданном давлении
        /// </summary>
        public double qI
        {
            get { return _qI; }
            set { _qI = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Допустимое относительное отклонение от нормативного выходного сигнала на заданном давлении
        /// </summary>
        public double qIn
        {
            get { return _qIn; }
            set { _qIn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Метка времени измерения
        /// </summary>
        public TimeSpan TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}