using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFUI
{

     internal class Navigator
     {
          // Util class
          // - go to sibling
          // - filter


          //   navigation   -----------------------------------------------------
          public static string GetSiblingFolder(string THIS, int index_delta)
          {
               // THIS can be:
               // file, folder, or even a file/folder no longer exist

               if (File.Exists(THIS)) // this function also takes image path
                    THIS = Directory.GetParent(THIS).FullName;

               if (useFilter)
                    return get_sibling_folder_filter(THIS, index_delta, options);
               else
                    return get_sibling_folder(THIS, index_delta);
          }

          static string get_sibling_folder(string THIS, int index_delta)
          {
               if (index_delta == 0 || THIS == null)
                    return "";

               List<string> children = null;
               if (Directory.Exists(THIS))
               {
                    var parent = Directory.GetParent(THIS);
                    if (parent != null)
                         children = new List<string>(Directory.GetDirectories(parent.FullName));
               }
               else
               {
                    // guess!
                    var end_of_parent = THIS.LastIndexOf("\\");
                    if (end_of_parent == -1)
                         return null;
                    var parent = THIS.Substring(0, end_of_parent);
                    if (Directory.Exists(parent))
                    {
                         children = new List<string>(Directory.GetDirectories(parent));
                         children.Add(THIS); // pretend THIS exist and continue, so later we can simply get index, see below
                    }
               }

               if (children == null)
                    return null;

               children.sort_file_explorer_style();

               var iTHIS = children.FindIndex(x => x == THIS);

               var iSib = iTHIS + index_delta;
               if (iSib >= 0 && iSib < children.Count)
                    return children[iSib]; // this will work even THIS doesn't exist

               return null;
          }


          static string get_sibling_folder_filter(string THIS, int index_delta, List<string> within)
          {
               if (index_delta == 0 || THIS == null || within.Count == 0)
                    return null;

               within.sort_file_explorer_style();

               if (within.Contains(THIS) == false) // was outside filter
                    return within[0];

               var iTHIS = within.FindIndex(x => x == THIS); // was in filter range
               var iSibling = iTHIS + index_delta;
               if (iSibling >= 0 && iSibling < within.Count)
                    return within[iSibling];

               return null;
          }

          //   filter   -----------------------------------------------------
          static HashSet<string> _cacheH = new HashSet<string>();

          public static string[] Search(string keyword, string in_directory)
          {
               try
               {
                    return Directory.GetDirectories(in_directory, "*" + keyword + "*", SearchOption.TopDirectoryOnly);
               }
               catch (Exception ex)
               {
                    Console.WriteLine("An error occurred: " + ex.Message);
                    return null;
               }
          }

          public static string[] Search(string[] keywords, string dir)
          {
               try
               {
                    _cacheH.Clear();

                    foreach (string k in keywords)
                         foreach (string folder in Search(k, dir))
                              _cacheH.Add(folder);

                    return _cacheH.ToArray();
               }
               catch (Exception ex)
               {
                    Console.WriteLine("An error occurred: " + ex.Message);
                    return null;
               }
          }



          public static string[] SearchTag(string tag, string in_directory)
          {
               try
               {
                    HashSet<string> id_list = null;
                    foreach (var pair in TagManager.Dict)
                         if (pair.Key == tag)
                         {
                              id_list = pair.Value;
                              break;
                         }

                    if (id_list == null)
                         return Array.Empty<string>();


                    var result = new List<string>();
                    foreach (var dir in Directory.GetDirectories(in_directory))
                    {
                         var p = Parser.Parse(Path.GetFileName(dir));
                         if (p == null)
                              continue;

                         var id = p.GetIdString();

                         foreach (var _id in id_list)
                              if (id == _id)
                                   result.Add(dir);
                    }

                    return result.ToArray();
               }
               catch (Exception ex)
               {
                    Console.WriteLine("An error occurred: " + ex.Message);
                    return null;
               }
          }



          public static bool useFilter { get; private set; }

          static List<string> filters = new List<string>();
          static List<string> options = new List<string>();
          static string curImage { get => Admin.appState.image; }


          public static void SetFilter(string keyword)
          {
               if (keyword == "" || keyword == null)
                    return;
               if (filters.Contains(keyword))
                    return;
               if (File.Exists(curImage) == false)
                    return;

               filters.Add(keyword);
               useFilter = true;

               // search folder name
               var rootFolder = Directory.GetParent(Directory.GetParent(curImage).FullName);
               var matches = Search(keyword, rootFolder.FullName);
               foreach (var m in matches)
                    if (options.Contains(m) == false) // can improve with a HashSet
                         options.Add(m);

               // search tag
               var matches2 = SearchTag(keyword, rootFolder.FullName);
               foreach (var m in matches2)
                    if (options.Contains(m) == false) // can improve with a HashSet
                         options.Add(m);

               options.sort_file_explorer_style();
          }

          public static void DropFilter()
          {
               useFilter = false;

               filters.Clear();
               options.Clear();
          }


          //   debug   ----------------------------------------------------
          public static void Print()
          {
               var text = "Filter: ";
               foreach (var f in filters)
                    text += f + ", ";
               Console.WriteLine(text);

               var text2 = "Filtered option (" + options.Count + "): ";
               if (options.Count > 0)
                    for (int i = 0; i < Math.Min(options.Count, 25); i++)
                         text2 += options[i] + ", ";
               Console.WriteLine(text2);

          }





     }











}
