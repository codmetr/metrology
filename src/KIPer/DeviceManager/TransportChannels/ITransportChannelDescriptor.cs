namespace DeviceManager.TransportChannels
{
    public interface ITransportChannelDescriptor
    {
        string Key { get; }
 
        string Name { get; }

        object Settings { get; }
    }
}