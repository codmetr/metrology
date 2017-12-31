using System.Collections.Generic;

namespace PressureSensorCheck.Report
{
    /// <summary>
    /// Представление всех данных проверки для основного отчета
    /// </summary>
    public class PressureSensorReportDto
    {
        /// <summary>
        /// Номер протокола
        /// </summary>
        public object ReportNumber { get; set; }

        /// <summary>
        /// Дата протокола
        /// </summary>
        public object ReportTime { get; set; }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public object TypeDevice { get; set; }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public object SerialNumber { get; set; }

        /// <summary>
        /// Принадлежит
        /// </summary>
        public object Owner { get; set; }

        /// <summary>
        /// Эталоны
        /// </summary>
        public IEnumerable<EthalonDto> Ethalons { get; set; }

        /// <summary>
        /// Температура
        /// </summary>
        public object Temperature { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        public object Humidity { get; set; }

        /// <summary>
        /// Атмосферное давление
        /// </summary>
        public object Pressure { get; set; }

        /// <summary>
        /// Напряжение питания
        /// </summary>
        public object Voltage { get; set; }

        /// <summary>
        /// Результат визуального осмотра
        /// </summary>
        public object VisualCheckResult { get; set; }

        /// <summary>
        /// Результат проверки на герметичность
        /// </summary>
        public object LeakCheckResult { get; set; }

        /// <summary>
        /// Результат опробирования
        /// </summary>
        public object Assay { get; set; }

        /// <summary>
        /// Результат определения основной приведенной погрешности
        /// </summary>
        public IEnumerable<MainAccurancyPointDto> MainAccurancy { get; set; }

        /// <summary>
        /// Результат проверки вариации выходного сигнала
        /// </summary>
        public IEnumerable<VariationAccurancyPointDto> VariationAccurancy { get; set; }

        /// <summary>
        /// Общий результат поверки
        /// </summary>
        public object CommonResult { get; set; }

        /// <summary>
        /// Номер свидетельства о проверке
        /// </summary>
        public object CertificateNumber { get; set; }

        /// <summary>
        /// Дата выпуска сертификата
        /// </summary>
        public object CertificateDate { get; set; }

        /// <summary>
        /// Проверку проводил
        /// </summary>
        public object User { get; set; }
    }

    /// <summary>
    /// Результат одной точки проверки вариации
    /// </summary>
    public class VariationAccurancyPointDto
    {
        /// <summary>
        /// Pвх, кПа
        /// </summary>
        public object PressurePoint { get; set; }

        /// <summary>
        /// Uпр, В
        /// </summary>
        public object Uf { get; set; }

        /// <summary>
        /// Uобр, В
        /// </summary>
        public object Ur { get; set; }

        /// <summary>
        /// dU, В
        /// </summary>
        public object dU { get; set; }

        /// <summary>
        /// dUэт, В
        /// </summary>
        public object dUet { get; set; }
    }

    /// <summary>
    /// Результат одной точки проверки основной погрешности
    /// </summary>
    public class MainAccurancyPointDto
    {
        /// <summary>
        /// Pвх, кПа
        /// </summary>
        public object PressurePoint { get; set; }

        /// <summary>
        /// Uэт, В
        /// </summary>
        public object Uet { get; set; }

        /// <summary>
        /// Uфакт, В
        /// </summary>
        public object U { get; set; }

        /// <summary>
        /// dU, В
        /// </summary>
        public object dU { get; set; }

        /// <summary>
        /// dUэт, В
        /// </summary>
        public object dUet { get; set; }
    }

    /// <summary>
    /// Описатель эталона
    /// </summary>
    public class EthalonDto
    {
        /// <summary>
        /// Название прибора
        /// </summary>
        public object Title { get; set; }

        /// <summary>
        /// Тип прибора
        /// </summary>
        public object Type { get; set; }

        /// <summary>
        /// Разряд, класс или погрешность
        /// </summary>
        public object RangeClass { get; set; }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public object SerialNumber { get; set; }

        /// <summary>
        /// Номер свидетельства о поверке
        /// </summary>
        public object CheckCertificateNumber { get; set; }

        /// <summary>
        /// Дата выдачи свидетельства о поверке
        /// </summary>
        public object CheckCertificateDate { get; set; }
    }
}
