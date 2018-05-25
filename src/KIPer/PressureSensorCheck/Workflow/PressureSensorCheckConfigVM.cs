﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ArchiveData.DTO;
using KipTM.Checks.ViewModel.Config;
using KipTM.EventAggregator;
using KipTM.Interfaces;
using KipTM.ViewModel.Events;
using PressureSensorData;
using Tools.View;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Конфигурация проверки
    /// </summary>
    public class PressureSensorCheckConfigVm : INotifyPropertyChanged
    {
        private ObservableCollection<BaseCheckData> _templates;
        private BaseCheckData _selectedTemplate;
        private bool _isTemplatePreview;
        private CheckPressureLogicConfigVm _config;
        private DPI620GeniiConfig _dpiConfig;
        private readonly IEventAggregator _agregator;

        public PressureSensorCheckConfigVm(TestResultID identificator, PressureSensorConfig configData, DPI620GeniiConfig dpiConf, IEventAggregator agregator)
        {
            Data = configData;
            Identificator = identificator;
            Config = new CheckPressureLogicConfigVm(configData);
            DpiConfig = dpiConf;
            _agregator = agregator;
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
        public PressureSensorConfig Data { get; }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public Units PressUnit {
            get { return Data.Unit; }
            set
            {
                if(value == Data.Unit)
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
            set { _config = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Конфигурация DPI620
        /// </summary>
        public DPI620GeniiConfig DpiConfig
        {
            get { return _dpiConfig; }
            set { _dpiConfig = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Набор доступных шаблонов
        /// </summary>
        public ObservableCollection<BaseCheckData> Templates { get { return _templates; } }

        /// <summary>
        /// Выбраный шаблон
        /// </summary>
        public BaseCheckData SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                _selectedTemplate = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Применить выбранный шаблон
        /// </summary>
        public ICommand ApplyTemplate
        {
            get { return new CommandWrapper(OnApplyTemplate); }
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

        private void OnApplyTemplate()
        {
            _agregator?.Post(new HelpMessageEventArg("Применен шаблон:"));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
