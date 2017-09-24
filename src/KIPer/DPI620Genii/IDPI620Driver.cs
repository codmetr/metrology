namespace DPI620Genii
{
    public interface IDPI620Driver
    {
        void Close();
        double GetValue(int slotId, string unitCode);
        void Open();
        void SetUnits(int slotId, string unitCode);
    }
}