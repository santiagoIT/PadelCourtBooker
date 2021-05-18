using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using PadelCourtBooker.App.Core;
using PadelCourtBooker.App.Services;
using PadelCourtBooker.App.ViewModels;

namespace PadelCourtBooker.App
{
  public class MainViewModel : ViewModelBase
  {
    private DateTime _bookingDate;
    private int _bookingHour;
    private int _bookingMinutes;
    private bool _instantBooking;
    private TimeSlotInfoViewModel _selectedTimeSlot;
    private int _delayedBookingHour = 23;
    private int _delayedBookingMinutes = 59;
    private DateTime _delayedBookingTime;
    private IConsoleOutputService _consoleService;

    private DispatcherTimer _delayedBookingTimer = null;
    private string _delayedBookingCountDown;
    private bool _shutDownComputer;
    private DelayedBookingInfo _delayedBookingInfo;
    private int _gameDuration = 90;

    public MainViewModel()
    {
      RegisterCommands();

      BookingDate = DateTime.Now.AddDays(8);

      for (int i = 6; i < 24; i++)
      {
        AvailableHours.Add(i);
      }

      AvailableMinutes.Add(0);
      AvailableMinutes.Add(30);

      AvailableGameDurations.Add(60);
      AvailableGameDurations.Add(90);
      AvailableGameDurations.Add(120);

      BookingHour = 8;
      BookingMinutes = 0;

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


    public int DelayedBookingHour
    {
      get => _delayedBookingHour;
      set { 
        _delayedBookingHour = value;
        RaisePropertyChanged();}
    }

    public int DelayedBookingMinutes
    {
      get => _delayedBookingMinutes;
      set {
        _delayedBookingMinutes = value;
        RaisePropertyChanged();}
    }

    public string DelayedBookingCountDown
    {
      get => _delayedBookingCountDown;
      set { _delayedBookingCountDown = value; RaisePropertyChanged();}
    }

    public int GameDuration
    {
      get => _gameDuration;
      set { _gameDuration = value; RaisePropertyChanged();}
    }

    public ObservableCollection<int> AvailableHours { get; } = new ObservableCollection<int>();

    public ObservableCollection<int> AvailableMinutes { get; } = new ObservableCollection<int>();

    public ObservableCollection<TimeSlotInfoViewModel> AvailableTimeSlots { get; } = new ObservableCollection<TimeSlotInfoViewModel>();

    public ObservableCollection<int> AvailableGameDurations { get; } = new ObservableCollection<int>();

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

    public TimeSlotInfoViewModel SelectedTimeSlot
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

    public bool DelayedBookingNotInProgress => _delayedBookingTimer == null;

    #endregion

    #region Commands

    public ICommand CmdGetTimeSlotInfo { get; private set; }

    public ICommand CmdEnterCredentials { get; private set; }

    public ICommand CmdLogin { get; private set; }

    public ICommand CmdBookCourt { get; private set; }

    public ICommand CmdClearConsole { get; private set; }

    public ICommand CmdCopyConsoleToClipboard { get; private set; }

    public ICommand CmdCancelDelayedBooking { get; private set; }

    public ICommand CmdMoveUp { get; private set; }
    public ICommand CmdMoveDown { get; private set; }

    public ICommand CmdLaunchQPadelReservationsPage { get; private set; }

    public ICommand CmdLaunchGitHubPage { get; private set; }

    public ICommand CmdTestPushNotification { get; private set; }

    private void RegisterCommands()
    {
      CmdGetTimeSlotInfo = new RelayCommand(() => CmdGetTimeSlotInfoExecute());
      CmdEnterCredentials = new RelayCommand(() => CmdEnterCredentialsExecute());
      CmdLogin = new RelayCommand(() => CmdLoginExecute());
      CmdLaunchQPadelReservationsPage = new RelayCommand(() => CmdLaunchQPadelReservationsPageExecute());
      CmdLaunchGitHubPage = new RelayCommand(() => CmdLaunchGitHubPageExecute());
      CmdBookCourt = new RelayCommand(() => CmdBookCourtExecute(), () => AvailableTimeSlots.Count > 0);
      CmdCancelDelayedBooking = new RelayCommand(() => CmdCancelDelayedBookingExecute(), () => _delayedBookingTimer != null);

      CmdClearConsole = new RelayCommand(() => CmdClearConsoleExecute());
      CmdCopyConsoleToClipboard = new RelayCommand(() => CmdCopyConsoleToClipboardExecute());

      CmdTestPushNotification = new RelayCommand(() => CmdTestPushNotificationExecute());

      CmdMoveUp = new RelayCommand(() => CmdMoveUpExecute(), () =>
      {
        return SelectedTimeSlot != null && AvailableTimeSlots.IndexOf(SelectedTimeSlot) > 0;
      });
      CmdMoveDown = new RelayCommand(() => CmdMoveDownExecute(), () =>
      {
        return SelectedTimeSlot != null && AvailableTimeSlots.Last() != SelectedTimeSlot;
      });
    }

    private void CmdTestPushNotificationExecute()
    {
      var pushService = App.Kernel.Get<IPushNotificationService>();
      pushService.SendNotification("This is just a test message");
    }

    private void CmdLaunchQPadelReservationsPageExecute()
    {
      AppUtilities.OpenBrowser(AppConstants.ReservationsPageUrl);
    }
    
    private void CmdLaunchGitHubPageExecute()
    {
      AppUtilities.OpenBrowser(AppConstants.SourceCodeRepoUrl);
    }

    private void CmdMoveDownExecute()
    {
      var index = AvailableTimeSlots.IndexOf(SelectedTimeSlot);
      if (index >= AvailableTimeSlots.Count - 1)
      {
        return;
      }

      var currentSlot = SelectedTimeSlot;
      AvailableTimeSlots.RemoveAt(index);
      AvailableTimeSlots.Insert(index + 1, currentSlot);
      SelectedTimeSlot = currentSlot;
    }

    private void CmdMoveUpExecute()
    {
      var index = AvailableTimeSlots.IndexOf(SelectedTimeSlot);
      if (index < 1)
      {
        return;
      }
      var currentSlot = SelectedTimeSlot;
      AvailableTimeSlots.RemoveAt(index);
      AvailableTimeSlots.Insert(index - 1, currentSlot);
      SelectedTimeSlot = currentSlot;
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

      var filter = $"{GameDuration}'";

      var availableCourts = Enum.GetValues(typeof(PadelCourt)).Cast<PadelCourt>();

      foreach (var court in availableCourts)
      {
        var action = new ObtainTimeSlotTokenAction();
        var timeSlots = action.Execute(bookingTime, court, false);
        if (timeSlots != null)
        {
          foreach (var timeSlotInfo in timeSlots)
          {
            if (!timeSlotInfo.Description.Contains(filter))
            {
              continue;
            }

            var vm = new TimeSlotInfoViewModel(timeSlotInfo, this)
            {
              BookingTime = bookingTime
            };

            if (AvailableTimeSlots.Count == 0)
            {
              AvailableTimeSlots.Add(vm);
            }
            else
            {
              switch (court)
              {
                // goes first
                case PadelCourt.Court3:
                {
                  AvailableTimeSlots.Insert(0, vm);
                  break;
                }

                // goes last
                case PadelCourt.Court4:
                {
                  AvailableTimeSlots.Add(vm);
                  break;
                }

                case PadelCourt.Court1:
                case PadelCourt.Court2:
                {
                  switch (AvailableTimeSlots.Last().TimeSlotInfo.Court)
                  {
                    case PadelCourt.Court4:
                    {
                      AvailableTimeSlots.Insert(AvailableTimeSlots.Count -1, vm);
                          break;
                    }
                    default:
                    {
                      AvailableTimeSlots.Add(vm);
                      break;
                    }
                    }
                  
                  break;
                }

              }
            }
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
        RaisePropertyChanged(nameof(DelayedBookingNotInProgress));
      }
      _delayedBookingInfo.Busy = false;
    }

    private void CmdLoginExecute()
    {
      var loginAction = new LoginAction();
      loginAction.Execute(true);
    }

    private void CmdBookCourtExecute()
    {
      if (InstantBooking)
      {
        BookCourtNow(true);
        return;
      }

      // delayed booking
      _delayedBookingInfo = new DelayedBookingInfo();

      // make sure time is in the future
      _delayedBookingTime = DateTime.Today.AddHours(DelayedBookingHour).AddMinutes(DelayedBookingMinutes);
      if (_delayedBookingTime <= DateTime.Now)
      {
        MessageBox.Show("Delayed booking time cannot be in the past!", "Error", MessageBoxButton.OK,
          MessageBoxImage.Error);
        return;
      }

      RefreshDelayedBookingTime();

      // start timer
      _consoleService.WriteStartAction("Delayed booking countdown started");
      _delayedBookingTimer = new DispatcherTimer
      {
        Interval = new TimeSpan(0,0,0,1),
        IsEnabled = true
      };
      _delayedBookingTimer.Tick += _delayedBookingTimer_Tick;
      RaisePropertyChanged(nameof(DelayedBookingNotInProgress));
    }

    private void _delayedBookingTimer_Tick(object sender, EventArgs e)
    {
      if (_delayedBookingInfo.Busy)
      {
        return;
      }

      if (_delayedBookingTime < DateTime.Now)
      {
        // proceed with booking
        if (_delayedBookingInfo.Attempts == 0)
        {
          _consoleService.WriteStartAction("Delayed booking started");
        }

        _delayedBookingInfo.Busy = true;
        try
        {
          if (BookCourtNow(false))
          {
            _delayedBookingTimer.Stop();
            _delayedBookingTimer = null;
            RaisePropertyChanged(nameof(DelayedBookingNotInProgress));

            if (ShutDownComputer)
            {
              _consoleService.WriteWarning("Computer will shutdown in 1 minute!!!");
              // shutdown PC in 1 minute
              Task.Delay(1000).ContinueWith(_ => AppUtilities.ShutdownComputer());
            }

            return;
          }
        }
        finally
        {
          _delayedBookingInfo.Busy = false;
        }
       

        // booking was not successful, try again!
        _delayedBookingInfo.Attempts++;

        // how long have we been retrying?
        var retryTimeSpan = DateTime.Now - _delayedBookingTime;
        if (retryTimeSpan.TotalMinutes >= 15)
        {
          _consoleService.WriteError("Failed to book court. Giving up after 15 minutes. Sorry...");
          _delayedBookingTimer.Stop();
          _delayedBookingTimer = null;
        }

        // display a message every 10 attempts
        if (_delayedBookingInfo.Attempts % 10 == 0)
        {
          _consoleService.WriteLine($"  Failed attempts: {_delayedBookingInfo.Attempts}");
        }

        return;
      }

      var timeSpan = _delayedBookingTime - DateTime.Now;
      DelayedBookingCountDown = ($"{timeSpan.Hours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}");

      if (!_delayedBookingInfo.LogInCalled && timeSpan.TotalSeconds < 15)
      {
        _delayedBookingInfo.Busy = true;
        try
        {
          var loginAction = new LoginAction();
          if (loginAction.Execute(true))
          {
            _delayedBookingInfo.LogInCalled = true;
          }
        }
        finally
        {
          _delayedBookingInfo.Busy = false;
        }
        
      }
    }

    #endregion

    private void RefreshDelayedBookingTime()
    {
      if (_delayedBookingTime > DateTime.Now)
      {
        var timeSpan = _delayedBookingTime - DateTime.Now;

        DelayedBookingCountDown = ($"{timeSpan.Hours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}");
      }
      else
      {
        DelayedBookingCountDown = "--";
      }
    }

    private bool BookCourtNow(bool instantBooking)
    {
      var loginAction = new LoginAction();
      if (!loginAction.Execute(false))
      {
        return false;
      }

      if (instantBooking)
      {
        foreach (var timeSlot in AvailableTimeSlots)
        {
          var bookAction2 = new BookAction();
          if (bookAction2.Execute(timeSlot.TimeSlotInfo))
          {
            return true;
          }
        }

        return false;
      }

      var preferredCourt = AvailableTimeSlots.FirstOrDefault();
      if (null == preferredCourt)
      {
        return false;
      }

      var bookAction = new BookAction();
      bookAction.Silent = true;

      if (bookAction.Execute(preferredCourt.TimeSlotInfo))
      {
        return true;
      }

      // check if the court is still available
      var checkAvailabilityAction = new CheckCourtAvailabilityAction();
      if (!checkAvailabilityAction.Execute(preferredCourt.BookingTime, preferredCourt.TimeSlotInfo.Court, true))
      {
        // if court is not available, then stop trying this timeslot
        AvailableTimeSlots.RemoveAt(0);
      }

      return false;
    }
  }
}
