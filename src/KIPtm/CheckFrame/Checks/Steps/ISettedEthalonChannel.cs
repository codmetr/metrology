using CheckFrame.Model.Channels;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;

namespace CheckFrame.Model.Checks.Steps
{
    /// <summary>
    /// Шаг поддерживает установку эталонного канала
    /// </summary>
    public interface ISettedEtalonChannel
    {
        /// <summary>
        /// Установить эталонный канал
        /// </summary>
        /// <param name="ehalon">эталонный канал</param>
        void SetEtalonChannel(IEtalonChannel ehalon);
    }
}
