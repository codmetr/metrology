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
            result.TargetDevice = new DeviceDescriptor()
            {
                DeviceType = new DeviceTypeDescriptor()
                {
                    Model = root["TargetDevice"]["DeviceType"]["Model"].Value,
                    DeviceCommonType = root["TargetDevice"]["DeviceType"]["DeviceCommonType"].Value,
                    DeviceManufacturer = root["TargetDevice"]["DeviceType"]["DeviceManufacturer"].Value,
                },
                PreviousCheckTime = DateTime.Parse(root["TargetDevice"]["PreviousCheckTime"].Value)
                SerialNumber = root["TargetDevice"]["SerialNumber"].Value,
            };


            //TODO load all other

            return result;

        }

        public void Save(ITreeEntity root, TestResult result)
        {

        }
    }
}
