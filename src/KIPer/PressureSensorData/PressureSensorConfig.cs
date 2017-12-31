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
        }

        /// <summary>
        /// Пользователь:
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Номер сертификата:
        /// </summary>
        public string SertificateNumber { get; set; }

        /// <summary>
        /// Дата сертификата:
        /// </summary>
        public string SertificateDate { get; set; }

        /// <summary>
        /// Принадлежит:
        /// </summary>
        public string Master { get; set; }

        /// <summary>
        /// Наименование:
        /// </summary>
        public string Title { get; set; }

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
        /// Единицы измерения
        /// </summary>
        public IEnumerable<string> Units { get; set; }

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
        /// относительная погрешность
        /// </summary>
        public double TolerancePercentSigma { get; set; }

        /// <summary>
        /// Варианты выходного диапазона
        /// </summary>
        public IEnumerable<OutGange> OutputRanges { get; set; }

        /// <summary>
        /// Выбранный выходной диапазон
        /// </summary>
        public OutGange OutputRange { get; set; }
    }
}