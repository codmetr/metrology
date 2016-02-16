using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
{
    static class Service
    {
        #region Набор статических методов для работы с ADTS
        /// <summary>
        /// Набор статических методов для работы с ADTS
        /// </summary>
        public class Service
        {

            #region Конвертер единиц давления
            /// <summary>
            /// Справочник коэффициентов конвертации давления
            /// </summary>
            /// <remarks>
            /// ключ - размерность давления
            /// значение - соответствующий коэффициент (отношение к Па)
            /// (см float ConvertPressure(float value, PressureUnits unitFrom, PressureUnits unitTo, int? round = null))
            /// </remarks>
            private static readonly IDictionary<PressureUnits, double> _dicTranslate =
              new Dictionary<PressureUnits, double>()
          {
            { PressureUnits.hPa, 100f},
            { PressureUnits.KgCm2, 98066.5f},
            { PressureUnits.mmHg, 133.322f},
            { PressureUnits.MBar, 100f},
            { PressureUnits.Pa, 1f},
            { PressureUnits.inH2O4, 249.089f},
            { PressureUnits.inH2O60F, 248.64135f},
            { PressureUnits.mmH2O4, 9.80665f},
            { PressureUnits.psi, 6894.76f},
          };

            /// <summary>
            /// Справочник коэффициентов конвертации единиц авиациоонных измерений
            /// </summary>
            /// <remarks>
            /// ключ - размерность авиационных единиц
            /// значение - соответствующая пара коэффициентов [рассотояние] и [скорость] (отношение к [м] и [м/с])
            /// (см float ConvertPressure(float value, PressureUnits unitFrom, PressureUnits unitTo, int? round = null))
            /// </remarks>
            private static readonly IDictionary<AeronauticalUnits, Tuple<double, double>> _dicTranslateAer =
                new Dictionary<AeronauticalUnits, Tuple<double, double>>()
            {
                {AeronauticalUnits.MKPH_Ms, new Tuple <double, double>(1.0, 1.0)},
                {AeronauticalUnits.FTKNTS, new Tuple <double, double>( 0.3048, 0.3048/60.0)},
                {AeronauticalUnits.MKPH, new Tuple <double, double>(1f, 1.0/60f)},//Устарело. Для обратной совместимости
                {AeronauticalUnits.MKPH_Mmin, new Tuple <double, double>(1f, 1.0/60f)},
                {AeronauticalUnits.MKPH_hMmin, new Tuple <double, double>(1f, 1.0/6f)},
            };

            /// <summary>
            /// Конвертирует знакение давления из одной размерности в другую
            /// </summary>
            /// <remarks>
            /// Для пересчета используется формула из описания ADTS
            /// 
            /// Единицы давления и коэффициенты пересчета:
            ///   __________________________________________________________________________________________________
            ///   Единица давления	      |Коэффициент (в Паскалях)	|Единица давления	    |Коэффициент (в Паскалях)
            ///   бар                     |100000                   |фунт-сила/кв.фут       |47,8803
            ///   фунт•с/ кв. дюйм (psi)  |6894,76	                |дюйм рт. ст.	        |3386,39
            ///   м вод. ст.              |9806,65	                |дюйм вод. ст. [1]	    |249,089
            ///   мбар                    |100                     	|фут вод. ст. [1]	    |2989,07
            ///   кгс/см2	              |98066,5	                |атмосфера	            |101325,0
            ///   кгс/м2	              |9,80665	                |паундаль/кв. фут	    |1,48816
            ///   мм рт. ст.	          |133,322	                |дина/см2	            |0,1
            ///   см. рт. ст.	          |1333,22	                |гбар	                |10000000
            ///   м рт. ст.	              |133322,0	                |тс/кв. фут (Вел.)	    |107252,0
            ///   мм вод. ст. [1]	      |9,80665	                |тс/кв.дюйм (Вел.)	    |15444300
            ///   см вод. ст. [1]	      |98,0665	                |дюйм вод.ст.(США) [2]	|248,64135
            ///   Н/м2	                  |1                        |фут вод. ст. (США) [2]	|2983,6983
            ///   гПа	                  |100	                    |кгс/мм2	            |9806650
            ///   кПа	                  |1000	                    |кгс/см2	            |98066,5
            ///   МПа	                  |1000000	                |кгс/м2	                |9,80665
            ///   торр	                  |133,322		            |                       |
            ///   -----------------------------------------------------------------------------------------------
            /// Примечание:
            /// Коэффициенты преобразования единиц давления, помеченных [1], получены при температуре воды 4°C.
            /// Единицы давления, помеченные [2], получены при температуре воды 68°F; эти единицы измерений
            /// традиционно используются в США.
            /// 
            /// 
            /// Преобразование единиц давления:
            /// Для пересчета ЗНАЧЕНИЯ 1 в ЕДИНИЦАХ 1в ЗНАЧЕНИЕ 2 в ЕДИНИЦАХ 2 используется формула:
            /// 
            /// 	ЗНАЧЕНИЕ 2	=	ЗНАЧЕНИЕ 1 × КОЭФФИЦИЕНТ 2 / КОЭФФИЦИЕНТ 1
            ///
            /// </remarks>
            /// <param name="value">Величина давления, котрую необходимо перевести</param>
            /// <param name="unitFrom">Единицы из которых необходимо перевести</param>
            /// <param name="unitTo">Единицы в которые необходимо перевести</param>
            /// <param name="round">При не равенстве null, ограничивает точность результата (число знаков после запятой)</param>
            public static double ConvertPressure(double value, PressureUnits unitFrom, PressureUnits unitTo, int? round = null)
            {
                if (unitFrom == unitTo)
                    return value;
                if (!_dicTranslate.ContainsKey(unitFrom) || !_dicTranslate.ContainsKey(unitTo))
                    return value;
                var res = value * _dicTranslate[unitFrom] / _dicTranslate[unitTo];
                if (round != null)
                    res = (float)Math.Round(res, round.Value);
                return res;
            }

            public static float ConvertPressure(float value, PressureUnits unitFrom, PressureUnits unitTo, int? round = null)
            {
                return (float)ConvertPressure((double)value, unitFrom, unitTo, round);
            }

            /// <summary>
            /// Конвертирует значение растояния из одной размерности в другую
            /// </summary>
            /// <param name="value">Величина расстояния</param>
            /// <param name="unitFrom">Единицы из которых необходимо перевести</param>
            /// <param name="unitTo">Единицы в которые необходимо перевести</param>
            /// <param name="round">При не равенстве null, ограничивает точность результата (число знаков после запятой)</param>
            /// <returns></returns>
            public static double ConvertAeronautical(double value, AeronauticalUnits unitFrom, AeronauticalUnits unitTo, int? round = null)
            {
                if (unitFrom == unitTo)
                    return value;
                if (!_dicTranslateAer.ContainsKey(unitFrom) || !_dicTranslateAer.ContainsKey(unitTo))
                    return value;
                var res = value * _dicTranslateAer[unitFrom].Item1 / _dicTranslateAer[unitTo].Item1;
                if (round != null)
                    res = (float)Math.Round(res, round.Value);
                return res;
            }

            /// <summary>
            /// Конвертирует знакение скорости из одной размерности в другую
            /// </summary>
            /// <param name="value">Величина скорости</param>
            /// <param name="unitFrom">Единицы из которых необходимо перевести</param>
            /// <param name="unitTo">Единицы в которые необходимо перевести</param>
            /// <param name="round">При не равенстве null, ограничивает точность результата (число знаков после запятой)</param>
            /// <returns></returns>
            public static double ConvertAeronauticalRate(double value, AeronauticalUnits unitFrom, AeronauticalUnits unitTo, int? round = null)
            {
                if (unitFrom == unitTo)
                    return value;
                if (!_dicTranslateAer.ContainsKey(unitFrom) || !_dicTranslateAer.ContainsKey(unitTo))
                    return value;
                var res = value * _dicTranslateAer[unitFrom].Item2 / _dicTranslateAer[unitTo].Item2;
                if (round != null)
                    res = (float)Math.Round(res, round.Value);
                return res;
            }

            private const double _koefQCtoCAS = 13.595100264;

            public static double FromQCtoCAS(double QC)
            {
                return QC * _koefQCtoCAS;
            }

            public static float FromQCtoCAS(float QC)
            {
                return (float)FromQCtoCAS((double)QC);
            }

            public static double FromCAStoQC(double CAS)
            {
                return CAS / _koefQCtoCAS;
            }

            public static float FromCAStoQC(float CAS)
            {
                return (float)FromCAStoQC((double)CAS);
            }

            private const double _koefALTtoPS = 12;

            public static double FromPStoALT(double PS)
            {
                return PS * _koefALTtoPS;
            }

            public static float FromPStoALT(float PS)
            {
                return (float)FromPStoALT((double)PS);
            }

            public static double FromALTtoPS(double ALT)
            {
                return ALT / _koefALTtoPS;
            }

            public static float FromALTtoPS(float ALT)
            {
                return (float)FromALTtoPS((double)ALT);
            }

            #endregion

        }
        #endregion Набор статических методов для работы с ADTS

    }
}
