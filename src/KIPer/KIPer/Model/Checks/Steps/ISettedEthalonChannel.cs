using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Channels;

namespace KipTM.Model.Checks.Steps
{
    /// <summary>
    /// Шаг поддерживает установку эталонного канала
    /// </summary>
    interface ISettedEthalonChannel
    {
        /// <summary>
        /// Установить эталонный канал
        /// </summary>
        /// <param name="ehalon">эталонный канал</param>
        void SetEthalonChannel(IEthalonChannel ehalon);
    }
}
