using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ArchiveData.DTO;
using KipTM.Checks.ViewModel.Config;
using KipTM.EventAggregator;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.ViewModel.Events;
using PressureSensorCheck.Workflow.Content;
using PressureSensorData;
using Tools.View;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Конфигурация проверки
    /// </summary>
    public class PressureSensorCheckConfigVm : INotifyPropertyChanged, IDisposable
    {
        private bool _isTemplatePreview;
        private CheckPressureLogicConfigVm _config;
        private DPI620GeniiConfigVm _dpiConfig;
        private readonly IContext _context;
        private readonly IEventAggregator _agregator;
        private readonly TemplateStore<PressureSensorConfig> _templates;
        private readonly ITamplateArchive<PressureSensorConfig> _confArch;
        private PressureSensorConfig _data;
        private Dictionary<string, IEtalonSourceCannelFactory<Units>> _ethalonsSources;
        private string _selectedSourceName;
        private object _selectedSourceViewModel;
        private string _serialNumber;
        private PressureSensorConfigVm _sensorConfig;

        /// <summary>
        /// Конфигурация проверки
        /// </summary>
        public PressureSensorCheckConfigVm(IContext context,
            TestResultID identificator, PressureSensorConfig configData, DPI620GeniiConfigVm dpiConf,
            IEventAggregator agregator, ITamplateArchive<PressureSensorConfig> archive,
            Dictionary<string, IEtalonSourceCannelFactory<Units>> ethalonsSources)
        {
            Data = configData;
            Identificator = identificator;
            Config = new CheckPressureLogicConfigVm(configData);
            DpiConfig = dpiConf;
            _context = context;
            _agregator = agregator;
            _confArch = archive;
            _templates = new TemplateStore<PressureSensorConfig>(archive);
            _templates.LastData = configData;
            _templates.UpdatedTemplate += TemplatesOnUpdatedTemplate;
            _ethalonsSources = ethalonsSources;
            SourceNames = _ethalonsSources.Keys;
            _selectedSourceName = SourceNames.FirstOrDefault();
            var confVmTemplate = _ethalonsSources[_selectedSourceName];
            SelectedSourceViewModel = confVmTemplate?.ConfigViewModel;

            _sensorConfig = new PressureSensorConfigVm(context);
        }

        /// <summary>
        /// Обновлен применяемый шаблон настроек
        /// </summary>
        /// <param name="pressureSensorConfig"></param>
        private void TemplatesOnUpdatedTemplate(PressureSensorConfig pressureSensorConfig)
        {
            Data.SetCopy(pressureSensorConfig);
            OnPropertyChanged(nameof(Data));
        }

        /// <summary>
        /// Список названий эталонных источников
        /// </summary>
        public IEnumerable<string> SourceNames { get; private set; }

        /// <summary>
        /// Выбранный эталонный источник
        /// </summary>
        public string SelectedSourceName
        {
            get { return _selectedSourceName; }
            set
            {
                _selectedSourceName = value;
                OnPropertyChanged();
                OnSelectedSource(_selectedSourceName);
                //var confVmTemplate = _ethalonsSources[_selectedSourceName];
                //SelectedSourceViewModel = confVmTemplate?.ConfigViewModel;
            }
        }
        
        /// <summary>
        /// VM выбранного эталонного источника
        /// </summary>
        public object SelectedSourceViewModel
        {
            get { return _selectedSourceViewModel; }
            set
            {
                _selectedSourceViewModel = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                OnPropertyChanged(nameof(SerialNumber));
                OnSerialNumberCanged(_serialNumber);
            }
        }

        public PressureSensorConfigVm SensorConfig
        {
            get { return _sensorConfig; }
        }

        /// <summary>
        /// Идентифитатор проверки
        /// </summary>
        public TestResultID Identificator { get; }

        /// <summary>
        /// Фактические данные конфигурации
        /// </summary>
        /// <remarks>
        /// Использовать на разметке экрана только в случае единственного места изменения, так как без INotifyPropertyChanged
        /// </remarks>
        public PressureSensorConfig Data
        {
            get { return _data; }
            private set
            {
                _data = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public Units PressUnit
        {
            get { return Data.Unit; }
            set
            {
                if (value == Data.Unit)
                    return;
                Data.Unit = value;
                foreach (var point in Config.Points)
                {
                    point.Unit = value;
                }
                OnPropertyChanged("PressUnit");
                OnSelectedUnit(Data.Unit);
            }
        }

        /// <summary>
        /// Конфигурация логики проверки
        /// </summary>
        public CheckPressureLogicConfigVm Config
        {
            get { return _config; }
            set
            {
                _config = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Конфигурация DPI620
        /// </summary>
        public DPI620GeniiConfigVm DpiConfig
        {
            get { return _dpiConfig; }
            set
            {
                _dpiConfig = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Набор шаблон настроек
        /// </summary>
        public TemplateStore<PressureSensorConfig> Templates
        {
            get { return _templates; }
        }

        /// <summary>
        /// Показывать представление выбранных шаблонов
        /// </summary>
        public bool IsTemplatePreview
        {
            get { return _isTemplatePreview; }
            set
            {
                _isTemplatePreview = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void Dispose()
        {
            _confArch.SetLast(Data);
        }

        public void SetSourceNames(IEnumerable<string> ethalons)
        {
            _context.Invoke(()=>SourceNames = ethalons);
        }

        public void SetSelectedSourceNames(string selectedEthalon, object ethalonConfVm)
        {
            _context.Invoke(() =>
            {
                SelectedSourceName = selectedEthalon;
                SelectedSourceViewModel = ethalonConfVm;
            });
        }

        public void SetSerialNumber(string serial)
        {
            _context.Invoke(() => SerialNumber = serial);
        }

        public event Action<string> SelectedSource;

        public event Action<Units> SelectedUnit;

        public event Action<string> SerialNumberCanged;

        protected virtual void OnSelectedSource(string obj)
        {
            SelectedSource?.Invoke(obj);
        }

        protected virtual void OnSelectedUnit(Units obj)
        {
            SelectedUnit?.Invoke(obj);
        }

        protected virtual void OnSerialNumberCanged(string obj)
        {
            SerialNumberCanged?.Invoke(obj);
        }
    }
}
