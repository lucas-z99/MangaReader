using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Threading;



namespace WPFUI
{

     internal class Admin
     {

          public static AppState appState { get; private set; }
          public static XmlSettings settings { get; private set; }

          public static readonly string fileName_app_state = "state";
          public static readonly string fileName_settings = "settings.xml";

          static DispatcherTimer timer;

          public static Action Update;


          public static void main()
          {
               Load();

               new ImagePanel();

               InitUpdate();
          }


          // update
          static void InitUpdate()
          {
               timer = new DispatcherTimer();
               timer.Interval = TimeSpan.FromMilliseconds(100);
               timer.Tick += new EventHandler(_Update);
               timer.Start();
          }
          static void _Update(object sender, EventArgs e)
          {
               Update?.Invoke();
          }


          // IO
          static void Load()
          {
               // state
               if (!File.Exists(IO.ToDefaultPath(fileName_app_state)))
                    appState = new AppState();
               else
                    appState = IO.Load(fileName_app_state) as AppState;


               // settings
               if (!File.Exists(IO.ToDefaultPath(fileName_settings)))
                    IO.SaveXml(new XmlSettings(), fileName_settings);

               settings = IO.LoadXml<XmlSettings>(fileName_settings);


               // others
               ThumbUp.Load();
               TagManager.Load();
          }

          public static void SaveAppState()
          {
               IO.Save(appState, fileName_app_state);
          }


          // exit
          public static void OnAppExit(object sender, ExitEventArgs e)
          {
               appState.path1 = MainWindow.inst.MoreButtons.input_moveFolder.Text;
               appState.path2 = MainWindow.inst.MoreButtons.input_moveFolder2.Text;

               SaveAppState();
          }


     }




}
