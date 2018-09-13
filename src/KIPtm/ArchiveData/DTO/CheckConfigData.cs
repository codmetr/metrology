using System.Collections.Generic;
using ArchiveData.DTO;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Универсальные данные конфигурации проверки
    /// </summary>
    public class CheckConfigData
    {
        public CheckConfigData()
        {
            TargetDevice = new DeviceWithChannel() {Device = new DeviceDescriptor()};
            Etalons = new Dictionary<ChannelDescriptor, DeviceWithChannel>();
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
        public Dictionary<ChannelDescriptor, DeviceWithChannel> Etalons { get; set; }

        /// <summary>
        /// Эталон - устройство без аппаратного интерфейса
        /// </summary>
        public bool IsAnalogEtalon;

        /// <summary>
        /// Тип поверки
        /// </summary>
        public string CheckType { get; set; }

        /// <summary>
        /// Пользователь проводивший проверку
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Заказчик
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Результат внешнего осмотра
        /// </summary>
        public string VisualInspection { get; set; }

        /// <summary>
        /// Атмосферное давление, гПа
        /// </summary>
        public string AtmospherePressure { get; set; }

        /// <summary>
        /// Температура, C
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        /// Относительная влажность, %
        /// </summary>
        public string Humidity { get; set; }

        /// <summary>
        /// Напряжение питания, В
        /// </summary>
        public string SourceVoltage { get; set; }

        /// <summary>
        /// Результат опробирования
        /// </summary>
        public string Assaying { get; set; }

        /// <summary>
        /// Пометка к поверке
        /// </summary>
        public string Note { get; set; }
    }
}
