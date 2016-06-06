using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
{
    public class FakeAdtsState
    {
        private DateTime startToAimPt;
        private DateTime startToAimPs;
        private DateTime startToAimAlt;

        private double _ptAim;
        private double _psAim;
        private double _altAim;

        public double PT;
        public double PS;
        public double ALT;

        public double RatePt;
        public double RatePs;
        public double RateAlt;

        public void SetAimPT(double aim, double rate)
        {
            
        }
    }
}
