using System;
using System.Collections.Generic;

namespace ArchiveData.DTO
{
    /// <summary>
    /// Контейнер результата одной проверки
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TestResultID
    {
        private int? _id = null;

        /// <summary>
        /// Контейнер результата одной проверки
        /// </summary>
        public TestResultID()
        {
            //Results = new List<TestStepResult>();
            Timestamp = DateTime.Now;
            CreateTime = DateTime.Now;
        }

        /// <summary>
        /// Идентификатор результата (если результат не сохранен - null)
        /// </summary>
        public int? Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Дата создания проверки
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Дата проверки
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Ключ типа проверяемого устройства
        /// </summary>
        public string TargetDeviceKey { get; set; }

        /// <summary>
        /// Tипа проверяемого устройства
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// Серийны номер
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Общий результат проверки
        /// </summary>
        public string CommonResult { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Note { get; set; }

        public override int GetHashCode()
        {
            return ComparrerId.GetHashVal(this);
        }

        public class ComparrerId:IEqualityComparer<TestResultID>
        {
            public bool Equals(TestResultID x, TestResultID y)
            {
                if (x.Id == null || y.Id == null)
                    return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(TestResultID obj)
            {
                return GetHashVal(obj);
            }

            public static int GetHashVal(TestResultID obj)
            {
                return obj.Id.HasValue ? obj.Id.Value : obj.Id.GetHashCode();
            }
        }
    }
}
