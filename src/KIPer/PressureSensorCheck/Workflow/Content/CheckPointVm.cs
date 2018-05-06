using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressureSensorCheck.Workflow.Content
{
    public class CheckPointVm : INotifyPropertyChanged
    {
        private string _formulaFinalyError;
        private double _P;
        private double _I;
        private string _formulaOutSignal = string.Empty;
        private string _formulaOutSignalTolerance = string.Empty;
        private string _formulaOutSignalError = string.Empty;

        /// <summary>
        /// Допуск по приведенной погрешности
        /// </summary>
        public double TolerancePercentSigma { get; set; }

        /// <summary>
        /// Допуск по проценту ВПИ
        /// </summary>
        public double TolerancePercentVpi { get; set; }

        public double Imax { get; set; }

        public double Imin { get; set; }

        public double Pmax { get; set; }

        public double Pmin { get; set; }

        /// <summary>
        /// Итоговая формула
        /// </summary>
        public string FormulaFinalyError
        {
            get { return _formulaFinalyError; }
            set
            {
                _formulaFinalyError = value;
                OnPropertyChanged(nameof(FormulaFinalyError));
            }
        }

        /// <summary>
        /// Tочка
        /// </summary>
        public string P
        {
            get { return _P.ToString("F2"); }
            set
            {
                double dval;
                if(!double.TryParse(value.Replace(',','.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dval))
                    return;
                _P = dval;
                UpdateFormulas();
            }
        }

        /// <summary>
        /// Эталонное значение
        /// </summary>
        public string I
        {
            get { return _I.ToString("F2"); }
            set
            {
                double dval;
                if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out dval))
                    return;
                _I = dval;
                UpdateFormulas();
            }
        }

        /// <summary>
        /// Рассчетное значение выходного сигнала для точки
        /// </summary>
        public string FormulaOutSignal
        {
            get { return _formulaOutSignal; }
            set
            {
                _formulaOutSignal = value;
                OnPropertyChanged(nameof(FormulaOutSignal));
            }
        }

        /// <summary>
        /// Рассчетное значение погрешности выходного сигнала для точки
        /// </summary>
        public string FormulaOutSignalTolerance
        {
            get { return _formulaOutSignalTolerance; }
            set
            {
                _formulaOutSignalTolerance = value;
                OnPropertyChanged(nameof(FormulaOutSignalTolerance));
            }
        }

        /// <summary>
        /// Итоговая погрешность
        /// </summary>
        public string FormulaOutSignalError
        {
            get { return _formulaOutSignalError; }
            set
            {
                _formulaOutSignalError = value;
                OnPropertyChanged(nameof(FormulaOutSignalError));
            }
        }

        public void UpdateFormulas()
        {
            var sb = new StringBuilder();
            //@"\delta(P) = (I_{max}-I_{min})\times\gamma_{vpi} + (I_{max}-I_{min})\times\frac{P-P_{min}}{P_{max}-P_{min}}\times\frac{\gamma}{100\%}"
            var strRes = sb.Append(@"\delta(P) = (").Append(Imax).Append("-").Append(Imin).Append(@")\times")
                .Append(TolerancePercentVpi.ToString())
                .Append(@" + (").Append(Imax).Append("-").Append(Imin).Append(@")\times\frac{P-").Append(Pmin)
                .Append(@"}{").Append(Pmax).Append("-").Append(Pmin).Append(@"}\times\frac{")
                .Append(TolerancePercentSigma.ToString()).Append(@"}{100\%}").ToString();

            FormulaFinalyError = strRes;

            //@"I(P) = I_{min} + (I_{max}-I_{min})\times\frac{P-P_{min}}{P_{max}-P_{min}}"
            var _Ip = Imin + ((Imax - Imin) * (_P - Pmin) / (Pmax - Pmin));
            var IpRes = double.IsNaN(_Ip) ? "Nan" : double.IsInfinity(_Ip) ? @"/infinity" : _Ip.ToString("F3");
            strRes = sb.Clear().Append(@"I(").Append(P).Append(@") = ").Append(Imin).Append("+(").Append(Imax).Append("-").Append(Imin).Append(@")\times\frac{")
                .Append(P).Append("-").Append(Pmin).Append(@"}{").Append(P).Append("-").Append(Pmin).Append(@"}=").Append(IpRes).ToString();
            FormulaOutSignal = strRes;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
