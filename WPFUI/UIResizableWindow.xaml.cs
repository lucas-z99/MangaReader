using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFUI
{
     /// <summary>
     /// Interaction logic for ResizableWindow.xaml
     /// </summary>
     public partial class UIResizableWindow : UserControl
     {
          public UIResizableWindow()
          {
               InitializeComponent();
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
               SendMessage(new System.Windows.Interop.WindowInteropHelper(MainWindow.inst).Handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + (int)direction), IntPtr.Zero);
          }

          void ResizeR(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.Right);
          void ResizeL(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.Left);
          void ResizeT(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.Top);
          void ResizeB(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.Bottom);

          void ResizeTL(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.TopLeft);
          void ResizeTR(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.TopRight);
          void ResizeBL(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.BottomLeft);
          void ResizeBR(object sender, MouseButtonEventArgs e) => ResizeWindow(ResizeDirection.BottomRight);



     }
}
