using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImageProcess
{
    public partial class Form1
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int FULLSCREEN_HOTKEY_ID = 1;
        private const int START_HOTKEY_ID = 2;
        private const int STOP_HOTKEY_ID = 3;
        private const int SHOWPANEL_HOTKEY_ID = 4;

        private const int ROI_UP_ID = 5;
        private const int ROI_DOWN_ID = 6;

        private bool fullscreen = false;

        public void RegisterHotKeys()
        {
		// Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
		// Compute the addition of each combination of the keys you want to be pressed
		// ALT+CTRL = 1 + 2 = 3 , CTRL+SHIFT = 2 + 4 = 6...
		// RegisterHotKey(this.Handle, MYACTION_HOTKEY_ID, 6, (int) Keys.F12);

            RegisterHotKey(this.Handle, START_HOTKEY_ID, 0, (int)Keys.F1);
            RegisterHotKey(this.Handle, STOP_HOTKEY_ID, 0, (int)Keys.F2);
            RegisterHotKey(this.Handle, FULLSCREEN_HOTKEY_ID, 0, (int)Keys.F11);
            RegisterHotKey(this.Handle, SHOWPANEL_HOTKEY_ID, 0, (int)Keys.F9);

            RegisterHotKey(this.Handle, ROI_UP_ID, 0, (int)Keys.Up);
            RegisterHotKey(this.Handle, ROI_DOWN_ID, 0, (int)Keys.Down);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                var id = m.WParam.ToInt32();
                switch (id)
                {
                    case START_HOTKEY_ID: Start(); break;
                    case STOP_HOTKEY_ID: Stop(); break;
                    case FULLSCREEN_HOTKEY_ID: ToggleFullscreen(); break;
                    case SHOWPANEL_HOTKEY_ID: TogglePanel(); break;
                    case ROI_UP_ID: RoiUp(); break;
                    case ROI_DOWN_ID: RoiDown(); break;
                }
            }
            base.WndProc(ref m);
        }

        public void RoiUp()
        {
            if (_verticalRoiPositionPercent - 10 >= 0)
            {
                _verticalRoiPositionPercent -= 10;
            }
        }

        public void RoiDown()
        {
            if (_verticalRoiPositionPercent + 10 <= 100)
            {
                _verticalRoiPositionPercent += 10;
            }
        }

        public void ToggleFullscreen()
        {
            if (!fullscreen)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Bounds = Screen.AllScreens.Length>1 ? Screen.AllScreens[1].Bounds : Screen.PrimaryScreen.Bounds;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
            fullscreen = !fullscreen;
        }

        public void TogglePanel()
        {
            if (panel1.Visible)
            {
                panel1.Hide();
            }
            else
            {
                panel1.Show();
            }
        }
    }
}
