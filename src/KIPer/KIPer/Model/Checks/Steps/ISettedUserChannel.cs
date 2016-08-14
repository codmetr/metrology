using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Channels;

namespace KipTM.Model.Checks.Steps
{
    /// <summary>
    /// Шаг поддерживает установку канала связи с пользователем
    /// </summary>
    interface ISettedUserChannel
    {
        /// <summary>
        /// Утановка канала связи с пользователем
        /// </summary>
        /// <param name="userChannel"></param>
        void SetUserChannel(IUserChannel userChannel);
    }
}
