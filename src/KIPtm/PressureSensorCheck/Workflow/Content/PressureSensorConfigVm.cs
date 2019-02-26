using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using KipTM.Interfaces;

namespace PressureSensorCheck.Workflow.Content
{
    public class PressureSensorOrgVm:INotifyPropertyChanged
    {
        internal struct PressureSensorOrgData
        {
            public readonly string User;
            public readonly string ChiefLab;
            public readonly string ReportNumber;
            public readonly string ReportDate;
            public readonly string CertificateNumber;
            public readonly string CertificateDate;
            public readonly string Validity;
            public readonly string Master;
            public readonly string Name;
            public readonly string RegNum;
            public readonly string SensorType;
            public readonly string SensorModel;
            public readonly string NumberLastCheck;
            public readonly string SerialNumber;
            public readonly string ChecklLawBase;
            public readonly string CheckedParameters;
            public readonly string Company;
            public readonly double Temperature;
            public readonly double Humidity;
            public readonly double DayPressure;
            public readonly double CommonVoltage;

            public PressureSensorOrgData(string user, string chiefLab, string reportNumber, string reportDate, string certificateNumber, string certificateDate, string validity, string master, string name, string regNum, string sensorType, string sensorModel, string numberLastCheck, string serialNumber, string checklLawBase, string checkedParameters, string company, double temperature, double humidity, double dayPressure, double commonVoltage)
            {
                User = user;
                ChiefLab = chiefLab;
                ReportNumber = reportNumber;
                ReportDate = reportDate;
                CertificateNumber = certificateNumber;
                CertificateDate = certificateDate;
                Validity = validity;
                Master = master;
                Name = name;
                RegNum = regNum;
                SensorType = sensorType;
                SensorModel = sensorModel;
                NumberLastCheck = numberLastCheck;
                SerialNumber = serialNumber;
                ChecklLawBase = checklLawBase;
                CheckedParameters = checkedParameters;
                Company = company;
                Temperature = temperature;
                Humidity = humidity;
                DayPressure = dayPressure;
                CommonVoltage = commonVoltage;
            }
        }

        private readonly IContext _context;
        private string _user;
        private string _chiefLab;
        private string _reportNumber;
        private string _reportDate;
        private string _certificateNumber;
        private string _certificateDate;
        private string _validity;
        private string _master;
        private string _name;
        private string _regNum;
        private string _sensorType;
        private string _sensorModel;
        private string _numberLastCheck;
        private string _serialNumber;
        private string _checklLawBase;
        private string _checkedParameters;
        private readonly EthalonDescriptorVm _ethalonPressure;
        private readonly EthalonDescriptorVm _ethalonOutSignal;
        private string _company;
        private double _temperature;
        private double _humidity;
        private double _dayPressure;
        private double _commonVoltage;

        public PressureSensorOrgVm(IContext context)
        {
            _context = context;
            _ethalonPressure = new EthalonDescriptorVm(context);
            _ethalonOutSignal = new EthalonDescriptorVm(context);
        }

