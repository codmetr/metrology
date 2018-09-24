using PACESeries;

namespace PACESeriesUtil
{
    /// <summary>
    /// VM описания модели
    /// </summary>
    public class ModelDescriptor
    {
        public string Name { get; private set; }
        internal readonly PACESeries.Model Id;

        public ModelDescriptor(Model id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}