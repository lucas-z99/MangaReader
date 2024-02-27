using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFUI
{
     public class BrowseHistory
     {
          static List<string> chain = new List<string>();
          static int index = -1; // current


          public static void visit_last()
          {
               int last = index;

               while (last > 0)
               {
                    last--;

                    if (Directory.Exists(chain[last]))
                    {
                         goto_history(last);
                         break;
                    }
                    else
                    {
                         chain.RemoveAt(last);
                    }
               }

               if (chain.Count == 0)
                    index = -1; // always stop at Count=0 and index=-1
          }

          public static void visit_next()
          {
               int next = index + 1;

               while (next <= chain.Count - 1)
               {
                    if (Directory.Exists(chain[next]))
                    {
                         goto_history(next);
                         break;
                    }
                    else
                    {
                         chain.RemoveAt(next);
                    }
               }

               if (chain.Count == 0)
                    index = -1; // always stop at Count=0 and index=-1
          }

          public static void OnChangeFolder(string dir)
          {
               if (Directory.Exists(dir) == false)
                    return;

               if (chain.Count > 0 && dir == chain[index])
                    return; // no change

               // remove everything after index
               int r_start = index + 1;
               int r_count = chain.Count - r_start;
               if (r_count > 0)
                    chain.RemoveRange(r_start, r_count);

               // add folder
               chain.Add(dir);
               index++;

          }

          public static bool has_next
          {
               get
               {
                    if (chain.Count <= 1) return false;
                    if (index == chain.Count - 1) return false;
                    return true;
               }
          }

          public static bool has_last
          {
               get
               {
                    if (chain.Count <= 1) return false;
                    if (index == 0) return false;
                    return true;
               }
          }

          static void goto_history(int _index)
          {
               index = _index;
               ImagePanel.inst.Goto(chain[index]);
          }




     }


}
