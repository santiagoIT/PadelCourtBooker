using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using PadelCourtBooker.App.UserControls;

namespace PadelCourtBooker.App.Services
{
  class ConsoleOutputService : IConsoleOutputService
  {
    public ConsoleUserControl Console { get; set; }

    public void WriteLine(string msg)
    {
      var run = new Run(msg + Environment.NewLine)
      {
        Foreground = Brushes.Snow
      };
      Console.ScrollViewer.ScrollToEnd();
      Console.OutputText.Inlines.Add(run);
    }

    public void WriteError(string msg)
    {
      var run = new Run(msg + Environment.NewLine)
      {
        Foreground = Brushes.Crimson
      };
      Console.ScrollViewer.ScrollToEnd();
      Console.OutputText.Inlines.Add(run);
    }

    public void WriteWarning(string msg)
    {
      var run = new Run(msg + Environment.NewLine)
      {
        Foreground = Brushes.Gold
      };
      Console.ScrollViewer.ScrollToEnd();
      Console.OutputText.Inlines.Add(run);
    }

    public void WriteSuccess(string msg)
    {
      var run = new Run(msg + Environment.NewLine)
      {
        Foreground = Brushes.Green
      };
      Console.ScrollViewer.ScrollToEnd();
      Console.OutputText.Inlines.Add(run);
    }

    public void WriteStartAction(string msg)
    {
      WriteLine(string.Empty);

      var timestamp = DateTime.Now.ToString("yyyy-MM-dd --- HH:mm:ss:fff");
      var run = new Run($"***  {msg} - {timestamp}." + Environment.NewLine)
      {
        Foreground = Brushes.Snow,
      };
      Console.ScrollViewer.ScrollToEnd();
      Console.OutputText.Inlines.Add(new Bold(run));

      WriteLine(string.Empty);
    }

    public void Clear()
    {
      Console.OutputText.Text = String.Empty;
    }

    public void CopyToClipboard()
    {
      try
      {
        Clipboard.SetText(Console.OutputText.Text);
      }
      catch (Exception e)
      {
        MessageBox.Show($"Failed to copy text to clipboard.\n\r{e.Message}");
      }
    }
  }
}
