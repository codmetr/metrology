using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PACESeries;
using Tools.View;

namespace PACESeriesUtil.VM
{
    /// <summary>
    /// Управление PACE
    /// </summary>
    public class PaceControlViewModel:INotifyPropertyChanged
    {
        private string _pressureStr;
        private string _limitsStr;
        private PressureUnits _unit;
        private IEnumerable<PressureUnits> _units;

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public IEnumerable<PressureUnits> Units
        {
            get { return _units; }
            set
            {
                _units = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public PressureUnits Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Установить единицы измерения
        /// </summary>
        public ICommand SetUnit { get {return new CommandWrapper(DoSetUnit);} }

        /// <summary>
        /// Целевое значение давления
        /// </summary>
        public string PressureStr
        {
            get { return _pressureStr; }
            set
            {
                _pressureStr = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Установить давление
        /// </summary>
        public ICommand SetPressure { get { return new CommandWrapper(DoSetPressure);} }

        /// <summary>
        /// Ограничение
        /// </summary>
        public string LimitsStr
        {
            get { return _limitsStr; }
            set
            {
                _limitsStr = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Установить ограничение
        /// </summary>
        public ICommand SetLimit { get { return new CommandWrapper(DoSetLimit);} }

        /// <summary>
        /// Запрошена установка ограничения
        /// </summary>
        public event Action<string> EvSetLimit;

        /// <summary>
        /// Запрошена установка давления
        /// </summary>
        public event Action<string> EvSetPress;

        /// <summary>
        /// Запрошена установка единиц измерения
        /// </summary>
        public event Action<PressureUnits> EvSetUnit;
        
        private void DoSetLimit()
        {
            OnEvSetLimit(_limitsStr);
        }

        private void DoSetPressure()
        {
            OnEvSetPress(_pressureStr);
        }

        private void DoSetUnit()
        {
            OnEvSetUnit(_unit);
        }

        protected virtual void OnEvSetLimit(string obj)
        {
            EvSetLimit?.Invoke(obj);
        }

        protected virtual void OnEvSetPress(string obj)
        {
            EvSetPress?.Invoke(obj);
        }

        protected virtual void OnEvSetUnit(PressureUnits obj)
        {
            EvSetUnit?.Invoke(obj);
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
