using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using PadelCourtBooker.App.Core;
using PadelCourtBooker.App.Services;

namespace PadelCourtBooker.App
{
  public class MainViewModel : ViewModelBase
  {
    private DateTime _bookingDate;
    private int _bookingHour;
    private int _bookingMinutes;
    private PadelCourt _bookingCourt;
    private bool _instantBooking;
    private TimeSlotInfo _selectedTimeSlot;
    private int _delayedBookingHour = 23;
    private int _delayedBookingMinutes = 59;
    private DateTime _delayedBookingTime;
    private IConsoleOutputService _consoleService;

    private DispatcherTimer _delayedBookingTimer = null;
    private string _delayedBookingCountDown;
    private bool _shutDownComputer;

    public MainViewModel()
    {
      RegisterCommands();

      BookingDate = DateTime.Now.AddDays(7);

      for (int i = 6; i < 24; i++)
      {
        AvailableHours.Add(i);
      }

      AvailableMinutes.Add(0);
      AvailableMinutes.Add(30);

      AvailableCourts.Add(PadelCourt.Court1);
      AvailableCourts.Add(PadelCourt.Court2);
      AvailableCourts.Add(PadelCourt.Court3);
      AvailableCourts.Add(PadelCourt.Court4);

      BookingHour = 8;
      BookingMinutes = 0;
      BookingCourt = PadelCourt.Court1;

      _consoleService = App.Kernel.Get<IConsoleOutputService>();

      ShutDownComputer = true;
#if DEBUG
      ShutDownComputer = false;
#endif
    }

    #region Properties

    public DateTime BookingDate
    {
      get => _bookingDate;
      set { _bookingDate = value; RaisePropertyChanged();}
    }

    public int BookingHour
    {
      get => _bookingHour;
      set { _bookingHour = value; RaisePropertyChanged(); }
    }

    public int BookingMinutes
    {
      get => _bookingMinutes;
      set { _bookingMinutes = value; RaisePropertyChanged();}
    }

    public PadelCourt BookingCourt
    {
      get => _bookingCourt;
      set { _bookingCourt = value; RaisePropertyChanged();}
    }

    public int DelayedBookingHour
    {
      get => _delayedBookingHour;
      set { _delayedBookingHour = value; RaisePropertyChanged();}
    }

    public int DelayedBookingMinutes
    {
      get => _delayedBookingMinutes;
      set { _delayedBookingMinutes = value; RaisePropertyChanged();}
    }

    public string DelayedBookingCountDown
    {
      get => _delayedBookingCountDown;
      set { _delayedBookingCountDown = value; RaisePropertyChanged();}
    }

    public ObservableCollection<int> AvailableHours { get; } = new ObservableCollection<int>();

    public ObservableCollection<int> AvailableMinutes { get; } = new ObservableCollection<int>();

    public ObservableCollection<PadelCourt> AvailableCourts { get; } = new ObservableCollection<PadelCourt>();

    public ObservableCollection<TimeSlotInfo> AvailableTimeSlots { get; } = new ObservableCollection<TimeSlotInfo>();

    public bool InstantBooking
    {
      get => _instantBooking;
      set { _instantBooking = value; RaisePropertyChanged();}
    }

    public bool DelayedBooking
    {
      get => !_instantBooking;
      set { _instantBooking = !value; RaisePropertyChanged();}
    }

    public TimeSlotInfo SelectedTimeSlot
    {
      get => _selectedTimeSlot;
      set { _selectedTimeSlot = value; RaisePropertyChanged();}
    }

    public bool TimeSlotsAvailable => AvailableTimeSlots.Count > 0;

    public bool ShutDownComputer
    {
      get => _shutDownComputer;
      set { _shutDownComputer = value; RaisePropertyChanged();}
    }

    #endregion

    #region Commands

    public ICommand CmdGetTimeSlotInfo { get; private set; }

    public ICommand CmdEnterCredentials { get; private set; }

    public ICommand CmdLogin { get; private set; }

    public ICommand CmdBookCourt { get; private set; }

    public ICommand CmdClearConsole { get; private set; }

    public ICommand CmdCopyConsoleToClipboard { get; private set; }

    public ICommand CmdCancelDelayedBooking { get; private set; }

