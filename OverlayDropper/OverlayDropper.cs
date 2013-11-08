using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using System.Runtime.InteropServices;
namespace OverlayDropper
{
    public partial class OverlayDropper : Form
    {
        private Boolean IsCtrlDown;
        private Boolean IsShiftDown;
        private Boolean IsAltDown;
        private String SelectedGame;
        private KeyboardHookListener m_KeyboardHookManager;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public OverlayDropper()
        {
            InitializeComponent();
            m_KeyboardHookManager = new KeyboardHookListener(new GlobalHooker());
        }

        #region Initialisation of events and application
        private void checkBoxEnabled_CheckedChanged(object sender, EventArgs e)
        {
            m_KeyboardHookManager.Enabled = checkBoxEnabled.Checked;
            if (checkBoxEnabled.Checked)
            {
                m_KeyboardHookManager.KeyDown += HookManager_KeyDown;
                m_KeyboardHookManager.KeyUp += HookManager_KeyUp;
            }
            else
            {
                m_KeyboardHookManager.KeyDown -= HookManager_KeyDown;
                m_KeyboardHookManager.KeyUp -= HookManager_KeyUp;
            }
        }
        #endregion

        #region Event handlers of particular events. They will be activated when an appropriate checkbox is checked.


        private void GameSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedGame = GameSelector.SelectedItem.ToString();
        }
        
        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "LControlKey")
            {
                IsCtrlDown = true;
            }
            if (e.KeyCode.ToString() == "LShiftKey")
            {
                IsShiftDown = true;
            }
            if (e.KeyCode.ToString() == "LAltKey")
            {
                IsAltDown = true;
            }

            Log(string.Format("KeyDown \t\t {0}\n", e.KeyCode));
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "LControlKey")
            {
                IsCtrlDown = false;
            }
            if (e.KeyCode.ToString() == "LShiftKey")
            {
                IsShiftDown = false;
            }
            if (e.KeyCode.ToString() == "LAltKey")
            {
                IsAltDown = false;
            }

            if (SelectedGame == "League of Legends" && e.KeyCode.ToString() == "A")
            {
                ToggleLeagueOfLegendsOverlays();
            }

            if (SelectedGame == "Starcraft 2" && IsCtrlDown && e.KeyCode.ToString() == "W")
            {
                ToggleStarcraft2Overlays();
            }

            Log(string.Format("KeyUp  \t\t {0}\n", e.KeyCode));
        }
        #endregion

        #region Game specific events
        private void ToggleLeagueOfLegendsOverlays()
        {
            // TODO
            Log("Toggling LoL\n");
        }

        private void ToggleStarcraft2Overlays()
        {
            // TODO
            Log("Toggling SC2\n");
        }
        #endregion

        private void Log(string text)
        {
            System.Console.WriteLine(string.Format("{0}, {1}", text, GetActiveWindowTitle()));
        }

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
    }
}
