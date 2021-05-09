using System.Windows.Controls;
using Ninject;
using PadelCourtBooker.App.Services;

namespace PadelCourtBooker.App.UserControls
{
  /// <summary>
  /// Interaction logic for ConsoleUserControl.xaml
  /// </summary>
  public partial class ConsoleUserControl : UserControl
  {
    public TextBlock OutputText { get; set; }
    public ScrollViewer ScrollViewer { get; set; }

    public ConsoleUserControl()
    {
      InitializeComponent();

      OutputText = OutputTextBlock;
      ScrollViewer = OutputScrollViewer;

      // register with console service
      var consoleService = App.Kernel.Get<IConsoleOutputService>();
      consoleService.Console = this;
    }
  }
}
