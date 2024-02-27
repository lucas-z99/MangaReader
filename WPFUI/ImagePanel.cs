using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WPFUI
{

     internal class ImagePanel
     {

          public static ImagePanel inst;
          public ScaleTransform scaler { get; private set; }
          public bool is_initialized { get; private set; }

          readonly int half_range = 3; // if half is 10, will load total 21 images
          readonly double max_scale = 5f;
          readonly double min_scale = 1f / 12f;

          double scroll_speed { get { return Admin.settings.scroll_speed; } }
          double zoom_speed { get { return Admin.settings.zoom_speed; } }
          double keyboard_scroll_speed = 500;

          AppState appState { get { return Admin.appState; } }

          Canvas canvas { get { return MainWindow.inst.canvas; } }
          Stack<Image> pool = new Stack<Image>();
          List<Image> ui_list = new List<Image>();
          double screen_center { get { return MainWindow.inst.Height / 2; } }


          public ImagePanel()
          {
               inst = this;

               //create image ui
               for (int i = 0; i < half_range * 2 + 10; i++)
               {
                    create_new_image_ui();
               }

               //add scaler on canvas
               scaler = canvas.RenderTransform as ScaleTransform;
               if (scaler == null)
               {
                    scaler = new ScaleTransform(1, 1);
                    canvas.RenderTransform = scaler;
               }
          }


          // basic ------------------------------------------------------------------------

          public bool Goto(string path, double offset = 0, bool top_align = false)
          {
               if (File.Exists(path))// if file
               {
                    if (!ImageLoader.IsImage(path))
                         return false;

                    go_to_image(path, offset, top_align);

                    is_initialized = true;
                    return true;
               }
               else if (Directory.Exists(path))
               {
                    var images = ImageLoader.ListImages(path);
                    if (images.Count > 0) // image folder
                    {
                         go_to_image(images[0], offset, top_align);
                    }
                    else  // not image folder
                    {
                         appState.SetDir_NoImage(path);

                         HideAll();
                         MainWindow.inst.OnFolderChanged();
                    }

                    is_initialized = true;
                    return true;
               }

               return false; // path not exist
          }

          void go_to_image(string path, double offset = 0, bool top_align = false)
          {
               // load image in the center, and siblings around it

               // update path
               var dir0 = appState.dir;
               appState.SetImage(path);
               bool is_folder_changed = dir0 != appState.dir;

               //
               HideAll();

               foreach (var ui in ui_list) // cache bitmaps
               {
                    var image = (BitmapImage)ui.Source;
                    if (image != null)
                         ImageLoader.Cache(image);
               }


               // load center image
               display_image(path, true, offset, top_align);


               // load around center image
               var all_images = ImageLoader.ListImages(Directory.GetParent(path).FullName);
               int CUR = all_images.FindIndex(x => x == path);

               for (int i = 1; i <= half_range; i++)
               {
                    int next = CUR + i;
                    int last = CUR - i;

                    if (next < all_images.Count)
                    {
                         display_image(all_images[next], true);
                    }

                    if (last >= 0)
                    {
                         display_image(all_images[last], false);
                    }
               }

               if (Admin.appState.one_page_mode || is_folder_changed)
               {
                    ZoomToFit();
               }

               if (is_folder_changed || !is_initialized)
               {
                    is_initialized = true;
                    MainWindow.inst.OnFolderChanged();
               }

               ImageLoader.EndOfCache();  // -> cache end
               System.GC.Collect();
          }


          void display_image(string file, bool at_bottom, double offset = 0, bool top_align = false)
          {
               var bitmap = ImageLoader.Load(file);
               var ui = get_image_ui();

               if (bitmap == null)
               {
                    //ui.Source = ImageLoader.get_placeholder();
                    //ui.Source = MainWindow.inst.placeholder.Source;
                    MessageBox.Show("Error loading: " + file);
                    return;
               }

               ui.Source = bitmap;
               ui.Width = bitmap.PixelWidth;
               ui.Height = bitmap.PixelHeight;
               ui.Visibility = Visibility.Visible;

               //alignment
               if (ui_list.Count == 0) //this is a new center image
               {
                    ui_list.Add(ui);

                    double top = 0;

                    if (top_align)
                    {
                         top = (height - 16) / 2 * (scaler.ScaleX - 1) / scaler.ScaleX;//16 is about the missing boarder bug
                    }
                    else
                    {
                         top = offset + (height - ui.Height) / 2;
                    }

                    Canvas.SetTop(ui, top);
               }
               else if (at_bottom)
               {
                    ui_list.Add(ui);

                    var last = ui_list[ui_list.Count - 2];
                    var bot_of_last = Canvas.GetTop(last) + last.Height;
                    Canvas.SetTop(ui, bot_of_last);
               }
               else
               {
                    ui_list.Insert(0, ui);//at top

                    var top_of_next = Canvas.GetTop(ui_list[1]);
                    Canvas.SetTop(ui, top_of_next - ui.Height);
               }

               Canvas.SetLeft(ui, (canvas.Width - ui.Width) / 2);
          }


          public Image get_center()
          {
               for (int i = 0; i < ui_list.Count; i++)
               {
                    var ui = ui_list[i];
                    var top = Canvas.GetTop(ui);
                    var bottom = top + ui.Height;

                    if (screen_center >= top && screen_center <= bottom)
                    {
                         return ui;
                    }
               }

               return null;
          }


          // move and zoom -----------------------------------------------------------------

          public void on_MouseMove(Vector movement)
          {
               if (Mouse.LeftButton == MouseButtonState.Pressed)
                    scroll(movement.Y);
               if (Mouse.RightButton == MouseButtonState.Pressed)
               {
                    zoom(movement.X * zoom_speed);
                    MainWindow.inst.ToggleOnePageMode(0);
               }
          }

          public void on_MouseWheel(double input)
          {
               if (appState.one_page_mode)
                    TurnPage(-Math.Sign(input));
               else
                    scroll(input * scroll_speed);
          }

          public void keyboard_scroll(double input)
          {
               if (appState.one_page_mode)
                    TurnPage(-Math.Sign(input));
               else
                    scroll(input * keyboard_scroll_speed);
          }

          void scroll(double delta)
          {
               foreach (var ui in ui_list)
               {
                    Canvas.SetTop(ui, Canvas.GetTop(ui) + delta / scaler.ScaleX);
               }

               var center_ui = get_center();
               if (center_ui == null)
                    return;

               var path = GetSourceImageFilePath(center_ui);
               var offset = Canvas.GetTop(center_ui) + center_ui.Height / 2 - screen_center;
               Goto(path, offset);
          }

          void zoom(double delta)
          {
               scaler.ScaleX *= (1 + delta / 1000);
               scaler.ScaleX = Util.clamp(scaler.ScaleX, min_scale, max_scale);
               scaler.ScaleY = scaler.ScaleX;
          }

          public void reset_scale()
          {
               scaler.ScaleX = scaler.ScaleY = 1;
          }

          public void refresh_image_pos()
          {
               scroll(0);
          }

          // one page mode ----------------------------

          float fullscreenAdj { get { return Admin.appState.full_screen ? 12 : 0; } }
          ////when full screen & boardless, the window size is always 8-12 pixel larger than screen size
          double height { get { return MainWindow.inst.Height; } }
          double width { get { return MainWindow.inst.Width; } }

          public void ZoomToFit()
          {
               var ui = get_center();
               if (ui == null)
                    return;

               if (width / ui.Width < height / ui.Height)
                    zoom_to_fit_width(ui);
               else
                    zoom_to_fit_height(ui);

          }
          void zoom_to_fit_width(Image ui)
          {
               scaler.ScaleX = scaler.ScaleY = (width - fullscreenAdj) / ui.Width;
          }
          void zoom_to_fit_height(Image ui)
          {
               scaler.ScaleX = scaler.ScaleY = (height - fullscreenAdj) / ui.Height;
          }

          public bool TurnPage(int delta, bool top_align = false)
          {
               var sibling = ImageLoader.GetSiblingImage(appState.image, delta);
               if (File.Exists(sibling) == false)
                    return false;

               Goto(sibling, 0, top_align);

               return true;
          }


          // ui pool ---------------------------------------------------------

          void create_new_image_ui()
          {
               var ui = new Image();
               ui.Stretch = Stretch.Fill;
               ui.Visibility = Visibility.Collapsed;

               canvas.Children.Add(ui);
               pool.Push(ui);
          }

          Image get_image_ui()
          {
               return pool.Pop();
          }

          void recycle_image_ui(Image ui)
          {
               ui.Visibility = Visibility.Collapsed;
               ui.Source = null;

               ui_list.Remove(ui);
               pool.Push(ui);
          }

          public void HideAll()
          {
               for (int i = ui_list.Count - 1; i >= 0; i--)
                    recycle_image_ui(ui_list[i]);
          }



          // other ----------------------------
          static string GetSourceImageFilePath(Image ui)
          {
               var bitmap = (BitmapImage)ui.Source;
               return bitmap.UriSource.LocalPath;
          }

     }






}
