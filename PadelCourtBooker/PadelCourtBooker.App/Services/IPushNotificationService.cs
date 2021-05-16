using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelCourtBooker.App.Services
{
  interface IPushNotificationService
  {
    void SendNotification(string message);
  }
}
