using System;

namespace KipTM.Model.Checks
{
    public class EventArgEnd:EventArgs
    {
        private readonly bool _result;

        public EventArgEnd(bool result)
        {
            _result = result;
        }

        /// <summary>
        /// Результат
        /// </summary>
        public bool Result
        {
            get { return _result; }
        }
    }
}
