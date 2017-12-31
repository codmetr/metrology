using System;
using System.Collections.Generic;
using ArchiveData;
using ArchiveData.DTO;
using CheckFrame;
using CheckFrame.Checks;
using CheckFrame.ViewModel.Archive;
using CheckFrame.Workflow;
using Core.Archive.DataTypes;
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
using KipTM.ViewModel.Checks.Config;
using KipTM.ViewModel.Report;
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
        private readonly IMarkerFactory<IParameterResultViewModel> _resultMaker;
        private readonly IDictionaryPool _dict;
        private readonly IFillerFactory<IParameterResultViewModel> _filler;
        private readonly IEnumerable<ICheckViewModelFactory> _factoriesViewModels;
        private readonly CustomConfigFactory _customFactory;
        private readonly IDataAccessor _archive;
        private readonly IEventAggregator _eventAggregator;
        private readonly IReportFactory _reportFactory;
        private readonly IDeviceManager _deviceManager;
        private readonly CheckPool _checkPool;

        /// <summary>
        /// Фабрика конвейера состояний проверки
        /// </summary>
        /// <param name="features">Забор возмодностей модулей</param>
        /// <param name="settings">Настройки</param>
        /// <param name="methodicService">Источник методик</param>
        /// <param name="resultMaker"></param>
        /// <param name="dict"></param>
        /// <param name="filler">Заполнение результата</param>
        /// <param name="factoriesViewModels">Преобразователи в визуальные модели</param>
        /// <param name="customFatories">Фабрики специализированных настроек</param>
        /// <param name="archive">Архив</param>
        /// <param name="eventAggregator">Сборщик событий</param>
        /// <param name="reportFactory">Фабрика отчетов</param>
        /// <param name="deviceManager">Пулл устройств</param>
        public CheckWorkflowFactory(
        FeatureDescriptorsCombiner features, IMainSettings settings, IMethodsService methodicService,
            IMarkerFactory<IParameterResultViewModel> resultMaker, IDictionaryPool dict,
            IFillerFactory<IParameterResultViewModel> filler, IEnumerable<ICheckViewModelFactory> factoriesViewModels,
            IDictionary<Type, ICustomConfigFactory> customFatories, IDataAccessor archive, IEventAggregator eventAggregator,
            IReportFactory reportFactory, IDeviceManager deviceManager)
        {
            _channelFactory = features.ChannelFactories;
            _settings = settings;
            _methodicService = methodicService;
            _resultMaker = resultMaker;
            _dict = dict;
            _filler = filler;
            _factoriesViewModels = factoriesViewModels;
            _customFactory = new CustomConfigFactory(customFatories);
            _archive = archive;
            _eventAggregator = eventAggregator;
            _reportFactory = reportFactory;
            _deviceManager = deviceManager;
            _checkPool = new CheckPool(_deviceManager, null/*TODO как-то получить настройки для конкретного типа провери*/, _factoriesViewModels);
        }

        /// <summary>
        /// Создать новый конвейер состояний для запуска новой проверки
        /// </summary>
        /// <returns></returns>
        public IWorkflow GetNew(DeviceTypeDescriptor devTypeKey)
        {
            var result = new TestResultID();

            // создание конфигурации конкретной проверки
            var checkConfigDevice = CheckConfigFactory.GenerateForType(devTypeKey, _settings, _methodicService,
                null/*TODO как-то получить настройки для конкретного типа провери*/, _dict.DeviceTypes, result);
            // TODO: добавить настроки не через IPropertyPool

            var checkConfigViewModel = new CheckConfigViewModel(checkConfigDevice, _channelFactory, _customFactory);
            var resFactory = new TestResultViewModelFactory(result, checkConfigDevice, _resultMaker, _filler, _archive);
            var checkFactory = new CheckFactory(_checkPool, checkConfigDevice, result, _eventAggregator);

            var resState = new ResultState(resFactory);
            var steps = new List<IWorkflowStep>()
            {
                new ConfigCheckState(checkConfigViewModel),
                new CheckState(checkFactory, _eventAggregator),
                resState,
                new ReportState(() => new ReportViewModel(_reportFactory, result, checkConfigDevice.Data, resState.ViewModel)),
            };

            return new LineWorkflow(steps);
        }

        public IEnumerable<DeviceViewDescriptor> GetAvailableKeys()
        {
            return _methodicService.GetDescriptors();
        } 
    }
}
