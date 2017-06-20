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
            var client = new RestClient();
            client.BaseUrl = new Uri("https://api.tfl.gov.uk");

            var request = new RestRequest();
            Console.WriteLine("Enter stop code:");
            request.Resource = "StopPoint/" + Console.ReadLine() + "/Arrivals";

            

            IRestResponse <List<Bus>> response = client.Execute<List<Bus>>(request);
            var result = client.Execute<List<Bus>>(request).Data;

            var resultNames = result
                .OrderBy(b => b.expectedArrival).ThenBy(b => b.expectedArrival).ToList()
                .Select(b => "Expected arrival: " + b.expectedArrival.Hour + ":" + b.expectedArrival.Minute + "\n" + "Line:" + b.lineName + "\n" + "Destination: " + b.destinationName + "\n" + "Towards: " + b.towards + "\n \n")
                .Take(5);
            //for (i=0; i<resultNames.Count; i++)

            Console.WriteLine("\n" + string.Concat(resultNames));
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
    
    public class postcode
    {
        public string longitude { get; set; }
        public string latitude { get; set; }
    }
}
