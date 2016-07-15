using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{

    public class ADTSPoint
    {
        public ADTSPoint()
        {
            IsAvailable = true;
        }

        /// <summary>
        /// Контрольное давление
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// Допустимая погрешность на контрольном давлении
        /// </summary>
        public double Tolerance { get; set; }

        /// <summary>
        /// Признак необходимости проверки указанной точки
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}
