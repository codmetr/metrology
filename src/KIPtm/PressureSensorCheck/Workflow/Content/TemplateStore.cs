using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using KipTM.Interfaces;
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
}
