using System;
using System.Collections.Generic;

namespace PadelCourtBooker.App.Core
{
  interface IObtainTimeSlotTokenAction
  {
    IList<TimeSlotInfo> Execute(DateTime date, PadelCourt court);
  }
}