        /// <summary>
        /// Поверитель:
        /// </summary>
        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Руководитель лаборатории
        /// </summary>
        public string ChiefLab
        {
            get { return _chiefLab; }
            set { _chiefLab = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Номер протокола:
        /// </summary>
        public string ReportNumber
        {
            get { return _reportNumber; }
            set { _reportNumber = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата протокола:
        /// </summary>
        public string ReportDate
        {
            get { return _reportDate; }
            set { _reportDate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Номер сертификата:
        /// </summary>
        public string CertificateNumber
        {
            get { return _certificateNumber; }
            set { _certificateNumber = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата сертификата:
        /// </summary>
        public string CertificateDate
        {
            get { return _certificateDate; }
            set { _certificateDate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Интервал между проверками:
        /// </summary>
        public string Validity
        {
            get { return _validity; }
            set { _validity = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Принадлежит:
        /// </summary>
        public string Master
        {
            get { return _master; }
            set { _master = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Наименование:
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Регистрационный номер в Федеральном информационном фонде по обеспечению единства измерений:
        /// </summary>
        public string RegNum
        {
            get { return _regNum; }
            set { _regNum = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Тип:
        /// </summary>
        public string SensorType
        {
            get { return _sensorType; }
            set { _sensorType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Модификация:
        /// </summary>
        public string SensorModel
        {
            get { return _sensorModel; }
            set { _sensorModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Серия и номер знака предыдущей проверки (если такие серия и номер имеются):
        /// </summary>
        public string NumberLastCheck
        {
            get { return _numberLastCheck; }
            set { _numberLastCheck = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Заводской номер (номера):
        /// </summary>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Поверено в соответствии с:
        /// </summary>
        /// <remarks>
        /// Наименование документа, на основании которого выполнена поверка
        /// </remarks>
        public string ChecklLawBase
        {
            get { return _checklLawBase; }
            set { _checklLawBase = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Поверено:
        /// </summary>
        /// <remarks>
        /// Наименование величин, диапазонов, на которых поверено средство измерений (если предусмотрено методикой поверки)
        /// </remarks>
        public string CheckedParameters
        {
            get { return _checkedParameters; }
            set { _checkedParameters = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Эталон давления
        /// </summary>
        public EthalonDescriptorVm EthalonPressure
        {
            get { return _ethalonPressure; }
        }

        /// <summary>
        /// Эталон выходного сигнала
        /// </summary>
        public EthalonDescriptorVm EthalonOutSignal
        {
            get { return _ethalonOutSignal; }
        }

        /// <summary>
        /// Организация
        /// </summary>
        ///<remarks>
        /// Наименование юридического лица или индивидуального предпринимателя, аккредитованного в установленном порядке на проведение поверки средств измерений, регистрационный номер аттестата аккредитации
        /// </remarks>
        public string Company
        {
            get { return _company; }
            set { _company = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Температура
        /// </summary>
        public double Temperature
        {
            get { return _temperature; }
            set { _temperature = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Влажность
        /// </summary>
        public double Humidity
        {
            get { return _humidity; }
            set { _humidity = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Давление дня
        /// </summary>
        public double DayPressure
        {
            get { return _dayPressure; }
            set { _dayPressure = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Напряжение сети
        /// </summary>
        public double CommonVoltage
        {
            get { return _commonVoltage; }
            set { _commonVoltage = value;
                OnPropertyChanged();
            }
        }

        internal void SetAllValues(PressureSensorOrgData data)
        {
            _context.Invoke(() =>
            {
                User = data.User;
                ChiefLab = data.ChiefLab;
                ReportNumber = data.ReportNumber;
                ReportDate = data.ReportDate;
                CertificateNumber = data.CertificateNumber;
                CertificateDate = data.CertificateDate;
                Validity = data.Validity;
                Master = data.Master;
                Name = data.Name;
                RegNum = data.RegNum;
                SensorType = data.SensorType;
                SensorModel = data.SensorModel;
                NumberLastCheck = data.NumberLastCheck;
                SerialNumber = data.SerialNumber;
                ChecklLawBase = data.ChecklLawBase;
                CheckedParameters = data.CheckedParameters;
                Company = data.Company;
                Temperature = data.Temperature;
                Humidity = data.Humidity;
                DayPressure = data.DayPressure;
                CommonVoltage = data.CommonVoltage;
            });
        }

        /// <summary>
        /// Настройки изменены
        /// </summary>
        internal event Action<PressureSensorOrgData> ConfigChanged; 

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            OnConfigChanged();
        }

        #endregion

        protected virtual void OnConfigChanged()
        {
            ConfigChanged?.Invoke(GetCurentValue());
        }

        private PressureSensorOrgData GetCurentValue()
        {
            return new PressureSensorOrgData(User, ChiefLab, ReportNumber, ReportDate, CertificateNumber,
                CertificateDate, Validity, Master, Name, RegNum, SensorType, SensorModel, NumberLastCheck, SerialNumber,
                ChecklLawBase, CheckedParameters, Company, Temperature, Humidity, DayPressure, CommonVoltage);
        }
    }
}
