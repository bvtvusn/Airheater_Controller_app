using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airheater_SCADA.DAL
{
    internal class Datapoint
    {
        public int ID { get; set; }
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public int SensorId { get; set; }
        public Datapoint(int iD, DateTime timestamp, double value, int sensorId)
        {
            ID = iD;
            Timestamp = timestamp;
            Value = value;
            SensorId = sensorId;
        }   
    }
}
