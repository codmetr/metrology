using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Результат на точке
    /// </summary>
    public class PointResultViewModel : INotifyPropertyChanged
    {
        private double _pressureReal;
        private double _real;
        private double _dIReal;
        private double _iback;
        private double _ivar;
        private double _dIvar;
        private bool _isCorrect;

        /// <summary>
        /// Фактическое давление
        /// </summary>
        public double PressureReal
        {
            get { return _pressureReal; }
            set { _pressureReal = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Фактическое напряжение (прямой ход)
        /// </summary>
        public double IReal
        {
            get { return _real; }
            set { _real = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Фактическая погрешность (прямой ход)
        /// </summary>
        public double dIReal
        {
            get { return _dIReal; }
            set { _dIReal = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Фактическое напряжение (обратный ход)
        /// </summary>
        public double Iback
        {
            get { return _iback; }
            set { _iback = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Фактическая вариация
        /// </summary>
        public double Ivar
        {
            get { return _ivar; }
            set { _ivar = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Фактическая погрешность вариации
        /// </summary>
        public double dIvar
        {
            get { return _dIvar; }
            set { _dIvar = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Напряжение на заданной точке в допуске
        /// </summary>
        public bool IsCorrect
        {
            get { return _isCorrect; }
            set { _isCorrect = value;
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