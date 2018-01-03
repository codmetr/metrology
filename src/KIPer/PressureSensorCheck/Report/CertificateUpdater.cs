﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using PressureSensorData;

namespace PressureSensorCheck.Report
{
    /// <summary>
    /// Обновление DTO для сертификата поверки
    /// </summary>
    public class CertificateUpdater
    {
        /// <summary>
        /// Обновить сертификат поверки по результату
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="config">конфигурация</param>
        /// <param name="result">результат</param>
        /// <param name="report">отчет</param>
        public void Update(TestResultID id, PressureSensorConfig config, PressureSensorResult result,
            PressureSensorCertificateDto report)
        {
            ApplyCommonData(id, config, result, report);

            ApplyEthalons(config, report);
        }

        private static void ApplyCommonData(TestResultID id, PressureSensorConfig config, PressureSensorResult result, PressureSensorCertificateDto report)
        {
            report.Organization = config.Company;
            report.CertificateNumber = config.CertificateNumber;
            report.Validity = "";
            report.Name = config.Name;
            report.Type = config.SensorType;
            report.Model = config.SensorModel;
            report.RegNumber = config.RegNum;
            report.LastCheckSerialAndNumber = config.NumberLastCheck;
            report.SerialNumber = id.SerialNumber;
            report.CheckedParameters = config.CheckedParameters;
            report.CheckLawBase = config.ChecklLawBase;
            report.Temperature = config.Temperature.ToString("F0");
            report.Humidity = config.Humidity.ToString("F0");
            report.Pressure = config.DayPressure.ToString("F0");
            report.User = config.User;
            report.Checker = config.User;
        }

        private void ApplyEthalons(PressureSensorConfig config, PressureSensorCertificateDto report)
        {
            report.Ethalons = new[]
            {
                ToDto(config.EthalonPressure),
                ToDto(config.EthalonVoltage),
            };
        }

        private EthalonDto ToDto(EthalonDescriptor descr)
        {
            return new EthalonDto()
            {
                Title = descr.Title,
                Type = $"тип {descr.SensorType}, разряд {descr.Category}",
                RangeClass = descr.ErrorClass,
                SerialNumber = descr.SerialNumber,
                CheckCertificateNumber = descr.CheckCertificateNumber,
                CheckCertificateDate = descr.CheckCertificateDate,
            };
        }
    }
}