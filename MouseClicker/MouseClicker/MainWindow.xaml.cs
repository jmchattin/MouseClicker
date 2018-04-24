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

using System.ComponentModel;
using System.Threading;

using System.Windows.Threading;

using System.Runtime.InteropServices;

using System.Windows.Forms;

using System.Drawing;

namespace MouseClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string clickStatus = "";
        private DispatcherTimer clickLotsTimer;

        private DispatcherTimer countdownTimer;

        private int timeToStart = 3;

        private bool isOn = false;

        private int msInterval = 25;


        [DllImport("user32.dll", CharSet = CharSet.Auto)] // , CallingConvention = CallingConvention.StdCall
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;


        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs,
                                           int cbSize);

        //input type constant
        const int INPUT_MOUSE = 0;

        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        public struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        };

        public MainWindow()
        {
            InitializeComponent();
            
            this.countdownTimer = new DispatcherTimer();
            this.countdownTimer.Tick += new EventHandler(cdTimer_Tick);
            this.countdownTimer.Interval = new TimeSpan(0, 0, 1);

            this.clickLotsTimer = new DispatcherTimer();
            this.clickLotsTimer.Tick += new EventHandler(clickLotsTimer_Tick);
            this.clickLotsTimer.Interval = new TimeSpan(0, 0, 0, 0, msInterval);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.isOn){
                this.clickLotsTimer.Stop();
                this.isOn=false;
                this.MyButton.Content = "Start Clicking";
                this.StatusLabel.Content = "";
            }else{
                this.countdownTimer.Start();

                this.isOn=true;
            }
        }

        private void cdTimer_Tick(object sender, EventArgs e)
        {
            if (this.timeToStart == 0)
            {
                StatusLabel.Content = "Timer Clicking";
                MyButton.Content = "Stop Clicking";

                this.timeToStart = 3;

                this.clickLotsTimer.Start();

                this.countdownTimer.Stop();
            }
            else
            {
                MyButton.Content = "Start Clicking";
                StatusLabel.Content = this.timeToStart;
                this.timeToStart--;
            }
        }

        private void clickLotsTimer_Tick(object sender, EventArgs e)
        {
            DoClick();
        }

        private void DoClick()
        {
            //set cursor position to memorized location
            //Cursor.Position = clickLocation;
            //set up the INPUT struct and fill it for the mouse down
            INPUT input = new INPUT();
            input.type = INPUT_MOUSE;
            input.mi.dx = 0;
            input.mi.dy = 0;
            input.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            input.mi.dwExtraInfo = IntPtr.Zero;
            input.mi.mouseData = 0;
            input.mi.time = 0;
            //send the input 
            SendInput(1, ref input, Marshal.SizeOf(input));
            //set the INPUT for mouse up and send it
            input.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            SendInput(1, ref input, Marshal.SizeOf(input));

            //System.Drawing.Point pt = new System.Drawing.Point();
            //pt = System.Windows.Forms.Control.MousePosition;
            
            //int X = System.Windows.Forms.Control.MousePosition.X;
            //int Y = System.Windows.Forms.Control.MousePosition.Y;

            //MeClick(X, Y);
        }

        private void MeClick(int X, int Y)
        {
            Thread.Sleep(250);

            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Debugger
            int X = System.Windows.Forms.Control.MousePosition.X;
            int Y = System.Windows.Forms.Control.MousePosition.Y;

            StatusLabel.Content = string.Format("X: {0} Y: {1}",X,Y);

        }

    }
}
