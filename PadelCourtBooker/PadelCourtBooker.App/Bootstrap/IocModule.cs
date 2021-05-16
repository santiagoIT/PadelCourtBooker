using Ninject.Modules;
using PadelCourtBooker.App.Login;
using PadelCourtBooker.App.Services;

namespace PadelCourtBooker.App.Bootstrap
{
  public class IocModule : NinjectModule
  {
    public override void Load()
    {
      Bind<ICredentialService>().To<CredentialService>().InSingletonScope();
      Bind<IBookingSessionService>().To<BookingSessionService>().InSingletonScope();
      Bind<IConsoleOutputService>().To<ConsoleOutputService>().InSingletonScope();
      Bind<ICloudLoggingService>().To<LogglyLoggingService>().InSingletonScope();
      Bind<IPushNotificationService>().To<PushNotificationService>().InSingletonScope();
      Bind<IAppDataService>().To<AppDataService>().InSingletonScope();
    }
  }
}
