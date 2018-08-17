using System.ComponentModel;

namespace PressureSensorData
{
    /// <summary>
    /// Описатель эталона
    /// </summary>
    public class EthalonDescriptor
    {
        /// <summary>
        /// Копия
        /// </summary>
        /// <returns></returns>
        public EthalonDescriptor DeepCopy()
        {
            return new EthalonDescriptor()
            {
                Title = Title,
                SensorType = SensorType,
                SerialNumber = SerialNumber,
                RegNum = RegNum,
                Category = Category,
                ErrorClass = ErrorClass,
                CheckCertificateNumber = CheckCertificateNumber,
                CheckCertificateDate = CheckCertificateDate,
            };
        }

        /// <summary>
        /// Наименование:
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Тип:
        /// </summary>
        /// <remarks>
        /// Обобщенное название типа прибора
        /// </remarks>
        public string SensorType { get; set; }

        /// <summary>
        /// Заводской номер (номера):
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Регистрационный номер:
        /// </summary>
        public string RegNum { get; set; }

        /// <summary>
        /// Разряд:
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Класс или погрешность:
        /// </summary>
        public string ErrorClass { get; set; }

        /// <summary>
        /// Номер свидетельства о поверке:
        /// </summary>
        public string CheckCertificateNumber { get; set; }

        /// <summary>
        /// Дата выдачи свидетельства о поверке:
        /// </summary>
        public string CheckCertificateDate { get; set; }
    }
}