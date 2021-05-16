using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loggly;
using Loggly.Config;
using Ninject;

namespace PadelCourtBooker.App.Services
{
  public class LogglyLoggingService : ICloudLoggingService
  {
    private ICredentialService _credentialService;
    public void Initialize()
    {
      var appDataService = App.Kernel.Get<IAppDataService>();

      _credentialService = App.Kernel.Get<ICredentialService>();
      var config = LogglyConfig.Instance;
      config.CustomerToken = appDataService.Data.LogglyCustomerToken;
      config.ApplicationName = $"QPadel-Booker";
      config.Transport.EndpointHostname = appDataService.Data.LogglyEndpointHostname;
      config.Transport.EndpointPort = 443;
      config.Transport.LogTransport = LogTransport.Https;

      var ct = new ApplicationNameTag();
      ct.Formatter = "application-{0}";
      config.TagConfig.Tags.Add(ct);
    }

    public void Log(string eventName, string description)
    {
      try
      {
        var loggly = new LogglyClient();
        var logEvent = new LogglyEvent();
        logEvent.Data.Add("data", new
        {
          Event = eventName,
          Description = description,
          UserName = _credentialService.UserName,
          App = "QPadel-Booker.Client"
        });
        loggly.Log(logEvent);
      }
      catch (Exception)
      {
        // ignored
      }
    }
  }
}
