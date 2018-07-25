using CheckFrame.ViewModel.Checks;

namespace KipTM.Checks.ViewModel.Config
{
    /// <summary>
    /// ������������� ������������� ������������������ ���������
    /// </summary>
    public interface ICustomConfigFactory
    {
        /// <summary>
        /// ������������ ���������� ������ ������������������ ���������
        /// </summary>
        /// <param name="customSettings">����������������� ���������</param>
        /// <returns>���������� ������ ������������������ ���������</returns>
        ICustomSettingsViewModel GetCustomSettings(object customSettings);
    }
}