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


        static void Main()
        {
            var api = new api();

            Console.WriteLine("Enter postcode");
            List<string> coordinates = api.Pcode(Console.ReadLine());
            List<Stops> Stops = api.Stops(coordinates);
            

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

            if(Stops.Count < n)
            {
                n = Stops.Count;
                Console.WriteLine("Only " + n + " stops found within 1000m");
            }



            //display the 5 next buses stopping at the number of stops requested/found
            for (var i = 0; i < n; i++)
            {
                var resultNames = api.Bus(Stops, i)
                    .OrderBy(b => b.expectedArrival).ThenBy(b => b.expectedArrival).ToList()    //order buses in from soonest to arrive to latest to arrive
                    .Select(b => "Expected arrival: " + b.expectedArrival.Hour + ":" + b.expectedArrival.Minute + "\n" + "Line: " + b.lineName + "\n" + "Destination: " + b.destinationName + "\n" + "Towards: " + b.towards + "\n \n")      //transform to a list of strings
                    .Take(5);       //select first 5 buses

                Console.WriteLine("\n" + string.Concat(resultNames));       //transform list to 1 string then display it
            }
            Console.ReadLine();
        }

    }


    public class api        //functions responsible for calling apis
    {
        RestClient client = new RestClient();
        public api()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;           
        }

        public List<string> Pcode(string pcode)
        {
            //request and extract coordinates of postcode           
            client.BaseUrl = new Uri("http://api.postcodes.io");
            var request = new RestRequest();
            request.Resource = "/postcodes/" + pcode;
            IRestResponse<PcodeResult> response = client.Execute<PcodeResult>(request);
            List<string> coordinates = new List<string>();
            coordinates.Add(response.Data.result.latitude);
            coordinates.Add(response.Data.result.longitude);
            return coordinates;
        }


        public List<Stops> Stops(List<string> coordinates)
        {
            //request nearest bus stop IDs to the received coordinates
            client.BaseUrl = new Uri("https://api.tfl.gov.uk");
            var request = new RestRequest();
            request.Resource = "StopPoint?stopTypes=NaptanPublicBusCoachTram&lat=" + coordinates[0] + "&lon=" + coordinates[1];
            IRestResponse<StopsOuter> response = client.Execute<StopsOuter>(request);
            return response.Data.stopPoints;
        }

        public List<Bus> Bus(List<Stops> Stops, int i)
        {
            var request = new RestRequest();
            request.Resource = "StopPoint/" + Stops[i].naptanId + "/Arrivals";
            IRestResponse<List<Bus>> response = client.Execute<List<Bus>>(request);
            return client.Execute<List<Bus>>(request).Data;
        }
    }

}
