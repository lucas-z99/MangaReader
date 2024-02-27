using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPFUI
{


     public partial class App: Application
     {


          private void OnAppExit(object sender, ExitEventArgs e)
          {
               Admin.OnAppExit(sender, e);
          }


     }
}
