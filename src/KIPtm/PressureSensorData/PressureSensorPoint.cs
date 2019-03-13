namespace PressureSensorData
{
    /// <summary>
    /// ��������� ����� ��������
    /// </summary>
    public class PressureSensorPoint
    {
        /// <summary>
        /// ������ �����
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// ������������ �����
        /// </summary>
        public PressureSensorPointConf Config { get; set; }

        /// <summary>
        /// ��������� �����
        /// </summary>
        public PressureSensorPointResult Result { get; set; }
    }
}