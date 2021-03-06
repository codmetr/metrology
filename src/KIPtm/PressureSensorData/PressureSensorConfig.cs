﻿using System;
using System.Collections.Generic;
using System.Linq;
using ArchiveData.DTO;
using KipTM.Interfaces;

namespace PressureSensorData
{
    /// <summary>
    /// Внутренний контейнер конфигурации проверки
    /// </summary>
    public class PressureSensorConfig : BaseCheckData
    {
        public PressureSensorConfig()
        {
            Temperature = 23;
            Humidity = 50;
            DayPressure = 760;
            CommonVoltage = 220;
            EtalonPressure = new EtalonDescriptor();
            EtalonOut = new EtalonDescriptor();
            Points = new List<PressureSensorPointConf>();
        }

        public static PressureSensorConfig GetDefault()
        {
            return new PressureSensorConfig()
            {
                ReportDate = DateTime.Now.ToString("dd.MM.yyyy"),
                CertificateDate = DateTime.Now.ToString("dd.MM.yyyy"),
                Note = "",
                Master = "[Организация]",
                Name = "[Наименование]",
                SensorType = "[Тип]",
                SensorModel = "[Модификация]",
                RegNum = "[Регистрационный номер]",
                SerialNumber = "[Заводской номер]",
                CheckedParameters = "абсолютное давление в диапазоне [диапазон]",
                ChecklLawBase = "[документ с методикой поверки]", //"Федеральный закон от 26 июня 2008 г. № 102-ФЗ \"Об обеспечении единства измерений\"",
                Company = "[Организация]",
                EtalonPressure = new EtalonDescriptor()
                {
                    SensorType = "Датчик давления",
                    Title = "[Наименование]"
                },
                EtalonOut = new EtalonDescriptor()
                {
                    SensorType = "Вольтметр",
                    Title = "[Наименование]"
                },
            };
        }

        protected PressureSensorConfig FillCopy(PressureSensorConfig data)
        {
            FillCopy((BaseCheckData)data);
            data.ReportNumber = ReportNumber;
            data.ReportDate = ReportDate;
            data.CertificateNumber = CertificateNumber;
            data.CertificateDate = CertificateDate;
            data.Note = Note;
            data.EtalonPressure = EtalonPressure.DeepCopy();
            data.EtalonOut = EtalonOut.DeepCopy();
            data.Points = Points.Select(el => el.DeepCopy()).ToList();
            data.VpiMax = VpiMax;
            data.VpiMin = VpiMin;
            data.Unit = Unit;
            data.TolerancePercentVpi = TolerancePercentVpi;
            data.ToleranceDelta = ToleranceDelta;
            data.TolerancePercentSigma = TolerancePercentSigma;
            data.OutputRange = OutputRange;
            return data;
        }

        public PressureSensorConfig DeepCopy()
        {
            var data = new PressureSensorConfig();
            FillCopy(data);
            return data;
        }

        public void SetCopy(PressureSensorConfig data)
        {
            data.FillCopy(this);
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
        /// Примечание
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Эталон давления
        /// </summary>
        public EtalonDescriptor EtalonPressure { get; set; }

        /// <summary>
        /// Эталон выходного сигнала
        /// </summary>
        public EtalonDescriptor EtalonOut { get; set; }

        /// <summary>
        /// Точки проверки
        /// </summary>
        public List<PressureSensorPointConf> Points { get; set; }

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