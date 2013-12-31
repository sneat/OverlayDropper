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
using System.Threading;

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
        private _BMDSwitcherTransitionStyle existing_style;
        private Boolean ResetTies = false;
        private Boolean ShouldATEMBeConnected = false;

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

            #if DEBUG
            TriggerTestButton.Visible = true;
            #endif

            ATEMIpAddressTextBox.Text = Properties.Settings.Default.IPAddress.ToString();
            m_KeyboardHookManager = new KeyboardHookListener(new GlobalHooker());
            m_KeyboardHookManager.Enabled = true;
            m_KeyboardHookManager.KeyDown += HookManager_KeyDown;
            m_KeyboardHookManager.KeyUp += HookManager_KeyUp;

            m_switcherMonitor = new SwitcherMonitor();
            // note: this invoke pattern ensures our callback is called in the main thread. We are making double
            // use of lambda expressions here to achieve this.
            // Essentially, the events will arrive at the callback class (implemented by our monitor classes)
            // on a separate thread. We must marshell these to the main thread, and we're doing this by calling
            // invoke on the Windows Forms object. The lambda expression is just a simplification.
            m_switcherMonitor.SwitcherDisconnected += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => SwitcherDisconnected())));

            m_mixEffectBlockMonitor = new MixEffectBlockMonitor();
            m_mixEffectBlockMonitor.ProgramInputChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => UpdateItems())));
            m_mixEffectBlockMonitor.InTransitionChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => OnInTransitionChanged())));

            m_keyMonitor = new KeyMonitor();
            m_dkeyMonitor = new DownStreamKeyMonitor();
            m_dkeyMonitor.IsAutoTransitioningChanged += new SwitcherEventHandler((s, a) => this.Invoke((Action)(() => DKeyerIsAutoTransitioningChanged())));
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
        private void ATEMIpAddressTextBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IPAddress = ATEMIpAddressTextBox.Text;
            Properties.Settings.Default.Save();
        }

        private void GameSourceSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameInputId = Convert.ToInt64(GameSourceSelector.SelectedItem);
            Properties.Settings.Default.GameSource = Convert.ToInt32(GameInputId);
            Properties.Settings.Default.Save();
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

            if (e.KeyCode.ToString() == "O" && IsCtrlDown && IsAltDown && IsShiftDown)
            {
                checkBoxEnabled.Checked = !checkBoxEnabled.Checked;
            }
            else if (checkBoxEnabled.Checked && SelectedGame == "League of Legends" && (e.KeyCode.ToString() == "A" || e.KeyCode.ToString() == "O" || e.KeyCode.ToString() == "H") && GetActiveWindowTitle() == "League of Legends (TM) Client")
            {
                ToggleLeagueOfLegendsOverlays();
            }
            else if (checkBoxEnabled.Checked && SelectedGame == "Starcraft 2" && IsCtrlDown && e.KeyCode.ToString() == "W" && GetActiveWindowTitle() == "StarCraft II")
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

        /// <summary>
        /// We ToggleOverlays by checking to see whether any Upstream keyers are/were On Air,
        /// if they were then we Tie them, disable Background Tie, enable any Downstream keyer
        /// Ties and trigger an AutoTransition.
        /// If there aren't/weren't any Upstream keyers On Air, we trigger AutoTransition
        /// for any Downstream keyers.
        /// Finally we ensure that the correct items are Tied at the end.
        /// </summary>
        private void ToggleOverlays()
        {
            if (m_mixEffectBlock1 == null || GameInputId == 0 || ProgramInputId != GameInputId)
            {
                return;
            }
            int dkey1 = 0;
            int dkey2 = 0;
            int dkey1tie = 0;
            int dkey2tie = 0;

            me1_dkey1.GetOnAir(out dkey1);
            me1_dkey2.GetOnAir(out dkey2);
            me1_dkey1.GetTie(out dkey1tie);
            me1_dkey2.GetTie(out dkey2tie);

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

            // Perform Toggle via main Transition if an Upstream Keyer is On Air,
            // or we used to have an Upstream Keyer On Air and we don't currently have a Downstream Keyer On Air
            if (key1 == 1 || key2 == 1 || key3 == 1 || key4 == 1 || ((me1_key1_on || me1_key2_on || me1_key3_on || me1_key4_on) && dkey1 == 0 && dkey2 == 0))
            {
                if (key1 == 1 || key2 == 1 || key3 == 1 || key4 == 1)
                {
                    // We are turning overlays off

                    // Store our existing Tie values
                    me1_dkey1_tie_on = (dkey1tie == 1);
                    me1_dkey2_tie_on = (dkey2tie == 1);
                    me1_key1_tie_on = (key1tie == 1);
                    me1_key2_tie_on = (key2tie == 1);
                    me1_key3_tie_on = (key3tie == 1);
                    me1_key4_tie_on = (key4tie == 1);

                    // Do the transition
                    if (key1 == 1)
                    {
                        me1_key1_on = true;
                        if (key1tie == 0)
                        {
                            ToggleKeyTie1(true);
                        }
                    }
                    else
                    {
                        if (key1tie == 1)
                        {
                            ToggleKeyTie1(false);
                        }
                    }
                    if (key2 == 1)
                    {
                        me1_key2_on = true;
                        if (key2tie == 0)
                        {
                            ToggleKeyTie2(true);
                        }
                    }
                    else
                    {
                        if (key2tie == 1)
                        {
                            ToggleKeyTie2(false);
                        }
                    }
                    if (key3 == 1)
                    {
                        me1_key3_on = true;
                        if (key3tie == 0)
                        {
                            ToggleKeyTie3(true);
                        }
                    }
                    else
                    {
                        if (key3tie == 1)
                        {
                            ToggleKeyTie3(false);
                        }
                    }
                    if (key4 == 1)
                    {
                        me1_key4_on = true;
                        if (key4tie == 0)
                        {
                            ToggleKeyTie4(true);
                        }
                    }
                    else
                    {
                        if (key4tie == 1)
                        {
                            ToggleKeyTie4(false);
                        }
                    }
                    if (dkey1 == 1)
                    {
                        me1_dkey1_on = true;
                        if (dkey1tie == 0)
                        {
                            me1_dkey1.SetTie(1);
                        }
                    }
                    else
                    {
                        me1_dkey1_on = false;
                        if (dkey1tie == 1)
                        {
                            me1_dkey1.SetTie(0);
                        }
                    }
                    if (dkey2 == 1)
                    {
                        me1_dkey2_on = true;
                        if (dkey2tie == 0)
                        {
                            me1_dkey2.SetTie(1);
                        }
                    }
                    else
                    {
                        me1_dkey2_on = false;
                        if (dkey2tie == 1)
                        {
                            me1_dkey2.SetTie(0);
                        }
                    }
                    if (me1_bkg_tie_on)
                    {
                        System.Threading.Thread.Sleep(50); // Give enough time to make sure that the Next Transitions are set
                        ToggleBkgdKeyTie(false);
                    }
                    // Make sure that we aren't set to transition the background
                    _BMDSwitcherTransitionSelection transitionselection;
                    m_transition.GetNextTransitionSelection(out transitionselection);
                    string stringtransitionselection = transitionselection.ToString();
                    if (stringtransitionselection != "bmdSwitcherTransitionSelectionBackground")
                    {
                        m_transition.GetNextTransitionStyle(out existing_style);
                        m_transition.SetNextTransitionStyle(_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleMix);
                        m_mixEffectBlock1.PerformAutoTransition();
                        ResetTies = false;
                    }
                }
                else
                {
                    // We are turning overlays on
                    if (me1_key1_on)
                    {
                        me1_key1_on = false;
                        if (key1tie == 0)
                        {
                            ToggleKeyTie1(true);
                        }
                    }
                    if (me1_key2_on)
                    {
                        me1_key2_on = false;
                        if (key2tie == 0)
                        {
                            ToggleKeyTie2(true);
                        }
                    }
                    if (me1_key3_on)
                    {
                        me1_key3_on = false;
                        if (key3tie == 0)
                        {
                            ToggleKeyTie3(true);
                        }
                    }
                    if (me1_key4_on)
                    {
                        me1_key4_on = false;
                        if (key4tie == 0)
                        {
                            ToggleKeyTie4(true);
                        }
                    }
                    if (me1_dkey1_on)
                    {
                        me1_dkey1_on = false;
                        if (dkey1tie == 0)
                        {
                            me1_dkey1.SetTie(1);
                        }
                    }
                    if (me1_dkey2_on)
                    {
                        me1_dkey2_on = false;
                        if (dkey2tie == 0)
                        {
                            me1_dkey2.SetTie(1);
                        }
                    }
                    if (me1_bkg_tie_on)
                    {
                        System.Threading.Thread.Sleep(50); // Give enough time to make sure that the Next Transitions are set
                        ToggleBkgdKeyTie(false); 
                    }
                    // Make sure that we aren't set to transition the background
                    _BMDSwitcherTransitionSelection transitionselection;
                    m_transition.GetNextTransitionSelection(out transitionselection);
                    string stringtransitionselection = transitionselection.ToString();
                    if (stringtransitionselection != "bmdSwitcherTransitionSelectionBackground")
                    {
                        m_transition.GetNextTransitionStyle(out existing_style);
                        m_transition.SetNextTransitionStyle(_BMDSwitcherTransitionStyle.bmdSwitcherTransitionStyleMix);
                        m_mixEffectBlock1.PerformAutoTransition();
                        ResetTies = true;
                    }
                }
            }
            else
            {
                // Perform Toggle via individual Downstream keyers
                if (dkey1 == 1 || dkey2 == 1)
                {
                    // We are turning overlays off

                    // Store our existing Tie values
                    me1_dkey1_tie_on = (dkey1tie == 1);
                    me1_dkey2_tie_on = (dkey2tie == 1);
                    me1_key1_tie_on = (key1tie == 1);
                    me1_key2_tie_on = (key2tie == 1);
                    me1_key3_tie_on = (key3tie == 1);
                    me1_key4_tie_on = (key4tie == 1);

                    // Reset Upstream Keyer status
                    me1_key1_on = me1_key2_on = me1_key3_on = me1_key4_on = false;

                    if (dkey1 == 1)
                    {
                        // Downstream Keyer is currently on, store this and switch it off
                        me1_dkey1_on = true;
                        me1_dkey1.PerformAutoTransition();
                    }
                    else
                    {
                        me1_dkey1_on = false;
                    }
                    if (dkey2 == 1)
                    {
                        // Downstream Keyer is currently on, store this and switch it off
                        me1_dkey2_on = true;
                        me1_dkey2.PerformAutoTransition();
                    }
                    else
                    {
                        me1_dkey2_on = false;
                    }
                    if (dkey1tie == 1)
                    {
                        // Downstream Keyer Tie is currently on, switch it off
                        me1_dkey1.SetTie(0);
                    }
                    if (dkey2tie == 1)
                    {
                        // Downstream Keyer Tie is currently on, switch it off
                        me1_dkey2.SetTie(0);
                    }
                }
                else
                {
                    // We are turning overlays on

                    if (me1_dkey1_on)
                    {
                        // Downstream Keyer is off but it used to be on, turn it on again by auto transition
                        me1_dkey1_on = false;
                        me1_dkey1.PerformAutoTransition();
                    }
                    if (me1_dkey2_on)
                    {
                        // Downstream Keyer is off but it used to be on, turn it on again by auto transition
                        me1_dkey2_on = false;
                        me1_dkey2.PerformAutoTransition();
                    }
                }
                ResetTies = true;
            }
        }

        private void ToggleBkgdKeyTie(bool SetActive)
        {
            _BMDSwitcherTransitionSelection transitionselection;
            m_transition.GetNextTransitionSelection(out transitionselection);
            string stringtransitionselection = transitionselection.ToString();

            if (stringtransitionselection == "bmdSwitcherTransitionSelectionBackground")
            {
                /*if (m_bkgdlayer)
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground + inttransitionselection);
                }
                else
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground + inttransitionselection - 2);
                }*/
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey1")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground + 2);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey2")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground + 4);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey3")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground + 8);
            }
            else if (stringtransitionselection == "bmdSwitcherTransitionSelectionKey4")
            {
                m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground + 16);
            }
            else
            {
                int inttransitionselection = Convert.ToInt16(stringtransitionselection);
                if (SetActive == true)
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground + inttransitionselection);
                }
                else
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground + inttransitionselection - 2);
                }
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
            {
                if (!SetActive)
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground);
                }
            
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
            {
                if (!SetActive)
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground);
                }
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
            {
                if (!SetActive)
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground);
                }
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
            {
                if (!SetActive)
                {
                    m_transition.SetNextTransitionSelection(_BMDSwitcherTransitionSelection.bmdSwitcherTransitionSelectionBackground);
                }
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

        /// <summary>
        /// Main Transition
        /// </summary>
        private void OnInTransitionChanged()
        {
            int inTransition;

            m_mixEffectBlock1.GetFlag(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdInTransition, out inTransition);

            // When inTransition == 0 then the transition has finished, trigger our other events
            if (inTransition == 0)
            {
                int dkey1tie = 0;
                int dkey2tie = 0;
                me1_dkey1.GetTie(out dkey1tie);
                me1_dkey2.GetTie(out dkey2tie);

                int key1tie = 0;
                int key2tie = 0;
                int key3tie = 0;
                int key4tie = 0;
                DetermineUpstreamKeyTies(out key1tie, out key2tie, out key3tie, out key4tie);
                if (ResetTies)
                {
                    // We have brought the overlays back, reset the Ties
                    if (!me1_bkg_tie_on) { ToggleBkgdKeyTie(true); }

                    if (me1_dkey1_tie_on && dkey1tie == 0)
                    {
                        // Tie is off, should be on
                        me1_dkey1.SetTie(1);
                    }
                    else if (!me1_dkey1_tie_on && dkey1tie == 1)
                    {
                        // Tie is on, should be off
                        me1_dkey1.SetTie(0);
                    }

                    if (me1_dkey2_tie_on && dkey2tie == 0)
                    {
                        // Tie is off, should be on
                        me1_dkey2.SetTie(1);
                    }
                    else if (!me1_dkey2_tie_on && dkey2tie == 1)
                    {
                        // Tie is on, should be off
                        me1_dkey2.SetTie(0);
                    }

                    if (me1_key1_tie_on && key1tie == 0)
                    {
                        // Tie is off, should be on
                        ToggleKeyTie1(true);
                    }
                    else if (!me1_key1_tie_on && key1tie == 1)
                    {
                        // Tie is on, should be off
                        ToggleKeyTie1(false);
                    }

                    if (me1_key2_tie_on && key2tie == 0)
                    {
                        // Tie is off, should be on
                        ToggleKeyTie2(true);
                    }
                    else if (!me1_key2_tie_on && key2tie == 1)
                    {
                        // Tie is on, should be off
                        ToggleKeyTie2(false);
                    }

                    if (me1_key3_tie_on && key3tie == 0)
                    {
                        // Tie is off, should be on
                        ToggleKeyTie3(true);
                    }
                    else if (!me1_key3_tie_on && key3tie == 1)
                    {
                        // Tie is on, should be off
                        ToggleKeyTie3(false);
                    }

                    if (me1_key4_tie_on && key4tie == 0)
                    {
                        // Tie is off, should be on
                        ToggleKeyTie4(true);
                    }
                    else if (!me1_key4_tie_on && key4tie == 1)
                    {
                        // Tie is on, should be off
                        ToggleKeyTie4(false);
                    }
                }
                else
                {
                    // We have just dropped the overlays, make sure just the Background is Tied
                    if (!me1_bkg_tie_on) { ToggleBkgdKeyTie(true); }
                    if (dkey1tie == 1) { me1_dkey1.SetTie(0); }
                    if (dkey2tie == 1) { me1_dkey2.SetTie(0); }
                    if (key1tie == 1) { ToggleKeyTie1(false); }
                    if (key2tie == 1) { ToggleKeyTie2(false); }
                    if (key3tie == 1) { ToggleKeyTie3(false); }
                    if (key4tie == 1) { ToggleKeyTie4(false); }
                }
                m_transition.SetNextTransitionStyle(existing_style);
            }
        }

        /// <summary>
        /// Downstream Keyer Auto Transition
        /// </summary>
        private void DKeyerIsAutoTransitioningChanged()
        {
            int dkey1;
            int dkey2;

            me1_dkey1.IsAutoTransitioning(out dkey1);
            me1_dkey2.IsAutoTransitioning(out dkey2);
            // When dkey1 == 0  and dkey2 == 0 then the transition has finished, trigger our other events
            if (ResetTies && dkey1 == 0 && dkey2 == 0)
            {
                if (!me1_bkg_tie_on) { ToggleBkgdKeyTie(true); }

                int dkey1tie = 0;
                int dkey2tie = 0;
                me1_dkey1.GetTie(out dkey1tie);
                me1_dkey2.GetTie(out dkey2tie);

                int key1tie = 0;
                int key2tie = 0;
                int key3tie = 0;
                int key4tie = 0;
                DetermineUpstreamKeyTies(out key1tie, out key2tie, out key3tie, out key4tie);

                if (me1_dkey1_tie_on && dkey1tie == 0)
                {
                    // Tie is off, should be on
                    me1_dkey1.SetTie(1);
                }
                else if (!me1_dkey1_tie_on && dkey1tie == 1)
                {
                    // Tie is on, should be off
                    me1_dkey1.SetTie(0);
                }

                if (me1_dkey2_tie_on && dkey2tie == 0)
                {
                    // Tie is off, should be on
                    me1_dkey2.SetTie(1);
                }
                else if (!me1_dkey2_tie_on && dkey2tie == 1)
                {
                    // Tie is on, should be off
                    me1_dkey2.SetTie(0);
                }

                if (me1_key1_tie_on && key1tie == 0)
                {
                    // Tie is off, should be on
                    ToggleKeyTie1(true);
                }
                else if (!me1_key1_tie_on && key1tie == 1)
                {
                    // Tie is on, should be off
                    ToggleKeyTie1(false);
                }

                if (me1_key2_tie_on && key2tie == 0)
                {
                    // Tie is off, should be on
                    ToggleKeyTie2(true);
                }
                else if (!me1_key2_tie_on && key2tie == 1)
                {
                    // Tie is on, should be off
                    ToggleKeyTie2(false);
                }

                if (me1_key3_tie_on && key3tie == 0)
                {
                    // Tie is off, should be on
                    ToggleKeyTie3(true);
                }
                else if (!me1_key3_tie_on && key3tie == 1)
                {
                    // Tie is on, should be off
                    ToggleKeyTie3(false);
                }

                if (me1_key4_tie_on && key4tie == 0)
                {
                    // Tie is off, should be on
                    ToggleKeyTie4(true);
                }
                else if (!me1_key4_tie_on && key4tie == 1)
                {
                    // Tie is on, should be off
                    ToggleKeyTie4(false);
                }
            }
            Log(String.Format("dkey1 auto: {0}", dkey1 == 1 ? "true" : "false"));
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
            if (ATEMConnectButton.Text == "Connect")
            {
                ConnectToATEM();
            }
            else
            {
                ShouldATEMBeConnected = false;
                SwitcherDisconnected();
            }
        }

        private Boolean ConnectToATEM(Boolean Retrying = false)
        {
            ATEMConnectButton.Enabled = false;
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
                if (!Retrying)
                {
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
                    ATEMConnectButton.Enabled = true;
                }
                return false;
            }

            SwitcherConnected();
            return true;
        }

        private void SwitcherConnected()
        {
            ShouldATEMBeConnected = true;
            TriggerTestButton.Enabled = true;

            // Install SwitcherMonitor callbacks:
            m_switcher.AddCallback(m_switcherMonitor);

            // We create input monitors for each input. To do this we iterator over all inputs:
            // This will allow us to update the combo boxes when input names change:
            IBMDSwitcherInputIterator inputIterator;
            if (SwitcherAPIHelper.CreateIterator(m_switcher, out inputIterator))
            {
                IBMDSwitcherInput input;
                long inputId;

                GameSourceSelector.Items.Clear();
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
                    if (GameSourceSelector.Items.Count >= Properties.Settings.Default.GameSource - 1)
                    {
                        GameSourceSelector.SelectedIndex = Properties.Settings.Default.GameSource - 1;
                    }
                    else
                    {
                        GameSourceSelector.SelectedIndex = 0;
                    }
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

            ATEMConnectButton.Text = "Disconnect";
            ATEMConnectButton.Enabled = true;
            UpdateItems();
        }

        private void SwitcherDisconnected()
        {
            ATEMConnectButton.Text = "Connect";
            ATEMConnectButton.Enabled = true;
            TriggerTestButton.Enabled = false;
            GameInputId = new long();
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

            if (m_transition != null)
            {
                // Remove callback
                m_transition.RemoveCallback(m_transitionMonitor);

                // Release reference
                m_transition = null;
            }

            if (m_switcher != null)
            {
                // Remove callback:
                m_switcher.RemoveCallback(m_switcherMonitor);

                // release reference:
                m_switcher = null;
            }

            if (ShouldATEMBeConnected)
            {
                ProgLabel.Text = "Retrying...";
                // Retry connection
                Retry.RetryMethod<Boolean>(() =>
                {
                    return ConnectToATEM(true);
                }, 240, 500); // Retry for the next 2 minutes; waiting 0.5 seconds before the next try
            }
            else
            {
                ProgLabel.Text = "Not Connected";
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

        private void TriggerTestButton_Click(object sender, EventArgs e)
        {
            ToggleOverlays();
        }
    }

    public class Retry
    {
        /// <summary>
        /// Retry calling of a method if it fails
        /// </summary>
        /// <typeparam name="T">Return data type</typeparam>
        /// <param name="method">Method</param>
        /// <param name="numRetries">Number of Retries</param>
        /// <param name="millisecondsToWaitBeforeRetry"></param>
        /// <returns>T</returns>
        public static T RetryMethod<T>(Func<T> method, int numRetries, int millisecondsToWaitBeforeRetry)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            T retval = default(T);
            do
            {
                try
                {
                    retval = method();
                    System.Console.WriteLine(string.Format("Retrying response: {0}", retval.ToString()));
                    if (Convert.ToBoolean(retval)) { return retval; }
                }
                catch (Exception)
                {
                    if (numRetries <= 0) throw;
                    Thread.Sleep(millisecondsToWaitBeforeRetry);
                }
            } while (numRetries-- > 0);
            return retval;
        }

        /// <summary>
        /// Retry calling of an Action if it fails
        /// </summary>
        /// <typeparam name="T">Return data type</typeparam>
        /// <param name="method">Method</param>
        /// <param name="numRetries">Number of Retries</param>
        /// <param name="millisecondsToWaitBeforeRetry"></param>
        public static void RetryAction(Action action, int numRetries, int millisecondsToWaitBeforeRetry)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            do
            {
                try { action(); return; }
                catch (Exception)
                {
                    if (numRetries <= 0) throw;
                    else Thread.Sleep(millisecondsToWaitBeforeRetry);
                }
            } while (numRetries-- > 0);
        }
    }
}
