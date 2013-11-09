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
using BMDSwitcherAPI;
using SwitcherPanelCSharp;

namespace OverlayDropper
{
    public partial class OverlayDropper : Form
    {
        #region Keybook hooks
        private Boolean IsCtrlDown;
        private Boolean IsShiftDown;
        private Boolean IsAltDown;
        private String SelectedGame;
        private KeyboardHookListener m_KeyboardHookManager;
        #endregion

        #region Blackmagic ATEM
        private IBMDSwitcherDiscovery m_switcherDiscovery;
        private IBMDSwitcher m_switcher;
        private IBMDSwitcherMixEffectBlock m_mixEffectBlock1;

        private SwitcherMonitor m_switcherMonitor;
        private MixEffectBlockMonitor m_mixEffectBlockMonitor;

        private Boolean m_moveSliderDownwards = false;
        private Boolean m_currentTransitionReachedHalfway = false;

        private List<InputMonitor> m_inputMonitors = new List<InputMonitor>();

        private long ProgramInputId;
        private String ProgramInputName;
        #endregion

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public OverlayDropper()
        {
            InitializeComponent();
            ATEMIpAddressTextBox.Text = Properties.Settings.Default.IPAddress.ToString();
            m_KeyboardHookManager = new KeyboardHookListener(new GlobalHooker());

            m_switcherMonitor = new SwitcherMonitor();
            // note: this invoke pattern ensures our callback is called in the main thread. We are making double
            // use of lambda expressions here to achieve this.
            // Essentially, the events will arrive at the callback class (implemented by our monitor classes)
            // on a separate thread. We must marshell these to the main thread, and we're doing this by calling
            // invoke on the Windows Forms object. The lambda expression is just a simplification.
            m_switcherMonitor.SwitcherDisconnected += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => SwitcherDisconnected())));

            m_mixEffectBlockMonitor = new MixEffectBlockMonitor();
            m_mixEffectBlockMonitor.ProgramInputChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => UpdateItems())));
            
            m_switcherDiscovery = new CBMDSwitcherDiscovery();
            if (m_switcherDiscovery == null)
            {
                MessageBox.Show("Could not create Switcher Discovery Instance.\nATEM Switcher Software may not be installed.", "Error");
                Environment.Exit(1);
            }

            SwitcherDisconnected();		// start with switcher disconnected
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

        private void ATEMIpAddressTextBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IPAddress = ATEMIpAddressTextBox.Text;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Event handlers of particular events. They will be activated when an appropriate checkbox is checked.


        private void GameSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedGame = GameSelector.SelectedItem.ToString();

            if (SelectedGame == "League of Legends")
            {
                HotKeyLabel.Text = "Toggle Overlay: A";
            }
            else if (SelectedGame == "Starcraft 2")
            {
                HotKeyLabel.Text = "Toggle Overlay: Ctrl+W";
            }
            else
            {
                HotKeyLabel.Text = "";
            }
        }
        
        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "LControlKey")
            {
                IsCtrlDown = true;
            }
            else if (e.KeyCode.ToString() == "LShiftKey")
            {
                IsShiftDown = true;
            }
            else if (e.KeyCode.ToString() == "LAltKey")
            {
                IsAltDown = true;
            }

            Log(string.Format("KeyDown \t {0}", e.KeyCode));
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "LControlKey")
            {
                IsCtrlDown = false;
            }
            else if (e.KeyCode.ToString() == "LShiftKey")
            {
                IsShiftDown = false;
            }
            else if (e.KeyCode.ToString() == "LAltKey")
            {
                IsAltDown = false;
            }

            if (SelectedGame == "League of Legends" && e.KeyCode.ToString() == "A")
            {
                ToggleLeagueOfLegendsOverlays();
            }
            else if (SelectedGame == "Starcraft 2" && IsCtrlDown && e.KeyCode.ToString() == "W")
            {
                ToggleStarcraft2Overlays();
            }

            Log(string.Format("KeyUp  \t {0}", e.KeyCode));
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

        #region ATEM Connections
        private void ATEMConnectButton_Click(object sender, EventArgs e)
        {
            _BMDSwitcherConnectToFailure failReason = 0;
            string address = ATEMIpAddressTextBox.Text;

            try
            {
                // Note that ConnectTo() can take several seconds to return, both for success or failure,
                // depending upon hostname resolution and network response times, so it may be best to
                // do this in a separate thread to prevent the main GUI thread blocking.
                m_switcherDiscovery.ConnectTo(address, out m_switcher, out failReason);
            }
            catch (COMException)
            {
                // An exception will be thrown if ConnectTo fails. For more information, see failReason.
                switch (failReason)
                {
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureNoResponse:
                        MessageBox.Show("No response from Switcher", "Error");
                        break;
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureIncompatibleFirmware:
                        MessageBox.Show("Switcher has incompatible firmware", "Error");
                        break;
                    default:
                        MessageBox.Show("Connection failed for unknown reason", "Error");
                        break;
                }
                return;
            }

            SwitcherConnected();
        }

        private void SwitcherConnected()
        {
            ATEMConnectButton.Enabled = false;

            // Install SwitcherMonitor callbacks:
            m_switcher.AddCallback(m_switcherMonitor);

            // We create input monitors for each input. To do this we iterator over all inputs:
            // This will allow us to update the combo boxes when input names change:
            IBMDSwitcherInputIterator inputIterator;
            if (SwitcherAPIHelper.CreateIterator(m_switcher, out inputIterator))
            {
                IBMDSwitcherInput input;
                inputIterator.Next(out input);
                while (input != null)
                {
                    InputMonitor newInputMonitor = new InputMonitor(input);
                    input.AddCallback(newInputMonitor);
                    newInputMonitor.LongNameChanged += new SwitcherEventHandler(OnInputLongNameChanged);

                    m_inputMonitors.Add(newInputMonitor);

                    inputIterator.Next(out input);
                }
            }

            // We want to get the first Mix Effect block (ME 1). We create a ME iterator,
            // and then get the first one:
            m_mixEffectBlock1 = null;
            IBMDSwitcherMixEffectBlockIterator meIterator;
            SwitcherAPIHelper.CreateIterator(m_switcher, out meIterator);

            if (meIterator != null)
            {
                meIterator.Next(out m_mixEffectBlock1);
            }

            if (m_mixEffectBlock1 == null)
            {
                MessageBox.Show("Unexpected: Could not get first mix effect block", "Error");
                return;
            }

            // Install MixEffectBlockMonitor callbacks:
            m_mixEffectBlock1.AddCallback(m_mixEffectBlockMonitor);
            UpdateItems();
        }

        private void SwitcherDisconnected()
        {
            ATEMConnectButton.Enabled = true;

            // Remove all input monitors, remove callbacks
            foreach (InputMonitor inputMon in m_inputMonitors)
            {
                inputMon.Input.RemoveCallback(inputMon);
                inputMon.LongNameChanged -= new SwitcherEventHandler(OnInputLongNameChanged);
            }
            m_inputMonitors.Clear();

            if (m_mixEffectBlock1 != null)
            {
                // Remove callback
                m_mixEffectBlock1.RemoveCallback(m_mixEffectBlockMonitor);

                // Release reference
                m_mixEffectBlock1 = null;
            }

            if (m_switcher != null)
            {
                // Remove callback:
                m_switcher.RemoveCallback(m_switcherMonitor);

                // release reference:
                m_switcher = null;
            }
        }


        private void UpdateItems()
        {
            m_mixEffectBlock1.GetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdProgramInput, out ProgramInputId);

            // Get an input iterator. We use the SwitcherAPIHelper to create the iterator for us:
            IBMDSwitcherInputIterator inputIterator;
            if (!SwitcherAPIHelper.CreateIterator(m_switcher, out inputIterator))
                return;

            IBMDSwitcherInput input;
            inputIterator.Next(out input);
            while (input != null)
            {
                string inputName;
                long inputId;

                input.GetInputId(out inputId);
                input.GetString(_BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdLongName, out inputName);

                if (inputId == ProgramInputId)
                {
                    ProgramInputName = inputName;
                }
            }

            if (ProgramInputName != "")
            {
                ProgLabel.Text = string.Format("Prog: {0}", ProgramInputName);
            }
            else
            {
                ProgLabel.Text = "";
            }
        }

        private void OnInputLongNameChanged(object sender, object args)
        {
            this.Invoke((Action)(() => UpdateItems()));
        }
        #endregion

        /// <summary>
        /// Used for putting other object types into combo boxes.
        /// </summary>
        struct StringObjectPair<T>
        {
            public string name;
            public T value;

            public StringObjectPair(string name, T value)
            {
                this.name = name;
                this.value = value;
            }

            public override string ToString()
            {
                return name;
            }
        }
    }
}
