namespace DPI620Genii
{
    public interface IDPI620Driver
    {
        void Close();
        double GetValue(int slotId);
        void Open();
        //void SetUnits(int slotId, string unitCode);
    }
}