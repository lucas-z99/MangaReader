using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Forms = System.Windows.Forms;


namespace WPFUI
{

     public partial class MainWindow : Window
     {

          public static MainWindow inst;

          AppState appState { get => Admin.appState; }
          string curImage { get => appState.image; }

          int min_load_cd = 100; // eg 100 = 0.1sec

          Point mouse_pos; // relative to window top/left
          Vector mouse_pos_delta;
          Point mouse_pos_screen; // relative to 1st monitor top/left

          DateTime t_load;
          Action OnLoseFocus;
          Action PendingDecision;
          bool is_typing = false;
          bool skip_event;
          static string _debug_log;


          public MainWindow()
          {
               InitializeComponent();

               inst = this;
               Admin.main();


               //window size
               WindowStyle = WindowStyle.None;
               ResizeMode = ResizeMode.CanResizeWithGrip;
               this.AllowsTransparency = true;


               ToggleFullScreen(appState.full_screen);

               if (!appState.full_screen)
               {
                    skip_event = true;
                    {
                         this.Left = Admin.appState.left;
                         this.Top = Admin.appState.top;
                         this.Height = Admin.appState.height;
                         this.Width = Admin.appState.width;
                    }
                    skip_event = false;
               }

               // ui
               OnLoseFocus += () => ToggleDeleteUI(0);
               ToggleDebugUI(appState.debug_window);
               ToggleDeleteUI(0);

               // user control 
               MoreButtons.Init();
               dir_browser.Init();

               //// monitors
               //screens = Forms.Screen.AllScreens;

               // debug
               Admin.Update += _debug_update;
               Admin.Update += _debug_reset_log;
          }

          //Forms.Screen[] screens;

          void _debug_update()
          {
               DebugMonitor("mouse_pos = " + Math.Round(mouse_pos.X, 2) + ", " + Math.Round(mouse_pos.Y, 2));
               DebugMonitor("mouse_pos_screen = " + mouse_pos_screen);
               //DebugProperty("mouse_pos_delta = " + Math.Round(mouse_pos_delta.X, 2) + ", " + Math.Round(mouse_pos_delta.Y, 2));
               //DebugProperty("scaleXY = " + Math.Round(ImagePanel.inst.scaler.ScaleX, 2) + ", " + Math.Round(ImagePanel.inst.scaler.ScaleY, 2));
               //DebugProperty("Mouse.Captured = " + Mouse.Captured);
               //DebugProperty("FocusedElement = " + FocusManager.GetFocusedElement(this));

               DebugMonitor("Left=" + this.Left + " Top=" + this.Top);


               //// check monitor
               //var p = new System.Drawing.Point((int)this.Left, (int)this.Top);
               //var cur_screen = Forms.Screen.FromPoint(p);

               //DebugMonitor("monitor =" + cur_screen.DeviceName);
               //DebugMonitor("Bounds =" + cur_screen.Bounds);
               //DebugMonitor("WorkingArea =" + cur_screen.WorkingArea);


               //DebugMonitor("-----------------------");

               //foreach (var s in screens)
               //{
               //     DebugMonitor("screen =" + s.DeviceName);
               //     DebugMonitor("Bounds =" + s.Bounds);
               //     DebugMonitor("WorkingArea =" + s.WorkingArea);

               //     DebugMonitor("Primary =" + s.Primary);
               //     DebugMonitor("");

               //}

          }

          void TEST(object sender, RoutedEventArgs e)
          {

          }

          void TEST2(object sender, RoutedEventArgs e)
          {

          }

          void TEST3(object sender, RoutedEventArgs e)
          {

          }



          //   resize   --------------------------------------------------

          public enum ResizeDirection
          {
               Left = 1,
               Right = 2,
               Top = 3,
               TopLeft = 4,
               TopRight = 5,
               Bottom = 6,
               BottomLeft = 7,
               BottomRight = 8,
          }

          [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
          public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

          [System.Runtime.InteropServices.DllImport("user32.dll")]
          public static extern bool ReleaseCapture();

          const int WM_SYSCOMMAND = 0x0112;
          const int SC_SIZE = 0xF000;
          void ResizeWindow(ResizeDirection direction)
          {
               ReleaseCapture();
               SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + (int)direction), IntPtr.Zero);
          }

