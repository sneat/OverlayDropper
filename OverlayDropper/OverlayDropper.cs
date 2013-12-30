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

namespace SwitcherPanelCSharp
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
        private TransitionMonitor m_transitionMonitor;
        private MixEffectBlockMonitor m_mixEffectBlockMonitor;
        private KeyMonitor m_keyMonitor;
        private DownStreamKeyMonitor m_dkeyMonitor;
        private IBMDSwitcherTransitionParameters m_transition;
        private IBMDSwitcherKey me1_key1, me1_key2, me1_key3, me1_key4;
        private IBMDSwitcherDownstreamKey me1_dkey1, me1_dkey2;

        private Boolean me1_dkey1_on, me1_dkey2_on, me1_dkey1_tie_on, me1_dkey2_tie_on, me1_key1_on, me1_key2_on, me1_key3_on, me1_key4_on, me1_bkg_tie_on, me1_key1_tie_on, me1_key2_tie_on, me1_key3_tie_on, me1_key4_tie_on = false;

        private List<InputMonitor> m_inputMonitors = new List<InputMonitor>();

        private long GameInputId;
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

            m_keyMonitor = new KeyMonitor();
            m_dkeyMonitor = new DownStreamKeyMonitor();
            m_transitionMonitor = new TransitionMonitor();
            
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

        private void GameSourceSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameInputId = Convert.ToInt64(GameSourceSelector.SelectedItem);
        }
        #endregion

        #region Event handlers of particular events. They will be activated when an appropriate checkbox is checked.


        private void GameSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedGame = GameSelector.SelectedItem.ToString();

            if (SelectedGame == "League of Legends")
            {
                HotKeyLabel.Text = "Toggle Overlay: A or H or O";
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
            if (e.KeyCode.ToString() == "LControlKey" || e.KeyCode.ToString() == "RControlKey")
            {
                IsCtrlDown = true;
            }
            else if (e.KeyCode.ToString() == "LShiftKey" || e.KeyCode.ToString() == "RShiftKey")
            {
                IsShiftDown = true;
            }
            else if (e.KeyCode.ToString() == "LMenu" || e.KeyCode.ToString() == "RMenu")
            {
                IsAltDown = true;
            }

            Log(string.Format("KeyDown \t {0}", e.KeyCode));
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "LControlKey" || e.KeyCode.ToString() == "RControlKey")
            {
                IsCtrlDown = false;
            }
            else if (e.KeyCode.ToString() == "LShiftKey" || e.KeyCode.ToString() == "RShiftKey")
            {
                IsShiftDown = false;
            }
            else if (e.KeyCode.ToString() == "LMenu" || e.KeyCode.ToString() == "RMenu")
            {
                IsAltDown = false;
            }

            if (SelectedGame == "League of Legends" && (e.KeyCode.ToString() == "A" || e.KeyCode.ToString() == "O" || e.KeyCode.ToString() == "H") && GetActiveWindowTitle() == "League of Legends (TM) Client")
            {
                ToggleLeagueOfLegendsOverlays();
            }
            else if (SelectedGame == "Starcraft 2" && IsCtrlDown && e.KeyCode.ToString() == "W" && GetActiveWindowTitle() == "StarCraft II")
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
            ToggleOverlays();
        }

        private void ToggleStarcraft2Overlays()
        {
            // TODO
            Log("Toggling SC2\n");
            ToggleOverlays();
        }

        private void ToggleOverlays()
        {
            if (GameInputId == 0 || ProgramInputId != GameInputId)
            {
                return;
            }
            int dkey1;
            int dkey2;
            int dkey1tie;
            int dkey2tie;

            me1_dkey1.GetOnAir(out dkey1);
            me1_dkey2.GetOnAir(out dkey2);
            me1_dkey1.GetTie(out dkey1tie);
            me1_dkey2.GetTie(out dkey2tie);

            // Check Downstream Keyer 1
            if (dkey1 == 1)
            {
                // DS Keyer is currently on, store this and switch it off
                me1_dkey1_on = true;
                me1_dkey1.SetOnAir(0);

                if (dkey1tie == 1)
                {
                    // DS Keyer Tie is currently on, store this and switch it off
                    me1_dkey1_tie_on = true;
                    me1_dkey1.SetTie(0);
                }
                else
                {
                    me1_dkey1_tie_on = false;
                }
            }
            else if (me1_dkey1_on)
            {
                // DS Keyer is off but it used to be on, turn it on again by auto transition
                me1_dkey1_on = false;
                me1_dkey1.PerformAutoTransition();

                if (me1_dkey1_tie_on)
                {
                    // Turn DS Keyer Tie on again
                    // TODO Auto transition prevent tie being set until after transition finishes, make the tie enable after transition completes
                    me1_dkey1.SetTie(1);
                }
            }
            else if (dkey1tie == 1)
            {
                // US Keyer Tie is currently on, store this and switch it off
                me1_dkey1_tie_on = true;
                me1_dkey1.SetTie(0);
            }
            else if (me1_dkey1_tie_on)
            {
                // Turn US Keyer Tie on again
                me1_dkey1.SetTie(1);
            }
            else
            {
                me1_dkey1_tie_on = false;
            }

            // Check Downstreak Keyer 2
            if (dkey2 == 1)
            {
                // DS Keyer is currently on, store this and switch it off
                me1_dkey2_on = true;
                me1_dkey2.SetOnAir(0);

                if (dkey2tie == 1)
                {
                    // DS Keyer Tie is currently on, store this and switch it off
                    me1_dkey2_tie_on = true;
                    me1_dkey2.SetTie(0);
                }
                else
                {
                    me1_dkey2_tie_on = false;
                }
            }
            else if (me1_dkey2_on)
            {
                // DS Keyer is off but it used to be on, turn it on again by auto transition
                me1_dkey2_on = false;
                me1_dkey2.PerformAutoTransition();

                if (me1_dkey2_tie_on)
                {
                    // Turn DS Keyer Tie on again
                    // TODO Auto transition prevent tie being set until after transition finishes, make the tie enable after transition completes
                    me1_dkey2.SetTie(1);
                }
            }
            else if (dkey2tie == 1)
            {
                // US Keyer Tie is currently on, store this and switch it off
                me1_dkey2_tie_on = true;
                me1_dkey2.SetTie(0);
            }
            else if (me1_dkey2_tie_on)
            {
                // Turn US Keyer Tie on again
                me1_dkey2.SetTie(1);
            }
            else
            {
                me1_dkey2_tie_on = false;
            }

            // Check Upstream Keyers
            int key1 = 0;
            int key2 = 0;
            int key3 = 0;
            int key4 = 0;

            if (me1_key1 != null) { me1_key1.GetOnAir(out key1); }
            if (me1_key2 != null) { me1_key2.GetOnAir(out key2); }
            if (me1_key3 != null) { me1_key3.GetOnAir(out key3); }
            if (me1_key4 != null) { me1_key4.GetOnAir(out key4); }

            int key1tie = 0;
            int key2tie = 0;
            int key3tie = 0;
            int key4tie = 0;
            DetermineUpstreamKeyTies(out key1tie, out key2tie, out key3tie, out key4tie);

            if (key1 == 1)
            {
                // US Keyer is currently on, store this and switch it off
                me1_key1_on = true;
                me1_key1.SetOnAir(0);

                if (key1tie == 1)
                {
                    // US Keyer Tie is currently on, store this and switch it off
                    me1_key1_tie_on = true;
                    ToggleKeyTie1(false);
                }
                else
                {
                    me1_key1_tie_on = false;
                }
            }
            else if (me1_key1_on)
            {
                // US Keyer is off but it used to be on, turn it on again straight away
                // TODO Delay this by 1 second? Or Transition to it?
                me1_key1_on = false;
                me1_key1.SetOnAir(1);

                if (me1_key1_tie_on)
                {
                    // Turn US Keyer Tie on again
                    ToggleKeyTie1(true);
                }
            }
            else if (key1tie == 1)
            {
                // US Keyer Tie is currently on, store this and switch it off
                me1_key1_tie_on = true;
                ToggleKeyTie1(false);
            }
            else if (me1_key1_tie_on)
            {
                // Turn US Keyer Tie on again
                ToggleKeyTie1(true);
            }
            else
            {
                me1_key1_tie_on = false;
            }

            if (key2 == 1)
            {
                // US Keyer is currently on, store this and switch it off
                me1_key2_on = true;
                me1_key2.SetOnAir(0);

                if (key2tie == 1)
                {
                    // US Keyer Tie is currently on, store this and switch it off
                    me1_key2_tie_on = true;
                    ToggleKeyTie2(false);
                }
                else
                {
                    me1_key2_tie_on = false;
                }
            }
            else if (me1_key2_on)
            {
                // US Keyer is off but it used to be on, turn it on again straight away
                // TODO Delay this by 1 second? Or Transition to it?
                me1_key2_on = false;
                me1_key2.SetOnAir(1);

                if (me1_key2_tie_on)
                {
                    // Turn US Keyer Tie on again
                    ToggleKeyTie2(true);
                }
            }
            else if (key2tie == 1)
            {
                // US Keyer Tie is currently on, store this and switch it off
                me1_key2_tie_on = true;
                ToggleKeyTie2(false);
            }
            else if (me1_key2_tie_on)
            {
                // Turn US Keyer Tie on again
                ToggleKeyTie2(true);
            }
            else
            {
                me1_key2_tie_on = false;
            }

            if (key3 == 1)
            {
                // US Keyer is currently on, store this and switch it off
                me1_key3_on = true;
                me1_key3.SetOnAir(0);

                if (key3tie == 1)
                {
                    // US Keyer Tie is currently on, store this and switch it off
                    me1_key3_tie_on = true;
                    ToggleKeyTie3(false);
                }
                else
                {
                    me1_key3_tie_on = false;
                }
            }
            else if (me1_key3_on)
            {
                // US Keyer is off but it used to be on, turn it on again straight away
                // TODO Delay this by 1 second? Or Transition to it?
                me1_key3_on = false;
                me1_key3.SetOnAir(1);

                if (me1_key3_tie_on)
                {
                    // Turn US Keyer Tie on again
                    ToggleKeyTie3(true);
                }
            }
            else if (key3tie == 1)
            {
                // US Keyer Tie is currently on, store this and switch it off
                me1_key3_tie_on = true;
                ToggleKeyTie3(false);
            }
            else if (me1_key3_tie_on)
            {
                // Turn US Keyer Tie on again
                ToggleKeyTie3(true);
            }
            else
            {
                me1_key3_tie_on = false;
            }

            if (key4 == 1)
            {
                // US Keyer is currently on, store this and switch it off
                me1_key4_on = true;
                me1_key4.SetOnAir(0);

                if (key4tie == 1)
                {
                    // US Keyer Tie is currently on, store this and switch it off
                    me1_key4_tie_on = true;
                    ToggleKeyTie4(false);
                }
                else
                {
                    me1_key4_tie_on = false;
                }
            }
            else if (me1_key4_on)
            {
                // US Keyer is off but it used to be on, turn it on again straight away
                // TODO Delay this by 1 second? Or Transition to it?
                me1_key4_on = false;
                me1_key4.SetOnAir(1);

                if (me1_key4_tie_on)
                {
                    // Turn US Keyer Tie on again
                    ToggleKeyTie4(true);
                }
            }
            else if (key4tie == 1)
            {
                // US Keyer Tie is currently on, store this and switch it off
                me1_key4_tie_on = true;
                ToggleKeyTie4(false);
            }
            else if (me1_key4_tie_on)
            {
                // Turn US Keyer Tie on again
                ToggleKeyTie4(true);
            }
            else
            {
                me1_key4_tie_on = false;
            }
        }

        private void ToggleKeyTie1(bool SetActive)
        {
            _BMDSwitcherTransitionSelection transitionselection;
            m_transition.GetNextTransitionSelection(out transitionselection);
            string stringtransitionselection = transitionselection.ToString();


            if (stringtransitionselection == "bmdSwitcherTransitionSelectionBackground")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey1 + 1);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey1")
            {/*
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey1 + 2);
            */
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey2")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey1 + 4);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey3")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey1 + 8);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey4")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey1 + 16);
            }
            else
            {
                int inttransitionselection = Convert.ToInt16(stringtransitionselection);
                if (SetActive == true)
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey1 + inttransitionselection);
                }
                else
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey1 + inttransitionselection - 4);
                }
            }
        }

        private void ToggleKeyTie2(bool SetActive)
        {
            _BMDSwitcherTransitionSelection transitionselection;
            m_transition.GetNextTransitionSelection(out transitionselection);
            string stringtransitionselection = transitionselection.ToString();


            if (stringtransitionselection == "bmdSwitcherTransitionSelectionBackground")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey2 + 1);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey1")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey2 + 2);

            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey2")
            {/*
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey2 + 4);
           */
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey3")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey2 + 8);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey4")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey2 + 16);
            }
            else
            {
                int inttransitionselection = Convert.ToInt16(stringtransitionselection);
                if (SetActive == true)
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey2 + inttransitionselection);
                }
                else
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey2 + inttransitionselection - 8);
                }
            }
        }

        private void ToggleKeyTie3(bool SetActive)
        {
            _BMDSwitcherTransitionSelection transitionselection;
            m_transition.GetNextTransitionSelection(out transitionselection);
            string stringtransitionselection = transitionselection.ToString();


            if (stringtransitionselection == "bmdSwitcherTransitionSelectionBackground")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey3 + 1);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey1")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey3 + 2);

            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey2")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey3 + 4);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey3")
            {/*
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey3 + 8);
            */
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey4")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey3 + 16);
            }
            else
            {
                int inttransitionselection = Convert.ToInt16(stringtransitionselection);
                if (SetActive == true)
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey3 + inttransitionselection);
                }
                else
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey3 + inttransitionselection - 16);
                }
            }
        }

        private void ToggleKeyTie4(bool SetActive)
        {
            _BMDSwitcherTransitionSelection transitionselection;
            m_transition.GetNextTransitionSelection(out transitionselection);
            string stringtransitionselection = transitionselection.ToString();


            if (stringtransitionselection == "bmdSwitcherTransitionSelectionBackground")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey4 + 1);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey1")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey4 + 2);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey2")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey4 + 4);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey3")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey4 + 8);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey4")
            {/*
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey4 + 16);
           */
            }
            else
            {
                int inttransitionselection = Convert.ToInt16(stringtransitionselection);
                if (SetActive == true)
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey4 + inttransitionselection);
                }
                else
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey4 + inttransitionselection - 32);
                }
            }
        }

        private void DetermineUpstreamKeyTies(out int key1tie, out int key2tie, out int key3tie, out int key4tie)
        {
            me1_bkg_tie_on = false;
            key1tie = 0;
            key2tie = 0;
            key3tie = 0;
            key4tie = 0;

            _BMDSwitcherTransitionSelection transitionselection;
            m_transition.GetTransitionSelection(out transitionselection);

            string stringtransitionselection = transitionselection.ToString();
            if (stringtransitionselection == "bmdSwitcherTransitionSelectionBackground" ||
                stringtransitionselection == "bmdSwitcherTransitionSelectionKey1" ||
                stringtransitionselection == "bmdSwitcherTransitionSelectionKey2" ||
                stringtransitionselection == "bmdSwitcherTransitionSelectionKey3" ||
                stringtransitionselection == "bmdSwitcherTransitionSelectionKey4")
            {
                switch (transitionselection)
                {
                    case (_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground):
                        {
                            me1_bkg_tie_on = true;
                            break;
                        }
                    case (_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey1):
                        {
                            key1tie = 1;
                            break;
                        }
                    case (_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey2):
                        {
                            key2tie = 1;
                            break;
                        }
                    case (_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey3):
                        {
                            key3tie = 1;
                            break;
                        }
                    case (_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionKey4):
                        {
                            key4tie = 1;
                            break;
                        }
                }
            }
            else
            {
                int inttransitionselection = Convert.ToInt16(stringtransitionselection);
                switch (inttransitionselection)
                {
                    case (3):
                        {
                            me1_bkg_tie_on = true;
                            key1tie = 1;
                            break;
                        }
                    case (5):
                        {
                            me1_bkg_tie_on = true;
                            key2tie = 1;
                            break;
                        }
                    case (6):
                        {
                            key1tie = 1;
                            key2tie = 1;
                            break;
                        }
                    case (7):
                        {
                            me1_bkg_tie_on = true;
                            key1tie = 1;
                            key2tie = 1;
                            break;
                        }
                    case (9):
                        {
                            me1_bkg_tie_on = true;
                            key3tie = 1;
                            break;
                        }
                    case (10):
                        {
                            key1tie = 1;
                            key3tie = 1;
                            break;
                        }
                    case (11):
                        {
                            me1_bkg_tie_on = true;
                            key1tie = 1;
                            key3tie = 1;
                            break;
                        }
                    case (12):
                        {
                            key2tie = 1;
                            key3tie = 1;
                            break;
                        }
                    case (13):
                        {
                            me1_bkg_tie_on = true;
                            key2tie = 1;
                            key3tie = 1;
                            break;
                        }
                    case (14):
                        {
                            key1tie = 1;
                            key2tie = 1;
                            key3tie = 1;
                            break;
                        }
                    case (15):
                        {
                            me1_bkg_tie_on = true;
                            key1tie = 1;
                            key2tie = 1;
                            key3tie = 1;
                            break;
                        }
                    case (17):
                        {
                            me1_bkg_tie_on = true;
                            key4tie = 1;
                            break;
                        }
                    case (18):
                        {
                            key1tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (19):
                        {
                            me1_bkg_tie_on = true;
                            key1tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (20):
                        {
                            key2tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (21):
                        {
                            me1_bkg_tie_on = true;
                            key2tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (22):
                        {
                            key1tie = 1;
                            key2tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (23):
                        {
                            key1tie = 1;
                            key2tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (24):
                        {
                            key3tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (25):
                        {
                            me1_bkg_tie_on = true;
                            key3tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (26):
                        {
                            key1tie = 1;
                            key3tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (27):
                        {
                            me1_bkg_tie_on = true;
                            key1tie = 1;
                            key3tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (28):
                        {
                            key2tie = 1;
                            key3tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (29):
                        {
                            me1_bkg_tie_on = true;
                            key2tie = 1;
                            key3tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (30):
                        {
                            key1tie = 1;
                            key2tie = 1;
                            key3tie = 1;
                            key4tie = 1;
                            break;
                        }
                    case (31):
                        {
                            me1_bkg_tie_on = true;
                            key1tie = 1;
                            key2tie = 1;
                            key3tie = 1;
                            key4tie = 1;
                            break;
                        }
                }
            }
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
                long inputId;

                inputIterator.Next(out input);
                while (input != null)
                {
                    input.GetInputId(out inputId);
                    if (inputId > 0 && inputId < 9)
                    {
                        // 0 = Black
                        // 9 = Bars
                        GameSourceSelector.Items.Add(inputId);
                    }
                    InputMonitor newInputMonitor = new InputMonitor(input);
                    input.AddCallback(newInputMonitor);
                    newInputMonitor.LongNameChanged += new SwitcherEventHandler(OnInputLongNameChanged);

                    m_inputMonitors.Add(newInputMonitor);

                    inputIterator.Next(out input);
                }
                if (GameSourceSelector.Items.Count > 0)
                {
                    GameSourceSelector.Enabled = true;
                    GameSourceSelector.SelectedIndex = 0;
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

            m_transition = (BMDSwitcherAPI.IBMDSwitcherTransitionParameters)m_mixEffectBlock1;
            m_transition.AddCallback(m_transitionMonitor);

            IBMDSwitcherDownstreamKeyIterator dkeyiterator;
            dkeyiterator = null;
            SwitcherAPIHelper.CreateIterator(m_switcher, out dkeyiterator);
            if (dkeyiterator != null)
            {
                dkeyiterator.Next(out me1_dkey1);
                dkeyiterator.Next(out me1_dkey2);
            }

            me1_dkey1.AddCallback(m_dkeyMonitor);
            me1_dkey2.AddCallback(m_dkeyMonitor);

            
            IBMDSwitcherKeyIterator keyIterator;
            SwitcherAPIHelper.CreateIterator(m_mixEffectBlock1, out keyIterator);

            if (keyIterator != null)
            {
                keyIterator.Next(out me1_key1);
                keyIterator.Next(out me1_key2);
                keyIterator.Next(out me1_key3);
                keyIterator.Next(out me1_key4);
            }

            if (me1_key1 != null) { me1_key1.AddCallback(m_keyMonitor); }
            if (me1_key2 != null) { me1_key2.AddCallback(m_keyMonitor); }
            if (me1_key3 != null) { me1_key3.AddCallback(m_keyMonitor); }
            if (me1_key4 != null) { me1_key4.AddCallback(m_keyMonitor); }
            

            UpdateItems();
        }

        private void SwitcherDisconnected()
        {
            ATEMConnectButton.Enabled = true;
            GameInputId = new long();
            GameSourceSelector.Items.Clear();
            GameSourceSelector.Enabled = false;

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

            if (me1_dkey1 != null)
            {
                // Remove callback
                me1_dkey1.RemoveCallback(m_dkeyMonitor);

                // Release reference
                me1_dkey1 = null;
            }

            if (me1_dkey2 != null)
            {
                // Remove callback
                me1_dkey2.RemoveCallback(m_dkeyMonitor);

                // Release reference
                me1_dkey2 = null;
            }

            
            if (me1_key1 != null)
            {
                // Remove callback
                me1_key1.RemoveCallback(m_keyMonitor);

                // Release reference
                me1_key1 = null;
            }
            if (me1_key2 != null)
            {
                // Remove callback
                me1_key2.RemoveCallback(m_keyMonitor);

                // Release reference
                me1_key2 = null;
            }
            if (me1_key3 != null)
            {
                // Remove callback
                me1_key3.RemoveCallback(m_keyMonitor);

                // Release reference
                me1_key3 = null;
            }
            if (me1_key4 != null)
            {
                // Remove callback
                me1_key4.RemoveCallback(m_keyMonitor);

                // Release reference
                me1_key4 = null;
            }

            if (m_switcher != null)
            {
                // Remove callback:
                m_switcher.RemoveCallback(m_switcherMonitor);

                // release reference:
                m_switcher = null;
            }

            ProgLabel.Text = "Not Connected";
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

                inputIterator.Next(out input);
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

        private void button1_Click(object sender, EventArgs e)
        {
            ToggleOverlays();
        }
    }
}
