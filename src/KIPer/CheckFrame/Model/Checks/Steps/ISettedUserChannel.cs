using CheckFrame.Model.Channels;

namespace CheckFrame.Model.Checks.Steps
{
    /// <summary>
    /// Шаг поддерживает установку канала связи с пользователем
    /// </summary>
    public interface ISettedUserChannel
    {
        /// <summary>
        /// Утановка канала связи с пользователем
        /// </summary>
        /// <param name="userChannel"></param>
        void SetUserChannel(IUserChannel userChannel);
    }
}
