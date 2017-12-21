namespace SQLiteArchive
{
    /// <summary>
    /// �����
    /// </summary>
    public interface IArchive
    {

        /// <summary>
        /// ��������� �� ������
        /// </summary>
        /// <typeparam name="T">��������� ��� ������</typeparam>
        /// <param name="key">���� ������</param>
        /// <param name="def">�������� � ������ ������� ������</param>
        /// <returns>����������� ������</returns>
        T Load<T>(string key, T def);
    }
}