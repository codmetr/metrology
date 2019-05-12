namespace IEEE488
{
    public interface ITransportIEEE488:ITransport
    {
        /// <summary>
        /// ������� �����������
        /// </summary>
        /// <param name="address">����� �������</param>
        /// <returns>true - ������� ������������</returns>
        bool Open(int address);
        
        /// <summary>
        /// ������� �����������
        /// </summary>
        /// <param name="address"></param>
        /// <returns>true - ������� ����������� ��� ������</returns>
        bool Close(int address);
    }
}