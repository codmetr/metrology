using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTS;
using IEEE488;
using MainLoop;
using NLog;
using PACESeries;

namespace KipTM.Model
{
    public class DeviceManager
    {
        private readonly NLog.Logger _logger;

        private readonly ILoops _loops = new Loops();

        private readonly ADTS.ADTSDriver _adts;
        private readonly PACEDriver _pace;

        private bool _isNeedAutoupdate;
        private readonly IDictionary<string, Tuple<ITransportIEEE488, SerialPort>> _ports = new Dictionary<string, Tuple<ITransportIEEE488, SerialPort>>();


        public DeviceManager(ADTSDriver adts, PACEDriver pace, Logger logger = null)
        {
            _adts = adts;
            _pace = pace;
            _
            _logger = logger;
        }

        public void Init()
        {
            _adts
        }

        #region IDeviceManager

        /// <summary>
        /// Запуск автоопроса модуля дискретных входов
        /// </summary>
        public void StartAutoUpdate()
        {
            _isNeedAutoupdate = true;
            if (IsPlateAvailable(StrConst.Relay2))
                _loops.StartUnimportantAction(_devicePorts[StrConst.Relay2].Name,
                    mb => _relay.HideException(r => r.UpdateAllStateRelay(mb as IModbusMaster)));
            if (IsPlateAvailable(StrConst.DiscretInput4) || IsPlateAvailable(StrConst.DiscretInput6))
                _loops.StartUnimportantAction(_devicePorts[StrConst.DiscretInput4].Name, AutoUpdateInputs);
        }

        /// <summary>
        /// Остановка автоопроса модуля дискретных входов
        /// </summary>
        public void StopAutoUpdate()
        {
            _isNeedAutoupdate = false;
        }

        /// <summary>
        /// Получить последнее состояние заданного дискретного входа
        /// </summary>
        /// <param name="index">индекс дискретного входа</param>
        /// <returns>значение</returns>
        public bool GetLastStateDi(int index)
        {
            if (index <= _discretInputs4.GetCountInputs())
                return _discretInputs4.GetLastInputState(index);
            return _discretInputs6.GetLastInputState(index - _discretInputs4.GetCountInputs());
        }

        /// <summary>
        /// Установить новое состояние реле
        /// </summary>
        /// <param name="index">индекс реле</param>
        /// <param name="value">состояние</param>
        public void SetRelayState(int index, bool value)
        {
            if (IsPlateAvailable(StrConst.Relay2))
                _loops.StartMiddleAction(_devicePorts[StrConst.Relay2].Name, (mb) => _relay.HideException(r => r.SetRelayState(mb as IModbusMaster, index, value)));
        }

        /// <summary>
        /// Получить последнее состояние реле
        /// </summary>
        /// <param name="index">индекс реле</param>
        /// <returns>состояние</returns>
        public bool GetLastStateRelay(int index)
        {
            return _relay.GetLastRelayState(index);
        }

        /// <summary>
        /// Возвращает состояние доступности к работе плат по имени
        /// </summary>
        /// <param name="namedevice">Имя интересующей платы</param>
        /// <returns></returns>
        public bool IsPlateAvailable(string namedevice)
        {
            if (!_devicePorts.ContainsKey(namedevice))
                return false;
            return _ports.ContainsKey(_devicePorts[namedevice].Name);
        }

        #region Events
        /// <summary>
        /// Событие о том что состояние дискретных входов изменилось (аргумент - сприсок изменившихся дискретных входов)
        /// </summary>
        public event Action<IEnumerable<int>> InputStateChanged;
        #endregion

        #endregion

        #region implementation IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposeAll">
        /// false - should only clean up native resources
        /// true - should clean up both managed and native resources
        /// </param>
        protected virtual void Dispose(bool disposeAll)
        {
            _loops.Dispose();
            foreach (var port in _ports.Values)
            {
                port.Item1.Dispose();
                if (port.Item2.IsOpen)
                    port.Item2.Close();
                port.Item2.Dispose();
            }
            if (!disposeAll)
                return;
        }
        #endregion

        #region Service members
        #region Event Invocators
        //protected virtual void OnDoorStateChanged(DoorStateEventArg e)
        //{
        //    EventHandler<DoorStateEventArg> handler = DoorStateChanged;
        //    if (handler != null) handler(this, e);
        //}

        protected virtual void OnInputStateChanged(IEnumerable<int> obj)
        {
            Action<IEnumerable<int>> handler = InputStateChanged;
            if (handler != null) handler(obj);
        }
        #endregion

        void AutoUpdateInputs(object mb)
        {
            var result = new List<int>();
            try
            {
                var result_2 = _discretInputs4.UpdateAllInputState(mb as IModbusMaster);
                if (result_2 != null && result_2.Any())
                    result.AddRange(result_2);
            }
            catch (Exception ex)
            {
                _logger.With(l => l.Error(string.Format("Error on UpdateAllInputState(2): {0}", ex)));
            }
            try
            {
                var result_4 = _discretInputs6.UpdateAllInputState(mb as IModbusMaster);
                if (result_4 != null && result_4.Any())
                {
                    result.AddRange(result_4.Select(i => i + _discretInputs4.GetCountInputs()));
                }
            }
            catch (Exception ex)
            {
                _logger.With(l => l.Error(string.Format("Error on UpdateAllInputState(4): {0}", ex)));
            }
            if (result != null && result.Any())
                OnInputStateChanged(result);
            if (_isNeedAutoupdate && (IsPlateAvailable(StrConst.DiscretInput6) || IsPlateAvailable(StrConst.DiscretInput6)))
                _loops.StartUnimportantAction(_devicePorts[StrConst.DiscretInput6].Name, AutoUpdateInputs);
        }

        #endregion
    }
}
