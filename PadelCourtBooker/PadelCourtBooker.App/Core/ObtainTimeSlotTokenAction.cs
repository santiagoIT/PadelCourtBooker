using System;
using System.Collections.Generic;

namespace PadelCourtBooker.App.Core
{
  public class ObtainTimeSlotTokenAction : TimeSlotActionBase, IObtainTimeSlotTokenAction
  {
    public IList<TimeSlotInfo> Execute(DateTime date, PadelCourt court)
    {
      var timeSlots = new List<TimeSlotInfo>();
      var timeslotsJson = PostRequest(date, court, false);
      if (null == timeslotsJson)
      {
        return timeSlots;
      }

      foreach (var timeSlotJson in timeslotsJson)
      {
        var timeSlot = new TimeSlotInfo
        {
          Token = timeSlotJson.Token,
          Description = timeSlotJson.Descripcion,
          Court = court,
          Time = date
        };

        timeSlots.Add(timeSlot);
      }

      return timeSlots;
    }
  }


  /*{"d":{"__type":"Matchpoint.Web.Booking.srvc+InformacionHuecoLibre","Titulo":"Cancha 4 AutoFenix 05/11/2021 12:00","Descripcion":"","IdImagen":0,"Opciones":[{"Token":"aeb20d8c2a689a5bcd57109b79e8865a1007e2abf2d81a471833f25760761049e4ce765c0259b5ed7e511671f0205cb7","Descripcion":"90\u0027 Minutos"},{"Token":"aeb20d8c2a689a5bcd57109b79e8865a1007e2abf2d81a471833f25760761049277d9215b80dafe259e987b3a6927af3","Descripcion":"60\u0027 Minutos"},{"Token":"aeb20d8c2a689a5bcd57109b79e8865a1007e2abf2d81a471833f25760761049dabadc69fbdb72a077c0f12eb14859a8","Descripcion":"120\u0027 Minutos"}],"Lugar":"Cancha 4 AutoFenix","FechaHora":"05/11/2021 12:00","Url_Imagen":"https://www.quitopadel.com/images.ashx?cmd=get&id=162"}} */
}
