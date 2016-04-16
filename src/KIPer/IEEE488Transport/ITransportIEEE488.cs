namespace IEEE488
{
    public interface ITransportIEEE488ByAddress
    {
        bool Open(int address);
        bool Close(int address);
        bool Send(int address, string data);
        string Receive(int address);
    }

    public interface ITransportIEEE488
    {
        bool Open(int address);
        bool Close(int address);
        bool Send(string data);
        string Receive();
    }
}