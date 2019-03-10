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
        private readonly IContext _context;
        private double _pressure;
        private Units _unit;
        private double _I;
        private double _dI;
        private double _ivar;

        public PointConfigViewModel(IContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Конфигурация точки
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pressure"></param>
        /// <param name="i"></param>
        /// <param name="di"></param>
        /// <param name="unit"></param>
        public PointConfigViewModel(IContext context, double pressure, double i, double di, Units unit)
        {
            _context = context;
            _pressure = pressure;
            _I = i;
            _dI = di;
            _unit = unit;
        }

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

        public void SetPressure(double val)
        {
            _context.Invoke(()=>Pressure = val);
        }
        public void SetUnit(Units val)
        {
            _context.Invoke(()=> Unit = val);
        }
        public void SetI(double val)
        {
            _context.Invoke(()=> I = val);
        }
        public void SetdI(double val)
        {
            _context.Invoke(()=> dI = val);
        }
        public void SetIvar(double val)
        {
            _context.Invoke(()=> Ivar = val);
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