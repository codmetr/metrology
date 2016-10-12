namespace CheckFrame.Model.Checks.EventArgs
{
    public class EventArgEnd:System.EventArgs
    {
        private readonly bool _result;
        private readonly string _key;

        public EventArgEnd(string key, bool result)
        {
            _result = result;
            _key = key;
        }

        /// <summary>
        /// Ид к чему относится результат
        /// </summary>
        public string Key
        {
            get { return _key; }
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
