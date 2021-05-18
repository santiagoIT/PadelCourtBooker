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
    public const string ReservationsPageUrl = @"https://www.quitopadel.com/Booking/Grid.aspx?id=4";
    public const string SourceCodeRepoUrl = @"https://github.com/santiagoIT/PadelCourtBooker";
  }
}
