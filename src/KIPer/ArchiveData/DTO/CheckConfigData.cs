using System.Collections.Generic;
using ArchiveData.DTO;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Конфигурация конкретной проверки
    /// </summary>
    public class CheckConfigData
    {
        public CheckConfigData()
        {
            TargetDevice = new DeviceWithChannel() {Device = new DeviceDescriptor()};
            Ethalons = new Dictionary<ChannelDescriptor, DeviceWithChannel>();
        }
        /// <summary>
        /// Ключ типа методики проверки
        /// </summary>
        public string CheckTypeKey;

        /// <summary>
        /// Целевое устройство
        /// </summary>
        public DeviceWithChannel TargetDevice { get; set; }

        /// <summary>
        /// Набор использованных эталонов
        /// </summary>
        /// <remarks>
        /// ключь - измерительный канал целевого устройства
        /// значение - описатель выбранного устройства и
        /// </remarks>
        public Dictionary<ChannelDescriptor, DeviceWithChannel> Ethalons { get; set; }


        /// <summary>
        /// Эталон - устройство без аппаратного интерфейса
        /// </summary>
        public bool IsAnalogEthalon;
    }
}
