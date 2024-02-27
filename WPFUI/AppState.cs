using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI
{

     [Serializable]
     internal class AppState
     {
          // cur directory, also use as last known location
          public string image { get; private set; }
          public string dir { get; private set; }
          public string parent { get; private set; }

          public void SetImage(string path)
          {
               var fi = new FileInfo(path);
               if (fi.Exists)
               {
                    image = path;
                    dir = fi.DirectoryName;
                    parent = fi.Directory.Parent.FullName;
               }
          }

          public void SetDir_NoImage(string path)
          {
               var di = new DirectoryInfo(path);
               if (di.Exists)
               {
                    image = "";
                    dir = path;
                    parent = di.Parent.FullName;
               }
          }


          // window mode
          public bool full_screen = true;
          public bool one_page_mode = false;
          public bool debug_window = false;


          // window size
          public double left = 500;
          public double top = 500;
          public double height = 720;
          public double width = 1080;


          // quick move folder
          public string path1 = "";
          public string path2 = "";

     }

}
