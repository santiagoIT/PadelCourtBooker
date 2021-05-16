using System;
using Ninject;
using RestSharp;

namespace PadelCourtBooker.App.Services
{
  class PushNotificationService : IPushNotificationService
  {
    public void SendNotification(string message)
    {
      var appData = App.Kernel.Get<IAppDataService>().Data;
      var credentialsService = App.Kernel.Get<ICredentialService>();

      var payload = new
      {
        app_key = appData.PushedAppKey,
        app_secret = appData.PushedAppSecret,
        target_type = "email",
        email = credentialsService.UserName,
        content =message
      };

      var client = new RestClient("https://api.pushed.co/");
      var request = new RestRequest("1/push", DataFormat.Json);
      request.AddJsonBody(payload);
      try
      {
        var response = client.Post(request);
      }
      catch (Exception)
      {
        // ignored
      }
    }
  }
}
