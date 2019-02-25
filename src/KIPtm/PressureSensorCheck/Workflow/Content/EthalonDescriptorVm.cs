using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KipTM.Interfaces;

namespace PressureSensorCheck.Workflow.Content
{
    public class EthalonDescriptorVm:INotifyPropertyChanged
    {
        internal struct EthalonDescriptorData
        {
            public readonly string Title;
            public readonly string SensorType;
            public readonly string SerialNumber;
            public readonly string RegNum;
            public readonly string Category;
            public readonly string ErrorClass;
            public readonly string CheckCertificateNumber;
            public readonly string CheckCertificateDate;

            public EthalonDescriptorData(string title, string sensorType, string serialNumber, string regNum, string category, string errorClass, string checkCertificateNumber, string checkCertificateDate)
            {
                Title = title;
                SensorType = sensorType;
                SerialNumber = serialNumber;
                RegNum = regNum;
                Category = category;
                ErrorClass = errorClass;
                CheckCertificateNumber = checkCertificateNumber;
                CheckCertificateDate = checkCertificateDate;
            }
        }

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
                OnPropertyChanged();
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
        /// Регистрационный номер:
        /// </summary>
        public string RegNum
        {
            get { return _regNum; }
            set { _regNum = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Разряд:
        /// </summary>
        public string Category
        {
            get { return _category; }
            set { _category = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Класс или погрешность:
        /// </summary>
        public string ErrorClass
        {
            get { return _errorClass; }
            set { _errorClass = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Номер свидетельства о поверке:
        /// </summary>
        public string CheckCertificateNumber
        {
            get { return _checkCertificateNumber; }
            set { _checkCertificateNumber = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата выдачи свидетельства о поверке:
        /// </summary>
        public string CheckCertificateDate
        {
            get { return _checkCertificateDate; }
            set { _checkCertificateDate = value;
                OnPropertyChanged();
            }
        }

        internal void SetAllValues(EthalonDescriptorData data)
        {
            _context.Invoke(() =>
            {
                Title = data.Title;
                SensorType = data.SensorType;
                SerialNumber = data.SerialNumber;
                RegNum = data.RegNum;
                Category = data.Category;
                ErrorClass = data.ErrorClass;
                CheckCertificateNumber = data.CheckCertificateNumber;
                CheckCertificateDate = data.CheckCertificateDate;
            });
        }

        /// <summary>
        /// Настройки изменены
        /// </summary>
        internal event Action<EthalonDescriptorData> ConfigChanged;

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
            ConfigChanged?.Invoke(GetValues());
        }

        internal EthalonDescriptorData GetValues()
        {
            return new EthalonDescriptorData(Title, SensorType, SerialNumber, RegNum, Category, ErrorClass,
                CheckCertificateNumber, CheckCertificateDate);
        }
    }
}