    private void RegisterCommands()
    {
      CmdGetTimeSlotInfo = new RelayCommand(() => CmdGetTimeSlotInfoExecute());
      CmdEnterCredentials = new RelayCommand(() => CmdEnterCredentialsExecute());
      CmdLogin = new RelayCommand(() => CmdLoginExecute());
      CmdBookCourt = new RelayCommand(() => CmdBookCourtExecute(), () => SelectedTimeSlot != null);
      CmdCancelDelayedBooking = new RelayCommand(() => CmdCancelDelayedBookingExecute(), () => _delayedBookingTimer != null);

      CmdClearConsole = new RelayCommand(() => CmdClearConsoleExecute());
      CmdCopyConsoleToClipboard = new RelayCommand(() => CmdCopyConsoleToClipboardExecute());

    }

    private void CmdClearConsoleExecute()
    {
      _consoleService.Clear();
    }

    private void CmdCopyConsoleToClipboardExecute()
    {
      _consoleService.CopyToClipboard();
    }

    private void CmdGetTimeSlotInfoExecute()
    {
      AvailableTimeSlots.Clear();

      var bookingTime = BookingDate.Date.Add(new TimeSpan(BookingHour, BookingMinutes, 0));

      var action = new ObtainTimeSlotTokenAction();
      var timeSlots = action.Execute(bookingTime, BookingCourt);
      if (timeSlots != null)
      {
        foreach (var timeSlotInfo in timeSlots)
        {
          AvailableTimeSlots.Add(timeSlotInfo);
          if (timeSlotInfo.Description.Contains("90"))
          {
            SelectedTimeSlot = timeSlotInfo;
          }
        }
      }

      RaisePropertyChanged(nameof(TimeSlotsAvailable));
    }

    private void CmdEnterCredentialsExecute()
    {
      App.Kernel.Get<ICredentialService>().DisplayCredentials();
    }

    private void CmdCancelDelayedBookingExecute()
    {
      if (_delayedBookingTimer != null)
      {
        _delayedBookingTimer.Stop();
        _delayedBookingTimer = null;

        DelayedBookingCountDown = string.Empty;

        _consoleService.WriteWarning("Delayed booking canceled");
      }
    }

    private void CmdLoginExecute()
    {
      var loginAction = new LoginAction();
      loginAction.Execute();
    }

    private void CmdBookCourtExecute()
    {
      if (InstantBooking)
      {
        BookCourtNow();
        return;
      }

      // delayed booking

      // make sure time is in the future
      _delayedBookingTime = DateTime.Today.AddHours(DelayedBookingHour).AddMinutes(DelayedBookingMinutes);
      if (_delayedBookingTime <= DateTime.Now)
      {
        MessageBox.Show("Delayed booking time cannot be in the past!", "Error", MessageBoxButton.OK,
          MessageBoxImage.Error);
        return;
      }

      // start timer
      _consoleService.WriteStartAction("Delayed booking countdown started");
      _delayedBookingTimer = new DispatcherTimer
      {
        Interval = new TimeSpan(0,0,0,1),
        IsEnabled = true
      };
      _delayedBookingTimer.Tick += _delayedBookingTimer_Tick;

    }

    private void _delayedBookingTimer_Tick(object sender, EventArgs e)
    {
      if (_delayedBookingTime < DateTime.Now)
      {
        _delayedBookingTimer.Stop();
        _delayedBookingTimer = null;

        // proceed with booking
        _consoleService.WriteStartAction("Delayed booking started");

        if (BookCourtNow())
        {
          if (ShutDownComputer)
          {
            _consoleService.WriteWarning("Computer will shutdown in 1 minute!!!");
            // shutdown PC in 1 minute
            Task.Delay(1000).ContinueWith(_ => AppUtilities.ShutdownComputer());
          }
        }
        return;
      }

      var timeSpan = _delayedBookingTime - DateTime.Now;
      DelayedBookingCountDown = ($"{timeSpan.Hours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}");
    }

    #endregion

    private bool BookCourtNow()
    {
      var loginAction = new LoginAction();
      if (!loginAction.Execute())
      {
        return false;
      }

      var bookAction = new BookAction();
      return bookAction.Execute(SelectedTimeSlot);
    }
  }
}
