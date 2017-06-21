using System.Web.Mvc;
using BusBoard.Web.Models;
using BusBoard.Web.ViewModels;
using BusBoard.Api;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;


namespace BusBoard.Web.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public ActionResult BusInfo(PostcodeSelection selection)
    {
            // Add some properties to the BusInfo view model with the data you want to render on the page.
            // Write code here to populate the view model with info from the APIs.
            // Then modify the view (in Views/Home/BusInfo.cshtml) to render upcoming buses.

            var api = new api();
            List<string> coordinates = api.Pcode(selection.Postcode);
            List<Stops> Stops = api.Stops(coordinates);

            //display the 5 next buses stopping at the number of stops requested/found
            List<List<Bus>> stopInfo = new List<List<Bus>>();           
            for (var i = 0; i < 2; i++)
            {
                var resultNames = api.Bus(Stops, i)
                    .OrderBy(b => b.expectedArrival).ThenBy(b => b.expectedArrival).ToList()    //order buses in from soonest to arrive to latest to arrive
                    //.Select(b => "Expected arrival: " + b.expectedArrival.Hour + ":" + b.expectedArrival.Minute + "\n" + "Line: " + b.lineName + "\n" + "Destination: " + b.destinationName + "\n" + "Towards: " + b.towards + "\n \n")      //transform to an IEnumerable of strings
                    .Take(5);       //select first 5 buses

                stopInfo.Add(resultNames.ToList());
            }

            var info = new BusInfo(selection.Postcode, stopInfo);
            return View(info);
    }

    public ActionResult About()
    {
      ViewBag.Message = "Information about this site";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Contact us!";

      return View();
    }
  }
}