using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ADTS
{
    public class ADTSParser
    {
        internal const string KeyGoToGround = "SOUR:GTGR";
        internal const string KeyGetIsGround = "SOUR:GTGR?";
        internal const string KeyGetDate = "SYST:DATE?";
        internal const string KeySetState = "SOUR:STAT <state>";
        internal const string KeyGetState = "SOUR:STAT?";
        internal const string KeyStartMainCalibration = "CAL:MAIN:CHAN <channel>";
        internal const string KeySetValueActualPressure = "CAL:MAIN:VAL <value>";
        internal const string KeyGetCalibrationResult = "CAL:MAIN:RES?";
        internal const string KeySetMainCalibrationAccept = "CAL:MAIN:ACC <state>";
        internal const string KeySetUnitPressure = "UNIT:PRES <units>";
        internal const string KeyGetUnitPressure = "UNIT:PRES?";
        internal const string KeySetRate = "SOUR:RATE <parameter>,<aim>";
        internal const string KeyGetRate = "SOUR:RATE? <parameter>";
        internal const string KeySetAim = "SOUR:PRES <parameter>,<aim>";
        internal const string KeyGetAim = "SOUR:PRES? <parameter>";
        internal const string KeyGetStatusOfADTS = "STAT:OPER:CON?";
        internal const string KeyGetPressure = "MEAS:PRES? <parameter>";

        #region GetSystemDate "SYST:DATE?"

        public string GetCommandGetSystemDate()
        {
            return KeyGetDate;
        }

        public bool ParseGetSystemDate(string message, out DateTime? date)
        {
            date = null;
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
            {
                {"year", PeremeterTypes.Integer},
                {"month", PeremeterTypes.Integer},
                {"day", PeremeterTypes.Integer}
            });
            date = new DateTime((int)answer["year"], (int)answer["month"], (int)answer["day"]);
            return true;
        }
        #endregion

        #region GoToGround "SOUR:GTGR"

        public string GetCommandGoToGround()
        {
            return KeyGoToGround;
        }
        #endregion

        #region GetIsGround "SOUR:GTGR?"

        public string GetCommandIsGround()
        {
            return KeyGetIsGround;
        }

        public bool ParseGetIsGround(string message, out bool? isGround)
        {
            isGround = null;
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
            {
                {"isGround", PeremeterTypes.Boolean}
            });
            isGround = (bool)answer["isGround"];
            return true;
        }
        #endregion

        #region CalibrationAbort "CAL:ABOR"

        public string GetCommandCalibrationAbort()
        {
            const string cmd = "CAL:ABOR";
            return cmd;
        }
        #endregion

        #region CalibrationStart "CAL:MAIN:CHAN <channel>"
        public string GetCommandCalibrationStart(CalibChannel channel)
        {
            return CompilCommand(KeyStartMainCalibration,
                new Dictionary<string, string>() {{"<channel>", CalibChannelToString(channel)}});
        }
        #endregion

        #region CalibrationEnd "CAL:CHEC:ENDP"

        public string GetCommandCalibrationEnd()
        {
            const string cmd = "CAL:CHEC:ENDP";
            return cmd;
        }
        #endregion

        #region CalibrationSetValue "CAL:MAIN:VAL <value>";
        public string GetCommandCalibrationSetValue(double value)
        {
            return CompilCommand(KeySetValueActualPressure,
                new Dictionary<string, string>() {{"<value>", value.ToString()}});
        }
        #endregion

        #region MainCalibrationAccept "CAL:MAIN:ACC <state>";
        public string GetCommandMainCalibrationAccept(bool accept)
        {
            return CompilCommand(KeySetMainCalibrationAccept,
                new Dictionary<string, string>() { { "<state>", accept ? "Yes" : "No" } });
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

        #region GetCalibrationResult "CAL:MAIN:RES?"

        public string GetCommandGetCalibrationResult()
        {
            return KeyGetCalibrationResult;
        }

        public bool ParseKeyGetCalibrationResult(string message, out double? slope, out double? zero)
        {
            slope = null;
            zero = null;
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
            {
                {"slope", PeremeterTypes.Real},
                {"zero", PeremeterTypes.Real}
            });
            slope = (double)answer["slope"];
            zero = (double)answer["zero"];
            return true;
        }
        #endregion

        #region GetState "SOUR:STAT?"
        public string GetCommandGetState()
        {
            const string cmd = "SOUR:STAT?";
            return cmd;
        }

        public bool ParseGetState(string message, out State? state)
        {
            state = null;
            switch (message.Trim())
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

        #region SetState "SOUR:STAT <state>"
        public string GetCommandSetState(State state)
        {
            return CompilCommand(KeySetState,
                new Dictionary<string, string>() { { "<state>", StateToString(state) } });
        }
        #endregion

        #region SetParameterRate "SOUR:RATE <parameter>,<aim>"

        public string GetCommandSetParameterRate(Parameters param, double value)
        {
            return CompilCommand(KeySetRate, new Dictionary<string, string>()
            {
                {"<parameter>", PressureTypeToString(param)},
                {"<aim>", value.ToString()}
            });
        }
        #endregion

        #region SetParameterAim "SOUR:PRES <parameter>,<aim>"

        public string GetCommandSetParameterAim(Parameters param, double value)
        {
            if (param == Parameters.None)
                throw new Exception(string.Format("can not set aim Parameters.None"));
            return CompilCommand(KeySetAim, new Dictionary<string, string>()
            {
                {"<parameter>", PressureTypeToString(param)},
                {"<aim>", value.ToString()}
            });
        }
        #endregion

        #region SetPressureUnit "UNIT:PRES <units>"

        public string GetCommandSetPressureUnit(PressureUnits unit)
        {
            string unitstr = null;
            if (unit == PressureUnits.None)
                    throw new Exception(string.Format("can not set pressure unit PressureUnits.None"));
            unitstr = PressureUnitToString(unit);
            return CompilCommand(KeySetUnitPressure, new Dictionary<string, string>() {{"<units>", unitstr}});
        }
        #endregion

        #region SetPressureUnit "UNIT:PRES?"

        public string GetCommandGetPressureUnit()
        {
            return KeyGetUnitPressure;
        }

        public bool ParseGetPressureUnit(string message, out PressureUnits? unit)
        {
            unit = null;
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "unit", PeremeterTypes.PressureUnit } });
            unit = (PressureUnits)answer["unit"];
            return true;
        }
        #endregion

        #region GetPressure "MEAS:PRES? <parameter>"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string GetCommandMeasurePress(Parameters param)
        {
            if (param == Parameters.None)
                throw new Exception(string.Format("can not get pressure Parameters.None"));
            return CompilCommand(KeyGetPressure, new Dictionary<string, string>()
            {{"<parameter>", PressureTypeToString(param)},});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ParseMeasurePress(string message, out double? value)
        {
            value = null;
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "value", PeremeterTypes.Real } });
            value = (double)answer["value"];
            return true;
        }
        #endregion

        #region GetStatus "STAT:OPER:CON?"

        public string GetCommandGetStatus()
        {
            return KeyGetStatusOfADTS;
        }

        public bool ParseGetStatus(string message, out Status? status)
        {
            status = null;
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>() {{"status", PeremeterTypes.Integer}});
            status = (Status)(int)answer["status"];
            return true;
        }

        #endregion

        #region Protocol parser
        private string CompilCommand(string frame, IDictionary<string, string> parameters)
        {
            string result= frame;
            if((frame.Contains('<') || frame.Contains('>')) && (parameters == null || parameters.Count==0))
                throw new Exception(string.Format("Try compil colmmand by frame[\"{0}\"] with parameters and not set parameters", frame));
            var countStartNameChar = frame.Count(el=>el=='<');
            var countEndNameChar = frame.Count(el => el == '>');
            if (countStartNameChar < countEndNameChar)
                throw new Exception(string.Format("Frame[\"{0}\"] contains \'<\' simbols less then \'>\'", frame));
            if (countStartNameChar > countEndNameChar)
                throw new Exception(string.Format("Frame[\"{0}\"] contains \'<\' simbols more then \'>\'", frame));
            
            var parts = frame.Split(new[] {' ', ','});
            for (int i = 1; i < parts.Length; i++)
            {
                var key = parts[i];
                if(!parameters.ContainsKey(key))
                    throw new Exception(string.Format("Frame[\"{0}\"] contains not setted parameter:{1}", frame, key));
                result = result.Replace(string.Format("{0}", key), parameters[key]);
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
                parameter = parameter.Trim();
                switch (parameterTypes[key])
                {
                    case PeremeterTypes.Real:
                        double doubleVal;
                        if (!double.TryParse(parameter, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleVal))
                            throw new Exception(string.Format("Can not cast \"{0}\" to double", parameter));
                        result.Add(key, doubleVal);
                        break;
                    case PeremeterTypes.Integer:
                        int intVal;
                        if (!int.TryParse(parameter, NumberStyles.Any, CultureInfo.InvariantCulture, out intVal))
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
                    case PeremeterTypes.State:
                        if(parameter == "ON")
                            result.Add(key, State.Control);
                        else if(parameter == "OFF")
                            result.Add(key, State.Measure);
                        else if(parameter == "HOLD")
                            result.Add(key, State.Hold);
                        else
                            throw new Exception(string.Format("Can not cast \"{0}\" to State", parameter));
                        break;
                    case PeremeterTypes.PressureUnit:
                        if (parameter == "MBAR")
                            result.Add(key, PressureUnits.MBar);
                        else if(parameter == "INH2O4")
                            result.Add(key, PressureUnits.inH2O4);
                        else if(parameter == "INH2O20")
                            result.Add(key, PressureUnits.inH2O20);
                        else if(parameter == "INHG")
                            result.Add(key, PressureUnits.inHg);
                        else if(parameter == "MMHG")
                            result.Add(key, PressureUnits.mmHg);
                        else if(parameter == "PA")
                            result.Add(key, PressureUnits.Pa);
                        else if(parameter == "HPA")
                            result.Add(key, PressureUnits.hPa);
                        else if(parameter == "PSI")
                            result.Add(key, PressureUnits.psi);
                        else if(parameter == "INH2O60F")
                            result.Add(key, PressureUnits.inH2O60F);
                        else if(parameter == "KGCM2")
                            result.Add(key, PressureUnits.KgCm2);
                        else if(parameter == "%FS")
                            result.Add(key, PressureUnits.FS);
                        else if(parameter == "MMH2O4")
                            result.Add(key, PressureUnits.mmH2O4);
                        else
                            throw new Exception(string.Format("Can not cast \"{0}\" to PressureUnit", parameter));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return result;
        }

        private string PressureUnitToString(PressureUnits unit)
        {
            string unitstr = null;
            switch (unit)
            {
                case PressureUnits.None:
                    throw new Exception(string.Format("can not set pressure unit PressureUnits.None"));
                case PressureUnits.MBar:
                    unitstr = "MBAR";
                    break;
                case PressureUnits.inH2O4:
                    unitstr = "INH2O4";
                    break;
                case PressureUnits.inH2O20:
                    unitstr = "INH2O20";
                    break;
                case PressureUnits.inHg:
                    unitstr = "INHG";
                    break;
                case PressureUnits.mmHg:
                    unitstr = "MMHG";
                    break;
                case PressureUnits.Pa:
                    unitstr = "PA";
                    break;
                case PressureUnits.hPa:
                    unitstr = "HPA";
                    break;
                case PressureUnits.psi:
                    unitstr = "PSI";
                    break;
                case PressureUnits.inH2O60F:
                    unitstr = "INH2O60F";
                    break;
                case PressureUnits.KgCm2:
                    unitstr = "KGCM2";
                    break;
                case PressureUnits.FS:
                    unitstr = "%FS";
                    break;
                case PressureUnits.mmH2O4:
                    unitstr = "MMH2O4";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unit");
            }
            return unitstr;
        }

        private string PressureTypeToString(Parameters param)
        {
            switch (param)
            {
                case Parameters.ALT:
                    return "ALT";
                case Parameters.CAS:
                    return "CAS";
                case Parameters.TAS:
                    return "TAS";
                case Parameters.MACH:
                    return "MACH";
                case Parameters.EPR:
                    return "EPR";
                case Parameters.PS:
                    return "PS";
                case Parameters.PT:
                    return "PT";
                case Parameters.QC:
                    return "QC";
                default:
                    throw new ArgumentOutOfRangeException("param");
            }
        }

        private string CalibChannelToString(CalibChannel channel)
        {
            switch (channel)
            {
                case CalibChannel.PT:
                    return "PT";
                case CalibChannel.PS:
                    return "PS";
                case CalibChannel.PTPS:
                    return "PSPT";
                default:
                    throw new ArgumentOutOfRangeException("channel");
            }
        }

        private string StateToString(State state)
        {
            switch (state)
            {
                case State.Control:
                    return "ON";
                case State.Measure:
                    return "OFF";
                case State.Hold:
                    return "HOLD";
                default:
                    throw new ArgumentOutOfRangeException("state");
            }
        }
        #endregion
    }
}
