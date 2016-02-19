using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
{
    public class ADTSParser
    {
        #region CalibrationAbort()

        public string GetCommandCalibrationAbort()
        {
            const string cmd = "CAL:ABOR";
            return cmd;
        }
        #endregion

        #region CalibrationStart
        public string GetCommandCalibrationStart(CalibChannel channel)
        {
            const string cmdFrame = "CAL:CHEC:CHAN {0}";
            var cmd = string.Format(cmdFrame, channel == CalibChannel.PT ? "PT" : channel == CalibChannel.PS ? "PS" : "PSPT");
            return cmd;
        }
        #endregion

        #region CalibrationEnd "CAL:CHEC:ENDP"

        public string GetCommandCalibrationEnd()
        {
            const string cmd = "CAL:CHEC:ENDP";
            return cmd;
        }
        #endregion

        #region MainCalibrationAccept "CAL:MAIN:ACC <state>";
        public string GetCommandMainCalibrationAccept(bool accept)
        {
            const string cmdFrame = "CAL:MAIN:ACC {0}";
            var cmd = string.Format(cmdFrame, accept ? "Yes" : "No");
            return cmd;
        }
        #endregion


        #region SetAdjustCalibration "CAL:ADJ <channel>,<span>,<zero>,<res(0)>,...,<res(11)>"
        public string GetCommandSetAdjustCalibration(CalibChannel channel, double span, double zero, int[] res)
        {
            const string cmdFrame = "CAL:ADJ {0},{1},{2}";
            var cmd = string.Format(cmdFrame, channel == CalibChannel.PT ? "PT" : "PS", span.ToString("##.##"), zero.ToString("###.##"));
            for (int i = 0; i < 12; i++)
            {
                cmd = cmd + string.Format(",{0}", res[i]);
            }
            return cmd;
        }
        #endregion

        #region GetAdjustCalibration "CAL:ADJ? <channel>";
        public string GetCommandGetAdjustCalibration(CalibChannel channel)
        {
            const string cmdFrame = "CAL:ADJ? {0}";
            var cmd = string.Format(cmdFrame, channel == CalibChannel.PT ? "PT" : "PS");
            return cmd;
        }

        public bool ParseGetAdjustCalibration(string message, out double? span, out double? zero, out int[] res)
        {
            span = null;
            zero = null;
            res = null;
            if(string.IsNullOrEmpty(message))
                return false;
            var parts = message.Split(new[] {','});
            if(parts.Length!=14)
                return false;
            
            double spanVal;
            if (!double.TryParse(parts[0], out spanVal))
                return false;
            span = spanVal;

            double zeroVal;
            if (!double.TryParse(parts[1], out zeroVal))
                return false;
            zero = zeroVal;

            res = new int[12];
            for (int i = 0; i < 12; i++)
            {
                int pointVal;
                if(!int.TryParse(parts[2+i], out pointVal))
                    return false;
                res[i] = pointVal;
            }

            return true;
        }

        #endregion

        #region GetState
        public string GetCommandGetState()
        {
            const string cmd = "SOUR:STAT?";
            return cmd;
        }

        public bool ParseGetState(string message, out State? state)
        {
            state = null;
            switch (message)
            {
                case "ON":
                    state = State.Control;
                    return true;
                case "OFF": 
                    state = State.Measure;
                    return true;
                case "HOLD":
                    state = State.Hold;
                    return true;
            }
            return false;
        }
        #endregion

        #region SetState
        public string GetCommandSetState(State state)
        {
            const string cmdFrame = "SOUR:STAT {0}";
            switch (state)
            {
                case State.Control:
                    return string.Format(cmdFrame, "ON");
                case State.Measure:
                    return string.Format(cmdFrame, "OFF");
                case State.Hold:
                    return string.Format(cmdFrame, "HOLD");
            }
            return null;
        }
        #endregion


        private string CompilCommand(string frame, IDictionary<string, string> parameters)
        {
            string result;
            if((frame.Contains('<') || frame.Contains('>')) && (parameters == null || parameters.Count==0))
                throw new Exception(string.Format("Try compil colmmand by frame[\"{0}\"] with parameters and not set parameters", frame));
            var countStartNameChar = frame.Count(el=>el=='<');
            var countEndNameChar = frame.Count(el => el == '>');
            if (countStartNameChar < countEndNameChar)
                throw new Exception(string.Format("Frame[\"{0}\"] contains \'<\' simbols less then \'>\'", frame));
            if (countStartNameChar > countEndNameChar)
                throw new Exception(string.Format("Frame[\"{0}\"] contains \'<\' simbols more then \'>\'", frame));
            var parts = frame.Split(new[] {'<', '>'});
            result = frame;
            for (int i = 1; i < parts.Length; i++)
            {
                var key = parts[i];
                if(!parameters.ContainsKey(key))
                    throw new Exception(string.Format("Frame[\"{0}\"] contains not setted parameter:{1}", frame, key));
                result = result.Replace(string.Format("<{0}>", key), parameters[key]);
            }
            return result;
        }

        private IDictionary<string, object> ParseAnswer(string answer, Dictionary<string, PeremeterTypes> parameterTypes)
        {
            var result = new Dictionary<string, object>();
            var parts = answer.Split(new[] {','});
            if(parts.Length!=parameterTypes.Count)
                throw new Exception(string.Format("Count[{0}] parts answer[{1}] not equal count[{2}] parameters", parts.Length, answer, parameterTypes.Count));
            var keys = parameterTypes.Keys.ToList();
            for (int i = 0; i < parts.Length; i++)
            {
                var key = keys[i];
                var parameter = parts[i];
                switch (parameterTypes[key])
                {
                    case PeremeterTypes.Real:
                        float floatVal;
                        if(!float.TryParse(parameter, out floatVal))
                            throw new Exception(string.Format("Can not cast \"{0}\" to float", parameter));
                        result.Add(key, floatVal);
                        break;
                    case PeremeterTypes.Integer:
                        int intVal;
                        if (!int.TryParse(parameter, out intVal))
                            throw new Exception(string.Format("Can not cast \"{0}\" to int", parameter));
                        result.Add(key, intVal);
                        break;
                    case PeremeterTypes.Boolean:
                        bool boolVal;
                        if (parameter == "ON" || parameter == "1")
                            boolVal = true;
                        else if (parameter == "OFF" || parameter == "0")
                            boolVal = false;
                        else
                            throw new Exception(string.Format("Can not cast \"{0}\" to bool", parameter));
                        result.Add(key, boolVal);
                        break;
                    case PeremeterTypes.String:
                        result.Add(key, parameter);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return result;
        }
    }
}
