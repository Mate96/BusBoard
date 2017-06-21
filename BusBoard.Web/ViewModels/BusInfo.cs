using System.Collections.Generic;
using BusBoard.Api;

namespace BusBoard.Web.ViewModels
{
  public class BusInfo
  {
    public BusInfo(string postCode, List<List<Bus>> stopInfo)
    {
      PostCode = postCode;
      Stop1 = stopInfo[0];
      Stop2 = stopInfo[1];
    }

    public string PostCode { get; set; }
    public List<Bus> Stop1 { get; set; }
    public List<Bus> Stop2 { get; set; }

    }

}