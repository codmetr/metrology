using CheckFrame.Model.Channels;

namespace CheckFrame.Model.Checks.Steps
{
    /// <summary>
    /// Шаг поддерживает установку эталонного канала
    /// </summary>
    public interface ISettedEthalonChannel
    {
        /// <summary>
        /// Установить эталонный канал
        /// </summary>
        /// <param name="ehalon">эталонный канал</param>
        void SetEthalonChannel(IEthalonChannel ehalon);
    }
}
