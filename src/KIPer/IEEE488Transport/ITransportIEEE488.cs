namespace IEEE488
{
    public interface ITransportIEEE488
    {
        bool Open(int address);
        bool Close(int address);
        bool Send(int address, string data);
        string Receive(int address);
    }
}