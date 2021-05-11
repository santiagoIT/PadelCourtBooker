namespace PadelCourtBooker.App
{
  public static class AppConstants
  {
    // hidden attributes
    public const string EventValidation = "__EVENTVALIDATION";
    public const string ViewState = "__VIEWSTATE";

    // cookies
    public const string IntranetMatchpointCookie = "IntranetMatchpointCookie";
    public const string AspNetSessionIdCookie = "ASP.NET_SessionId";
    public const string CloudFareIdCookie = "__cfduid";

    // headers
    public const string LocationHeader = "Location";

    // host, urls
    public const string Host = "https://www.quitopadel.com/";
    public const string ObtainTimeSlotInformationUrl = "booking/srvc.aspx/ObtenerInformacionHuecoLibre";
    public const string BookCourtUrl = "Booking/Info.aspx";
    public const string LoginUrl = "Login.aspx";
    public const string BookingConfirmationUrl = "/Booking/Confirmacion.aspx";

    public static string LogglyCustomerToken = "2c869ad3-64ed-4c0a-9bcc-0f9e396964b0";
  }
}