          void ResizeR(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.Right);
          void ResizeL(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.Left);
          void ResizeT(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.Top);
          void ResizeB(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.Bottom);

          void ResizeTL(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.TopLeft);
          void ResizeTR(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.TopRight);
          void ResizeBL(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.BottomLeft);
          void ResizeBR(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.BottomRight);


          //   monitor   --------------------------------------------------
          Forms.Screen GetMonitor()
          {
               var p = new System.Drawing.Point((int)this.Left, (int)this.Top);
               return Forms.Screen.FromPoint(p);
          }

          //static double GetPrimaryScreenDisplayScale(Window window)
          //{
          //     PresentationSource source = PresentationSource.FromVisual(window);
          //     if (source != null && source.CompositionTarget != null)
          //     {
          //          double dpiX = source.CompositionTarget.TransformToDevice.M11;
          //          double scale = dpiX; // This already represents the scale factor
          //          return scale;
          //     }
          //     return 1.0; // Default scale if no window is available
          //}

          //void GetWindowPositionAndScreen()
          //{
          //     var p = new System.Drawing.Point((int)this.Left, (int)this.Top);  // convert for compatibility with Screen
          //     var monitor = Forms.Screen.FromPoint(p); // contains the majority of the window

          //     MessageBox.Show($"Window Position: ={this.Left},{this.Top}\n" +
          //        $"Current Screen: {monitor.DeviceName}");
          //}


          //   navigation   --------------------------------------------------
          public void Btn_NextFolder(object sender, RoutedEventArgs e)
          {
               FolderJump(1);
          }
          void Btn_LastFolder(object sender, RoutedEventArgs e)
          {
               FolderJump(-1);
          }
          void FolderJump(int delta)
          {
               if (DateTime.Now < t_load + new TimeSpan(0, 0, 0, 0, min_load_cd))
                    return;

               t_load = DateTime.Now;

               var folder = Navigator.GetSiblingFolder(appState.dir, delta);
               ImagePanel.inst.Goto(folder);
          }
          void ContinueLastTime()
          {
               if (!ImagePanel.inst.Goto(appState.image))
                    if (!ImagePanel.inst.Goto(appState.dir))
                         ImagePanel.inst.Goto(appState.parent);
          }

          //   call back when navigate   --------------------------------------------------
          public void OnFolderChanged()
          {
               UpdateThumbUp();
               dir_browser.UpdateContent(appState.dir);

               var di = new DirectoryInfo(appState.dir);

               text_info.Content = di.Name;

               BrowseHistory.OnChangeFolder(di.FullName);
               MoreButtons.RefreshHistory();

               var id = Parser.GetCurID();
               var tags = TagManager.GetTags(id);

               input_tags.Text = "";
               foreach (var t in tags)
                    input_tags.Text += t + "  ";
          }


          //   delete   --------------------------------------------------
          void DeleteCurImage()
          {
               var _to_delete = curImage; // cache. since before delete we will jump to another
               if (!File.Exists(_to_delete))
                    return;

               if (ImagePanel.inst.TurnPage(1) == false) // jump
                    ImagePanel.inst.TurnPage(-1);

               Util.DeleteFile(_to_delete);

               ImagePanel.inst.refresh_image_pos();
          }
          void DeleteCurFolder()
          {
               var _to_delete = appState.dir; // cache. since before delete we will jump to another
               if (!Directory.Exists(_to_delete))
                    return;

               if (ImageLoader.ListImages(_to_delete).Count == 0) // safety measure
                    return;

               var next = Navigator.GetSiblingFolder(_to_delete, 1); // jump
               if (!ImagePanel.inst.Goto(next))
               {
                    var last = Navigator.GetSiblingFolder(_to_delete, -1);
                    if (!ImagePanel.inst.Goto(last))
                    {
                         ImagePanel.inst.HideAll();
                    }
               }

               Util.DeleteFolder(_to_delete);
          }


          //   full screen   --------------------------------------------------
          void ToggleFullScreen(bool fullScreen)
          {
               if (fullScreen)
                    WindowState = WindowState.Maximized;
               else
                    WindowState = WindowState.Normal;

               appState.full_screen = fullScreen;
          }

          // ------------------------------------------------------------------------
          //                                   UI
          // ------------------------------------------------------------------------


