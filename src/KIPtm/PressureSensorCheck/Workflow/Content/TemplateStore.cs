using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KipTM.ViewModel.Events;
using PressureSensorData;
using Tools.View;

namespace PressureSensorCheck.Workflow.Content
{
    /// <summary>
    /// Коллекция шаблонов настройки
    /// </summary>
    public class TemplateStore<T>:INotifyPropertyChanged
    {
        private TemplateViewModel<T> _selectedTemplate;

        public TemplateStore(ITamplateArchive<T> archive)
        {
            Templates = new ObservableCollection<TemplateViewModel<T>>();
        }

        //public TemplateStore()
        //{
        //    Templates = new ObservableCollection<TemplateViewModel<T>>();
        //}

        /// <summary>
        /// Коллекция шаблонов настроек
        /// </summary>
        public ObservableCollection<TemplateViewModel<T>> Templates { get; private set; }

        /// <summary>
        /// Выбранный шаблон
        /// </summary>
        public TemplateViewModel<T> SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if(value == _selectedTemplate)
                    return;
                _selectedTemplate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Добавить в коллекцию шаблон
        /// </summary>
        /// <param name="template"></param>
        public void RemoveTemplate(TemplateViewModel<T> template)
        {
            Templates.Remove(template);
        }

        public bool Validate(TemplateViewModel<T> template)
        {
            return Templates.Any(el => el.Name == template.Name);
        }

        /// <summary>
        /// Применить выбранный шаблон
        /// </summary>
        public ICommand ApplyTemplate
        {
            get { return new CommandWrapper(OnApplyTemplate); }
        }

        /// <summary>
        /// Имя создаваемого шаблона
        /// </summary>
        public string NameTemplate { get; set; }

        /// <summary>
        /// Заполнить шаблон на базе текущих настроек
        /// </summary>
        public ICommand AddTemplate
        {
            get { return new CommandWrapper(OnApplyTemplate); }
        }

        private void OnApplyTemplate()
        {
            Templates.Add(SelectedTemplate);
            //_agregator?.Post(new HelpMessageEventArg("Применен шаблон:"));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public interface ITamplateArchive<T>
    {
        /// <summary>
        /// Прочитать все наборы конфигурации
        /// </summary>
        /// <returns></returns>
        Dictionary<string, T> GetArchive();
        void AddTemplate(string name, T conf);
        void Update(string name, T conf);
        void Remove(string name);
    }

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
