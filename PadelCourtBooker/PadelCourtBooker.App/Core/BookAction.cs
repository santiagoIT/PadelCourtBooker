using System.Linq;
using System.Net;
using Ninject;
using PadelCourtBooker.App.Services;
using RestSharp;

namespace PadelCourtBooker.App.Core
{
  class BookAction : IBookAction
  {
    private IConsoleOutputService _consoleService;
    public BookAction()
    {
      _consoleService = App.Kernel.Get<IConsoleOutputService>();
    }

    public bool Silent { get; set; }

    public bool Execute(TimeSlotInfo timeSlot)
    {
      if (!Silent)
      {
        _consoleService.WriteStartAction("Booking court");
      }

      var bookingSessionService = App.Kernel.Get<IBookingSessionService>();

      var client = new RestClient(AppConstants.Host);
      client.FollowRedirects = false;
      var request = new RestRequest(AppConstants.BookCourtUrl, DataFormat.Json);

      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");


      request.AddHeader("Accept", "text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,*/*;q=0.8");
      request.AddHeader("Accept-Language", "en-US,en;q=0.5");
      request.AddHeader("Origin", AppConstants.Host);

      request.AddParameter("token", timeSlot.Token, ParameterType.QueryString);

      request.AddParameter("ctl00$ContentPlaceHolderContenido$ButtonPagoCentro", "Reservar", ParameterType.GetOrPost);
      request.AddParameter("ctl00$ContentPlaceHolderContenido$CheckBoxAceptoCondicionesLegales", "on", ParameterType.GetOrPost);
      request.AddParameter("__EVENTTARGET", string.Empty);
      request.AddParameter("__EVENTARGUMENT", string.Empty);

      request.AddCookie("cb-enabled", "accepted");
      // add session cookies
      foreach (var cookie in bookingSessionService.Cookies)
      {
        request.AddCookie(cookie.Name, cookie.Value);
      }

      var response = client.Post(request);

      // booking was successful if a redirect to booking confirmation page is returned
      if (response.StatusCode == HttpStatusCode.Redirect)
      {
        // find redirect url
        var locationHeader = response.Headers.FirstOrDefault(x => x.Name == AppConstants.LocationHeader);
        if (locationHeader != null && locationHeader.Value != null && locationHeader.Value.ToString().StartsWith(AppConstants.BookingConfirmationUrl))
        {
          _consoleService.WriteSuccess("Court booked.");
          return true;
        }
      }

      if (!Silent)
      {
        _consoleService.WriteError("Failed to book court. Most likely court is not yet bookable.");
        _consoleService.WriteError($"  HttpStatusCode: {response.StatusCode}");
      }

      return false;
    }
  }
}
