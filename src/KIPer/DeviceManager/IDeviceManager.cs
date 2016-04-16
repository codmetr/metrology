namespace DeviceManager
{
    public interface IDeviceManager
    {
        void Init();

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