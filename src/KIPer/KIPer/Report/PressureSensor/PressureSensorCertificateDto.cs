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
        /// Номер свивдетельства о поверке
        /// </summary>
        public object CertificateNumber { get; set; }

        /// <summary>
        /// Время действия сертификата
        /// </summary>
        public object Validity { get; set; }
    }
}
