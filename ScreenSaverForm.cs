using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Screensaver
{
    public partial class ScreenSaverForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        private Random rand = new Random();
        private Timer moveTimer = new Timer();
        private bool previewMode = false;

        public ScreenSaverForm(Rectangle Bounds)
        {
            InitializeComponent();
            this.Bounds = Bounds;
            this.WindowState = FormWindowState.Maximized;
            this.Load += ScreenSaverForm_Load;
            this.MouseMove += ScreenSaverForm_MouseMove;
            this.MouseClick += ScreenSaverForm_MouseClick;
            this.KeyPress += ScreenSaverForm_KeyPress;
        }

        public ScreenSaverForm(IntPtr PreviewWndHandle)
        {
            InitializeComponent();
        
            SetParent(this.Handle, PreviewWndHandle);
        
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));
        
            Rectangle ParentRect;
            GetClientRect(PreviewWndHandle, out ParentRect);
            Size = ParentRect.Size;
            Location = new Point(0, 0);

            moveTimer.Interval = 3000;
            moveTimer.Tick += new EventHandler(moveTimer_Tick);
            moveTimer.Start();

            previewMode = true;
        }

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\ServantChild_ScreenSaver");
     
            if (key == null)
                textLabel.Text = "Hello";
            else
                textLabel.Text = (string)key.GetValue("text");

            Cursor.Hide();
            TopMost = true;

            moveTimer.Interval = 3000;
            moveTimer.Tick += new EventHandler(moveTimer_Tick);
            moveTimer.Start();
        }

        private void moveTimer_Tick(object sender, System.EventArgs e)
        {
            // Move text to new location
            textLabel.Left = rand.Next(Math.Max(1, Bounds.Width - textLabel.Width));
            textLabel.Top = rand.Next(Math.Max(1, Bounds.Height - textLabel.Height));           
        }

        private Point mouseLocation;
 
        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseLocation.IsEmpty)
            {
                if (Math.Abs(mouseLocation.X - e.X) > 5 ||
                    Math.Abs(mouseLocation.Y - e.Y) > 5)
                    Application.Exit();
            }
        
            mouseLocation = e.Location;
        }
        
        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }
        
        private void ScreenSaverForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

    }
}
