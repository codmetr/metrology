using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KIPer.Interfaces;
using KIPer.Model;
using KIPer.View;

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

        private readonly ServiceViewModel _services;

        private readonly Dictionary<Type, Type> ViewModelViewDic;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.LoadSettings();
            _dataService.InitDevices();

            _services = new ServiceViewModel(new List<IService>()
            {
                new Pace5000ViewModel(_dataService.Pace5000)
            });
            ViewModelViewDic = new Dictionary<Type, Type>()
            {
                {typeof(ServiceViewModel), typeof(ServicesView)},
                {typeof(Pace5000ViewModel), typeof(PACE5000View)}
            };
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

                try
                {
                    foreach (var mod in ViewModelViewDic)
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
                    throw;
                }
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


        /// <summary>
        /// Выбрана вкладка Настройки
        /// </summary>
        public ICommand SelectService{get
        {
            return new RelayCommand(() =>
            {
                SelectedAction = _services;
                HelpMessage = "Сервисная вкладка для отладки различных механизмов";
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