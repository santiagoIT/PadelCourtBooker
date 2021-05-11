using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelCourtBooker.App.Core
{
  public class CheckCourtAvailabilityAction : TimeSlotActionBase
  {
    public bool Execute(DateTime date, PadelCourt court, bool silent)
    {
      if (!silent)
      {
        _consoleService.WriteStartAction("Check time slot availability");
      }

      var timeslotsJson = PostRequest(date, court, silent);
      if (null == timeslotsJson)
      {
        return true;
      }

      foreach (var timeSlotJson in timeslotsJson)
      {
        return true;
      }

      return false;
    }
  }
}
