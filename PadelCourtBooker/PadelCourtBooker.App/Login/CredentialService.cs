using Microsoft.Win32;
using PadelCourtBooker.App.Services;

namespace PadelCourtBooker.App.Login
{
  class CredentialService : ICredentialService
  {
    private const string RegistryKey = @"SOFTWARE\itWORKS\PadelCourtBooker\Credentials";
    private const string UserNameKey = "UserName";
    private const string PasswordKey = "Password";

    public void DisplayCredentials()
    {
      var vm = new WndCredentialsViewModel
      {
        UserName = UserName,
        Password = Password
      };

      var wndCredentials = new WndCredentials
      {
        DataContext = vm
      };
      var result = wndCredentials.ShowDialog();
      if (!result.HasValue || !result.Value)
      {
        return;
      }

      UserName = vm.UserName;
      Password = vm.Password;

      SaveCredentials();
    }

    private void SaveCredentials()
    {
      using var registry = Registry.CurrentUser.CreateSubKey(RegistryKey, true);
      if (registry != null)
      {
        // user name
        registry.SetValue(UserNameKey, UserName);

        // password
        registry.SetValue(PasswordKey, Password);
      }
    }

    public void LoadCredentials()
    {
      using var registry = Registry.CurrentUser.OpenSubKey(RegistryKey);
      if (registry != null)
      {
        // user name
        var registryValue = registry.GetValue(UserNameKey);
        if (registryValue != null)
        {
          UserName = registryValue.ToString();
        }

        // password
        registryValue = registry.GetValue(PasswordKey);
        if (registryValue != null)
        {
          Password = registryValue.ToString();
        }
      }
    }

    public string UserName { get; private set; }
    public string Password { get; private set; }
  }
}
