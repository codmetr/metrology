using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KIPer.Model;

namespace KIPer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                });
        }

        private string _helpMessage;
        public string HelpMessage
        {
            get { return _helpMessage; }
            set { Set(ref _helpMessage, value); }
        }

        private object _selectedAction;

        public object SelectedAction
        {
            get { return _selectedAction; }
            set { Set(ref _selectedAction, value); }
        }

        public ICommand LoadView{get{return new RelayCommand<object>(
            (mainView) =>
            {
                var view = mainView as Window;
                if(view==null)
                    return;


                /*
                try
                { 
                    foreach (var mod in _model.Viewes)
                    {
                        var typeModel = mod.Key;
                        var typeView = mod.Value;
                        var template = new DataTemplate
                        {
                            DataType = typeModel,
                            VisualTree = new FrameworkElementFactory(typeView)
                        };
                        view.Resources.Add(new DataTemplateKey(typeModel), template);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.ToString());
                    throw;
                }
                */
            });}
        }

        /// <summary>
        /// Выбрана вкладка Проверки
        /// </summary>
        public ICommand SelectChecks{get
        {
            return new RelayCommand(() =>
            {
                SelectedAction = "Проверки";//todo установить выбор соответсвующего ViewModel
                HelpMessage = "Список Проверок";
            });
        }}

        /// <summary>
        /// Выбрана вкладка Приборы
        /// </summary>
        public ICommand SelectTargetDevices{get
        {
            return new RelayCommand(() =>
            {
                SelectedAction = "Приборы";//todo установить выбор соответсвующего ViewModel
                HelpMessage = "Список проверяемых приборов";
            });
        }}

        /// <summary>
        /// Выбрана вкладка Эталоны
        /// </summary>
        public ICommand SelectEtalonDevices{get
        {
            return new RelayCommand(() =>
            {
                SelectedAction = "Приборы";//todo установить выбор соответсвующего ViewModel
                HelpMessage = "Список эталонных приборов";
            });
        }}

        /// <summary>
        /// Выбрана вкладка Настройки
        /// </summary>
        public ICommand SelectSettings{get
        {
            return new RelayCommand(() =>
            {
                SelectedAction = "Настройки";//todo установить выбор соответсвующего ViewModel
                HelpMessage = "Список настроек";
            });
        }}


        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        private void Loadsettings()
        {
            
        }
    
        private readonly List<TestViewModel> _tests = new List<TestViewModel>();
        public IEnumerable<TestViewModel> Tests { get { return _tests; } }
    }
}