using System;
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
    private IPushNotificationService _pushNotificationService;
    public BookAction()
    {
      _consoleService = App.Kernel.Get<IConsoleOutputService>();
      _pushNotificationService = App.Kernel.Get<IPushNotificationService>();
    }

    public bool Silent { get; set; }

    public bool TimeSlotAvailable { get; private set; }

    public bool Execute(TimeSlotInfo timeSlot)
    {
      TimeSlotAvailable = true;
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
          var courtName = AppUtilities.GetDescriptionFor(timeSlot.Court);
          var msg = $"{courtName} booked. {timeSlot.Time.ToLongDateString()}. {timeSlot.Time.ToShortTimeString()}";

          _pushNotificationService.SendNotification(msg);
          _consoleService.WriteSuccess("Court booked.");
          var logger = App.Kernel.Get<ICloudLoggingService>();
          logger.Log(msg, DateTime.Now.ToLongTimeString());
          return true;
        }
      }

      // the matchpoint software has a bug.
      // it will provide a token for a timeslot that is actually not bookable.
      // Non-bookable timeslots should not be assigned any tokens.
      // For example, if court 2 is booked from 7h30 to 8h30 and you check availability at
      // 8h00 am, matchpoint will provide a token (it should not!!!), making it seem as if the court is bookable!
      // we can tell that the court is not bookable if the response contains a submit button with the name:
      // ctl00$ContentPlaceHolderContenido$ButtonModificarJugadoresAdicionales

      if (response.Content.IndexOf("ctl00$ContentPlaceHolderContenido$ButtonModificarJugadoresAdicionales") > 0)
      {
        var courtName = AppUtilities.GetDescriptionFor(timeSlot.Court);
        _consoleService.WriteWarning($"{courtName} is not bookable at {timeSlot.Time.ToShortTimeString()}. Matchpoint bug. Earlier overlapping timeslot already taken.");
        TimeSlotAvailable = false;
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
