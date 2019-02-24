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
    public class PressureSensorConfigVm:INotifyPropertyChanged
    {
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

        public PressureSensorConfigVm(IContext context)
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
                OnPropertyChanged(nameof(User));
            }
        }

        /// <summary>
        /// Руководитель лаборатории
        /// </summary>
        public string ChiefLab
        {
            get { return _chiefLab; }
            set { _chiefLab = value;
                OnPropertyChanged(nameof(ChiefLab));
            }
        }

        /// <summary>
        /// Номер протокола:
        /// </summary>
        public string ReportNumber
        {
            get { return _reportNumber; }
            set { _reportNumber = value;
                OnPropertyChanged(nameof(ReportNumber));
            }
        }

        /// <summary>
        /// Дата протокола:
        /// </summary>
        public string ReportDate
        {
            get { return _reportDate; }
            set { _reportDate = value;
                OnPropertyChanged(nameof(ReportDate));
            }
        }

        /// <summary>
        /// Номер сертификата:
        /// </summary>
        public string CertificateNumber
        {
            get { return _certificateNumber; }
            set { _certificateNumber = value;
                OnPropertyChanged(nameof(CertificateNumber));
            }
        }

        /// <summary>
        /// Дата сертификата:
        /// </summary>
        public string CertificateDate
        {
            get { return _certificateDate; }
            set { _certificateDate = value;
                OnPropertyChanged(nameof(CertificateDate));
            }
        }

        /// <summary>
        /// Интервал между проверками:
        /// </summary>
        public string Validity
        {
            get { return _validity; }
            set { _validity = value;
                OnPropertyChanged(nameof(Validity));
            }
        }

        /// <summary>
        /// Принадлежит:
        /// </summary>
        public string Master
        {
            get { return _master; }
            set { _master = value;
                OnPropertyChanged(nameof(Master));
            }
        }

        /// <summary>
        /// Наименование:
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }


        /// <summary>
        /// Регистрационный номер в Федеральном информационном фонде по обеспечению единства измерений:
        /// </summary>
        public string RegNum
        {
            get { return _regNum; }
            set { _regNum = value;
                OnPropertyChanged(nameof(RegNum));
            }
        }

        /// <summary>
        /// Тип:
        /// </summary>
        public string SensorType
        {
            get { return _sensorType; }
            set { _sensorType = value;
                OnPropertyChanged(nameof(SensorType));
            }
        }

        /// <summary>
        /// Модификация:
        /// </summary>
        public string SensorModel
        {
            get { return _sensorModel; }
            set { _sensorModel = value;
                OnPropertyChanged(nameof(SensorModel));
            }
        }

        /// <summary>
        /// Серия и номер знака предыдущей проверки (если такие серия и номер имеются):
        /// </summary>
        public string NumberLastCheck
        {
            get { return _numberLastCheck; }
            set { _numberLastCheck = value;
                OnPropertyChanged(nameof(NumberLastCheck));
            }
        }

        /// <summary>
        /// Заводской номер (номера):
        /// </summary>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value;
                OnPropertyChanged(nameof(SerialNumber));
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
                OnPropertyChanged(nameof(ChecklLawBase));
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
                OnPropertyChanged(nameof(CheckedParameters));
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
                OnPropertyChanged(nameof(Company));
            }
        }

        /// <summary>
        /// Температура
        /// </summary>
        public double Temperature
        {
            get { return _temperature; }
            set { _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }

        /// <summary>
        /// Влажность
        /// </summary>
        public double Humidity
        {
            get { return _humidity; }
            set { _humidity = value;
                OnPropertyChanged(nameof(Humidity));
            }
        }

        /// <summary>
        /// Давление дня
        /// </summary>
        public double DayPressure
        {
            get { return _dayPressure; }
            set { _dayPressure = value;
                OnPropertyChanged(nameof(DayPressure));
            }
        }

        /// <summary>
        /// Напряжение сети
        /// </summary>
        public double CommonVoltage
        {
            get { return _commonVoltage; }
            set { _commonVoltage = value;
                OnPropertyChanged(nameof(CommonVoltage));
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


    public class EthalonDescriptorVm:INotifyPropertyChanged
    {
        private readonly IContext _context;
        private string _title;
        private string _sensorType;
        private string _serialNumber;
        private string _regNum;
        private string _category;
        private string _errorClass;
        private string _checkCertificateNumber;
        private string _checkCertificateDate;

        public EthalonDescriptorVm(IContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Наименование:
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        /// <summary>
        /// Тип:
        /// </summary>
        /// <remarks>
        /// Обобщенное название типа прибора
        /// </remarks>
        public string SensorType
        {
            get { return _sensorType; }
            set { _sensorType = value;
                OnPropertyChanged(nameof(SensorType));
            }
        }

        /// <summary>
        /// Заводской номер (номера):
        /// </summary>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value;
                OnPropertyChanged(nameof(SerialNumber));
            }
        }

        /// <summary>
        /// Регистрационный номер:
        /// </summary>
        public string RegNum
        {
            get { return _regNum; }
            set { _regNum = value;
                OnPropertyChanged(nameof(RegNum));
            }
        }

        /// <summary>
        /// Разряд:
        /// </summary>
        public string Category
        {
            get { return _category; }
            set { _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }

        /// <summary>
        /// Класс или погрешность:
        /// </summary>
        public string ErrorClass
        {
            get { return _errorClass; }
            set { _errorClass = value;
                OnPropertyChanged(nameof(ErrorClass));
            }
        }

        /// <summary>
        /// Номер свидетельства о поверке:
        /// </summary>
        public string CheckCertificateNumber
        {
            get { return _checkCertificateNumber; }
            set { _checkCertificateNumber = value;
                OnPropertyChanged(nameof(CheckCertificateNumber));
            }
        }

        /// <summary>
        /// Дата выдачи свидетельства о поверке:
        /// </summary>
        public string CheckCertificateDate
        {
            get { return _checkCertificateDate; }
            set { _checkCertificateDate = value;
                OnPropertyChanged(nameof(CheckCertificateDate));
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
