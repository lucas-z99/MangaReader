using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;


namespace WPFUI
{

     internal class IO
     {

          public static readonly string default_path;
          public static bool log = true;


          static IO()
          {
               default_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data\\"); ;

               if (Directory.Exists(default_path) == false)
                    Directory.CreateDirectory(default_path);
          }


          //   public   ----------------------------------------------------

          public static void Save<T>(T obj, string path)
          {
               _save(obj, ToDefaultPath(path));
          }
          public static object Load(string path)
          {
               return _load(ToDefaultPath(path));
          }

          public static bool Exist(string file_name)
          {
               return Directory.Exists(ToDefaultPath(file_name));
          }
          public static string ToDefaultPath(string path)
          {
               return default_path + path;
          }


          //   save to xml   ----------------------------------------------------

          public static void SaveXml<T>(T obj, string path)
          {
               var s = new XmlSerializer(typeof(T));
               using (StreamWriter writer = new StreamWriter(ToDefaultPath(path)))
                    s.Serialize(writer, obj);
          }

          public static T LoadXml<T>(string path)
          {
               var s = new XmlSerializer(typeof(T));
               using (var stream = new FileStream(ToDefaultPath(path), FileMode.Open))
                    return (T)s.Deserialize(stream);
          }


          //   private   ----------------------------------------------------

          static void _save<T>(T obj, string path)
          {
               try
               {
                    using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                    {
                         var s = new BinaryFormatter();
                         s.Serialize(fs, obj);
                    }
               }
               catch (Exception e)
               {
                    Console.Error.WriteLine("Error - IO.Save: " + path);
                    Console.Error.WriteLine(e.Message);
               }

          }

          static object _load(string path)
          {
               try
               {
                    using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                         var s = new BinaryFormatter();
                         object data = s.Deserialize(fs);
                         return data;
                    }
               }
               catch (Exception e)
               {
                    Console.Error.WriteLine("Error - IO.Load: " + path);
                    Console.Error.WriteLine(e.Message);
                    return null;
               }
          }





          //for Unity
          //public static string savePath = Application.persistentDataPath + "/Save";

          //Windows Store Apps: Application.persistentDataPath points to %userprofile%\AppData\Local\Packages\<productname>\LocalState.
          //Windows Editor and Standalone Player: Application.persistentDataPath usually points to %userprofile%\AppData\LocalLow\<companyname>\<productname>. It is resolved by SHGetKnownFolderPath with FOLDERID_LocalAppDataLow, or SHGetFolderPathW with CSIDL_LOCAL_APPDATA if the former is not available.
          //WebGL: Application.persistentDataPath points to /idbfs/<md5 hash of data path> where the data path is the URL stripped of everything including and after the last '/' before any '?' components.
          //Linux: Application.persistentDataPath points to $XDG_CONFIG_HOME/unity3d or $HOME/.config/unity3d.
          //iOS: Application.persistentDataPath points to /var/mobile/Containers/Data/Application/<guid>/Documents.
          //tvOS: Application.persistentDataPath is not supported and returns an empty string.
          //Android: Application.persistentDataPath points to /storage/emulated/0/Android/data/<packagename>/files on most devices (some older phones might point to location on SD card if present), the path is resolved using android.content.Context.getExternalFilesDir.
          //Mac: Application.persistentDataPath points to the user Library folder. (This folder is often hidden.) In recent Unity releases user data is written into ~/Library/Application Support/company name/product name. Older versions of Unity wrote into the ~/Library/Caches folder, or ~/Library/Application Support/unity.company name.product name. These folders are all searched for by Unity. The application finds and uses the oldest folder with the required data on your system.



     }





}
