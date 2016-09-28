using ArchiveData.DTO;
using SQLiteArchive.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTSData
{
    public class ADTSRpo
    {
        public TestResult Load(ITreeEntity root)
        {
            var result= new TestResult();
            result.CheckType = root["CheckType"].Value;
            result.Timestamp = DateTime.Parse(root["Timestamp"].Value);
            result.TargetDeviceKey = root["TargetDeviceKey"].Value;
            result.User = root["User"].Value;
            result.Note = root["Note"].Value;
            result.AtmospherePressure = root["AtmospherePressure"].Value;
            result.Temperature = root["Temperature"].Value;
            result.Humidity = root["Humidity"].Value;
            result.Client = root["Client"].Value;
            result.Channel = root["Channel"].Value;
            result.TargetDevice = LoadDeviceDescriptor(root["TargetDevice"]);
            result.Etalon = root["Etalon"].Childs.Select(LoadDeviceDescriptor).ToList();
            result.Results = root["Results"].Childs.Select(LoadTestStepResult).ToList();
            //TODO load all other

            return result;

        }

        public void Save(ITreeEntity root, TestResult result)
        {
            root["CheckType"] = new TreeEntity(root.Id) { Value = result.CheckType };
            root["Timestamp"] = new TreeEntity(root.Id) { Value = result.Timestamp.ToString() };
            root["TargetDeviceKey"] = new TreeEntity(root.Id) { Value = result.TargetDeviceKey };
            root["User"] = new TreeEntity(root.Id) { Value = result.User };
            root["Note"] = new TreeEntity(root.Id) { Value = result.Note };
            root["AtmospherePressure"] = new TreeEntity(root.Id) { Value = result.AtmospherePressure };
            root["Temperature"] = new TreeEntity(root.Id) { Value = result.Temperature };
            root["Humidity"] = new TreeEntity(root.Id) { Value = result.Humidity };
            root["Client"] = new TreeEntity(root.Id) { Value = result.Client };
            root["Channel"] = new TreeEntity(root.Id) { Value = result.Channel };
            root["TargetDevice"] = new TreeEntity(root.Id) { Value = result.Channel };
        }


        private DeviceDescriptor LoadDeviceDescriptor(ITreeEntity root)
        {
            return new DeviceDescriptor()
            {
                DeviceType = LoadDeviceTypeDescriptor(root["DeviceType"]),
                PreviousCheckTime = DateTime.Parse(root["PreviousCheckTime"].Value),
                SerialNumber = root["SerialNumber"].Value,
            };
        }

        private TreeEntity Save(DeviceDescriptor obj, ITreeEntity parrent)
        {
            var result = new TreeEntity(parrent.Id);
            result["PreviousCheckTime"] = TreeEntity.Make(result.Id, obj.PreviousCheckTime.ToString());
            result["SerialNumber"] = TreeEntity.Make(result.Id, obj.SerialNumber);
            result["DeviceType"] = Save(obj.DeviceType, result);
            return result;
        }

        private DeviceTypeDescriptor LoadDeviceTypeDescriptor(ITreeEntity root)
        {
            return new DeviceTypeDescriptor()
            {   Model = root["Model"].Value,
                DeviceCommonType = root["DeviceCommonType"].Value,
                DeviceManufacturer = root["DeviceManufacturer"].Value};
        }

        private TreeEntity Save(DeviceTypeDescriptor obj, ITreeEntity parrent)
        {
            var result = new TreeEntity(parrent.Id);
            result["Model"] = TreeEntity.Make(result.Id, obj.Model);
            result["DeviceCommonType"] = TreeEntity.Make(result.Id, obj.DeviceCommonType);
            result["DeviceManufacturer"] = TreeEntity.Make(result.Id, obj.DeviceManufacturer);
            return result;
        }

        private TestStepResult LoadTestStepResult(ITreeEntity root)
        {
            return new TestStepResult()
            {   ChannelKey = root["ChannelKey"].Value,
                CheckKey = root["CheckKey"].Value,
                StepKey = root["StepKey"].Value,
                Result = LoadAdtsPointResult(root["Result"])};
        }

        private AdtsPointResult LoadAdtsPointResult(ITreeEntity root)
        {
            return new AdtsPointResult()
            {   Point = double.Parse(root["Point"].Value),
                Tolerance = double.Parse(root["Point"].Value),
                RealValue = double.Parse(root["Point"].Value),
                Error = double.Parse(root["Point"].Value),
                IsCorrect = bool.Parse(root["Point"].Value)};
        }
    }
}
