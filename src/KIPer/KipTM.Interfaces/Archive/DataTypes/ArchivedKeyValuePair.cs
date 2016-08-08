namespace KipTM.Archive.DataTypes
{
    public class ArchivedKeyValuePair
    {
        public ArchivedKeyValuePair()
        {
            Key = string.Empty;
            Value = null;
        }

        public ArchivedKeyValuePair(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public object Value { get; set; }
    }
}
