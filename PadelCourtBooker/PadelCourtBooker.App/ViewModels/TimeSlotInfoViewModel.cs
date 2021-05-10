using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PadelCourtBooker.App.Core;

namespace PadelCourtBooker.App.ViewModels
{
  public class TimeSlotInfoViewModel : ViewModelBase
  {
    private ObservableCollection<TimeSlotInfoViewModel> _parentList;
    private MainViewModel _parent;

    public TimeSlotInfoViewModel(TimeSlotInfo timeSlotInfo,  MainViewModel parent)
    {
      TimeSlotInfo = timeSlotInfo;
      _parent = parent;
      _parentList = _parent.AvailableTimeSlots;
      

      RegisterCommands();
    }

    public string DisplayName => TimeSlotInfo.ToString();

    public TimeSlotInfo TimeSlotInfo { get; }

    public ICommand CmdDelete { get; private set; }

    private void RegisterCommands()
    {
      CmdDelete = new RelayCommand(() => CmdDeleteExecute());
    }

    private void CmdDeleteExecute()
    {
      _parentList.Remove(this);
    }
  }
}
