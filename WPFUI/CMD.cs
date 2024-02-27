using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace WPFUI
{
     internal class CMD
     {
          // NOTE:
          // command cannot have SPACE in them, use kebab notation instead
          // '_' one argument
          // '_ _ _' many arguments seperate by SPACE

          //filter _ _ _      apply more filter
          //clearfilter       remove all filter

          //tag _             + tag for current folder
          //removetag _       - tag for current folder
          //showtag           print tags for current folder

          public static void Enter(string input)
          {
               var p = ParseCmd(input);
               var cmd = p.Item1;
               var args = p.Item2;

               try
               {
                    switch (cmd)
                    {
                         case "filter":
                              foreach (var a in args)
                                   Navigator.SetFilter(a);
                              Navigator.Print();
                              break;
                         case "-filter":
                              Navigator.DropFilter();
                              Navigator.Print();
                              break;

                         case "tag":
                              if (args.Count() > 0)
                                   TagManager.Tag(Parser.GetCurID(), args[0]);
                              TagManager.Print();
                              break;
                         case "-tag":
                              TagManager.RemoveTag(Parser.GetCurID(), args[0]);
                              TagManager.Print();
                              break;
                         default:
                              Console.WriteLine("Unknown cmd: " + input);
                              break;
                    }
               }
               catch
               {
                    //
               }
          }

          static (string, string[]) ParseCmd(string input)
          {
               int i = input.IndexOf(' ');
               if (i == -1)
                    return (input, Array.Empty<string>());

               var command = input.Substring(0, i);
               string[] args = input.Substring(i + 1).Split(' ');

               return (command, args);
          }




     }



}

