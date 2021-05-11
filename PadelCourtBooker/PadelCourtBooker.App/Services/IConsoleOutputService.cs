using System;
using System.Collections.Generic;
using System.Text;
using PadelCourtBooker.App.UserControls;

namespace PadelCourtBooker.App.Services
{
  public interface IConsoleOutputService
  {
    ConsoleUserControl Console { get; set; }
    void WriteLine(string msg);
    void WriteError(string msg);
    void WriteSuccess(string msg);
    void WriteWarning(string msg);
    void WriteStartAction(string msg);
    void Clear();
    void CopyToClipboard();
  }
}
