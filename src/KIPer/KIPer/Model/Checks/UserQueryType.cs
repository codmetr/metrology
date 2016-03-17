using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    public enum UserQueryType
    {
        /// <summary>
        /// Запрос на ввод реального значения
        /// </summary>
        GetRealValue,
        /// <summary>
        /// Запрос на подтверждение/опровержение
        /// </summary>
        GetAccept,
    }
}
