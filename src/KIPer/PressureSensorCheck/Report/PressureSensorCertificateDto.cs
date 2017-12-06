using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Report.PressureSensor
{
    public class PressureSensorCertificateDto
    {
        /// <summary>
        /// Название организации
        /// </summary>
        public object Organization { get; set; }

        /// <summary>
        /// Номер свидетельства о поверке
        /// </summary>
        public object CertificateNumber { get; set; }

        /// <summary>
        /// Время действия свидетельства
        /// </summary>
        public object Validity { get; set; }

        /// <summary>
        /// Наименование средства измерения
        /// </summary>
        public object Name { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public object Type { get; set; }

        /// <summary>
        /// Модификация
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// Регистрационный номер в Федеральном информационном фонде по обеспечению единства измерений
        /// </summary>
        public object RegNumber { get; set; }

        /// <summary>
        /// Серия и номер знака предыдущей проверки
        /// </summary>
        public object LastCheckSerialAndNumber { get; set; }

        /// <summary>
        /// Заводской номер (номера)
        /// </summary>
        public object SerialNumber { get; set; }

        /// <summary>
        /// Поверено
        /// </summary>
        public object CheckedParameters { get; set; }

        /// <summary>
        /// Поверено в соответствии с
        /// </summary>
        public object CheckLawBase { get; set; }

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
        /// Проверку проводил
        /// </summary>
        public object User { get; set; }

        /// <summary>
        /// Поверитель
        /// </summary>
        public object Checker { get; set; }

    }
}
