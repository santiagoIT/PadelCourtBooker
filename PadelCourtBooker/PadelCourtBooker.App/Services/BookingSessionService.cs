using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace PadelCourtBooker.App.Services
{
  class BookingSessionService : IBookingSessionService
  {
    private DateTime? _lastSessionStartTime;
    /// <summary>
    /// List of cookies obtained after login. Will be send with the booking request
    /// </summary>
    public IList<RestResponseCookie> Cookies { get; } = new List<RestResponseCookie>();

    /// <summary>
    /// Reset all state
    /// </summary>
    public void Reset()
    {
      if (Cookies != null)
      {
        Cookies.Clear();
      }
    }

    public void SessionStarted(IList<RestResponseCookie> responseCookies)
    {
      _lastSessionStartTime = DateTime.Now;
      
      foreach (var cookie in responseCookies)
      {
        Cookies.Add(cookie);
      }
    }

    public bool SessionStillValid()
    {
      if (!_lastSessionStartTime.HasValue)
      {
        return false;
      }

      // we assume the session to be valid for 5 minutes
      var timespan = DateTime.Now - _lastSessionStartTime.Value;
      if (timespan.TotalMinutes >= 5)
      {
        return false;
      }

      return true;
    }
  }
}
