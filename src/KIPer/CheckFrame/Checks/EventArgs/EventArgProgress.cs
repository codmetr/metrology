namespace CheckFrame.Model.Checks.EventArgs
{
    public class EventArgProgress : System.EventArgs
    {
        public EventArgProgress(double? progress, string note)
        {
            Note = note;
            Progress = progress;
        }
        public string Note;
        public double? Progress;
    }
}
