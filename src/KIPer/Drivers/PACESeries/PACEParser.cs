using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PACESeries
{
    public class PACEParser
    {
        internal const string KeyGetGetIdentificator = "*IDN?";
        internal const string KeyGetGetPressure = ":INS:UNIT?";

        #region GetIdentificator "*IDN?"
        public string GetCommandGetIdentificator()
        {
            return KeyGetGetIdentificator;
        }

        public bool ParseGetIdentificator(string message, out string idn)
        {
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
            {
                {"Idn", PeremeterTypes.String}
            });
            idn = (string)answer["Idn"];
            return true;
        }
        #endregion

        #region Protocol parser
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

        private IDictionary<string, object> ParseAnswer(string answer, Dictionary<string, PeremeterTypes> parameterTypes)
        {
            var result = new Dictionary<string, object>();
            var parts = answer.Split(new[] { ',' });
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
                        else if (parameter == "INH2O4")
                            result.Add(key, PressureUnits.inH2O4);
                        else if (parameter == "INH2O20")
                            result.Add(key, PressureUnits.inH2O20);
                        else if (parameter == "INHG")
                            result.Add(key, PressureUnits.inHg);
                        else if (parameter == "MMHG")
                            result.Add(key, PressureUnits.mmHg);
                        else if (parameter == "PA")
                            result.Add(key, PressureUnits.Pa);
                        else if (parameter == "HPA")
                            result.Add(key, PressureUnits.hPa);
                        else if (parameter == "PSI")
                            result.Add(key, PressureUnits.psi);
                        else if (parameter == "INH2O60F")
                            result.Add(key, PressureUnits.inH2O60F);
                        else if (parameter == "KGCM2")
                            result.Add(key, PressureUnits.KgCm2);
                        else if (parameter == "%FS")
                            result.Add(key, PressureUnits.FS);
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

        private string PressureUnitToString(PressureUnits unit)
        {
            string unitstr = null;
            switch (unit)
            {
                case PressureUnits.None:
                    throw new Exception(string.Format("can not set pressure unit PressureUnits.None"));
                    break;
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
        #endregion
    }
}
