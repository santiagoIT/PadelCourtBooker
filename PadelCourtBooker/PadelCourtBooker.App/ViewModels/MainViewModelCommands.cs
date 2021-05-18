using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using PadelCourtBooker.App.Core;
using PadelCourtBooker.App.Services;

namespace PadelCourtBooker.App.ViewModels
{
  public partial class MainViewModel : ViewModelBase
  {
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
      var bookingTime = BookingDate.Date.Add(new TimeSpan(BookingHour, BookingMinutes, 0));

      var consoleService = App.Kernel.Get<IConsoleOutputService>();
      var timeStr = $"{bookingTime.ToLongDateString()} at {bookingTime.Hour:D2}:{bookingTime.Minute:D2}";
      consoleService.WriteStartAction($"Retrieve time slot info [{timeStr}]");

      var unsortedList = new List<TimeSlotInfoViewModel>(); 

      

      var filter = $"{GameDuration}'";

      var availableCourts = Enum.GetValues(typeof(PadelCourt)).Cast<PadelCourt>();

      foreach (var court in availableCourts)
      {
        var courtStr = AppUtilities.GetDescriptionFor(court);
        var action = new ObtainTimeSlotTokenAction();
        var timeSlots = action.Execute(bookingTime, court);
        if (timeSlots.Count > 0)
        {
          bool courtCanBeUsed = false;
          foreach (var timeSlotInfo in timeSlots)
          {
            if (!timeSlotInfo.Description.Contains(filter))
            {
              continue;
            }

            var vm = new TimeSlotInfoViewModel(timeSlotInfo, this)
            {
              BookingTime = bookingTime,
              BookingPreferencePriority = (uint) timeSlotInfo.Court
            };

            unsortedList.Add(vm);
            courtCanBeUsed = true;
          }
          
          if (courtCanBeUsed)
          {
            consoleService.WriteSuccess($"{courtStr} is available.");
          }
          else
          {
            consoleService.WriteWarning($"{courtStr} is available, but not for desired duration.");
          }
        }
        else
        {
          consoleService.WriteError($"{courtStr} is not available.");
        }
      }

      // sort by booking priority
      var sortedList = unsortedList.OrderBy(x => x.BookingPreferencePriority);

      // populate collection
      AvailableTimeSlots.Clear();
      foreach (var viewModel in sortedList)
      {
        AvailableTimeSlots.Add(viewModel);
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
        Interval = new TimeSpan(0, 0, 0, 1),
        IsEnabled = true
      };
      _delayedBookingTimer.Tick += _delayedBookingTimer_Tick;
      RaisePropertyChanged(nameof(DelayedBookingNotInProgress));
    }
  }
}
