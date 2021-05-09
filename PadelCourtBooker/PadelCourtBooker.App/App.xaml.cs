using System.Windows;
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
    }
    public static IKernel Kernel { get; private set; }
  }
}
