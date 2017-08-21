using System;
using System.Threading;
using System.Threading.Tasks;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;
using PACEChecks.Devices;

namespace PACEChecks.Channels
{
    public class PACEEthalonChannel : IEthalonChannel
    {
        public static string Key = "PACEChannel";

        private readonly PACE1000Model _paseModel;

        private bool _isAutoupdate = false;
        private TimeSpan _autoupdatePeriod = TimeSpan.FromMilliseconds(500);
        private bool _isActive;
        private bool _isConnected;

        public PACEEthalonChannel(PACE1000Model paseModel)
        {
            _paseModel = paseModel;
            _paseModel.PressureChanged += _paseModel_PressureChanged;
            _paseModel.PressureUnitChanged += _paseModel_PressureUnitChanged;
        }

        #region Implementation IEthalonChannel
        /// <summary>
        /// Активация канала эталона
        /// </summary>
        /// <returns></returns>
        public bool Activate(ITransportChannelType transport)
        {
            IsActive = true;
            _isAutoupdate = true;
            _paseModel.Start(transport);
            return AutoUpdate();
        }

        /// <summary>
        /// Остановка канала эталона
        /// </summary>
        public void Stop()
        {
            _isAutoupdate = false;
            IsActive = false;
        }

        /// <summary>
        /// Получить эталонное значение
        /// </summary>
        /// <param name="point"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public double GetEthalonValue(double point, CancellationToken cancel)
        {
            var result = double.NaN;
            var wh = new ManualResetEvent(false);
            _paseModel.UpdatePressure(wh);
            while (!cancel.IsCancellationRequested)
            {
                if(wh.WaitOne(TimeSpan.FromMilliseconds(20)))
                    break;
            }
            if (!cancel.IsCancellationRequested)
                result = _paseModel.Pressure;
            OnStateUpdated();
            return result;
        }
        #endregion

        #region Публичные состояния
        /// <summary>
        /// Текущая величина давления
        /// </summary>
        public double Pressure{get { return _paseModel.Pressure; }}

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public string PressureUnit{get { return _paseModel.PressureUnit.ToString(); }}

        /// <summary>
        /// Активность канала
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if(value == _isActive)
                    return;
                _isActive = value;
                OnActiveStateChange();
            }
        }

        /// <summary>
        /// Наличие связи
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (value == _isConnected)
                    return;
                _isConnected = value;
                OnConnectedStateChange();
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Измеение активности канала
        /// </summary>
        public event EventHandler ActiveStateChange;

        protected virtual void OnActiveStateChange()
        {
            EventHandler handler = ActiveStateChange;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Изменение состояния наличия связи
        /// </summary>
        public event EventHandler ConnectedStateChange;

        protected virtual void OnConnectedStateChange()
        {
            EventHandler handler = ConnectedStateChange;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Состояние обновлено
        /// </summary>
        public event EventHandler StateUpdated;

        protected virtual void OnStateUpdated()
        {
            EventHandler handler = StateUpdated;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        #endregion

        #region Service

        /// <summary>
        /// Автообновление
        /// </summary>
        /// <param name="wh"></param>
        /// <returns></returns>
        private bool AutoUpdate(EventWaitHandle wh = null)
        {
            if (!_isAutoupdate)
                return true;
            IsConnected = UpdateState();
            Task.Run(() =>
            {
                Thread.Sleep(_autoupdatePeriod);
                AutoUpdate();
            });
            return IsConnected;
        }

        private bool UpdateState()
        {
            
            //todo: make eroor reaction from model
            var result = _paseModel.UpdatePressure();
            result = result && _paseModel.UpdateUnit();
            return result;
        }

        void _paseModel_PressureUnitChanged(object sender, EventArgs e)
        {
            OnStateUpdated();
        }

        void _paseModel_PressureChanged(object sender, EventArgs e)
        {
            OnStateUpdated();
        }
        #endregion
    }
}
