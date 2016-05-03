namespace PACESeries
{
    public interface ITransport
    {
        void Send(string command, int address);
        string Receive(int address);
    }
}