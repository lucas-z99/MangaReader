using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace WPFUI
{

     public partial class UIDirectoryBrowser : UserControl
     {

          string toggle_btn = "   +   ";
          string back_btn = "   ↑   ";
          int width = 485;
          int width_mini = 28;
          int height_mini = 28;
          bool is_toggled;
          string cur_dir;

          AppState appState { get => Admin.appState; }


          public UIDirectoryBrowser()
          {
               InitializeComponent();
          }

          public void Init()
          {
               UpdateContent();
          }



          //   browser   --------------------------------------------------


          public void UpdateContent(string dir = "") // start here
          {
               if (dir == "")
                    dir = appState.dir;

               list_box.Items.Clear();

               if (!Directory.Exists(dir))
               {
                    UpdateSize();
                    return;
               }
               else
               {
                    cur_dir = dir;
                    is_toggled = true;

                    var child = Directory.GetDirectories(dir);
                    if (child.Length > 0)
                    {
                         // show toggle/back + all child 
                         list_box.Items.Add(toggle_btn);
                         list_box.Items.Add(back_btn);

                         var _cache = new List<string>();
                         foreach (var c in child)
                              _cache.Add(c);
                         _cache.sort_file_explorer_style();

                         foreach (var c in _cache)
                              list_box.Items.Add(Path.GetFileName(c));

                         UpdateSize();
                    }
                    else
                    {
                         // only show back
                         list_box.Items.Add(back_btn);

                         //DirBrowser.Height = height_mini; // resize
                         //DirBrowser.Width = width_mini;

                         UpdateSize();
                    }
               }


          }

          public void UpdateSize()
          {
               var h = list_box.Items.Count * 24 + 4; // +4 margin
               h = Math.Min(h, (int)MainWindow.inst.Height - 200);
               h = Math.Min(h, (int)SystemParameters.WorkArea.Height - 200);
               h = Math.Max(h, height_mini);
               list_box.Height = h;


               if (list_box.Items.Count > 1)
                    list_box.Width = width;
               else
                    list_box.Width = width_mini;
          }

          void DirBrowser_Click(object sender, MouseButtonEventArgs e)
          {
               if (list_box.SelectedItem == null)
                    return;

               var clicking = list_box.SelectedItem.ToString();
               if (clicking == toggle_btn)
               {
                    ToggleDirBrowser();
                    return;
               }

               string dir = "";
               if (clicking == back_btn)
               {
                    var p = Directory.GetParent(cur_dir);
                    if (p != null) // p = null when we are at root
                         dir = p.FullName;
               }
               else
                    dir = Path.Combine(cur_dir, clicking);

               if (dir == appState.dir)
               {
                    UpdateContent(dir); // if we loop back
                    return;
               }

               if (ImageLoader.ListImages(dir).Count > 0)
                    ImagePanel.inst.Goto(dir);
               else
                    UpdateContent(dir); // just browsing in this UI, no jump

          }

          void ToggleDirBrowser()
          {
               is_toggled = !is_toggled;

               if (is_toggled)
                    UpdateContent(cur_dir);
               else
               {
                    list_box.Items.Clear();
                    list_box.Items.Add(toggle_btn);

                    UpdateSize();
               }
          }










     }
}
