using System;
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
    public class TemplateStore<T>: INotifyPropertyChanged where T : class

    {
        private TemplateViewModel<T> _selectedTemplate;
        private ITamplateArchive<T> _archive;
        private T _currentDate;

        public TemplateStore(ITamplateArchive<T> archive)
        {
            _archive = archive;
            Templates = new ObservableCollection<TemplateViewModel<T>>();
            foreach (var item in _archive.GetArchive())
            {
                Templates.Add(new TemplateViewModel<T>() { Name = item.Key, Data = item.Value });
            }
        }

        /// <summary>
        /// Коллекция шаблонов настроек
        /// </summary>
        public ObservableCollection<TemplateViewModel<T>> Templates { get; private set; }

        /// <summary>
        /// Выбранный из списка шаблон
        /// </summary>
        public TemplateViewModel<T> SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (value == _selectedTemplate)
                    return;
                _selectedTemplate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущее состояние настроек
        /// </summary>
        public T LastData
        {
            get { return _currentDate; }
            set
            {
                if (value == _currentDate)
                    return;
                _currentDate = value;
                OnUpdatedTemplate(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Обновлен шаблон
        /// </summary>
        public event Action<T> UpdatedTemplate;

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
            get { return new CommandWrapper(OnAddTemplate); }
        }

        /// <summary>
        /// Применить шаблон
        /// </summary>
        private void OnApplyTemplate()
        {
            LastData = SelectedTemplate.Data;
            //_agregator?.Post(new HelpMessageEventArg("Применен шаблон:"));
        }

        /// <summary>
        /// Добавить шаблон
        /// </summary>
        private void OnAddTemplate()
        {
            Templates.Add(new TemplateViewModel<T>() { Name = NameTemplate, Data = LastData });
        }

        /// <summary>
        /// Вызвать событие о том, что шаблон был обновлен
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void OnUpdatedTemplate(T obj)
        {
            UpdatedTemplate?.Invoke(obj);
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
