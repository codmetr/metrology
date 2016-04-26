using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
{
    public class ADTSDriver:IDisposable
    {
        private readonly int _address;
        private readonly ADTSParser _parser;
        private IEEE488.ITransportIEEE488 _transport;

        public ADTSDriver(int address, IEEE488.ITransportIEEE488 transport)
        {
            _parser = new ADTSParser();
            _address = address;
            _transport = transport;
        }

        public bool Open()
        {
            if (_transport == null)
                return false;
            return _transport.Open(_address);
        }

        /// <summary>
        /// Abort calibration
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        public bool CalibrationAbort()
        {
            var cmd = _parser.GetCommandCalibrationAbort();
            return _transport.Send(cmd);
        }

        /// <summary>
        /// Start main calibration
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool StartCalibration(CalibChannel channel)
        {
            var cmd = _parser.GetCommandCalibrationStart(channel);
            return _transport.Send(cmd);
        }

        /// <summary>
        /// Get system date
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool GetDate(out DateTime? date)
        {
            date = null;
            var cmd = _parser.GetCommandGetSystemDate();
            if (!_transport.Send(cmd))
                return false;
            var answer = _transport.Receive();
            return _parser.ParseGetSystemDate(answer, out date);
        }

        /// <summary>
        /// Go to ground ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        public bool GoToGround()
        {
            var cmd = _parser.GetCommandGoToGround();
            return _transport.Send(cmd);
        }

        /// <summary>
        /// Set state ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SetState(State state)
        {
            var cmd = _parser.GetCommandSetState(state);
            return _transport.Send(cmd);
        }

        /// <summary>
        /// Get current state ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool GetState(out State? state)
        {
            state = null;
            var cmd = _parser.GetCommandGetState();
            if (!_transport.Send(cmd))
                return false;
            var answer = _transport.Receive();
            return _parser.ParseGetState(answer, out state);
        }

        /// <summary>
        /// Set pressure units ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool SetUnits(PressureUnits unit)
        {
            var cmd = _parser.GetCommandSetPressureUnit(unit);
            return _transport.Send(cmd);
        }

        /// <summary>
        /// Get pressure units ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool GetUnits(out PressureUnits? unit)
        {
            unit = null;
            var cmd = _parser.GetCommandGetPressureUnit();
            if (!_transport.Send(cmd))
                return false;
            var answer = _transport.Receive();
            return _parser.ParseGetPressureUnit(answer, out unit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="param"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public bool SetRate(Parameters param, double rate)
        {
            var cmd = _parser.GetCommandSetParameterRate(param, rate);
            return _transport.Send(cmd);
        }

        /// <summary>
        /// Set pressure aim
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="param"></param>
        /// <param name="aim"></param>
        /// <returns></returns>
        public bool SetAim(Parameters param, double aim)
        {
            var cmd = _parser.GetCommandSetParameterAim(param, aim);
            return _transport.Send(cmd);
        }

        /// <summary>
        /// Read measure by parameter
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ReadMeasure(Parameters param, out double? value)
        {
            value = null;
            var cmd = _parser.GetCommandMeasurePress(param);
            if (!_transport.Send(cmd))
                return false;
            var answer = _transport.Receive();
            return _parser.ParseMeasurePress(answer, out value);
        }

        /// <summary>
        /// Get status ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool GetStatus(out Status? status)
        {
            status = null;
            var cmd = _parser.GetCommandGetStatus();
            if(!_transport.Send(cmd))
                return false;
            var answer = _transport.Receive();
            return _parser.ParseGetStatus(answer, out status);
        }

        /// <summary>
        /// Set actual calibration value
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetCalibrationValue(double value)
        {
            var cmd = _parser.GetCommandCalibrationSetValue(value);
            return _transport.Send(cmd);
        }

        /// <summary>
        /// Get galibretion result
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="slope"></param>
        /// <param name="zero"></param>
        /// <returns></returns>
        public bool GetCamibrationResult(out double? slope, out double? zero)
        {
            slope = null;
            zero = null;
            var cmd = _parser.GetCommandGetCalibrationResult();
            if (!_transport.Send(cmd))
                return false;
            var answer = _transport.Receive();
            return _parser.ParseKeyGetCalibrationResult(answer, out slope, out zero);
        }

        /// <summary>
        /// Accept or not calibration result
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        public bool SetCalibrationAccept(bool accept)
        {
            var cmd = _parser.GetCommandMainCalibrationAccept(accept);
            return _transport.Send(cmd);
        }

        public void Dispose()
        {
            if (_transport != null)
                _transport.Close(_address);
        }
    }
}
