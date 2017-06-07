using SQLite;
using System;

namespace DAL
{
    [Table("Tides")]
    public class TideDataObject
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Station { get; set; }
        public string DateDisplay { get; set; }
        public DateTime DateActual { get; set; }
        public string Day { get; set; }
        public string Level { get; set; }
        public string Time { get; set; }
        public string PredFt { get; set; }
        public string PredCm { get; set; }
    }
}
