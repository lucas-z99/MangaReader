using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace WPFUI
{

     public partial class UIMoreButtons : UserControl
     {

          AppState appState { get => Admin.appState; }


          public UIMoreButtons()
          {
               InitializeComponent();
          }

          public void Init()
          {
               Toggle(0);

               input_moveFolder.Text = appState.path1;
               input_moveFolder2.Text = appState.path2;

               RefreshHistory();
          }

          // toggle
          public void Toggle(int hide0 = -1)
          {
               Visibility v;
               if (hide0 == 0)
                    v = Visibility.Collapsed;
               else
                    v = (root.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;

               root.Visibility = v;
          }
          void Btn_Toggle(object sender, RoutedEventArgs e)
          {
               Toggle();
          }

          // settings
          void OpenXmlSetting(object sender, RoutedEventArgs e)
          {
               var file = IO.ToDefaultPath(Admin.fileName_settings);
               if (File.Exists(file))
                    Util.OpenInFileExplorer(file);
          }

          // 
          void RemoveComiket(object sender, RoutedEventArgs e)
          {
               int count = 0;

               try
               {
                    var parent = new DirectoryInfo(appState.parent);

                    // cache cur folder
                    var cur_image_name = Path.GetFileName(appState.image);
                    var cur_dir = appState.dir;
                    var cur_dir_NEW = "";

                    //
                    foreach (var sibling in parent.GetDirectories())
                    {
                         var new_dir_name = Parser.RemoveComiket(sibling.Name);
                         if (new_dir_name == sibling.Name)
                              continue;

                         var new_dir = Path.Combine(parent.FullName, new_dir_name);
                         Directory.Move(sibling.FullName, new_dir);
                         count++;

                         // if cur folder is renamed
                         if (cur_dir == sibling.FullName && cur_dir != new_dir)
                              cur_dir_NEW = Path.Combine(new_dir, cur_image_name);
                    }

                    if (cur_dir_NEW != "")
                         ImagePanel.inst.Goto(cur_dir_NEW);

                    if (count > 0)
                         MessageBox.Show("[Succuss] Remove = " + count);
               }
               catch
               {
                    if (count > 0)
                         MessageBox.Show("[Error] Remove = " + count);
               }
          }


          // quick move folder
          void Btn_MoveFolder(object sender, RoutedEventArgs e)
          {
               MoveFolder(input_moveFolder.Text);
          }
          void Btn_MoveFolder2(object sender, RoutedEventArgs e)
          {
               MoveFolder(input_moveFolder2.Text);
          }

          void MoveFolder(string new_parent)
          {
               if (File.Exists(appState.image) == false || Directory.Exists(appState.dir) == false)
                    return;

               if (Directory.Exists(new_parent) == false)
               {
                    MessageBox.Show("Error - target not exist:\n" + new_parent);
                    return;
               }

               var new_path = Path.Combine(new_parent, Path.GetFileName(appState.dir));
               if (Directory.Exists(new_path))
               {
                    MessageBox.Show("Error - already already exist:\n" + new_path);
                    return;
               }

               Directory.Move(appState.dir, new_path);

               MainWindow.inst.Btn_NextFolder(null, null);
          }

          // history
          void Btn_HistoryLast(object sender, RoutedEventArgs e)
          {
               BrowseHistory.visit_last();
               RefreshHistory();
          }

          void Btn_HistoryNext(object sender, RoutedEventArgs e)
          {
               BrowseHistory.visit_next();
               RefreshHistory();
          }

          public void RefreshHistory()
          {
               btn_history_last.IsEnabled = BrowseHistory.has_last;
               btn_history_next.IsEnabled = BrowseHistory.has_next;
          }









     }
}
