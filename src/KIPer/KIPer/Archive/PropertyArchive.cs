using System.Collections.Generic;
using KipTM.Archive.DataTypes;

namespace KipTM.Archive
{
    public class PropertyArchive : ArchiveBase
    {
        public static ArchiveBase GetDefault()
        {
            return new ArchiveBase(GetDefaultData());
        }

        public PropertyArchive():base()
        {}

        public PropertyArchive(List<ArchivedKeyValuePair> data)
            : base(data)
        {}

        #region GetDefault
        public static List<ArchivedKeyValuePair> GetDefaultData()
        {
            return new List<ArchivedKeyValuePair> { new ArchivedKeyValuePair(CheckUVModel.Key, GetDefaultForCheckUV()) };
        }

        private static List<ArchivedKeyValuePair> GetDefaultForCheckUV()
        {
            return new List<ArchivedKeyValuePair>() { new ArchivedKeyValuePair(TestErrorUv.Key, GetDefaultForTestErrorUv()) };
        }

        private static List<ArchivedKeyValuePair> GetDefaultForTestErrorUv()
        {
            var points = new List<TestErrorUVPoint>()
            {
                new TestErrorUVPoint(5000.0, 1, 419.5,  4580.5, 0.0,        5.0,    10.0,   0.14),
                new TestErrorUVPoint(5000.0, 1, 507.0,  4493.0, 300.0,      5.0,    11.6,   0.14),
                new TestErrorUVPoint(5000.0, 1, 594.0,  4406.0, 600.0,      10.0,   13.2,   0.16),
                new TestErrorUVPoint(5000.0, 1, 680.5,  4319.5, 900.0,      10.0,   14.8,   0.16),
                new TestErrorUVPoint(5000.0, 1, 767.5,  4232.5, 1200.0,     10.0,   16.4,   0.18),
                new TestErrorUVPoint(5000.0, 1, 854.5,  4145.5, 1500.0,     10.0,   18.0,   0.18),
                new TestErrorUVPoint(5000.0, 1, 941.5,  4058.5, 1800.0,     10.0,   19.6,   0.18),
                new TestErrorUVPoint(5000.0, 1, 1028.5, 3971.5, 2100.0,     10.0,   21.2,   0.18),
                new TestErrorUVPoint(5000.0, 1, 1115.5, 3884.5, 2400.0,     10.0,   22.8,   0.20),
                new TestErrorUVPoint(5000.0, 1, 1202.5, 3797.5, 2700.0,     10.0,   24.4,   0.20),
                new TestErrorUVPoint(5000.0, 1, 1289.5, 3710.5, 3000.0,     15.0,   26.0,   0.20),
                new TestErrorUVPoint(5000.0, 1, 1376.0, 3624.0, 3300.0,     15.0,   27.6,   0.20),
                new TestErrorUVPoint(5000.0, 1, 1463.0, 3537.0, 3600.0,     15.0,   29.2,   0.20),
                new TestErrorUVPoint(5000.0, 1, 1550.0, 3450.0, 3900.0,     15.0,   30.8,   0.20),
                new TestErrorUVPoint(5000.0, 1, 1637.0, 3363.0, 4200.0,     15.0,   32.4,   0.20),
                new TestErrorUVPoint(5000.0, 1, 1724.0, 3276.0, 4500.0,     15.0,   34.0,   0.20),
                new TestErrorUVPoint(5000.0, 1, 1811.0, 3189.0, 4800.0,     15.0,   35.6,   0.23),
                new TestErrorUVPoint(5000.0, 1, 1869.0, 3131.0, 5000.0,     15.0,   36.67,  0.23),
                new TestErrorUVPoint(5000.0, 1, 1898.0, 3102.0, 5100.0,     15.0,   37.2,   0.23),
                new TestErrorUVPoint(5000.0, 1, 1985.0, 3015.0, 5400.0,     15.0,   38.8,   0.23),
                new TestErrorUVPoint(5000.0, 1, 2072.0, 2928.0, 5700.0,     15.0,   40.4,   0.23),
                new TestErrorUVPoint(5000.0, 1, 2158.5, 2841.5, 6000.0,     15.0,   42.0,   0.23),
                new TestErrorUVPoint(5000.0, 1, 2332.5, 2667.5, 6600.0,     15.0,   45.02,  0.23),
                new TestErrorUVPoint(5000.0, 1, 2506.5, 2493.5, 7200.0,     15.0,   48.04,  0.23),
                new TestErrorUVPoint(5000.0, 1, 2680.5, 2319.5, 7800.0,     15.0,   51.06,  0.23),
                new TestErrorUVPoint(5000.0, 1, 2854.0, 2146.0, 8400.0,     15.0,   54.08,  0.24),
                new TestErrorUVPoint(5000.0, 1, 3028.0, 1972.0, 9000.0,     15.0,   58.0,   0.24),
                new TestErrorUVPoint(5000.0, 1, 3318.0, 1682.0, 10000.0,    15.0,   63.33,  0.24),
                new TestErrorUVPoint(5000.0, 1, 3607.5, 1392.5, 11000.0,    15.0,   68.66,  0.24),
                new TestErrorUVPoint(5000.0, 1, 3897.5, 1102.5, 12000.0,    15.0,   74.0,   0.24),
                new TestErrorUVPoint(5000.0, 1, 4187.5, 812.5,  13000.0,    15.0,   79.33,  0.24),
                new TestErrorUVPoint(5000.0, 1, 4477.0, 523.0,  14000.0,    15.0,   84.66,  0.26),
                new TestErrorUVPoint(5000.0, 1, 4767.0, 233.0,  15000.0,    15.0,   90.0,   0.26),
            };
            return new List<ArchivedKeyValuePair>() { new ArchivedKeyValuePair(TestErrorUv.KeyPropertyPoints, points) };
        }
        #endregion
    }
}
