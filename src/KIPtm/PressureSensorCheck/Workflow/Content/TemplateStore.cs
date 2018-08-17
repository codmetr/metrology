using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PressureSensorData;

namespace PressureSensorCheck.Workflow.Content
{
    /// <summary>
    /// Коллекция шаблонов настройки
    /// </summary>
    public class TemplateStore:INotifyPropertyChanged
    {
        private TemplateViewModel _selectedTemplate;

        public TemplateStore()
        {
            Templates = new ObservableCollection<TemplateViewModel>();
        }

        /// <summary>
        /// Коллекция шаблонов настроек
        /// </summary>
        public ObservableCollection<TemplateViewModel> Templates { get; private set; }

        /// <summary>
        /// Выбранный шаблон
        /// </summary>
        public TemplateViewModel SelectedTemplate
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
        public void AddTemplate(TemplateViewModel template)
        {
            Templates.Add(template);
        }

        /// <summary>
        /// Добавить в коллекцию шаблон
        /// </summary>
        /// <param name="template"></param>
        public void RemoveTemplate(TemplateViewModel template)
        {
            Templates.Remove(template);
        }

        public bool Validate(TemplateViewModel template)
        {
            return Templates.Any(el => el.Name == template.Name);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Шаблон настроек
    /// </summary>
    public class TemplateViewModel : INotifyPropertyChanged
    {
        private string _name;
        private PressureSensorConfig _data;

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
        public PressureSensorConfig Data
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
