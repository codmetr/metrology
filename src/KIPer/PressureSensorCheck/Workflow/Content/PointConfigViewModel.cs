using System.ComponentModel;
using System.Runtime.CompilerServices;
using KipTM.Interfaces;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Конфигурация точки
    /// </summary>
    public class PointConfigViewModel : INotifyPropertyChanged
    {
        private double _pressure;
        private Units _unit;
        private double _I;
        private double _dI;
        private double _ivar;

        /// <summary>
        /// Проверяемая точка давления
        /// </summary>
        public double Pressure
        {
            get { return _pressure; }
            set { _pressure = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public Units Unit
        {
            get { return _unit; }
            set { _unit = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Ожидаемое значение тока
        /// </summary>
        public double I
        {
            get { return _I; }
            set { _I = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Допуск по току
        /// </summary>
        public double dI
        {
            get { return _dI; }
            set { _dI = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Допуск по вариации напряжения
        /// </summary>
        public double Ivar
        {
            get { return _ivar; }
            set { _ivar = value;
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