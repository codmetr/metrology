using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Checks.Steps
{
    /// <summary>
    /// Шаг поддерживает установку текущей точки как ключевой
    /// </summary>
    interface IStoppedOnPoint
    {
        /// <summary>
        /// Установить текущую точку как ключевую
        /// </summary>
        void SetCurrentValueAsPoint();
    }
}
