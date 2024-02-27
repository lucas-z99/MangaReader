using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFUI
{

     public class Util
     {
          //    log   -----------------------------------------------------
          public static Dictionary<string, string> call_book = new Dictionary<string, string>();

          //return name of the variable
          public static string name_of(object obj, int level = 2)
          {
               //this is achieved by searching XX line in the script file
               //level=1, if you directly call this function
               //level=2, if called in another funcion, and so on
               StackFrame frame = new StackTrace(true).GetFrame(level);
               var fileName = frame.GetFileName();
               var lineNumber = frame.GetFileLineNumber();
               var ID = fileName + lineNumber;

               if (call_book.ContainsKey(ID))
                    return call_book[ID];
               else
               {
                    var line = File.ReadLines(fileName).Skip(lineNumber - 1).Take(1).First();
                    var braket0 = line.IndexOf('(');
                    var braket1 = line.LastIndexOf(')');
                    if (braket0 == -1 || braket1 == -1)
                    {
                         Console.WriteLine("Warning: NameOf() maybe wrong line: " + line);
                         return "";
                    }
                    var variableName = line.Substring(braket0 + 1, braket1 - braket0 - 1);

                    call_book.Add(ID, variableName);
                    return variableName;
               }
          }

          public static void try_this_log<T>(T _var)
          {
               //example
               Console.WriteLine("{0}: {1}\n", name_of(_var), _var);
          }


          //    math   -----------------------------------------------------
          public static float clamp(float x, float min, float max)
          {
               if (x < min) x = min;
               else if (x > max) x = max;
               return x;
          }
          public static int clamp(int x, int min, int max)
          {
               if (x < min) x = min;
               else if (x > max) x = max;
               return x;
          }
          public static double clamp(double x, double min, double max)
          {
               if (x < min) x = min;
               else if (x > max) x = max;
               return x;
          }

          public static double lerp(double a, double b, double t)
          {
               return a + (b - a) * t;
          }
          public static float lerp(float a, float b, float t)
          {
               return a + (b - a) * t;
          }



          //   delete   -----------------------------------------------------
          public static void DeleteFolder(string path)
          {
               if (Directory.Exists(path))
                    Directory.Delete(path, true);
               else
                    Console.WriteLine("Warning: delete fail: " + path);
          }

          public static void DeleteFile(string path)
          {
               if (File.Exists(path))
                    File.Delete(path);
               else
                    Console.WriteLine("Warning: delete fail: " + path);
          }



          //   other   -----------------------------------------------------
          public static void OpenInFileExplorer(string path)
          {
               if (Directory.Exists(path) || File.Exists(path))
                    Process.Start("explorer.exe", $"/select,\"{path}\"");
          }


     }

     public static class MyExtension
     {

          //sort List<string> like windows file explorer
          [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
          static extern int StrCmpLogicalW(string x, string y);
          public static void sort_file_explorer_style(this List<string> list)
          {
               list.Sort(StrCmpLogicalW);
          }

     }





}





//public static string GetSiblingFolder(string THIS, int indexDelta, bool THISExist = true)
//{
//     if (indexDelta == 0)
//          return "";
//     var exist = Directory.Exists(THIS);
//     if (THISExist && exist == false)
//          return "";

//     List<string> children;
//     if (exist)
//     {
//          var parent = Directory.GetParent(THIS);
//          if (parent != null)
//               children = new List<string>(Directory.GetDirectories(Directory.GetParent(THIS).FullName));
//          else
//               return "";
//     }
//     else
//     {
//          //try find siblings via parent
//          var indexParent = THIS.LastIndexOf("\\");
//          if (indexParent == -1)
//               return "";
//          var parent = THIS.Substring(0, indexParent);
//          if (Directory.Exists(parent))
//          {
//               children = new List<string>(Directory.GetDirectories(parent));
//               children.Add(THIS);//continue search as if THIS exist
//          }
//          else
//               return "";
//     }
//     children.Sort_FileExplorerStyle();

//     int iTHIS = 0;
//     for (int i = 0; i < children.Count; i++)
//     {
//          if (THIS == children[i])
//          {
//               iTHIS = i;
//               break;
//          }
//     }

//     var sibling = iTHIS + indexDelta;
//     if (sibling >= 0 && sibling < children.Count)
//     {
//          return children[sibling];
//     }
//     else
//          return "";
//}