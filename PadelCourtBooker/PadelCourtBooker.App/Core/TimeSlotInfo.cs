namespace PadelCourtBooker.App.Core
{
  public class TimeSlotInfo
  {
    public string Token { get; set; }
    public string Description { get; set; }

    public override string ToString()
    {
      return Description;
    }
  }
}
