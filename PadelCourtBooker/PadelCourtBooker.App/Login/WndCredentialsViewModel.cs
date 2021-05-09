using GalaSoft.MvvmLight;

namespace PadelCourtBooker.App.Login
{
  public class WndCredentialsViewModel : ViewModelBase
  {
    private string _userName;
    private string _password;

    public string UserName
    {
      get => _userName;
      set { _userName = value; RaisePropertyChanged(); }
    }

    public string Password
    {
      get => _password;
      set { _password = value; RaisePropertyChanged(); }
    }
  }
}
