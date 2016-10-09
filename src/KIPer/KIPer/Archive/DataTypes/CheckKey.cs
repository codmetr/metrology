using System;
using CheckFrame.Archive;

namespace KipTM.Archive.DataTypes
{
    public class CheckKey
    {
        public static string KeyString{get { return "CheckKey"; }}

        private const string DevTypeKey = "DevType";
        private const string SerialNumberKey = "SerialNumber";
        private const string TimestampKey = "Timestamp";

        private string _deviceType;
        private string _serialNumber;
        private DateTime _timestamp;

        public static CheckKey Load(IPropertyPool propertyPool)
        {
            var deviceType = propertyPool.GetProperty<string>(DevTypeKey);
            var serialNumber = propertyPool.GetProperty<string>(SerialNumberKey);
            var timestamp = propertyPool.GetProperty<DateTime>(TimestampKey);
            return new CheckKey(deviceType, serialNumber, timestamp);
        }

        public CheckKey(string deviceType, string serialNumber, DateTime timestamp)
        {
            _deviceType = deviceType;
            _serialNumber = serialNumber;
            _timestamp = timestamp;
        }

        public void Save(IArchivePool archivePool)
        {
            archivePool.AddOrUpdateProperty(DevTypeKey, _deviceType);
            archivePool.AddOrUpdateProperty(SerialNumberKey, _serialNumber);
            archivePool.AddOrUpdateProperty(TimestampKey, _timestamp);
        }

        public string DeviceType
        {
            get { return _deviceType; }
//            set { _deviceType = value; }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
//            set { _serialNumber = value; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
//            set { _timestamp = value; }
        }
    }
}
