using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPI620Genii
{
    /// <summary>
    /// Запись об одном измерении
    /// </summary>
    public class Record
    {
        public Record(int id, DateTime timeStamp, double mainReading, double secondReding)
        {
            Id = id;
            TimeStamp = timeStamp;
            MainReading = mainReading;
            SecondReding = secondReding;
        }

        /// <summary>
        /// ID record in file stream
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Timestamp create record
        /// </summary>
        public DateTime TimeStamp { get; private set; }
        /// <summary>
        /// Result call main function
        /// </summary>
        public double MainReading { get; private set; }
        /// <summary>
        /// Result call second function
        /// </summary>
        public double SecondReding { get; private set; }
    }
}
