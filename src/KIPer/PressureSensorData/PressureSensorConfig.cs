using System;
using System.Collections.Generic;
using ArchiveData.DTO;
using KipTM.Interfaces;

namespace PressureSensorData
{
    /// <summary>
    /// Внутренний контейнер конфигурации проверки
    /// </summary>
    public class PressureSensorConfig:BaseCheckData
    {
        public PressureSensorConfig()
        {
            Temperature = 23;
            Humidity = 50;
            DayPressure = 760;
            CommonVoltage = 220;
            EthalonPressure = new EthalonDescriptor();
            EthalonOut = new EthalonDescriptor();
            Points = new List<PressureSensorPoint>();
        }

        public static PressureSensorConfig GetDefault()
        {
            return new PressureSensorConfig()
            {
                ReportDate = DateTime.Now.ToString("dd.MM.yyyy"),
                CertificateDate = DateTime.Now.ToString("dd.MM.yyyy"),
                Master = "[Организация]",
                Name = "[Наименование]",
                SensorType = "[Тип]",
                SensorModel = "[Модификация]",
                RegNum = "[Регистрационный номер]",
                SerialNumber = "[Заводской номер]",
                CheckedParameters = "абсолютное давление в диапазоне [диапазон]",
                ChecklLawBase = "[документ с методикой поверки]", //"Федеральный закон от 26 июня 2008 г. № 102-ФЗ \"Об обеспечении единства измерений\"",
                Company = "[Организация]",
                EthalonPressure = new EthalonDescriptor()
                {
                    SensorType = "Датчик давления",
                    Title = "[Наименование]"
                },
                EthalonOut = new EthalonDescriptor()
                {
                    SensorType = "Вольтметр",
                    Title = "[Наименование]"
                },
        };
        }

        /// <summary>
        /// Номер протокола:
        /// </summary>
        public string ReportNumber { get; set; }

        /// <summary>
        /// Дата протокола:
        /// </summary>
        public string ReportDate { get; set; }

        /// <summary>
        /// Номер сертификата:
        /// </summary>
        public string CertificateNumber { get; set; }

        /// <summary>
        /// Дата сертификата:
        /// </summary>
        public string CertificateDate { get; set; }

        /// <summary>
        /// Эталон давления
        /// </summary>
        public EthalonDescriptor EthalonPressure { get; set; }

        /// <summary>
        /// Эталон выходного сигнала
        /// </summary>
        public EthalonDescriptor EthalonOut { get; set; }

        /// <summary>
        /// Точки проверки
        /// </summary>
        public List<PressureSensorPoint> Points { get; set; }

        /// <summary>
        /// Максимум диапазона
        /// </summary>
        public double VpiMax { get; set; }

        /// <summary>
        /// Минимум диапазона
        /// </summary>
        public double VpiMin { get; set; }

        /// <summary>
        /// Выбранная единица измерения
        /// </summary>
        public Units Unit { get; set; }

        /// <summary>
        /// Допуск по проценту ВПИ
        /// </summary>
        public double TolerancePercentVpi { get; set; }

        /// <summary>
        /// Абсолютный допуск
        /// </summary>
        public double ToleranceDelta { get; set; }

        /// <summary>
        /// Допуск по относительной погрешности
        /// </summary>
        public double TolerancePercentSigma { get; set; }

        /// <summary>
        /// Выбранный выходной диапазон
        /// </summary>
        public OutGange OutputRange { get; set; }
    }
}