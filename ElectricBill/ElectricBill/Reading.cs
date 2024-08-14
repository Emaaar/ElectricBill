using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElectricBill
{
    public class Reading
    {
        [PrimaryKey, AutoIncrement]
        public int MeterNumber { get; set; }
        public double PresentReading { get; set; }
        public double PreviousReading { get; set; }
        public double VAT { get; set; }
        public double PA { get; set; }
        public double AP { get; set; }
    }
}
