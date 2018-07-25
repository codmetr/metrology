using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ArchiveData;
using ArchiveData.DTO;
using KipTM.EventAggregator;
using KipTM.ViewModel.Events;
using PressureSensorData;
using Tools.View;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Визуальная модель результата поверки датчика давления
    /// </summary>
    public class PressureSensorResultVM:INotifyPropertyChanged
    {
        /// <summary>
        /// Хранилище результата для конкретной проверки
        /// </summary>
        private readonly IDataAccessor _accessor;

        private PressureSensorConfig _conf;
        private PressureSensorResult _data;
        private string _assay;
        private string _leak;
        private string _commonResult;
        private string _visualCheckResult;
        private DateTime? _timeStamp;
        private readonly IEventAggregator _agregator;
        private bool _isSaveEnable = true;

        public PressureSensorResultVM(TestResultID checkResId, IDataAccessor accessor, PressureSensorResult result, PressureSensorConfig conf, IEventAggregator agregator)
        {
            Identificator = checkResId;
            _accessor = accessor;
            PointResults = new ObservableCollection<PointViewModel>();
            Data = result;
            _conf = conf;
            _agregator = agregator;
        }

        /// <summary>
        /// Текущий результат проверки
        /// </summary>
        public PressureSensorResult Data
        {
            get { return _data; }
            set { _data = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущий результат проверки
        /// </summary>
        public PressureSensorConfig Conf
        {
            get { return _conf; }
        }

        /// <summary>
        /// Идентифитатор проверки
        /// </summary>
        public TestResultID Identificator { get; }

        /// <summary>
        /// Результат опробирования
        /// </summary>
        public string Assay
        {
            get { return _assay; }
            set { _assay = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Результат проверки на герметичность
        /// </summary>
        public string Leak
        {
            get { return _leak; }
            set { _leak = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Общий результат поверки
        /// </summary>
        public string CommonResult
        {
            get { return _commonResult; }
            set { _commonResult = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Результат визуального осмотра
        /// </summary>
        public string VisualCheckResult
        {
            get { return _visualCheckResult; }
            set { _visualCheckResult = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PointViewModel> PointResults { get; set; }

        /// <summary>
        /// Дата протокола
        /// </summary>
        public DateTime? TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Операция сохранения доступна
        /// </summary>
        public bool IsSaveEnable
        {
            get { return _isSaveEnable; }
            set
            {
                _isSaveEnable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Сохранить
        /// </summary>
        public ICommand Save{ get { return new CommandWrapper(OnSave); } }

        /// <summary>
        /// Фактическое выполнение сохранение
        /// </summary>
        private void OnSave()
        {
            IsSaveEnable = false;
            try
            {
                _agregator?.Post(new HelpMessageEventArg("Сохранение.."));
                if (Identificator.Id == null)
                {
                    Identificator.CreateTime = DateTime.Now;
                    Identificator.Timestamp = DateTime.Now;
                    _accessor.Add(Identificator, Data, _conf);
                }
                else
                {
                    Identificator.Timestamp = DateTime.Now;
                    _accessor.Update(Identificator);
                    _accessor.Save(Identificator, Data);
                    _accessor.SaveConfig(Identificator, _conf);
                }
                _agregator?.Post(new HelpMessageEventArg("Сохранено"));
            }
            finally
            {
                IsSaveEnable = true;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {

            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}



