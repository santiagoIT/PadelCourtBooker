namespace PadelCourtBooker.App.Core
{
  public class TimeSlotInfo
  {
    public string Token { get; set; }
    public string Description { get; set; }

    public PadelCourt Court { get; set; }

    public override string ToString()
    {
      return $"{AppUtilities.GetDescriptionFor(Court)} - {Description}";
    }
  }
}
