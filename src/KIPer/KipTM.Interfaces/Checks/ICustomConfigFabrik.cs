using CheckFrame.ViewModel.Checks;

namespace KipTM.Checks.ViewModel.Config
{
    /// <summary>
    /// ������������� ������������� ������������������ ���������
    /// </summary>
    public interface ICustomConfigFactory
    {
        ICustomSettingsViewModel GetCustomSettings(object customSettings);
    }
}