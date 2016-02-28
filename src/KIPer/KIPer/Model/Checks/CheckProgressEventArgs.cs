using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    public class CheckProgressEventArgs
    {
        public CheckProgressEventArgs(double? progress, string note)
        {
            Note = note;
            Progress = progress;
        }
        public string Note;
        public double? Progress;
    }
}
