using System.Windows;

namespace PadelCourtBooker.App.Login
{
  /// <summary>
  /// Interaction logic for WndCredentials.xaml
  /// </summary>
  public partial class WndCredentials : Window
  {
    public WndCredentials()
    {
      InitializeComponent();
    }

    private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }
  }
}
