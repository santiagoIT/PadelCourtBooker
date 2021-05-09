using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace PadelCourtBooker.App.Services
{
  class BookingSessionService : IBookingSessionService
  {
    /// <summary>
    /// List of cookies obtained after login. Will be send with the booking request
    /// </summary>
    public IList<RestResponseCookie> Cookies { get; set; }

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
  }
}
