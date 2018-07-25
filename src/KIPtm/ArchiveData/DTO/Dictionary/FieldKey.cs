using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchiveData.DTO.Dictionary
{
    static class FieldKey
    {
        /// <summary>
        /// Поверитель:
        /// </summary>
        public static string User = "User";

        /// <summary>
        /// Руководитель лаборатории
        /// </summary>
        public static string ChiefLab = "ChiefLab";

        /// <summary>
        /// Номер протокола:
        /// </summary>
        public static string ReportNumber = "ReportNumber";

        /// <summary>
        /// Дата протокола:
        /// </summary>
        public static string ReportDate = "ReportDate";

        /// <summary>
        /// Номер сертификата:
        /// </summary>
        public static string CertificateNumber = "CertificateNumber";

        /// <summary>
        /// Дата сертификата:
        /// </summary>
        public static string CertificateDate = "CertificateDate";

        /// <summary>
        /// Время действия свидетельства:
        /// </summary>
        public static string Validity = "Validity";

        /// <summary>
        /// Принадлежит:
        /// </summary>
        public static string Master = "Master";

        /// <summary>
        /// Наименование:
        /// </summary>
        public static string Name = "Name";

        /// <summary>
        /// Тип:
        /// </summary>
        public static string SensorType = "SensorType";

        /// <summary>
        /// Модификация:
        /// </summary>
        public static string SensorModel = "SensorModel";

        /// <summary>
        /// Регистрационный номер в Федеральном информационном фонде по обеспечению единства измерений:
        /// </summary>
        public static string RegNum = "RegNum";

        /// <summary>
        /// Серия и номер знака предыдущей проверки (если такие серия и номер имеются):
        /// </summary>
        public static string NumberLastCheck = "NumberLastCheck";

        /// <summary>
        /// Заводской номер (номера):
        /// </summary>
        public static string SerialNumber = "SerialNumber";

        /// <summary>
        /// Поверено:
        /// </summary>
        /// <remarks>
        /// Наименование величин, диапазонов, на которых поверено средство измерений (если предусмотрено методикой поверки)
        /// </remarks>
        public static string CheckedParameters = "CheckedParameters";

        /// <summary>
        /// Поверено в соответствии с:
        /// </summary>
        /// <remarks>
        /// Наименование документа, на основании которого выполнена поверка
        /// </remarks>
        public static string ChecklLawBase = "ChecklLawBase";

        /// <summary>
        /// Организация
        /// </summary>
        ///<remarks>
        /// Наименование юридического лица или индивидуального предпринимателя, аккредитованного в установленном порядке на проведение поверки средств измерений, регистрационный номер аттестата аккредитации
        /// </remarks>
        public static string Company = "Company";

        /// <summary>
        /// Температура
        /// </summary>
        public static string Temperature = "Temperature";

        /// <summary>
        /// Влажность
        /// </summary>
        public static string Humidity = "Humidity";

        /// <summary>
        /// Давление дня
        /// </summary>
        public static string DayPressure = "DayPressure";

        /// <summary>
        /// Напряжение сети
        /// </summary>
        public static string CommonVoltage = "CommonVoltage";
    }
}
