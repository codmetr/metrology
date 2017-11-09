using System;

namespace Tools.View.Busy
{
    /// <summary>
    /// Обертка для гарантированного выключения IsBusy
    /// </summary>
    public class LockControl : IDisposable
    {
        private readonly IBusy _vm;

        /// <summary>
        /// Обертка для гарантированного выключения IsBusy
        /// При создании включает "занятость"
        /// </summary>
        public LockControl(IBusy vm)
        {
            _vm = vm;
            _vm.IsBusy = true;
        }

        /// <summary>
        /// Отключить "занятость"
        /// </summary>
        public void Dispose()
        {
            _vm.IsBusy = false;
        }
    }
}
