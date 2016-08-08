namespace KipTM.Model.TransportChannels
{
    public interface ITransportChannelType
    {
        string Key { get; }
 
        string Name { get; }

        object Settings { get; }
    }
}