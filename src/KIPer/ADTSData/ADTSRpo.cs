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
            result.Results = root["Results"].Childs.Select(LoadDeviceDescriptor).ToList();

            //TODO load all other

            return result;

        }

        public void Save(ITreeEntity root, TestResult result)
        {

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

        private DeviceTypeDescriptor LoadDeviceTypeDescriptor(ITreeEntity root)
        {
            return new DeviceTypeDescriptor()
                {
                    Model = root["Model"].Value,
                    DeviceCommonType = root["DeviceCommonType"].Value,
                    DeviceManufacturer = root["DeviceManufacturer"].Value,
                };
        }

        private TestStepResult LoadTestStepResult(ITreeEntity root)
        {
            return new TestStepResult()
            {
                ChannelKey = root["ChannelKey"].Value,
                CheckKey = root["CheckKey"].Value,
            }
        }
    }
}
