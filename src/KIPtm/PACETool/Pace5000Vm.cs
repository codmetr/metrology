using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using KipTM.Model.Devices;
using PACESeries;
using Tools.View;

namespace PACETool
{
    class Pace5000Vm:BaseVm, IPace5000Vm
    {
        private double _pressure;
        private string _unit;
        private UnitDescriptor<PressureUnits> _selectedUnit;
        private bool _isAutoRead;
        private TimeSpan _autoreadPeriod;
        private string _paceMode;
        private string _aim;
        private double _aimDouble;
        private bool _isSetAvailable;

        public Pace5000Vm(Dispatcher disp) : base(disp)
        {
            AvalableUnits = new[]
            {
                new UnitDescriptor<PressureUnits>(PressureUnits.MBar,_pressureUnitToString(PressureUnits.MBar)),
                new UnitDescriptor<PressureUnits>(PressureUnits.Bar,_pressureUnitToString(PressureUnits.Bar)),
                new UnitDescriptor<PressureUnits>(PressureUnits.inH2O4,_pressureUnitToString(PressureUnits.inH2O4)),
                new UnitDescriptor<PressureUnits>(PressureUnits.inH2O,_pressureUnitToString(PressureUnits.inH2O)),
                new UnitDescriptor<PressureUnits>(PressureUnits.inHg,_pressureUnitToString(PressureUnits.inHg)),
                new UnitDescriptor<PressureUnits>(PressureUnits.mmHg,_pressureUnitToString(PressureUnits.mmHg)),
                new UnitDescriptor<PressureUnits>(PressureUnits.Pa,_pressureUnitToString(PressureUnits.Pa)),
                new UnitDescriptor<PressureUnits>(PressureUnits.hPa,_pressureUnitToString(PressureUnits.hPa)),
                new UnitDescriptor<PressureUnits>(PressureUnits.kPa,_pressureUnitToString(PressureUnits.kPa)),
                new UnitDescriptor<PressureUnits>(PressureUnits.psi,_pressureUnitToString(PressureUnits.psi)),
                new UnitDescriptor<PressureUnits>(PressureUnits.inH2O60F,_pressureUnitToString(PressureUnits.inH2O60F)),
                new UnitDescriptor<PressureUnits>(PressureUnits.KgCm2,_pressureUnitToString(PressureUnits.KgCm2)),
                new UnitDescriptor<PressureUnits>(PressureUnits.ATM,_pressureUnitToString(PressureUnits.ATM)),
                new UnitDescriptor<PressureUnits>(PressureUnits.mmH2O4,_pressureUnitToString(PressureUnits.mmH2O4)),
            };
            SelectedUnit = AvalableUnits.First();
            IsAutoRead = false;
            AutoreadPeriod = TimeSpan.FromMilliseconds(300);
        }

        public double Pressure
        {
            get { return _pressure; }
            set
            {
                _pressure = value;
                OnPropertyChanged();
            }
        }

        public string Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdatePressureAndUnits
        {
            get { return new CommandWrapper(OnCallUpdatePressureAndUnits); }
        }

        public IEnumerable<UnitDescriptor<PressureUnits>> AvalableUnits { get; }

        public UnitDescriptor<PressureUnits> SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                _selectedUnit = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateUnits
        {
            get { return new CommandWrapper(OnCallUpdateUnits); }
        }

        public ICommand SetSelectedUnit
        {
            get { return new CommandWrapper(()=>OnCallSetSelectedUnit(SelectedUnit.Unit)); }
        }

        public ICommand SetLloOn
        {
            get { return new CommandWrapper(OnCallSetLloOn); }
        }

        public ICommand SetLloOff
        {
            get { return new CommandWrapper(OnCallSetLloOff); }
        }

        public ICommand SetLocal
        {
            get { return new CommandWrapper(OnCallSetLocal); }
        }

        public ICommand SetRemote
        {
            get { return new CommandWrapper(OnCallSetRemote); }
        }

        public bool IsAutoRead
        {
            get { return _isAutoRead; }
            set
            {
                _isAutoRead = value;
                if (_isAutoRead)
                    OnCallStartAutoUpdate(AutoreadPeriod);
                else
                    OnCallStopAutoUpdate();
                OnPropertyChanged();
            }
        }

        public TimeSpan AutoreadPeriod
        {
            get { return _autoreadPeriod; }
            set
            {
                _autoreadPeriod = value;
                OnPropertyChanged();
            }
        }

