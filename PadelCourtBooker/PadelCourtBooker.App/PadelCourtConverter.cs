using System;
using System.Globalization;
using System.Windows.Data;
using PadelCourt = PadelCourtBooker.App.Core.PadelCourt;

namespace PadelCourtBooker.App
{
  public class PadelCourtConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      PadelCourt court = (PadelCourt) value;
      return AppUtilities.GetDescriptionFor(court);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return string.Empty;
    }
  }
}
