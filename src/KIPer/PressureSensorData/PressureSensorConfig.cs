using System;
using System.Collections.Generic;

namespace PressureSensorData
{
    /// <summary>
    /// Внутренний контейнер конфигурации проверки
    /// </summary>
    public class PressureSensorConfig
    {
        public PressureSensorConfig()
        {
            Temperature = 23;
            Humidity = 50;
            DayPressure = 760;
            CommonVoltage = 220;
            EthalonPressure = new EthalonDescriptor();
            EthalonVoltage = new EthalonDescriptor();
            Points = new List<PressureSensorPoint>();
        }

        public static PressureSensorConfig GetDefault()
        {
            return new PressureSensorConfig()
            {
                ReportDate = DateTime.Now.ToString("yy.MM.dd"),
                CertificateDate = DateTime.Now.ToString("yy.MM.dd"),
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
                EthalonVoltage = new EthalonDescriptor()
                {
                    SensorType = "Вольтметр",
                    Title = "[Наименование]"
                },
        };
        }

        /// <summary>
        /// Пользователь:
        /// </summary>
        public string User { get; set; }

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
        /// Принадлежит:
        /// </summary>
        public string Master { get; set; }

        /// <summary>
        /// Наименование:
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип:
        /// </summary>
        public string SensorType { get; set; }

        /// <summary>
        /// Модификация:
        /// </summary>
        public string SensorModel { get; set; }

        /// <summary>
        /// Регистрационный номер в Федеральном информационном фонде по обеспечению единства измерений:
        /// </summary>
        public string RegNum { get; set; }

        /// <summary>
        /// Серия и номер знака предыдущей проверки (если такие серия и номер имеются):
        /// </summary>
        public string NumberLastCheck { get; set; }

        /// <summary>
        /// Заводской номер (номера):
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Поверено:
        /// </summary>
        /// <remarks>
        /// Наименование величин, диапазонов, на которых поверено средство измерений (если предусмотрено методикой поверки)
        /// </remarks>
        public string CheckedParameters { get; set; }

        /// <summary>
        /// Поверено в соответствии с:
        /// </summary>
        /// <remarks>
        /// Наименование документа, на основании которого выполнена поверка
        /// </remarks>
        public string ChecklLawBase { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        ///<remarks>
        /// Наименование юридического лица или индивидуального предпринимателя, аккредитованного в установленном порядке на проведение поверки средств измерений, регистрационный номер аттестата аккредитации
        /// </remarks>
        public string Company { get; set; }

        /// <summary>
        /// Температура
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        public double Humidity { get; set; }

        /// <summary>
        /// Давление дня
        /// </summary>
        public double DayPressure { get; set; }

        /// <summary>
        /// Напряжение сети
        /// </summary>
        public double CommonVoltage { get; set; }

        /// <summary>
        /// Эталон давления
        /// </summary>
        public EthalonDescriptor EthalonPressure { get; set; }

        /// <summary>
        /// Эталон напряжения
        /// </summary>
        public EthalonDescriptor EthalonVoltage { get; set; }

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
        public string Unit { get; set; }

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