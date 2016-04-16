using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
{
    public class ADTSDriverByCommonChannel
    {
        private readonly int _address;
        private readonly ADTSParser _parser;

        public ADTSDriverByCommonChannel(int address)
        {
            _parser = new ADTSParser();
            _address = address;
        }

        /// <summary>
        /// Abort calibration
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        public bool CalibrationAbort(IEEE488.ITransportIEEE488 transport)
        {
            var cmd = _parser.GetCommandCalibrationAbort();
            return transport.Send(_address, cmd);
        }

        /// <summary>
        /// Start main calibration
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool StartCalibration(IEEE488.ITransportIEEE488 transport, CalibChannel channel)
        {
            var cmd = _parser.GetCommandCalibrationStart(channel);
            return transport.Send(_address, cmd);
        }

        /// <summary>
        /// Get system date
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool GetDate(IEEE488.ITransportIEEE488 transport, out DateTime? date)
        {
            date = null;
            var cmd = _parser.GetCommandGetSystemDate();
            if(!transport.Send(_address, cmd))
                return false;
            var answer = transport.Receive(_address);
            return _parser.ParseGetSystemDate(answer, out date);
        }

        /// <summary>
        /// Set state ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SetState(IEEE488.ITransportIEEE488 transport, State state)
        {
            var cmd = _parser.GetCommandSetState(state);
            return transport.Send(_address, cmd);
        }

        /// <summary>
        /// Get current state ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool GetState(IEEE488.ITransportIEEE488 transport, out State? state)
        {
            state = null;
            var cmd = _parser.GetCommandGetState();
            if (!transport.Send(_address, cmd))
                return false;
            var answer = transport.Receive(_address);
            return _parser.ParseGetState(answer, out state);
        }

        /// <summary>
        /// Set pressure units ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool SetUnits(IEEE488.ITransportIEEE488 transport, PressureUnits unit)
        {
            var cmd = _parser.GetCommandSetPressureUnit(unit);
            return transport.Send(_address, cmd);
        }

        /// <summary>
        /// Get pressure units ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool GetUnits(IEEE488.ITransportIEEE488 transport, out PressureUnits? unit)
        {
            unit = null;
            var cmd = _parser.GetCommandGetPressureUnit();
            if (!transport.Send(_address, cmd))
                return false;
            var answer = transport.Receive(_address);
            return _parser.ParseGetPressureUnit(answer, out unit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="param"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public bool SetRate(IEEE488.ITransportIEEE488 transport, Parameters param, double rate)
        {
            var cmd = _parser.GetCommandSetParameterRate(param, rate);
            return transport.Send(_address, cmd);
        }

        /// <summary>
        /// Set pressure aim
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="param"></param>
        /// <param name="aim"></param>
        /// <returns></returns>
        public bool SetAim(IEEE488.ITransportIEEE488 transport, Parameters param, double aim)
        {
            var cmd = _parser.GetCommandSetParameterAim(param, aim);
            return transport.Send(_address, cmd);
        }

        /// <summary>
        /// Read measure by parameter
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ReadMeasure(IEEE488.ITransportIEEE488 transport, Parameters param, out double? value)
        {
            value = null;
            var cmd = _parser.GetCommandMeasurePress(param);
            if (!transport.Send(_address, cmd))
                return false;
            var answer = transport.Receive(_address);
            return _parser.ParseMeasurePress(answer, out value);
        }

        /// <summary>
        /// Get status ADTS
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool GetStatus(IEEE488.ITransportIEEE488 transport, out Status? status)
        {
            status = null;
            var cmd = _parser.GetCommandGetStatus();
            if(!transport.Send(_address, cmd))
                return false;
            var answer = transport.Receive(_address);
            return _parser.ParseGetStatus(answer, out status);
        }

        /// <summary>
        /// Set actual calibration value
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetCalibrationValue(IEEE488.ITransportIEEE488 transport, double value)
        {
            var cmd = _parser.GetCommandCalibrationSetValue(value);
            return transport.Send(_address, cmd);
        }

        /// <summary>
        /// Get galibretion result
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="slope"></param>
        /// <param name="zero"></param>
        /// <returns></returns>
        public bool GetCamibrationResult(IEEE488.ITransportIEEE488 transport, out double? slope, out double? zero)
        {
            slope = null;
            zero = null;
            var cmd = _parser.GetCommandGetCalibrationResult();
            if (!transport.Send(_address, cmd))
                return false;
            var answer = transport.Receive(_address);
            return _parser.ParseKeyGetCalibrationResult(answer, out slope, out zero);
        }

        /// <summary>
        /// Accept or not calibration result
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="accept"></param>
        /// <returns></returns>
        public bool SetCalibrationAccept(IEEE488.ITransportIEEE488 transport, bool accept)
        {
            var cmd = _parser.GetCommandMainCalibrationAccept(accept);
            return transport.Send(_address, cmd);
        }
    }
}
