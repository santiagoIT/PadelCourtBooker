using System;

namespace PadelCourtBooker.App.Core
{
  class DelayedBookingInfo
  {
    public int Attempts { get; set; }
    public DateTime StartedAt { get; set; }

    public bool LogInCalled { get; set; }

    public bool Busy { get; set; }
  }
}
