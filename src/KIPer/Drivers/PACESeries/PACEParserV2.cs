using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using static PACESeries.PACEParserV2;

namespace PACESeries
{
    /// <summary>
    /// Транслятор протокола PACE
    /// </summary>
    public class PACEParserV2
    {
        /*
         * :CALibration - calibration commands.
         * :DIAGnostic - instrument generated condition data.
         * INPut - switch input of the control module.
         * :INSTrument - instrument specific commands.
         * :OUTPut - controls the output pressure and logical outputs.
         * :SENSe - directs the instrument to measure selected parameters.
         * :SOURce - the commands that control the pressure outputs.
         * :STATus - instrument state.
         * :SYSTem - errors and SCPI version.
         * :UNIT - sets the units for the instrument.
         * Common SCPI commands - three letter commands, prefixed by *.
         * Instrument control commands - three letter commands, prefixed by :.
         */

        #region Keys
        public enum Codes
        {
            [CodeDescriptorAttribue("ABOR", "Abort")]
            ABOR,
            [CodeDescriptorAttribue("LIM", "Limit")]
            LIM,
            [CodeDescriptorAttribue("ADDR", "Address")]
            ADDR,
            [CodeDescriptorAttribue("LLO", "Local lock out")]
            LLO,
            [CodeDescriptorAttribue("AVER", "Average")]
            AVER,
            [CodeDescriptorAttribue("LOG", "Logical")]
            LOG,
            [CodeDescriptorAttribue("ALT", "Altitude")]
            ALT,
            [CodeDescriptorAttribue("LPAS", "Low pass (filter)")]
            LPAS,
            [CodeDescriptorAttribue("AMPL", "Amplitude")]
            AMPL,
            [CodeDescriptorAttribue("MAV", "Message available in output queue")]
            MAV,
            [CodeDescriptorAttribue("ATOD", "Analog to Digital")]
            ATOD,
            [CodeDescriptorAttribue("MEAS", "Measure")]
            MEAS,
            [CodeDescriptorAttribue("BAR", "Barometer")]
            BAR,
            [CodeDescriptorAttribue("MSS", "Summary bit after SRQ")]
            MSS,
            [CodeDescriptorAttribue("BRID", "Bridge")]
            BRID,
            [CodeDescriptorAttribue("NEGC", "Negative Calibration")]
            NEGC,
            [CodeDescriptorAttribue("CAL", "Calibration")]
            CAL,
            [CodeDescriptorAttribue("OFFS", "Offset")]
            OFFS,
            [CodeDescriptorAttribue("CAT", "Catalogue")]
            CAT,
            [CodeDescriptorAttribue("OPC", "Operational condition")]
            OPC,
            [CodeDescriptorAttribue("CDIS", "Cdisable (calibration disable)")]
            CDIS,
            [CodeDescriptorAttribue("OPER", "Operation")]
            OPER,
            [CodeDescriptorAttribue("CEN", "Cenable (calibration enable)")]
            CEN,
            [CodeDescriptorAttribue("OPT", "Option")]
            OPT,
            [CodeDescriptorAttribue("CLS", "Clear")]
            CLS,
            [CodeDescriptorAttribue("OSB", "Summary bit from standard operations status register")]
            OSB,
            [CodeDescriptorAttribue("COMM", "Communicate")]
            COMM,
            [CodeDescriptorAttribue("OVER", "Overshoot")]
            OVER,
            [CodeDescriptorAttribue("COMP", "Compensate")]
            COMP,
            [CodeDescriptorAttribue("OUTP", "Output")]
            OUTP,
            [CodeDescriptorAttribue("COND", "Condition")]
            COND,
            [CodeDescriptorAttribue("PAR", "Parity")]
            PAR,
            [CodeDescriptorAttribue("CONF", "Configuration")]
            CONF,
            [CodeDescriptorAttribue("PASS", "Password")]
            PASS,
            [CodeDescriptorAttribue("CONT", "Controller")]
            CONT,
            [CodeDescriptorAttribue("PERC", "Percent")]
            PERC,
            [CodeDescriptorAttribue("CONV", "Convert")]
            CONV,
            [CodeDescriptorAttribue("POIN", "Points")]
            POIN,
            [CodeDescriptorAttribue("CORR", "Correction")]
            CORR,
            [CodeDescriptorAttribue("PRES", "Preset")]
            PRESet,
            [CodeDescriptorAttribue("COUN", "Count")]
            COUN,
            [CodeDescriptorAttribue("PRES", "Pressure")]
            PRESsure,
            [CodeDescriptorAttribue("DEF", "Define")]
            DEF,
            [CodeDescriptorAttribue("QUE", "Queue")]
            QUE,
            [CodeDescriptorAttribue("DIAG", "Diagnostic")]
            DIAG,
            [CodeDescriptorAttribue("QUES", "Questionable")]
            QUES,
            [CodeDescriptorAttribue("DIOD", "Diode")]
            DIOD,
            [CodeDescriptorAttribue("RANG", "Range")]
            RANG,
            [CodeDescriptorAttribue("DISP", "Display")]
            DISP,
            [CodeDescriptorAttribue("REF", "Reference")]
            REF,
            [CodeDescriptorAttribue("EAV", "Error in error queue")]
            EAV,
            [CodeDescriptorAttribue("RES", "Resolution")]
            RESol,
            [CodeDescriptorAttribue("EFF", "Effort")]
            EFF,
            [CodeDescriptorAttribue("RES", "RESet")]
            RESet,
            [CodeDescriptorAttribue("ENAB", "Enable")]
            ENAB,
            [CodeDescriptorAttribue("SAMP", "Sample")]
            SAMP,
            [CodeDescriptorAttribue("EOI", "End of input")]
            EOI,
            [CodeDescriptorAttribue("SENS", "Sense")]
            SENS,
            [CodeDescriptorAttribue("ERR", "Error")]
            ERR,
            [CodeDescriptorAttribue("SEPT", "Set-point")]
            SEPT,
            [CodeDescriptorAttribue("ESB", "Summary bit from standard event")]
            ESB,
            [CodeDescriptorAttribue("SER", "Serial")]
            SER,
            [CodeDescriptorAttribue("ESE", "Event status enable")]
            ESE,
            [CodeDescriptorAttribue("SOUR", "Source")]
            SOUR,
            [CodeDescriptorAttribue("ESR", "Event status register")]
            ESR,
            [CodeDescriptorAttribue("SPE", "Speed")]
            SPE,
            [CodeDescriptorAttribue("EVEN", "Event")]
            EVEN,
            [CodeDescriptorAttribue("SRE", "Service request enable")]
            SRE,
            [CodeDescriptorAttribue("FILT", "Filter")]
            FILT,
            [CodeDescriptorAttribue("SRQ", "Service request")]
            SRQ,
            [CodeDescriptorAttribue("FREQ", "Frequency")]
            FREQ,
            [CodeDescriptorAttribue("STAR", "Start")]
            STAR,
            [CodeDescriptorAttribue("FULL", "Fullscale")]
            FULL,
            [CodeDescriptorAttribue("STB", "Status register query")]
            STB,
            [CodeDescriptorAttribue("GTL", "Go to local")]
            GTL,
            [CodeDescriptorAttribue("STAT", "State")]
            STAT,
            [CodeDescriptorAttribue("HEAD", "Head")]
            HEAD,
            [CodeDescriptorAttribue("SYST", "System")]
            SYST,
            [CodeDescriptorAttribue("IDN", "Identification")]
            IDN,
            [CodeDescriptorAttribue("TIM", "Time to set-point")]
            TIM,
            [CodeDescriptorAttribue("IMM", "Immediate")]
            IMM,
            [CodeDescriptorAttribue("UNIT", "Unit of pressure")]
            UNIT,
            [CodeDescriptorAttribue("INL", "In limit")]
            INL,
            [CodeDescriptorAttribue("VAL", "Value")]
            VAL,
            [CodeDescriptorAttribue("INP", "Input")]
            INP,
            [CodeDescriptorAttribue("VALV", "Valve")]
            VALV,
            [CodeDescriptorAttribue("INST", "Instrument")]
            INST,
            [CodeDescriptorAttribue("VERS", "Version")]
            VERS,
            [CodeDescriptorAttribue("ISOL", "Isolation")]
            ISOL,
            [CodeDescriptorAttribue("VOL", "Volume")]
            VOL,
            [CodeDescriptorAttribue("LEV", "Leve")]
            LEV,
        }
        #endregion
        
