﻿using System;
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
        private CheckPressureLogicConfig _logic;

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
            FillVM(_vm, configData, archive, ethalonsSources);
        }

        /// <summary>
        /// Заполнить визуальную модель базовыми 
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="configData"></param>
        /// <param name="archive"></param>
        /// <param name="ethalonsSources"></param>
        private void FillVM(PressureSensorCheckConfigVm vm, PressureSensorConfig configData, ITamplateArchive<PressureSensorConfig> archive, Dictionary<string, IEtalonSourceCannelFactory<Units>> ethalonsSources)
        {
            vm.SetSourceNames(ethalonsSources.Keys);
            var selected = ethalonsSources.Keys.FirstOrDefault();
            vm.SetSelectedSourceNames(selected, ethalonsSources[selected]?.ConfigViewModel);
            vm.SetSerialNumber(_identificator.SerialNumber);
            FillCommonData(vm.CommonData, configData);
            FillLogicConf(vm.Config, configData);
            vm.SelectedSource += VmOnSelectedSource;
            vm.SerialNumberCanged += VmOnSerialNumberCanged;
        }

        private void FillLogicConf(CheckPressureLogicConfigVm vmConfig, PressureSensorConfig configData)
        {
            _logic = new CheckPressureLogicConfig(configData, vmConfig);
        }

        /// <summary>
        /// Заполнить базовые не технические данные поверки
        /// </summary>
        /// <param name="vmCommonData"></param>
        /// <param name="configData"></param>
        private void FillCommonData(PressureSensorOrgVm vmCommonData, PressureSensorConfig configData)
        {
            vmCommonData.SetAllValues(GetCurentValue(configData));
            vmCommonData.EthalonPressure.SetAllValues(GetCurentValue(configData.EtalonPressure));
            vmCommonData.EthalonOutSignal.SetAllValues(GetCurentValue(configData.EtalonOut));
            vmCommonData.ConfigChanged += UpdateFromCommonData;
            vmCommonData.EthalonPressure.ConfigChanged += EthalonPressureOnConfigChanged;
            vmCommonData.EthalonOutSignal.ConfigChanged += EthalonOutSignalOnConfigChanged;
        }

        /// <summary>
        /// Пользователем обновлена конфигурация эталона давления
        /// </summary>
        /// <param name="ethalonDescriptorData"></param>
        private void EthalonPressureOnConfigChanged(EthalonDescriptorVm.EthalonDescriptorData ethalonDescriptorData)
        {
            UpdateEthalon(_configData.EtalonPressure, ethalonDescriptorData);
        }

        /// <summary>
        /// Пользователем обновлена конфигурация эталона выходного сигнала
        /// </summary>
        /// <param name="ethalonDescriptorData"></param>
        private void EthalonOutSignalOnConfigChanged(EthalonDescriptorVm.EthalonDescriptorData ethalonDescriptorData)
        {
            UpdateEthalon(_configData.EtalonOut, ethalonDescriptorData);
        }

        /// <summary>
        /// Пользователем обновлена конфигурация эталона
        /// </summary>
        /// <param name="conf"></param>
        /// <param name="data"></param>
        private void UpdateEthalon(EtalonDescriptor conf,
            EthalonDescriptorVm.EthalonDescriptorData data)
        {
            conf.Title = data.Title;
            conf.SensorType = data.SensorType;
            conf.SerialNumber = data.SerialNumber;
            conf.RegNum = data.RegNum;
            conf.Category = data.Category;
            conf.ErrorClass = data.ErrorClass;
            conf.CheckCertificateNumber = data.CheckCertificateNumber;
            conf.CheckCertificateDate = data.CheckCertificateDate;
        }

        /// <summary>
        /// Преобразовать конфигурацию проверки в транспортные данные для VM
        /// </summary>
        /// <param name="configData"></param>
        /// <returns></returns>
        private PressureSensorOrgVm.PressureSensorOrgData GetCurentValue(PressureSensorConfig configData)
        {
            return new PressureSensorOrgVm.PressureSensorOrgData(configData.User, configData.ChiefLab,
                configData.ReportNumber, configData.ReportDate, configData.CertificateNumber,
                configData.CertificateDate, configData.Validity, configData.Master, configData.Name, configData.RegNum,
                configData.SensorType, configData.SensorModel, configData.NumberLastCheck, configData.SerialNumber,
                configData.ChecklLawBase, configData.CheckedParameters, configData.Company, configData.Temperature,
                configData.Humidity, configData.DayPressure, configData.CommonVoltage, configData.Note);
        }

        /// <summary>
        /// Преобразовать конфигурацию эталонов в транспортные данные для VM
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private EthalonDescriptorVm.EthalonDescriptorData GetCurentValue(EtalonDescriptor data)
        {
            return new EthalonDescriptorVm.EthalonDescriptorData(data.Title, data.SensorType, data.SerialNumber,
                data.RegNum, data.Category, data.ErrorClass, data.CheckCertificateNumber, data.CheckCertificateDate);
        }

        /// <summary>
        /// Изменены данные пользователем
        /// </summary>
        /// <param name="data"></param>
        private void UpdateFromCommonData(PressureSensorOrgVm.PressureSensorOrgData data)
        {
            _configData.User = data.User;
            _configData.ChiefLab = data.ChiefLab;
            _configData.ReportNumber = data.ReportNumber;
            _configData.ReportDate = data.ReportDate;
            _configData.CertificateNumber = data.CertificateNumber;
            _configData.CertificateDate = data.CertificateDate;
            _configData.Validity = data.Validity;
            _configData.Master = data.Master;
            _configData.Name = data.Name;
            _configData.RegNum = data.RegNum;
            _configData.SensorType = data.SensorType;
            _configData.SensorModel = data.SensorModel;
            _configData.NumberLastCheck = data.NumberLastCheck;

            _identificator.SerialNumber = data.SerialNumber;
            //TODO избавиться от дублирования
            _configData.SerialNumber = data.SerialNumber;

            _configData.ChecklLawBase = data.ChecklLawBase;
            _configData.CheckedParameters = data.CheckedParameters;
            _configData.Company = data.Company;

            _identificator.Note = data.Note;
            //TODO избавиться от дублирования
            _configData.Note = data.Note;

            _configData.Temperature = data.Temperature;
            _configData.Humidity = data.Humidity;
            _configData.DayPressure = data.DayPressure;
            _configData.CommonVoltage = data.CommonVoltage;
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

        public CheckPressureLogicConfig Logic
        {
            get { return _logic; }
        }
    }
}
