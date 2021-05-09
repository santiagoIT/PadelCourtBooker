using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PadelCourtBooker.App.Core;

namespace PadelCourtBooker.App
{
  public static class AppUtilities
  {
    public static string GetDescriptionFor(PadelCourt court)
    {
      switch (court)
      {
        case PadelCourt.Court1: return "Court 1";
        case PadelCourt.Court2: return "Court 2";
        case PadelCourt.Court3: return "Court 3";
        case PadelCourt.Court4: return "Court 4";
      }

      throw new ApplicationException($"Court {court}not yet supported");
    }

    public static void ShutdownComputer()
    {
      Process.Start("shutdown", "/s /t 0");
    }
  }
}
