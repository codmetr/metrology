namespace PACESeriesUtil
{
    /// <summary>
    /// Описатель канала
    /// </summary>
    public class ChannelDiscriptor
    {
        private string _name;

        /// <summary>
        /// Описатель канала
        /// </summary>
        /// <param name="name"></param>
        public ChannelDiscriptor(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Название
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
    }
}