using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zaliczenie
{
    class Incident
    {
        public ObjectId Id { get; set; }
        public long IncidntNum { get; set; }
        public string Category { get; set; }
        public string Descript { get; set; }
        public string DayOfWeek { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string PdDistrict { get; set; }
        public string Address { get; set; }
        public string Resolution { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Location { get; set; }
        public long PdId { get; set; }
    }
}
