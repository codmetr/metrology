﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PACESeriesUtil.VM;

namespace PACESeriesUtil
{
    public class PaceViewModel : WpfContext, INotifyPropertyChanged
    {
        private readonly PaceConfigViewModel _config;
        private readonly SettingsViewModel _settings;
        private readonly TraceViewModel _trace;
        private readonly PaceMeasuringViewModel _measuring;
        private readonly PaceControlViewModel _controlState;

        public PaceViewModel():base()
        {
            _config = new PaceConfigViewModel();
            _settings = new SettingsViewModel();
            _measuring = new PaceMeasuringViewModel();
            _controlState = new PaceControlViewModel();
            _trace = new TraceViewModel();
        }

        /// <summary>
        /// Конфигурация
        /// </summary>
        public PaceConfigViewModel Config
        {
            get { return _config; }
        }

        /// <summary>
        /// Настройки
        /// </summary>
        public SettingsViewModel Settings
        {
            get { return _settings; }
            //set
            //{
            //    _settings = value;
            //    OnPropertyChanged();
            //}
        }

        /// <summary>
        /// Измерение
        /// </summary>
        public PaceMeasuringViewModel MeasuringState
        {
            get { return _measuring; }
            //set
            //{
            //    _measuring = value; 
            //    OnPropertyChanged();
            //}
        }

        /// <summary>
        /// Контроль
        /// </summary>
        public PaceControlViewModel ControlState
        {
            get { return _controlState; }
            //set
            //{
            //    _controlState = value; 
            //    OnPropertyChanged();
            //}
        }

        /// <summary>
        /// Лог обмена
        /// </summary>
        public TraceViewModel Trace
        {
            get { return _trace; }
            //set
            //{
            //    _trace = value; 
            //    OnPropertyChanged();
            //}
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
