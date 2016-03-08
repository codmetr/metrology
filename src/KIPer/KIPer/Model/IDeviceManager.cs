using KipTM.Model.Devices;

namespace KipTM.Model
{
    public interface IDeviceManager
    {
        void Init();

        PACE5000Model Pace5000 { get; }
        
        ADTSModel ADTS { get; }

        /// <summary>
        /// Запуск автоопроса модуля дискретных входов
        /// </summary>
        void StartAutoUpdate();

        /// <summary>
        /// Остановка автоопроса модуля дискретных входов
        /// </summary>
        void StopAutoUpdate();
    }
}