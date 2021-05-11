using System;
using System.Reflection;
using System.Windows;
using Loggly;
using Loggly.Config;
using Ninject;
using Ninject.Activation;
using PadelCourtBooker.App.Core;
using PadelCourtBooker.App.Services;


namespace PadelCourtBooker.App
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      
      DataContext = new MainViewModel();

      this.BookingDatePicker.Minimum = DateTime.Today;

     
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      var consoleService = App.Kernel.Get<IConsoleOutputService>();
      var version = Assembly.GetExecutingAssembly().GetName().Version;

      consoleService.WriteLine($"Padel Court Booker App. [version: {version.Major}.{version.Minor}.{version.Build}.{version.Revision}]");
      consoleService.WriteLine($"© {DateTime.Now.Year} sb");
#if DEBUG
      consoleService.WriteWarning("This is a DEBUG version!");
#endif
      consoleService.WriteLine(string.Empty);
    }
  }
}
