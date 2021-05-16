using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelCourtBooker.App.Core
{
  class AppData
  {
    public string LogglyEndpointHostname { get; set; }
    public string LogglyCustomerToken { get; set; }

    public string PushedAppKey { get; set; }
    public string PushedAppSecret { get; set; }
  }
}