          //   window   --------------------------------------------------

          void OnMouseMove(object sender, MouseEventArgs e)
          {
               // will update mouse position
               // on any UI, or outside window
               var new_pos = Mouse.GetPosition(this);
               mouse_pos_delta = new_pos - mouse_pos;
               mouse_pos = new_pos;
               mouse_pos_screen = PointToScreen(mouse_pos);
          }

          void OnKeyDown(object sender, KeyEventArgs e)
          {
               if (is_typing)
                    return;

               if (e.IsRepeat) //if user holddown a key
                    return;

               switch (e.Key)
               {
                    case Key.Delete:
                         ToggleDeleteUI(1);
                         PendingDecision = DeleteCurImage;
                         e.Handled = true;
                         break;
                    case Key.Enter:

                         if (FocusManager.GetFocusedElement(this) == input_debug) //focus on
                         {
                              CMD.Enter(input_debug.Text);
                              input_debug.Text = "";
                              e.Handled = true;
                         }
                         else if (PendingDecision != null)
                         {
                              ToggleDeleteUI(0);
                              var temp = PendingDecision;
                              PendingDecision = null;
                              temp.Invoke();
                              e.Handled = true;
                         }

                         break;
                    case Key.Up:
                         ImagePanel.inst.keyboard_scroll(1);
                         e.Handled = true;
                         break;
                    case Key.Down:
                         ImagePanel.inst.keyboard_scroll(-1);
                         e.Handled = true;
                         break;
                    case Key.A:
                         Btn_LastFolder(null, null);
                         break;
                    case Key.D:
                         Btn_NextFolder(null, null);
                         break;
               }
          }

          void OnDropFile(object sender, DragEventArgs e)
          {
               if (e.Data.GetDataPresent(DataFormats.FileDrop) == false)
                    return;

               var dropList = (string[])e.Data.GetData(DataFormats.FileDrop);

               foreach (var path in dropList)
                    if (ImagePanel.inst.Goto(path))
                         return;
          }

          void OnResizeWindow(object sender, SizeChangedEventArgs e)
          {
               if (skip_event)
                    return;

               canvas.Width = this.Width;
               canvas.Height = this.Height;

               Admin.appState.height = this.Height;
               Admin.appState.width = this.Width;

               if (appState.one_page_mode)
               {
                    ToggleOnePageMode(1);
               }

               ImagePanel.inst.refresh_image_pos();

               dir_browser.UpdateContent();
          }

          void OnMoveWindow(object sender, EventArgs e)
          {
               if (skip_event)
                    return;

               Admin.appState.left = this.Left;
               Admin.appState.top = this.Top;
          }


          //   title bar   --------------------------------------------------

          void Btn_FullScreen(object sender, RoutedEventArgs e)
          {
               ToggleFullScreen(!appState.full_screen);
          }

          void Btn_CloseApp(object sender, RoutedEventArgs e)
          {
               this.Close();
          }

          void Btn_Minimize(object sender, RoutedEventArgs e)
          {
               WindowState = WindowState.Minimized;
          }

          void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
          {
               if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                    ToggleFullScreen(!appState.full_screen);
          }

          void TitleBar_MouseMove(object sender, MouseEventArgs e)
          {
               // drag the window 'out' of full screen
               if (e.LeftButton == MouseButtonState.Pressed)
               {
                    if (appState.full_screen)
                    {
                         ToggleFullScreen(!appState.full_screen);

                         var monitor = GetMonitor();

                         this.Top = mouse_pos_screen.Y - monitor.Bounds.Y / 2 - 30;
                         //this.Left = mouse_pos_screen.X - monitor.Bounds.X; // TODO: too much work maybe later

                    }
                    this.DragMove();
               }
          }


          //   image click area   --------------------------------------------------

          void ImageArea_MouseDown(object sender, MouseButtonEventArgs e)
          {
               ui_clickArea.CaptureMouse();
               //once captured, this UI will continue to detect mouse event even if outside the window
               //so we can scroll or zoom freely while holding any mouse button
               //call ReleaseMouseCapture() to end

               if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
               {
                    ToggleFullScreen(!appState.full_screen);
               }

               if (e.ChangedButton == MouseButton.Middle)
               {
                    ToggleOnePageMode();
               }

               //lose focus
               OnLoseFocus?.Invoke();
               PendingDecision = null;
          }

