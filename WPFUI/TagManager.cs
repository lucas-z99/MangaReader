using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI
{
     internal class TagManager
     {

          public static IReadOnlyDictionary<string, HashSet<string>> Dict { get => dict; }
          static Dictionary<string, HashSet<string>> dict; // <tag,id[]>


          public static bool Tag(string id, string tag)
          {
               if (id == null || tag == null)
                    return false;

               if (dict.ContainsKey(tag) == false)
                    dict.Add(tag, new HashSet<string>());

               dict[tag].Add(id);

               Save();
               return true;
          }

          public static bool RemoveTag(string id, string tag)
          {
               // only allow removing ONE tag for A folder at a time
               if (dict.ContainsKey(tag) == false)
                    return false;

               if (dict[tag].Contains(id) == false)
                    return false;

               dict[tag].Remove(id);

               if (dict[tag].Count == 0)
                    dict.Remove(tag);

               Save();

               return true;
          }

          public static List<string> GetTags(string id)
          {
               if (id == null)
                    return null;

               var list = new List<string>();

               foreach (var pair in dict)
                    if (pair.Value.Contains(id))
                         list.Add(pair.Key);

               return list;
          }


          //   IO   ----------------------------------------------------

          static readonly string file_name = "tags";

          public static void Save()
          {
               IO.Save(dict, file_name);
          }

          public static void Load()
          {
               dict = IO.Load(file_name) as Dictionary<string, HashSet<string>>;
               if (dict == null)
                    _new_save_file();
          }

          static void _new_save_file()
          {
               if (IO.Exist(file_name) == false)
               {
                    dict = new Dictionary<string, HashSet<string>>();
                    Save();
               }
          }


          //   debug   ----------------------------------------------------

          public static void Print()
          {
               var id = Parser.GetCurID();
               var tags = GetTags(id);

               string text = "Tag: ";
               foreach (var t in tags)
                    text += t + ", ";

               text += "  ->  " + id;

               Console.WriteLine(text);
          }



     }







}
