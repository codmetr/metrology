namespace Tools.View.Busy
{
    /// <summary>
    /// ���������������� ��������� ������� �������� "� �����"
    /// </summary>
    public interface IBusy
    {
        /// <summary>
        /// ��������� "� �����"
        /// </summary>
        bool IsBusy { get; set; }
    }
}