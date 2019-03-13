using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.Interfaces;
using PressureSensorData;

namespace PressureSensorCheck.Workflow.Content
{
    /// <summary>
    /// Конфигурация логики проверки
    /// </summary>
    public class CheckPressureLogicConfig
    {
        private readonly CheckPressureLogicConfigVm _vm;
        private int _pointsOnRange = 5;
        
        /// <summary>
        /// Фактические данные конфигурации
        /// </summary>
        /// <remarks>
        /// Использовать на разметке экрана только в случае единственного места изменения, так как без INotifyPropertyChanged
        /// </remarks>
        private readonly PressureSensorConfig _data;

        private readonly Dictionary<int, PointConfigViewModel> _vmPoints = new Dictionary<int, PointConfigViewModel>();

        /// <summary>
        /// Конфигурация логики проверки
        /// </summary>
        public CheckPressureLogicConfig(PressureSensorConfig data, CheckPressureLogicConfigVm vm)
        {
            _data = data;
            _vm = vm;
            var uMin = 0.0;
            var uMax = 5.0;
            if (_data.OutputRange == OutGange.I4_20mA)
            {
                uMin = 4;
                uMax = 20;
            }
            _vm.SetBaseStates(_data.VpiMax, _data.VpiMin, UnitDict.GetUnitsForType(ChannelType.Pressure),
                _data.Unit, new[] { OutGange.I4_20mA, OutGange.I0_5mA, }, _data.OutputRange,
                _data.TolerancePercentSigma, _data.TolerancePercentVpi, uMin, uMax);
            _vm.SettedData += VmOnSettedData;
            var points = RecalcPoints(_data.OutputRange, _data.VpiMax, _data.VpiMin, _pointsOnRange, _data.TolerancePercentVpi, _data.TolerancePercentSigma, _data.Unit).ToArray();
            UpdatePoints(points);
            //_vm.SelectedRange
        }

        private void UpdatePoints(PressureSensorPointConf[] points)
        {
            _data.Points.Clear();
            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];
                _data.Points.Add(point);
                if (!_vmPoints.ContainsKey(i))
                {
                    _vmPoints[i] = _vm.AddPoint(point.PressurePoint, point.OutPoint, point.Tollerance,
                        point.PressureUnit);
                    continue;
                }
                _vmPoints[i].SetPressure(point.PressurePoint);
                _vmPoints[i].SetI(point.OutPoint);
                _vmPoints[i].SetdI(point.Tollerance);
                _vmPoints[i].SetUnit(point.PressureUnit);
            }
        }

        private void VmOnSettedData(CheckPressureLogicConfigVm.ConfData confData)
        {
            _data.TolerancePercentSigma = confData.TolerancePercentSigma;
            _data.TolerancePercentVpi = confData.TolerancePercentVpi;
            _data.VpiMin = confData.VpiMin;
            _data.VpiMax = confData.VpiMax;
            _data.Unit = confData.PressureUnit;
            _data.OutputRange = confData.OutputRange;

            var points = RecalcPoints(_data.OutputRange, _data.VpiMax, _data.VpiMin, _pointsOnRange, _data.TolerancePercentVpi, _data.TolerancePercentSigma, _data.Unit).ToArray();
            UpdatePoints(points);
        }

        /// <summary>
        /// Перерассчитать точки
        /// </summary>
        private IEnumerable<PressureSensorPointConf> RecalcPoints(OutGange range, double Pmax, double Pmin, int countPoints, double toleranceVpi, double toleranceSigma, Units unit)
        {

            var uMin = 0.0;
            var uMax = 5.0;
            if (range == OutGange.I4_20mA)
            {
                uMin = 4;
                uMax = 20;
            }
            if (Pmin >= Pmax)
                yield break;

            var step = (Pmax - Pmin) / (countPoints - 1);
            for (double i = 0; i < countPoints; i++)
            {
                var point = Pmin + (i * step);

                var pointOut = CalcRes(point, Pmin, Pmax, uMin, uMax, toleranceVpi, toleranceSigma);
                yield return new PressureSensorPointConf()
                {
                    PressurePoint = point,
                    OutPoint = pointOut.Ip,
                    PressureUnit = unit,
                    OutUnit = Units.mA,
                    Tollerance = pointOut.dIp,
                };
            }
        }

        /// <summary>
        /// Результат рассчета на точке
        /// </summary>
        public struct PointLimit
        {
            /// <summary>
            /// Рассчетное значение выходного сигнала для точки
            /// </summary>
            public readonly double Ip;
            /// <summary>
            /// Рассчетное значение погрешности выходного сигнала для точки
            /// </summary>
            public readonly double dIp;

            /// <summary>
            /// Результат рассчета на точке
            /// </summary>
            /// <param name="Ip">Рассчетное значение выходного сигнала для точки</param>
            /// <param name="dIp">Рассчетное значение погрешности выходного сигнала для точки</param>
            public PointLimit(double Ip, double dIp)
            {
                this.Ip = Ip;
                this.dIp = dIp;
            }
        }


        /// <summary>
        /// Рассчитать погрешность и ожидаемую точку
        /// </summary>
        /// <param name="P">давлени (входной сигнал)</param>
        /// <param name="Pmin">Минимум давления</param>
        /// <param name="Pmax">Максимум давления</param>
        /// <param name="Imin">Минимум тока</param>
        /// <param name="Imax">Максимум тока</param>
        /// <param name="tolerVpi">допуск от ВПИ</param>
        /// <param name="tolerSigm">допуск приведенной погрешности</param>
        /// <returns></returns>
        public static PointLimit CalcRes(double P, double Pmin, double Pmax, double Imin, double Imax, double tolerVpi, double tolerSigm)
        {
            var Ip = Imin + ((Imax - Imin) * (P - Pmin) / (Pmax - Pmin));
            var dIp = (Imax - Imin) * tolerVpi / 100.0 + ((tolerSigm / 100.0) * (Imax - Imin) * (P - Pmin) / (Pmax - Pmin));
            return new PointLimit(Ip, dIp);
        }
    }
}
