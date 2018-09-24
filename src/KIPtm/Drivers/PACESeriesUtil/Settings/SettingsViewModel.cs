using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro;
using Tools.View;

namespace PACESeriesUtil.VM
{
    public class SettingsViewModel:INotifyPropertyChanged
    {
        private Dictionary<string, string> _themes = new Dictionary<string, string>()
        {
            {"Blue"     , "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml"},
            {"Brown"    , "pack://application:,,,/MahApps.Metro;component/Styles/Accents/brown.xaml"},
            {"Amber"    , "pack://application:,,,/MahApps.Metro;component/Styles/Accents/amber.xaml"},
            {"Basedark" , "pack://application:,,,/MahApps.Metro;component/Styles/Accents/basedark.xaml"},
            {"Baselight", "pack://application:,,,/MahApps.Metro;component/Styles/Accents/baselight.xaml"},
            {"Cobalt"   , "pack://application:,,,/MahApps.Metro;component/Styles/Accents/cobalt.xaml"},
            {"Crimson"  , "pack://application:,,,/MahApps.Metro;component/Styles/Accents/crimson.xaml"},
            {"Cyan"     , "pack://application:,,,/MahApps.Metro;component/Styles/Accents/cyan.xaml"},
            {"Emerald"  , "pack://application:,,,/MahApps.Metro;component/Styles/Accents/emerald.xaml"},
            {"Green"    , "pack://application:,,,/MahApps.Metro;component/Styles/Accents/green.xaml"},
            {"Indigo"   , "pack://application:,,,/MahApps.Metro;component/Styles/Accents/indigo.xaml"},
        };

        public SettingsViewModel()
        {
        }

        public ICommand ChangeTheme
        {
            get
            {
                return new CommandWrapper((arg) =>
                {
                    var theme = _themes[arg as string];
                    var app = (App)Application.Current;
                    app.ChangeTheme(new Uri(theme));
                });
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