        public class CodeDescriptorAttribue : Attribute
        {

            public string Code { get; private set; }

            public string Note { get; private set; }

            public CodeDescriptorAttribue(string code, string note, string predicate = ":")
            {
                Code = predicate + code;
                Note = note;
            }
        }

        public class LexNode
        {
            private readonly StringBuilder _command = new StringBuilder();
            
            /// <summary>
            /// Добавить параметр
            /// </summary>
            /// <param name="option">параметр</param>
            /// <returns></returns>
            public LexNode Add(int option)
            {
                _command.Append(option.ToString());
                return this;
            }

            /// <summary>
            /// Добавить команду
            /// </summary>
            /// <param name="code">команда</param>
            /// <returns></returns>
            public LexNode Add(Codes code)
            {
                var attr = GetAtr<CodeDescriptorAttribue>(code);
                if(attr == null)
                    throw new NullReferenceException(string.Format("for {0} not found CodeDescriptorAttribue", code));
                _command.Append(attr.Code);
                return this;
            }

            /// <summary>
            /// Получить команду установку
            /// </summary>
            /// <param name="parameters"></param>
            /// <returns></returns>
            public string Set(params object[] parameters)
            {
                var isFirst = true;
                foreach (var parameter in parameters)
                {
                    if(isFirst)
                        _command.Append(" " + parameter.ToString());
                    else
                        _command.Append(" " + parameter.ToString());
                    isFirst = false;
                }
                return ToString();
            }

            /// <summary>
            /// Получить команд запрос
            /// </summary>
            /// <returns></returns>
            public string Get()
            {
                _command.Append("?");
                return ToString();
            }

