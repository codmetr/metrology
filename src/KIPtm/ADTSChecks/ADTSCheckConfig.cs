using System.Collections.Generic;
using ADTS;
using ADTSChecks.Checks.Data;
using ADTSChecks.Devices;
using ADTSChecks.Properties;
using ArchiveData.DTO;
using CheckFrame.Archive;

namespace ADTSChecks
{
    /// <summary>
    /// Конфиурация проверки ADTS
    /// </summary>
    public class ADTSCheckConfig
    {
        private ADTSChannelConfig _ps;
        private ADTSChannelConfig _pt;

        /// <summary>
        /// Канал PS
        /// </summary>
        public ADTSChannelConfig Ps
        {
            get { return _ps; }
        }

        /// <summary>
        /// Канал PT
        /// </summary>
        public ADTSChannelConfig Pt
        {
            get { return _pt; }
        }

        /// <summary>
        /// Получить значение по умолчанию
        /// </summary>
        /// <returns></returns>
        public static ADTSCheckConfig GetDefault()
        {
            return new ADTSCheckConfig()
            {
                _ps = new ADTSChannelConfig()
                {
                    Channel = new ChannelDescriptor()
                    {
                        Key = BasicKeys.KeyChannel,
                        Name = ADTSModel.Ps,
                        Max = 1355.0,
                        Order = ChannelOrder.Source,
                        TypeChannel = ChannelType.Pressure,
                        Min = 3.0,
                        Error = 0.01,
                    }.SetLocalizedNameFunc(() => Resources.ChannelPs),
                    Rate = 50.0,
                    Unit = PressureUnits.MBar,
                    Poits = new List<ADTSPoint>()
                    {
                        new ADTSPoint(){Pressure = 1100.00, Tolerance = 0.1},
                        new ADTSPoint(){Pressure = 1013.00, Tolerance = 0.1},
                        new ADTSPoint(){Pressure = 843.00,  Tolerance = 0.1},
                        new ADTSPoint(){Pressure = 697.00,  Tolerance = 0.1},
                        new ADTSPoint(){Pressure = 466.00,  Tolerance = 0.1},
                        new ADTSPoint(){Pressure = 189.00,  Tolerance = 0.1},
                        new ADTSPoint(){Pressure = 72.00,   Tolerance = 0.1},
                        new ADTSPoint(){Pressure = 27.62,   Tolerance = 0.1},
                    }
                },
                _pt = new ADTSChannelConfig()
                {
                    Channel = new ChannelDescriptor()
                    {
                        Key = BasicKeys.KeyChannel,
                        Name = ADTSModel.Pt,
                        Max = 2700.0,
                        Order = ChannelOrder.Source,
                        TypeChannel = ChannelType.Pressure,
                        Min = 3.0,
                        Error = 0.01,
                    }.SetLocalizedNameFunc(() => Resources.ChannelPt),
                    Rate = 50.0,
                    Unit = PressureUnits.MBar,
                    Poits = new List<ADTSPoint>()
                    {
                        new ADTSPoint(){Pressure = 3500.00, Tolerance = 0.49},
                        new ADTSPoint(){Pressure = 3000.00, Tolerance = 0.46},
                        new ADTSPoint(){Pressure = 2590.00, Tolerance = 0.40},
                        new ADTSPoint(){Pressure = 2200.00, Tolerance = 0.36},
                        new ADTSPoint(){Pressure = 1655.00, Tolerance = 0.32},
                        new ADTSPoint(){Pressure = 1100.00, Tolerance = 0.28},
                        new ADTSPoint(){Pressure = 1013.00, Tolerance = 0.27},
                        new ADTSPoint(){Pressure = 843.00,  Tolerance = 0.27},
                        new ADTSPoint(){Pressure = 697.00,  Tolerance = 0.26},
                        new ADTSPoint(){Pressure = 466.00,  Tolerance = 0.25},
                        new ADTSPoint(){Pressure = 189.00,  Tolerance = 0.25},
                        new ADTSPoint(){Pressure = 72.00,   Tolerance = 0.24},
                        new ADTSPoint(){Pressure = 27.62,   Tolerance = 0.24},
                    }
                },
            };
        }
    }

    /// <summary>
    /// Конфигурация канала
    /// </summary>
    public class ADTSChannelConfig
    {
        private ChannelDescriptor _channel;

        private List<ADTSPoint> _poits;

        private double _rate;

        private PressureUnits _unit;

        /// <summary>
        /// Описатель канала
        /// </summary>
        public ChannelDescriptor Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        /// <summary>
        /// Проверяемые точки
        /// </summary>
        public List<ADTSPoint> Poits
        {
            get { return _poits; }
            set { _poits = value; }
        }

        /// <summary>
        /// Скорость установки величины
        /// </summary>
        public double Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public PressureUnits Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
    }
}
