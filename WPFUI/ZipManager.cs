using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Windows;


namespace WPFUI
{
     internal class ZipManager
     {

          public static Action OnUnzipComplete;


          public static async void UnzipAll(string root)
          {
               await _UnzipAll(root);
          }

          static async Task _UnzipAll(string root)
          {
               if (!Directory.Exists(root))
                    return;

               int skip = 0;
               int exist = 0;

               await Task.Run(() =>
               {
                    foreach (var zip in Directory.GetFiles(root, "*.zip"))
                    {

                         try
                         {
                              string folder_name = "";
                              using (var archive = ZipFile.OpenRead(zip)) // check folder name
                              {
                                   folder_name = archive.Entries[0].FullName.TrimEnd('/');

                                   var p = Parser.Parse(folder_name);
                                   if (p == null || p.folder == p.title)
                                   {
                                        skip++;
                                        continue;
                                   }
                              }
                              if (File.Exists(Path.Combine(root, folder_name))) // check exist
                              {
                                   exist++;
                                   continue;
                              }
                              ZipFile.ExtractToDirectory(zip, root); // unzip
                              File.Delete(zip);
                         }
                         catch (Exception ex)
                         {
                              Console.WriteLine($"Error - unzip: {ex.Message}  ->  {zip}");
                         }

                    }
               });


               OnUnzipComplete?.Invoke();


               string notice =
                    (skip > 0 ? $"Skip {skip} zip files\n" : "") +
                    (exist > 0 ? $"{exist} folders already exist\n" : "");
               if (notice.Length > 0)
                    MessageBox.Show(notice);

          }




     }




}

