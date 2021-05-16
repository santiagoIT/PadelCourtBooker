using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Ninject;
using PadelCourtBooker.App.Core;

namespace PadelCourtBooker.App.Services
{
  class AppDataService : IAppDataService
  {
    public void Initialize()
    {
      if (!LoadJsonFile())
      {
        var consoleService = App.Kernel.Get<IConsoleOutputService>();
        consoleService.WriteError("Failed to load json app data");
      }
    }

    private bool LoadJsonFile()
    {
      try
      {
        var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
        var jsonFile = Path.Combine(assemblyPath, "Resources/appData.json");
        if (!File.Exists(jsonFile))
        {
          return false;
        }

        // load data from json file
        var json = File.ReadAllText(jsonFile, Encoding.UTF8);
        Data = JsonConvert.DeserializeObject<AppData>(json);

        return true;
      }
      catch (Exception)
      {
        return false;
      }
      
    }

    public AppData Data { get; private set; }
  }
}
