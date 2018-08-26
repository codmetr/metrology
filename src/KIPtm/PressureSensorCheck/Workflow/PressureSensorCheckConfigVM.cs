using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ArchiveData.DTO;
using KipTM.Checks.ViewModel.Config;
using KipTM.EventAggregator;
using KipTM.Interfaces;
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
        private DPI620GeniiConfig _dpiConfig;
        private readonly IEventAggregator _agregator;
        private readonly TemplateStore<PressureSensorConfig> _templates;
        private readonly ITamplateArchive<PressureSensorConfig> _confArch;
        private PressureSensorConfig _data;

        public PressureSensorCheckConfigVm(
            TestResultID identificator, PressureSensorConfig configData, DPI620GeniiConfig dpiConf,
            IEventAggregator agregator, ITamplateArchive<PressureSensorConfig> archive)
        {
            Data = configData;
            Identificator = identificator;
            Config = new CheckPressureLogicConfigVm(configData);
            DpiConfig = dpiConf;
            _agregator = agregator;
            _confArch = archive;
            _templates = new TemplateStore<PressureSensorConfig>(archive);
            _templates.LastData = configData;
            _templates.UpdatedTemplate += TemplatesOnUpdatedTemplate;
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

        public string SerialNumber
        {
            get { return Identificator.SerialNumber; }
            set
            {
                Identificator.SerialNumber = value;
                Config.Data.SerialNumber = value;
                OnPropertyChanged("SerialNumber");
            }
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
        public DPI620GeniiConfig DpiConfig
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
    }
}
