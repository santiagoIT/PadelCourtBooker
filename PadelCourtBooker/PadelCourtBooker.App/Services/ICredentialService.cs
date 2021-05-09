namespace PadelCourtBooker.App.Services
{
  public interface ICredentialService
  {
    void DisplayCredentials();

    void LoadCredentials();

    string UserName { get; }
    string Password { get; }
  }
}
