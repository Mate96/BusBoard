using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace BusBoard.ConsoleApp
{
    class Program
    {


        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //request and extract coordinates of postcode
            var client = new RestClient();
            client.BaseUrl = new Uri("http://api.postcodes.io");
            var requestPcode = new RestRequest();
            Console.WriteLine("Enter postcode:");
            requestPcode.Resource = "/postcodes/" + Console.ReadLine();
            IRestResponse<PcodeResult> responsePcode = client.Execute<PcodeResult>(requestPcode);

            //request nearest bus stop IDs to the received coordinates
            client.BaseUrl = new Uri("https://api.tfl.gov.uk");
            var requestStops = new RestRequest();
            requestStops.Resource = "StopPoint?stopTypes=NaptanPublicBusCoachTram&lat=" + responsePcode.Data.result.latitude + "&lon=" + responsePcode.Data.result.longitude;
            IRestResponse<StopsOuter> responseStops = client.Execute<StopsOuter>(requestStops);


            bool ask = true;
            int n = 0;
            Console.WriteLine("Enter number of bus stops to be displayed");
            while (ask)
            {
                bool parse = Int32.TryParse(Console.ReadLine(), out n);
                if(n<1 || !parse)
                {
                    Console.WriteLine("Please enter a positive integer");
                }
                else
                {
                    ask = false;
                }
            }

            if(responseStops.Data.stopPoints.Count < n)
            {
                n = responseStops.Data.stopPoints.Count;
                Console.WriteLine("Only " + n + " stops found within 1000m");
            }

            //display the 5 next buses stopping at the number of stops requested/found
            for (var i = 0; i < n; i++)
            {
                var requestBuses = new RestRequest();
                requestBuses.Resource = "StopPoint/" + responseStops.Data.stopPoints[i].naptanId + "/Arrivals";
                IRestResponse<List<Bus>> response = client.Execute<List<Bus>>(requestBuses);
                var result = client.Execute<List<Bus>>(requestBuses).Data;

                var resultNames = result
                    .OrderBy(b => b.expectedArrival).ThenBy(b => b.expectedArrival).ToList()    //order buses in from soonest to arrive to latest to arrive
                    .Select(b => "Expected arrival: " + b.expectedArrival.Hour + ":" + b.expectedArrival.Minute + "\n" + "Line: " + b.lineName + "\n" + "Destination: " + b.destinationName + "\n" + "Towards: " + b.towards + "\n \n")      //transform to a list of strings
                    .Take(5);       //select first 5 buses

                Console.WriteLine("\n" + string.Concat(resultNames));       //transform list to 1 string then display it
            }
            Console.ReadLine();
        }

    }

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
