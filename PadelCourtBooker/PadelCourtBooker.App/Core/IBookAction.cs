using System;
using System.Collections.Generic;
using System.Text;

namespace PadelCourtBooker.App.Core
{
  interface IBookAction
  {
    bool Execute(TimeSlotInfo timeSlot);
  }
}
