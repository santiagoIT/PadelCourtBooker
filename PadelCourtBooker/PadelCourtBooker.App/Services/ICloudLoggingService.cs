using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelCourtBooker.App.Services
{
  public interface ICloudLoggingService
  {
    void Initialize();
    void Log(string eventName, string description);
  }
}
