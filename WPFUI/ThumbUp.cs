using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFUI
{


     internal class ThumbUp
     {

          static Dictionary<string, int> vote_list;

          public static void Vote(string id, int delta)
          {
               Console.WriteLine("Vote(): id = " + id);

               if (vote_list.ContainsKey(id))
                    vote_list[id] += delta;
               else
                    vote_list.Add(id, delta);

               if (vote_list[id] == 0)
                    vote_list.Remove(id);

               Save();
          }

          public static int GetVote(string id)
          {
               if (id == "")
                    return 0;

               if (vote_list.ContainsKey(id))
                    return vote_list[id];

               return 0;
          }



          //   IO   ----------------------------------------------------

          static readonly string file_name = "thumbUp";

          public static void Save()
          {
               IO.Save(vote_list, file_name);
          }

          public static void Load()
          {
               vote_list = IO.Load(file_name) as Dictionary<string, int>;
               if (vote_list == null)
                    _new_save_file();
          }

          static void _new_save_file()
          {
               if (IO.Exist(file_name) == false)
               {
                    vote_list = new Dictionary<string, int>();
                    Save();
               }
          }


          //   debug   ----------------------------------------------------
          public static void DebugPrint()
          {
               Console.WriteLine("--------------  ThumbUp.vote_list  --------------");

               foreach (var pair in vote_list)
                    Console.WriteLine("[x{0}] {1}", pair.Value, pair.Key);

               Console.WriteLine("Total = " + vote_list.Count + "\n");
               Console.WriteLine("-------------------------------");

          }





     }















}