            public override string ToString()
            {
                return _command.ToString();
            }

            /// <summary>
            /// Получить арибуты
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="code"></param>
            /// <returns></returns>
            private T GetAtr<T>(Codes code) where T : System.Attribute
            {
                var type = code.GetType();
                var memInfo = type.GetMember(code.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
                return (attributes.Length > 0) ? (T)attributes[0] : (T) null;
            }
        }

        #region Protocol parser
        /// <summary>
        /// Сбор команды
        /// </summary>
        /// <param name="frame">форма команды</param>
        /// <param name="parameters">набор параметров</param>
        /// <returns>команда</returns>
        private string CompilCommand(string frame, IDictionary<string, string> parameters)
        {
            string result = frame;
            if ((frame.Contains('<') || frame.Contains('>')) && (parameters == null || parameters.Count == 0))
                throw new Exception(string.Format("Try compil colmmand by frame[\"{0}\"] with parameters and not set parameters", frame));
            var countStartNameChar = frame.Count(el => el == '<');
            var countEndNameChar = frame.Count(el => el == '>');
            if (countStartNameChar < countEndNameChar)
                throw new Exception(string.Format("Frame[\"{0}\"] contains \'<\' simbols less then \'>\'", frame));
            if (countStartNameChar > countEndNameChar)
                throw new Exception(string.Format("Frame[\"{0}\"] contains \'<\' simbols more then \'>\'", frame));

            var parts = frame.Split(new[] { ' ', ',' });
            for (int i = 1; i < parts.Length; i++)
            {
                var key = parts[i];
                if (!parameters.ContainsKey(key))
                    throw new Exception(string.Format("Frame[\"{0}\"] contains not setted parameter:{1}", frame, key));
                result = result.Replace(string.Format("{0}", key), parameters[key]);
            }
            return result;
        }

        /// <summary>
        /// Разбор ответа
        /// </summary>
        /// <param name="answer">Ответ</param>
        /// <returns>Набор параметров</returns>
        private string[] ParseAnswer(string answer)
        {
            return answer.Trim().Split(' ')[1].Split(',');
        }

        /// <summary>
        /// Разбор ответа
        /// </summary>
        /// <param name="answer">Ответ</param>
        /// <param name="parameterTypes">Типы параметров</param>
        /// <returns>Набор разобранных параметров</returns>
        private IDictionary<string, object> ParseAnswer(string answer, Dictionary<string, PeremeterTypes> parameterTypes)
        {
            var result = new Dictionary<string, object>();
            var parts = ParseAnswer(answer);
            if (parts.Length != parameterTypes.Count)
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
                    case PeremeterTypes.PressureUnit:
                        if (parameter == "MBAR")
                            result.Add(key, PressureUnits.MBar);
                        else if (parameter == "BAR")
                            result.Add(key, PressureUnits.Bar);
                        else if (parameter == "INH2O4")
                            result.Add(key, PressureUnits.inH2O4);
                        else if (parameter == "INH2O")
                            result.Add(key, PressureUnits.inH2O);
                        else if (parameter == "INHG")
                            result.Add(key, PressureUnits.inHg);
                        else if (parameter == "MMHG")
                            result.Add(key, PressureUnits.mmHg);
                        else if (parameter == "PA")
                            result.Add(key, PressureUnits.Pa);
                        else if (parameter == "HPA")
                            result.Add(key, PressureUnits.hPa);
                        else if (parameter == "KPA")
                            result.Add(key, PressureUnits.kPa);
                        else if (parameter == "PSI")
                            result.Add(key, PressureUnits.psi);
                        else if (parameter == "INH2O60")
                            result.Add(key, PressureUnits.inH2O60F);
                        else if (parameter == "KG/CM2")
                            result.Add(key, PressureUnits.KgCm2);
                        else if (parameter == "ATM")
                            result.Add(key, PressureUnits.ATM);
                        else if (parameter == "MMH2O4")
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

        /// <summary>
        /// Единицы измерения в строку
        /// </summary>
        /// <param name="unit">Единицы измерения давления</param>
        /// <returns>строковое представление единицы измерения давления</returns>
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
                case PressureUnits.Bar:
                    unitstr = "BAR";
                    break;
                case PressureUnits.inH2O4:
                    unitstr = "INH2O4";
                    break;
                case PressureUnits.inH2O:
                    unitstr = "INH2O";
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
                case PressureUnits.kPa:
                    unitstr = "KPA";
                    break;
                case PressureUnits.psi:
                    unitstr = "PSI";
                    break;
                case PressureUnits.inH2O60F:
                    unitstr = "INH2O60";
                    break;
                case PressureUnits.KgCm2:
                    unitstr = "KG/CM2";
                    break;
                case PressureUnits.ATM:
                    unitstr = "ATM";
                    break;
                case PressureUnits.mmH2O4:
                    unitstr = "MMH2O4";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unit");
            }
            return unitstr;
        }
        #endregion

        #region helpers

        #endregion
    }
}
