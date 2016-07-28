using System;

namespace MarkerService.Filler
{
    public class FillerAttribute:Attribute
    {
        public FillerAttribute(object key)
        {
            Key = key;
        }

        public object Key { get; private set; }
    }
}
