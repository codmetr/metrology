namespace CheckFrame.Model.Checks.EventArgs
{
    public class EventArgTestStepResult
    {
        public EventArgTestStepResult(string key, object res)
        {
            Key = key;
            Result = res;
        }

        public string Key { get; private set; }

        public object Result { get; private set; }
    }
}
