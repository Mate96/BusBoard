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
            request.Resource = "StopPoint/490008660N/Arrivals";
            
            IRestResponse<List<Bus>> response = client.Execute<List<Bus>>(request);
            var result = client.Execute<List<Bus>>(request).Data;

            var resultNames = result.Select(b => b.expectedArrival + b.destinationName);

            //for (i=0; i<resultNames.Count; i++)

            Console.WriteLine(string.Concat(resultNames));
            Console.ReadLine();
        }

    }

    public class Bus
    {
        public DateTime expectedArrival { get; set; }
        public string destinationName { get; set; }
    }
    
}
