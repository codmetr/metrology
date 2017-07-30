using System;
using System.Collections.Generic;

namespace ArchiveData.DTO
{
    /// <summary>
    /// Контейнер результата одной проверки
    /// </summary>
    public class TestResult
    {
        private int? _resultId = null;

        public TestResult()
        {
            TargetDevice = new DeviceWithChannel();
            Etalons = new List<DeviceWithChannel>();
            Results = new List<TestStepResult>();
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Идентификатор результата (если результат не сохранен - null)
        /// </summary>
        public int? ResultId
        {
            get { return _resultId; }
            set { _resultId = value; }
        }

        /// <summary>
        /// Тип поверки
        /// </summary>
        public string CheckType { get; set; }

        /// <summary>
        /// Дата проверки
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Ключ типа проверяемого устройства
        /// </summary>
        public string TargetDeviceKey { get; set; }

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

        /// <summary>
        /// Целевое устройство
        /// </summary>
        public DeviceWithChannel TargetDevice { get; set; }

        /// <summary>
        /// Набор использованных эталонов
        /// </summary>
        public List<DeviceWithChannel> Etalons { get; set; }

        /// <summary>
        /// Результаты
        /// </summary>
        public List<TestStepResult> Results { get; set; }
    }
}
