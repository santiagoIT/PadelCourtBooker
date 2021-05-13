using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using Ninject;
using PadelCourtBooker.App.Bootstrap;
using PadelCourtBooker.App.Services;

namespace PadelCourtBooker.App
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      Kernel = new StandardKernel(new IocModule());

      // initialize dispatcher helper
      DispatcherHelper.Initialize();

      // load credentials
      Kernel.Get<ICredentialService>().LoadCredentials();

      Kernel.Get<ICloudLoggingService>().Initialize();

      RegisterGlobalExceptionHandling();
    }
    public static IKernel Kernel { get; private set; }

    private void RegisterGlobalExceptionHandling()
    {
      // this is the line you really want 
      AppDomain.CurrentDomain.UnhandledException +=
          (sender, args) => CurrentDomainOnUnhandledException(args);

      // optional: hooking up some more handlers
      // remember that you need to hook up additional handlers when 
      // logging from other dispatchers, shedulers, or applications

      Application.Current.Dispatcher.UnhandledException +=
          (sender, args) => DispatcherOnUnhandledException(args);

      Application.Current.DispatcherUnhandledException +=
          (sender, args) => CurrentOnDispatcherUnhandledException(args);

      TaskScheduler.UnobservedTaskException +=
          (sender, args) => TaskSchedulerOnUnobservedTaskException(args);
    }

    private static void TaskSchedulerOnUnobservedTaskException(UnobservedTaskExceptionEventArgs args)
    {
      args.SetObserved();
    }

    private static void CurrentOnDispatcherUnhandledException(DispatcherUnhandledExceptionEventArgs args)
    {
      LogException(args.Exception);
    }

    private static void DispatcherOnUnhandledException(DispatcherUnhandledExceptionEventArgs args)
    {
      LogException(args.Exception);
    }

    private static void CurrentDomainOnUnhandledException(UnhandledExceptionEventArgs args)
    {
      var exception = args.ExceptionObject as Exception;
      LogException(exception);

      var terminatingMessage = args.IsTerminating ? " The application is terminating." : string.Empty;
      var exceptionMessage = exception?.Message ?? "An unmanaged exception occured.";
      var message = string.Concat(exceptionMessage, terminatingMessage);

      MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private static void LogException(Exception e)
    {
      try
      {
        if (e != null)
        {
          var loggingService = Kernel.Get<ICloudLoggingService>();
          loggingService.Log("Error", e.ToString());
        }
      }
      catch (Exception)
      {
        // ignored
      }
    }
  }
}
