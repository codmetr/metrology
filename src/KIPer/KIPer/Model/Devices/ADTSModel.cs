using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using IEEE488;
using MainLoop;

namespace KipTM.Model.Devices
{
    public class ADTSModel
    {
        private ADTSDriver _adts;
        private ILoops _loops;
        private string _loopKey;
        private bool _isNeedAutoupdate;


        private DateTime? _pressureTime;
        private double? _pressure;

        private DateTime? _pitotTime;
        private double? _pitot;

        private DateTime? _pressureUnitTime;
        private PressureUnits? _pressureUnit;

        private DateTime? _stateTime;
        private State? _state;

        public ADTSModel(string title, ILoops loops, string loopKey, ADTSDriver driver)
        {
            Title = title;
            _loopKey = loopKey;
            _adts = driver;
        }

        public string Title
        {
            get;
            private set;
        }

        internal static string Key { get { return "ADTS"; } }

        public void Init()
        {
            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, UpdateUnit);
        }
        
        /// <summary>
        /// Запуск автоопроса модуля дискретных входов
        /// </summary>
        public void StartAutoUpdate()
        {
            _isNeedAutoupdate = true;
            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, AutoUpdateState);
            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, AutoUpdatePressure);
            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, AutoUpdatePitot);
        }
        
        /// <summary>
        /// Остановка автоопроса модуля дискретных входов
        /// </summary>
        public void StopAutoUpdate()
        {
            _isNeedAutoupdate = false;
        }

        public DateTime? StartCalibration(CalibChannel channel)
        {
            var isCommpete = new AutoResetEvent(false);
            DateTime? date = null;
            _loops.StartUnimportantAction(_loopKey, (transport) =>
            {
                var ieee488 = transport as ITransportIEEE488;
                if (ieee488 == null)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.GetDate(ieee488, out date))
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.GetDate(ieee488, out date))
                {
                    isCommpete.Set();
                    return;
                }

                isCommpete.Set();
            });
            isCommpete.WaitOne();
            return date;
        }

        #region Service members

        void AutoUpdateState(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if (ieee488 == null)
                return;

            if (!_isNeedAutoupdate)
                return;
            if (!_adts.GetState(ieee488, out _state))
                return;
            _stateTime = DateTime.Now;
            if (!_isNeedAutoupdate)
                return;

            _loops.StartUnimportantAction(_loopKey, AutoUpdateState);
        }

        void AutoUpdatePressure(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if (ieee488 == null)
                return;

            if (!_isNeedAutoupdate)
                return;
            if (!_adts.ReadMeasure(ieee488, Parameters.PS, out _pressure))
                return;
            _pressureTime = DateTime.Now;
            if (!_isNeedAutoupdate)
                return;

            _loops.StartUnimportantAction(_loopKey, AutoUpdatePressure);
        }

        void AutoUpdatePitot(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if (ieee488 == null)
                return;

            if (!_isNeedAutoupdate)
                return;
            if (!_adts.ReadMeasure(ieee488, Parameters.PT, out _pitot))
                return;
            _pitotTime = DateTime.Now;
            if (!_isNeedAutoupdate)
                return;

            _loops.StartUnimportantAction(_loopKey, AutoUpdatePitot);
        }

        void UpdateUnit(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if (ieee488 == null)
                return;

            if (!_adts.GetUnits(ieee488, out _pressureUnit))
                return;
            _pressureUnitTime = DateTime.Now;
        }
        #endregion


    }
}
