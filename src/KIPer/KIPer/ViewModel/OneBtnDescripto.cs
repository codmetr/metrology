using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace KipTM.ViewModel
{
    /// <summary>
    /// Набор свойств одной кнопки для Ribbon
    /// </summary>
    public class OneBtnDescripto:INotifyPropertyChanged
    {
        private bool _isSelected = false;

        /// <summary>
        /// Набор свойств одной кнопки для Ribbon
        /// </summary>
        /// <param name="key">Ключ кнопки</param>
        /// <param name="title">Название кнопки</param>
        /// <param name="lagreImage">Большая иконка кнопки</param>
        /// <param name="smallImage">Маленькая иконка кнопки</param>
        /// <param name="btnCmd">Команда</param>
        public OneBtnDescripto(string key, string title, BitmapImage lagreImage, BitmapImage smallImage, ICommand btnCmd)
        {
            Key = key;
            Title = title;
            LagreImage = lagreImage;
            SmallImage = smallImage;
            BtnCmd = btnCmd;
        }

        /// <summary>
        /// Ключ кнопки
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Название кнопки
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Большая иконка кнопки
        /// </summary>
        public BitmapImage LagreImage { get; private set; }

        /// <summary>
        /// Маленькая иконка кнопки
        /// </summary>
        public BitmapImage SmallImage { get; private set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Команда
        /// </summary>
        public ICommand BtnCmd { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
