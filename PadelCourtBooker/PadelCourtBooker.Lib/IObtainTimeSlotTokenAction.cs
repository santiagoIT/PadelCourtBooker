using System;
using System.Collections.Generic;

namespace PadelCourtBooker.Lib
{
  interface IObtainTimeSlotTokenAction
  {
    IList<TimeSlotInfo> Execute(DateTime date, PadelCourt court);
  }
}
