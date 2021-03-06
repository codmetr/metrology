﻿using System;
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
        private double _P = 0.0;
        private string _Pstr = 0.0.ToString();
        private double _I = 0.0;
        private string _Istr = 0.0.ToString();
        private string _formulaOutSignal = string.Empty;
        private string _formulaOutSignalTolerance = string.Empty;
        private string _formulaOutSignalError = string.Empty;

        /// <summary>
        /// Допуск по приведенной погрешности
        /// </summary>
        internal double TolerancePercentSigma { get; set; }

        /// <summary>
        /// Допуск по проценту ВПИ
        /// </summary>
        internal double TolerancePercentVpi { get; set; }

        internal double Imax { get; set; }

        internal double Imin { get; set; }

        internal double Pmax { get; set; }

        internal double Pmin { get; set; }

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
            get { return _Pstr; }
            set
            {
                _Pstr = value;
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
            get { return _Istr; }
            set
            {
                _Istr = value;
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
            //@"\Delta I(P) = (I_{max}-I_{min})\times\frac{\gamma_{vpi}}{100\%} + (I_{max}-I_{min})\times\frac{P-P_{min}}{P_{max}-P_{min}}\times\frac{\gamma}{100\%}"
            var strRes = sb.Append(@"\Delta I(P) = (").Append(Imax).Append("-").Append(Imin).Append(@")\times\frac{")
                .Append(TolerancePercentVpi.ToString())
                .Append(@"}{100\%} + (").Append(Imax).Append("-").Append(Imin).Append(@")\times\frac{P-").Append(Pmin)
                .Append(@"}{").Append(Pmax).Append("-").Append(Pmin).Append(@"}\times\frac{")
                .Append(TolerancePercentSigma.ToString()).Append(@"}{100\%}").ToString();

            FormulaFinalyError = strRes;

            var res = CheckPressureLogicConfig.CalcRes(_P, Pmin, Pmax, Imin, Imax, TolerancePercentVpi, TolerancePercentSigma);

            //@"I(P) = I_{min} + (I_{max}-I_{min})\times\frac{P-P_{min}}{P_{max}-P_{min}}"
            var IpRes = double.IsNaN(res.Ip) ? "Nan" : double.IsInfinity(res.Ip) ? @"/infinity" : res.Ip.ToString("F3");
            strRes = sb.Clear().Append(@"I(").Append(_P.ToString()).Append(@") = ").Append(Imin).Append("+(").Append(Imax).Append("-").Append(Imin).Append(@")\times\frac{")
                .Append(P).Append("-").Append(Pmin).Append(@"}{").Append(Pmax).Append("-").Append(Pmin).Append(@"}=").Append(IpRes).ToString();
            FormulaOutSignal = strRes;

            //@"\Delta I(P) = (I_{max}-I_{min})\times\gamma_{vpi} + (I_{max}-I_{min})\times\frac{P-P_{min}}{P_{max}-P_{min}}\times\frac{\gamma}{100\%}"
            var dIpRes = double.IsNaN(res.dIp) ? "Nan" : double.IsInfinity(res.dIp) ? @"/infinity" : res.dIp.ToString("F3");
            strRes = sb.Clear().Append(@"\Delta(").Append(_P.ToString()).Append(") = (").Append(Imax).Append("-").Append(Imin).Append(@")\times\frac{")
                .Append(TolerancePercentVpi.ToString())
                .Append(@"}{100\%} + (").Append(Imax).Append("-").Append(Imin).Append(@")\times\frac{").Append(_P.ToString()).Append("-").Append(Pmin)
                .Append(@"}{").Append(Pmax).Append("-").Append(Pmin).Append(@"}\times\frac{")
                .Append(TolerancePercentSigma.ToString()).Append(@"}{100\%} = ").Append(dIpRes).ToString();
            FormulaOutSignalTolerance = strRes;

            //@"\Delta I = I-I(P)"
            var dI = _I - res.Ip;
            var dIRes = double.IsNaN(dI) ? "Nan" : double.IsInfinity(dI) ? @"/infinity" : dI.ToString("F3");
            strRes = sb.Clear().Append(@"\Delta I = I-I(P) = ").Append(_I.ToString("F3")).Append(" - ").Append(IpRes).Append("=").Append(dIRes).ToString();
            FormulaOutSignalError = strRes;
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
