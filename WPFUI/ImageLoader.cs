using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace WPFUI
{

     internal class ImageLoader
     {

          static Dictionary<string, BitmapImage> _cache = new Dictionary<string, BitmapImage>();

          static List<string> image_ext = new List<string> { ".jpg", ".jpeg", ".png", ".webp", ".gif" };


          public static List<string> ListImages(string dir)
          {
               var list = new List<string>();

               foreach (var ext in image_ext)
                    list.AddRange(Directory.GetFiles(dir, "*" + ext));

               list.sort_file_explorer_style();

               return list;
          }

          public static bool IsImage(string file_path)
          {
               if (!File.Exists(file_path))
                    return false;

               return image_ext.Contains(Path.GetExtension(file_path).ToLower());
          }

          public static string GetSiblingImage(string image, int index_delta)
          {
               if (File.Exists(image) == false)
                    return "";

               var all_images = ListImages(Directory.GetParent(image).FullName);
               var pivot_index = all_images.FindIndex(x => x == image);

               var target_index = pivot_index + index_delta;
               if (target_index >= 0 && target_index < all_images.Count)
               {
                    return all_images[target_index];
               }

               return "";
          }

          public static BitmapImage Load(string file_path)
          {
               if (_cache.ContainsKey(file_path))
                    return _cache[file_path];

               var bitmap = new BitmapImage();

               try
               {
                    using (FileStream fs = File.Open(file_path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                         bitmap.BeginInit();
                         bitmap.StreamSource = fs;
                         bitmap.CacheOption = BitmapCacheOption.OnLoad;
                         bitmap.UriSource = new Uri(file_path);
                         bitmap.EndInit();
                    }
               }
               catch
               {
                    return null;// probably broken image
               }

               return bitmap;
          }

          public static void Cache(BitmapImage image)
          {
               _cache.Add(image.UriSource.LocalPath, image);
          }

          public static void EndOfCache()
          {
               _cache.Clear();
          }



     }




}
