using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    /// <summary>
    /// The app version helper.
    /// </summary>
    public static class AppVersionHelper
    {
        /// <summary>
        ///     The app version.
        /// </summary>
        public enum AppVersionType
        {
            /// <summary>
            ///     The release.
            /// </summary>
            Release,
            /// <summary>
            ///     The trial.
            /// </summary>
            Emulation,

            /// <summary>
            ///     The debug.
            /// </summary>
            Debug,
        }

        /// <summary>
        /// Initializes static members of the <see cref="AppVersionHelper"/> class.
        /// </summary>
        static AppVersionHelper()
        {
#if DEBUG
            CurrentAppVersionType = AppVersionType.Debug;
#endif

#if RELEASE
            CurrentAppVersionType = AppVersionType.Release;
#endif

#if EMULATION
            CurrentAppVersionType = AppVersionType.Emulation;
#endif
        }

        public static AppVersionType CurrentAppVersionType { get; private set; }
    }

}
