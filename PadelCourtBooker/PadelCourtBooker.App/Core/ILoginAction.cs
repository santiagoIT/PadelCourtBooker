using System;
using System.Collections.Generic;
using System.Text;

namespace PadelCourtBooker.App.Core
{
  interface ILoginAction
  {
    bool Execute(bool forceLogin);
  }
}
