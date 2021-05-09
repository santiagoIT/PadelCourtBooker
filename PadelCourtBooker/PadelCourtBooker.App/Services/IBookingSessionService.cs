using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace PadelCourtBooker.App.Services
{
  interface IBookingSessionService
  {
    IList<RestResponseCookie> Cookies { get; set; }

    void Reset();

  }
}
