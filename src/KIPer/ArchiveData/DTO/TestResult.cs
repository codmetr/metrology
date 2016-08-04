using System;
using System.Collections.Generic;

namespace ArchiveData.DTO
{
    public class TestResult
    {
        public TestResult()
        {
            Etalon = new List<DeviceDescriptor>();
            Results = new List<TestStepResult>();
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Пользователь проводивший проверку
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Пометка к поверке
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Дата проверки
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Атмосферное давление, гПа
        /// </summary>
        public string AtmospherePressure { get; set; }

        /// <summary>
        /// Температура
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        /// Относительная влажность, %
        /// </summary>
        public string Humidity { get; set; }

        /// <summary>
        /// Заказчик
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Тип поверки
        /// </summary>
        public string CheckType { get; set; }

        /// <summary>
        /// Канал устройства
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Ключь типа проверяемого устройства
        /// </summary>
        public string TargetDeviceKey { get; set; }

        /// <summary>
        /// Целевое устройство
        /// </summary>
        public DeviceDescriptor TargetDevice { get; set; }

        /// <summary>
        /// Набор использованных эталонов
        /// </summary>
        public List<DeviceDescriptor> Etalon { get; set; }

        /// <summary>
        /// Результаты
        /// </summary>
        public List<TestStepResult> Results { get; set; }
    }
}
