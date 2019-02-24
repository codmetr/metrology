using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using PressureSensorCheck.Workflow.Content;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Модель конфигурирования проверки
    /// </summary>
    public class PressureSensorCheckConfigurator
    {
        private readonly TestResultID _identificator;
        private readonly PressureSensorConfig _configData;
        private readonly DPI620GeniiConfig _dpiConf;
        private readonly ITamplateArchive<PressureSensorConfig> _archive;
        private readonly Dictionary<string, IEtalonSourceCannelFactory<Units>> _ethalonsSources;
        private readonly PressureSensorCheckConfigVm _vm;

        /// <summary>
        /// Кофигурация проверки датчика давления
        /// </summary>
        /// <param name="identificator"></param>
        /// <param name="configData"></param>
        /// <param name="dpiConf"></param>
        /// <param name="archive"></param>
        /// <param name="ethalonsSources"></param>
        /// <param name="vm"></param>
        public PressureSensorCheckConfigurator(TestResultID identificator, PressureSensorConfig configData, DPI620GeniiConfig dpiConf,
            ITamplateArchive<PressureSensorConfig> archive, Dictionary<string, IEtalonSourceCannelFactory<Units>> ethalonsSources, PressureSensorCheckConfigVm vm)
        {
            _identificator = identificator;
            _configData = configData;
            _dpiConf = dpiConf;
            _archive = archive;
            _ethalonsSources = ethalonsSources;
            _vm = vm;
            _vm.SetSourceNames(_ethalonsSources.Keys);
            var selected = _ethalonsSources.Keys.FirstOrDefault();
            _vm.SetSelectedSourceNames(selected, _ethalonsSources[selected].ConfigViewModel);
            _vm.SetSerialNumber(_identificator.SerialNumber);

            _vm.SelectedSource += VmOnSelectedSource;
            _vm.SerialNumberCanged += VmOnSerialNumberCanged;
        }

        private void VmOnSerialNumberCanged(string s)
        {
            _identificator.SerialNumber = s;
            //TODO избавиться от дублирования
            _configData.SerialNumber = s;
        }

        private void VmOnSelectedSource(string s)
        {
            _vm.SetSelectedSourceNames(s, _ethalonsSources[s].ConfigViewModel);
        }

        /// <summary>
        /// Идентификатор проверки
        /// </summary>
        public TestResultID Identificator
        {
            get { return _identificator; }
        }

        /// <summary>
        /// Общая информаия о проверке
        /// </summary>
        public PressureSensorConfig ConfigData
        {
            get { return _configData; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DPI620GeniiConfig DpiConf
        {
            get { return _dpiConf; }
        }

        public ITamplateArchive<PressureSensorConfig> Archive
        {
            get { return _archive; }
        }
    }
}
