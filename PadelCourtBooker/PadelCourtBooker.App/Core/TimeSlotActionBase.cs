using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ninject;
using PadelCourtBooker.App.Services;
using RestSharp;

namespace PadelCourtBooker.App.Core
{
  public class TimeSlotActionBase
  {
    protected IConsoleOutputService _consoleService;

    protected TimeSlotActionBase()
    {
      _consoleService = App.Kernel.Get<IConsoleOutputService>();
    }
    protected object ComposePayload(DateTime date, PadelCourt court)
    {
      // json payload format
      // { "idCuadro":4,"idRecurso":"16","idmodalidad":8,"fecha":"6/5/2021","hora":"09:00"}

      var payload = new
      {
        idCuadro = 4,
        idRecurso = $"{(int)court}",
        idmodalidad = 8,
        fecha = $"{date.Day}/{date.Month}/{date.Year}",
        hora = $"{date.Hour:D2}:{date.Minute:D2}"
      };

      return payload;
    }

    protected dynamic PostRequest(DateTime date, PadelCourt court, bool silent)
    {
      var payload = ComposePayload(date, court);

      var client = new RestClient(AppConstants.Host);
      var request = new RestRequest(AppConstants.ObtainTimeSlotInformationUrl, DataFormat.Json);
      request.AddJsonBody(payload);
      var response = client.Post(request);

      if (response.StatusCode != HttpStatusCode.OK)
      {
        if (!silent)
        {
          _consoleService.WriteError($"Failed to retrieve time slot information. {response.StatusCode}");
        }

        return null;
      }

      dynamic jsonObject = JsonConvert.DeserializeObject(response.Content);
      return jsonObject.d.Opciones;
    }
  }
}
