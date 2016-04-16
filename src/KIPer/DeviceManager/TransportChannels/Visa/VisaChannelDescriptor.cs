namespace DeviceManager.TransportChannels.Visa
{
    public class VisaChannelDescriptor: ITransportChannelDescriptor
    {
        public static string KeyType = "Visa";
        private VisaSettings _settings;

        public VisaChannelDescriptor()
        {
            Name = "Канал VISA";
            Key = KeyType;
        }

        public string Key { get; private set; }
        public string Name { get; private set; }
        public object Settings { get { return _settings; } }
    }
}
