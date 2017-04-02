using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchiveData.DTO
{
    /// <summary>
    /// Описатель результата конкретного шага
    /// </summary>
    public class TestStepResult
    {
        private string _checkKey;
        private string _channelKey;
        private string _stepKey;
        private object _result;

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
            _channelKey = channelKey;
            _checkKey = checkKey;
            _stepKey = stepKey;
            _result = result;
        }

        /// <summary>
        /// Ключ канала
        /// </summary>
        public string ChannelKey
        {
            get { return _channelKey; }
            set { _channelKey = value; }
        }

        /// <summary>
        /// Ключ типа проверки
        /// </summary>
        public string CheckKey
        {
            get { return _checkKey; }
            set { _checkKey = value; }
        }
        /// <summary>
        /// Ключь результата
        /// </summary>
        public string StepKey
        {
            get { return _stepKey; }
            set { _stepKey = value; }
        }

        public object Result
        {
            get { return _result; }
            set { _result = value; }
        }
    }
}
