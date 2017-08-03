using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using CheckFrame;
using CheckFrame.ViewModel.Archive;
using KipTM.Archive.ViewModel;
using KipTM.Checks;
using KipTM.Checks.ViewModel;
using KipTM.Checks.ViewModel.Config;
using KipTM.EventAggregator;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Settings;
using KipTM.ViewModel;
using KipTM.ViewModel.Checks;
using KipTM.ViewModel.Checks.Config;
using KipTM.ViewModel.Report;
using KipTM.ViewModel.Workflow;
using KipTM.ViewModel.Workflow.States;
using MarkerService;
using MarkerService.Filler;
using ReportService;
using SQLiteArchive;

namespace KipTM.Workflow.States
{
    /// <summary>
    /// Фабрика конвейера состояний проверки
    /// </summary>
    public class CheckWorkflowFactory
    {
        private readonly IMainSettings _settings;
        private readonly IChannelsFactory _channelFactory;
        private readonly IMethodsService _methodicService;
        private readonly IPropertiesLibrary _propertiesLibrary;
        private readonly IMarkerFactory<IParameterResultViewModel> _resultMaker;
        private readonly IFillerFactory<IParameterResultViewModel> _filler;
        private readonly IEnumerable<ICheckViewModelFactory> _factoriesViewModels;
        private readonly CustomConfigFactory _customFactory;
        private readonly IObjectiveArchive _archive;
        private readonly IEventAggregator _eventAggregator;
        private readonly IReportFactory _reportFactory;
        private readonly IDeviceManager _deviceManager;

        /// <summary>
        /// Фабрика конвейера состояний проверки
        /// </summary>
        /// <param name="eventAggregator">Сборщик событий</param>
        /// <param name="methodicService">Источник методик</param>
        /// <param name="settings">Настройки</param>
        /// <param name="propertiesLibrary">Свойства</param>
        /// <param name="archive">Архив</param>
        /// <param name="resultMaker"></param>
        /// <param name="filler">Заполнение результата</param>
        /// <param name="reportFactory">Фабрика отчетов</param>
        /// <param name="features">Забор возмодностей модулей</param>
        /// <param name="customFatories">Фабрики специализированных настроек</param>
        /// <param name="deviceManager">Пулл устройств</param>
        /// <param name="factoriesViewModels">Преобразователи в визуальные модели</param>
        public CheckWorkflowFactory(
        FeatureDescriptorsCombiner features, IMainSettings settings, IMethodsService methodicService,
            IPropertiesLibrary propertiesLibrary, IMarkerFactory<IParameterResultViewModel> resultMaker,
            IFillerFactory<IParameterResultViewModel> filler, IEnumerable<ICheckViewModelFactory> factoriesViewModels,
            IDictionary<Type, ICustomConfigFactory> customFatories, IObjectiveArchive archive, IEventAggregator eventAggregator,
            IReportFactory reportFactory, IDeviceManager deviceManager)
        {
            _channelFactory = features.ChannelFactories;
            _settings = settings;
            _methodicService = methodicService;
            _propertiesLibrary = propertiesLibrary;
            _resultMaker = resultMaker;
            _filler = filler;
            _factoriesViewModels = factoriesViewModels;
            _customFactory = new CustomConfigFactory(customFatories);
            _archive = archive;
            _eventAggregator = eventAggregator;
            _reportFactory = reportFactory;
            _deviceManager = deviceManager;
        }

        /// <summary>
        /// Создать новый конвейер состояний для запуска новой проверки
        /// </summary>
        /// <returns></returns>
        public IWorkflow GetNew(string devTypeKey)
        {
            var result = new TestResult();

            // создание конфигурации конкретной проверки
            var checkConfigDevice = CheckConfig.GenerateForType(devTypeKey, _settings, _methodicService,
                _propertiesLibrary.PropertyPool, _propertiesLibrary.DictionariesPool, result);

            var checkConfigViewModel = new CheckConfigViewModel(checkConfigDevice, _channelFactory, _customFactory);
            var resFactory = new TestResultViewModelFactory(result, checkConfigDevice, _resultMaker, _filler, _archive);
            var checkPool = new CheckPool(_deviceManager, _propertiesLibrary.PropertyPool, _factoriesViewModels);
            var checkFactory = new CheckFactory(checkPool, checkConfigDevice, result, _eventAggregator);

            var steps = new List<IWorkflowStep>()
            {
                new ConfigCheckState(checkConfigViewModel),
                new CheckState(checkFactory, _eventAggregator),
                new ResultState(resFactory),
                new ReportState(() => new ReportViewModel(_reportFactory, result)),
            };

            return new LineWorkflow(steps);
        }

        public IEnumerable<DeviceViewDescriptor> GetAvailableKeys()
        {
            return _methodicService.GetDescriptors();
        } 
    }
}
