using System;

namespace ArchiveData.DTO
{
    /// <summary>
    /// Описатель результата конкретного шага
    /// </summary>
    [Obsolete]
    public class TestStepResult
    {
        /// <summary>
        /// Описатель результата конкретного шага
        /// </summary>
        public TestStepResult()
        {
        }

        /// <summary>
        /// Описатель результата конкретного шага
        /// </summary>
        /// <param name="checkKey"></param>
        /// <param name="channelKey"></param>
        /// <param name="stepKey"></param>
        /// <param name="result"></param>
        public TestStepResult(string checkKey, string channelKey, string stepKey, object result)
            : this()
        {
            ChannelKey = channelKey;
            CheckKey = checkKey;
            StepKey = stepKey;
            Result = result;
        }

        /// <summary>
        /// Ключ канала
        /// </summary>
        public string ChannelKey { get; set; }

        /// <summary>
        /// Ключ типа проверки
        /// </summary>
        public string CheckKey { get; set; }

        /// <summary>
        /// Ключь результата
        /// </summary>
        public string StepKey { get; set; }

        public object Result { get; set; }
    }
}
