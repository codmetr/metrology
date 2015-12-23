using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIPer.Model
{
    public class PACE5000Model
    {
        public PACE5000Model(string title)
        {
            Title = title;
        }

        public string Title
        {
            get;
            private set;
        }
    }
}
