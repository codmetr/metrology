using MahApps.Metro;

namespace PACESeriesUtil.VM
{
    /// <summary>
    /// Описатель темы
    /// </summary>
    public class ThemeDescriptor
    {
        private string _name;
        private AppTheme _theme;

        /// <summary>
        /// Описатель темы
        /// </summary>
        /// <param name="name">Название темы</param>
        /// <param name="theme">Тема</param>
        public ThemeDescriptor(string name, AppTheme theme)
        {
            _name = name;
            _theme = theme;
        }

        /// <summary>
        /// Название темы
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Тема
        /// </summary>
        public AppTheme Theme
        {
            get { return _theme; }
        }
    }
}