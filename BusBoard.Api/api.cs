using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using RestSharp;

namespace BusBoard.Api
{
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
