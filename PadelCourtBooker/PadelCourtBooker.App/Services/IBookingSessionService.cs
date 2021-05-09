using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace PadelCourtBooker.App.Services
{
  interface IBookingSessionService
  {
    IList<RestResponseCookie> Cookies { get; }

    bool SessionStillValid();

    void Reset();

    void SessionStarted(IList<RestResponseCookie> responseCookies);
  }
}
