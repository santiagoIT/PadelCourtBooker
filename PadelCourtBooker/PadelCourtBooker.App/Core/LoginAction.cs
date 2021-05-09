using System.Linq;
using System.Net;
using Ninject;
using PadelCourtBooker.App.Services;
using RestSharp;

namespace PadelCourtBooker.App.Core
{
  class LoginAction : ILoginAction
  {
    public bool Execute()
    {
      var bookingSessionService = App.Kernel.Get<IBookingSessionService>();
      
      // check if session is still valid
      if (bookingSessionService.SessionStillValid())
      {
        return true;
      }

      var consoleService = App.Kernel.Get<IConsoleOutputService>();
      consoleService.WriteStartAction("User authentication");

      var credentialService = App.Kernel.Get<ICredentialService>();
      bookingSessionService.Reset();

      var client = new RestClient(AppConstants.Host);
      client.FollowRedirects = false;
      var request = new RestRequest(AppConstants.LoginUrl, DataFormat.None);

      // get login page => gain access to __VIEWSTATE and __EVENTVALIDATION
      var response = client.Get(request);

      ExtractHiddenFieldValues(response, out string viewState, out string eventValidation);

      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

      request.AddParameter(AppConstants.EventValidation, eventValidation);
      request.AddParameter(AppConstants.ViewState, viewState);

      request.AddParameter("ctl00$ContentPlaceHolderContenido$Login1$UserName", credentialService.UserName);
      request.AddParameter("ctl00$ContentPlaceHolderContenido$Login1$Password", credentialService.Password);
      request.AddParameter("ctl00$ContentPlaceHolderContenido$Login1$LoginButton", "Entrar");

      // submit values
      response = client.Post(request);

      if (response.StatusCode != HttpStatusCode.Redirect)
      {
        consoleService.WriteError($"User authentication failed. Http Status Code: {response.StatusCode}");
        return false;
      }

      // request was successful if the IntranetMatchpointCookie is obtained
      var cookie = response.Cookies.FirstOrDefault(x => x.Name == AppConstants.IntranetMatchpointCookie);
      if (cookie == null)
      {
        consoleService.WriteError("User authentication failed. IntranetMatchpointCookie missing.");
        return false;
      }

      // store session cookies
      bookingSessionService.SessionStarted(response.Cookies);
      consoleService.WriteSuccess("User authentication successful.");

      return true;
    }

    private const string ValueAttribute = "value=";

    private bool ExtractHiddenFieldValues(IRestResponse response, out string viewState, out string eventValidation)
    {
      viewState = null;
      eventValidation = null;

      if (!ExtractHiddenField(AppConstants.ViewState, response, out viewState))
      {
        return false;
      }

      if (!ExtractHiddenField(AppConstants.EventValidation, response, out eventValidation))
      {
        return false;
      }

      return true;
    }

    private bool ExtractHiddenField(string attibuteName, IRestResponse response, out string attributeValue)
    {
      attributeValue = null;

      // find viewstate
      var indexViewState = response.Content.IndexOf($"name=\"{attibuteName}\"");
      if (indexViewState == -1)
      {
        return false;
      }

      var indexStart = response.Content.LastIndexOf("<input", indexViewState);
      if (indexStart == -1)
      {
        return false;
      }

      var indexValue = response.Content.IndexOf(ValueAttribute, indexStart);
      if (indexValue == -1)
      {
        return false;
      }
      indexValue += ValueAttribute.Length + 1; // remove start quote

      var indexEnd = response.Content.IndexOf("\"", indexValue);
      if (indexEnd == -1)
      {
        return false;
      }

      attributeValue = response.Content.Substring(indexValue, (indexEnd - indexValue)); // remove quote

      return true;
    }
  }
}
