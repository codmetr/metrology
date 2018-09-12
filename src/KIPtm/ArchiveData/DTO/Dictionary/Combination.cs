﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchiveData.DTO.Dictionary
{
    /// <summary>
    /// Комбинация параметров описывающих исполнителя заказчика и среду проведения метрологической операции
    /// </summary>
    public class Combination
    {
        /// <summary>
        /// Поверитель:
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Руководитель лаборатории
        /// </summary>
        public string ChiefLab { get; set; }

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
        /// Время действия свидетельства:
        /// </summary>
        public string Validity { get; set; }

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
    }
}