        public string PaceMode
        {
            get { return _paceMode; }
            set
            {
                _paceMode = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToControll
        {
            get { return new CommandWrapper(OnCallToControll);}
        }

        public ICommand ToMeasuring
        {
            get { return new CommandWrapper(OnCallToMeasuring);}
        }

        public string Aim
        {
            get { return _aim; }
            set
            {
                _aim = value;
                OnPropertyChanged();
                IsSetAvailable = ConvertAim(_aim, out _aimDouble);
            }
        }

        public bool IsSetAvailable
        {
            get { return _isSetAvailable; }
            set
            {
                _isSetAvailable = value;
                OnPropertyChanged();
            }
        }

        public ICommand SetAim
        {
            get { return new CommandWrapper(()=>OnCallSetAim(_aimDouble)); }
        }

        public ICommand ReadAim
        {
            get { return new CommandWrapper(OnCallReadAim); }
        }

        #region Public interface 

        public void SetPressure(double pressure)
        {
            Sync(()=>Pressure = pressure);
        }

        public void SetUnit(PressureUnits unit)
        {
            Sync(()=>Unit = unit.ToString());
        }

        public void SetPaceMode(bool isControll)
        {
            Sync(() => PaceMode = isControll ? "Контроль" : "Измерение");
        }

        public void SetAimVal(double aim)
        {
            Sync(() => Aim = aim.ToString("F3"));
        }

        public event Action<TimeSpan> CallStartAutoUpdate;
        public event Action CallStopAutoUpdate;
        public event Action CallUpdatePressureAndUnits;
        public event Action CallUpdateUnits;
        public event Action<PressureUnits> CallSetSelectedUnit;
        public event Action CallSetLloOn;
        public event Action CallSetLloOff;
        public event Action CallSetLocal;
        public event Action CallSetRemote;
        public event Action CallToControll;
        public event Action CallToMeasuring;
        public event Action<double> CallSetAim;
        public event Action CallReadAim;
        #endregion

        private string _pressureUnitToString(PressureUnits unit)
        {
            switch (unit)
            {
                case PressureUnits.None:
                    break;
                case PressureUnits.MBar:
                    return "мБар";
                case PressureUnits.Bar:
                    return "бар";
                case PressureUnits.inH2O4:
                    return "дюйм вод.ст. 4С";
                case PressureUnits.inH2O:
                    return "дюйм вод.ст. 20С";
                case PressureUnits.inHg:
                    return "дюйм рт.ст.";
                case PressureUnits.mmHg:
                    return "мм рт.ст.";
                case PressureUnits.Pa:
                    return "Па";
                case PressureUnits.hPa:
                    return "гПа";
                case PressureUnits.kPa:
                    return "кПа";
                case PressureUnits.psi:
                    return "фтс/дюйм";
                case PressureUnits.inH2O60F:
                    return "дюйм вод.ст. 60F";
                case PressureUnits.KgCm2:
                    return "кгс/см";
                case PressureUnits.ATM:
                    return "атм";
                case PressureUnits.mmH2O4:
                    return "мм вод.ст. 4С";
                default:
                    throw new ArgumentOutOfRangeException("unit");
            }
            return string.Empty;
        }

        public bool ConvertAim(string data, out double val)
        {
            if (data.EndsWith(".") || data.EndsWith(","))
                data = data.Substring(0, data.Length - 1);
            return double.TryParse(data.Replace(",", "."),NumberStyles.Any, CultureInfo.InvariantCulture, out val);
        }

        protected virtual void OnCallUpdatePressureAndUnits()
        {
            CallUpdatePressureAndUnits?.Invoke();
        }

        protected virtual void OnCallUpdateUnits()
        {
            CallUpdateUnits?.Invoke();
        }

        protected virtual void OnCallSetSelectedUnit(PressureUnits obj)
        {
            CallSetSelectedUnit?.Invoke(obj);
        }

        protected virtual void OnCallSetLloOn()
        {
            CallSetLloOn?.Invoke();
        }

        protected virtual void OnCallSetLloOff()
        {
            CallSetLloOff?.Invoke();
        }

        protected virtual void OnCallSetLocal()
        {
            CallSetLocal?.Invoke();
        }

        protected virtual void OnCallSetRemote()
        {
            CallSetRemote?.Invoke();
        }

        protected virtual void OnCallToControll()
        {
            CallToControll?.Invoke();
        }

        protected virtual void OnCallToMeasuring()
        {
            CallToMeasuring?.Invoke();
        }

        protected virtual void OnCallSetAim(double val)
        {
            CallSetAim?.Invoke(val);
        }

        protected virtual void OnCallReadAim()
        {
            CallReadAim?.Invoke();
        }

        protected virtual void OnCallStartAutoUpdate(TimeSpan obj)
        {
            CallStartAutoUpdate?.Invoke(obj);
        }

        protected virtual void OnCallStopAutoUpdate()
        {
            CallStopAutoUpdate?.Invoke();
        }
    }
}
