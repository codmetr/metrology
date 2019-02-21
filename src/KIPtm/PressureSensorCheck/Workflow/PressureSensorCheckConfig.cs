﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    public class PressureSensorCheckConfig
    {
        private readonly TestResultID _identificator;
        private readonly PressureSensorConfig _configData;
        private readonly DPI620GeniiConfigVm _dpiConf;
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
        public PressureSensorCheckConfig(TestResultID identificator, PressureSensorConfig configData, DPI620GeniiConfigVm dpiConf,
            ITamplateArchive<PressureSensorConfig> archive, Dictionary<string, IEtalonSourceCannelFactory<Units>> ethalonsSources, PressureSensorCheckConfigVm vm)
        {
            _identificator = identificator;
            _configData = configData;
            _dpiConf = dpiConf;
            _archive = archive;
            _ethalonsSources = ethalonsSources;
            _vm = vm;

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
        public DPI620GeniiConfigVm DpiConf
        {
            get { return _dpiConf; }
        }

        public ITamplateArchive<PressureSensorConfig> Archive
        {
            get { return _archive; }
        }
    }
}
