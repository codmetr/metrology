using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PressureSensorCheck.Workflow.Content
{
    /// <summary>
    /// Шаблон настроек
    /// </summary>
    public class TemplateViewModel<T> : INotifyPropertyChanged
    {
        private string _name;
        private T _data;

        /// <summary>
        /// Название шаблона
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if(value == _name)
                    return;
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Сами настройки
        /// </summary>
        public T Data
        {
            get { return _data; }
            set
            {
                _data = value; 
                OnPropertyChanged();
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