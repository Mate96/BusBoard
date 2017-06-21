using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBoard.Api
{
    public class Bus
    {
        public DateTime expectedArrival { get; set; }
        public string lineName { get; set; }
        public string destinationName { get; set; }
        public string towards { get; set; }
    }

    public class Postcode
    {
        public string longitude { get; set; }
        public string latitude { get; set; }
    }

    public class PcodeResult
    {
        public Postcode result { get; set; }
    }

    public class Stops
    {
        public string naptanId { get; set; }
        public short distance { get; set; }
    }

    public class StopsOuter
    {
        public List<Stops> stopPoints { get; set; }
    }
}
