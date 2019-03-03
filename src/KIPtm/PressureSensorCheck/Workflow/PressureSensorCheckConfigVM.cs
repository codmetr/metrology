using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ArchiveData.DTO;
using KipTM.Checks.ViewModel.Config;
using KipTM.Interfaces;
using KipTM.ViewModel.Events;
using PressureSensorCheck.Workflow.Content;
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

        //private readonly TemplateStore<PressureSensorConfig> _templates;
        private string _selectedSourceName;
        private object _selectedSourceViewModel;
        private string _serialNumber;
        private readonly PressureSensorOrgVm _sensorConfig;

        /// <summary>
        /// Конфигурация проверки
        /// </summary>
        public PressureSensorCheckConfigVm(IContext context, CheckPressureLogicConfigVm configData, DPI620GeniiConfigVm dpiConf)
        {
            Config = configData;
            DpiConfig = dpiConf;
            _context = context;
            //_templates = new TemplateStore<PressureSensorConfig>(archive);
            //_templates.LastData = configData;
            //_templates.UpdatedTemplate += TemplatesOnUpdatedTemplate;
            
            _sensorConfig = new PressureSensorOrgVm(context);
        }

        /// <summary>
        /// Список названий эталонных источников
        /// </summary>
        public IEnumerable<string> SourceNames { get; private set; }

        /// <summary>
        /// Конфиг
        /// </summary>
        public PressureSensorOrgVm CommonData { get { return _sensorConfig; } }

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

        ///// <summary>
        ///// Набор шаблон настроек
        ///// </summary>
        //public TemplateStore<PressureSensorConfig> Templates
        //{
        //    get { return _templates; }
        //}

        ///// <summary>
        ///// Показывать представление выбранных шаблонов
        ///// </summary>
        //public bool IsTemplatePreview
        //{
        //    get { return _isTemplatePreview; }
        //    set
        //    {
        //        _isTemplatePreview = value;
        //        OnPropertyChanged();
        //    }
        //}

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void Dispose()
        {
            //_confArch.SetLast(Data);
        }

        /// <summary>
        /// Задать список названий эталонных источников
        /// </summary>
        /// <param name="ethalons"></param>
        public void SetSourceNames(IEnumerable<string> ethalons)
        {
            _context.Invoke(()=>SourceNames = ethalons);
        }

        /// <summary>
        /// Задать выбранный эталонный источник
        /// </summary>
        /// <param name="selectedEthalon"></param>
        /// <param name="ethalonConfVm"></param>
        public void SetSelectedSourceNames(string selectedEthalon, object ethalonConfVm)
        {
            _context.Invoke(() =>
            {
                SelectedSourceName = selectedEthalon;
                SelectedSourceViewModel = ethalonConfVm;
            });
        }

        /// <summary>
        /// Затать серийный номер
        /// </summary>
        /// <param name="serial"></param>
        public void SetSerialNumber(string serial)
        {
            _context.Invoke(() => SerialNumber = serial);
        }

        public event Action<string> SelectedSource;

        public event Action<Units> SelectedUnit;

        public event Action<string> SerialNumberCanged;

        ///// <summary>
        ///// Обновлен применяемый шаблон настроек
        ///// </summary>
        ///// <param name="pressureSensorConfig"></param>
        //private void TemplatesOnUpdatedTemplate(PressureSensorConfig pressureSensorConfig)
        //{
        //    Data.SetCopy(pressureSensorConfig);
        //    OnPropertyChanged(nameof(Data));
        //}

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
