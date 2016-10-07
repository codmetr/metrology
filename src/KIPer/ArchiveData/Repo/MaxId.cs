namespace ArchiveData.Repo
{
    internal class MaxId
    {
        private static int _maxIn = 0;

        public static int Next { get { return _maxIn++; } }
    }
}
