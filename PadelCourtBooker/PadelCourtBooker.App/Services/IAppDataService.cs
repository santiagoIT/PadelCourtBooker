using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PadelCourtBooker.App.Core;

namespace PadelCourtBooker.App.Services
{
  interface IAppDataService
  {
    void Initialize();

    public AppData Data { get; }
  }
}