          void ImageArea_MouseUp(object sender, MouseButtonEventArgs e)
          {
               if (Mouse.LeftButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Released)
                    ui_clickArea.ReleaseMouseCapture();
          }

          void ImageArea_MouseMove(object sender, MouseEventArgs e)
          {
               ImagePanel.inst.on_MouseMove(mouse_pos_delta);
          }

          void ImageArea_MouseWheel(object sender, MouseWheelEventArgs e)
          {
               ImagePanel.inst.on_MouseWheel(e.Delta);
          }



          //   buttons   --------------------------------------------------

          void OpenInFileExplorer(object sender, RoutedEventArgs e)
          {
               if (!ImagePanel.inst.is_initialized)
               {
                    ContinueLastTime(); // double as unlock button
                    return;
               }

               if (File.Exists(curImage) == false)
                    return;

               var parent = Directory.GetParent(curImage).FullName;

               Util.OpenInFileExplorer(parent);
               Clipboard.SetText(Path.GetFileName(parent));
          }

          void Btn_ThumbUp(object sender, MouseButtonEventArgs e)
          {
               if (File.Exists(curImage))
               {
                    var id = Parser.GetCurID();

                    if (e.ChangedButton == MouseButton.Left)
                         ThumbUp.Vote(id, 1);
                    else if (e.ChangedButton == MouseButton.Right)
                         ThumbUp.Vote(id, -1);

                    UpdateThumbUp();
               }
          }

          void UpdateThumbUp()
          {
               var vote = ThumbUp.GetVote(Parser.GetCurID());

               if (vote > 0)
                    b_thumbUp.Content = vote + "up";
               else if (vote < 0)
                    b_thumbUp.Content = vote;
               else
                    b_thumbUp.Content = "UP";
          }

          //
          void Btn_OnePageMode(object sender, RoutedEventArgs e)
          {
               ToggleOnePageMode();
          }

          public void ToggleOnePageMode(int ZeroNo_OneYes_TwoToggle = 2)
          {
               if (ZeroNo_OneYes_TwoToggle == 0)
                    appState.one_page_mode = false;
               else if (ZeroNo_OneYes_TwoToggle == 1)
                    appState.one_page_mode = true;
               else if (ZeroNo_OneYes_TwoToggle == 2)
                    appState.one_page_mode = !appState.one_page_mode;

               b_onePageMode.Content = appState.one_page_mode ? "PG" : "pg";

               if (appState.one_page_mode)
                    if (ImagePanel.inst.is_initialized)
                         ImagePanel.inst.Goto(appState.image);
          }


          //
          void ToggleDeleteUI(int hide0_show1 = -1)
          {
               Visibility v;
               if (hide0_show1 == 0)
                    v = Visibility.Collapsed;
               else if (hide0_show1 == 1)
                    v = Visibility.Visible;
               else
                    v = (b_deleteImage.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;

               b_deleteImage.Visibility = v;
               b_deleteFolder.Visibility = v;
          }
          void Btn_ToggleDeleteUI(object sender, RoutedEventArgs e)
          {
               ToggleDeleteUI();
          }
          void DeleteFolder(object sender, RoutedEventArgs e)
          {
               DeleteCurFolder();
               ToggleDeleteUI(0);
               PendingDecision = null;
          }
          void DeleteImage(object sender, RoutedEventArgs e)
          {
               DeleteCurImage();
               ToggleDeleteUI(0);
               PendingDecision = null;
          }


          //   debug   --------------------------------------------------
          static void DebugMonitor(string text)
          {
               _debug_log += text + "\n";
          }
          static void print_var_inspector<T>(T _var)
          {
               _debug_log += Util.name_of(_var, 2) + "= " + _var + "\n";
          }

          void _debug_reset_log()
          {
               text_log.Text = _debug_log;
               _debug_log = "";
          }

          void ToggleDebugUI(bool show)
          {
               appState.debug_window = show;

               var _state = Visibility.Collapsed;
               if (show)
                    _state = Visibility.Visible;

               debugUI.Visibility = _state;
          }
          void b_ToggleDebugUI(object sender, RoutedEventArgs e)
          {
               ToggleDebugUI(!appState.debug_window);
          }


     }





}

