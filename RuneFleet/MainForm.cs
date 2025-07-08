using RuneFleet.Interop;
using RuneFleet.Services;
using System.Windows.Forms;

namespace RuneFleet
{
    public partial class MainForm : Form
    {
        private readonly AccountManager accountManager = new();
        // Service handling client processes and thumbnails
        private ClientProcessService clientService;
        // Keybinds
        private const int WM_HOTKEY = 0x0312;
        // Unique IDs for each keybind
        private const int HOTKEY_ID_PGDN = 1;
        private const int HOTKEY_ID_PGUP = 2;
        private const int HOTKEY_ID_DEL = 3;
        // Character accounts.csv watch/generation
        private CancellationTokenSource watchTokenSource;
        private Task watchTask;


        public MainForm()
        {
            InitializeComponent();
            // Load and show information from CSV file
            accountManager.Load("accounts.csv");
            UpdateGroupView();
            UpdateListView("All");

            // TODO clean up addition of the numeric client scale control
            // Scale button
            ToolStripControlHost hostScale = new ToolStripControlHost(numericClientScale);
            int scaleButtonIndex = toolStrip1.Items.IndexOf(toolStripButtonScale);
            toolStrip1.Items.Insert(scaleButtonIndex + 1, hostScale);
            // Group selection dropdown
            ToolStripControlHost hostGroup = new ToolStripControlHost(groupSelection);
            int groupButtonIndex = toolStrip1.Items.IndexOf(toolStripButtonLaunch);
            toolStrip1.Items.Insert(groupButtonIndex, hostGroup);

            clientService = new ClientProcessService(
                this,
                flowPanelProcesses,
                accountManager.Accounts,
                () => UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All"));

            flowPanelProcesses.Scroll += flowPanelProcesses_Scroll;
            flowPanelProcesses.Resize += flowPanelProcesses_Resize;

            // Handling the keybinds
            NativeMethods.RegisterHotKey(this.Handle, HOTKEY_ID_PGDN, 0, (uint)Keys.PageDown);
            NativeMethods.RegisterHotKey(this.Handle, HOTKEY_ID_PGUP, 0, (uint)Keys.PageUp);
            NativeMethods.RegisterHotKey(this.Handle, HOTKEY_ID_DEL, 0, (uint)Keys.Delete);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // removing the keybinds
            NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_ID_PGDN);
            NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_ID_PGUP);
            NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_ID_DEL);

            clientService.Dispose();
        }



        // Modifies visible account list based on group
        private void UpdateListView(string group)
        {
            //
            listViewAccounts.Items.Clear();
            foreach (var acc in accountManager.Accounts)
            {
                var item = new ListViewItem(acc.DisplayName);
                item.SubItems.Add(acc.Pid?.ToString() ?? "");
                item.SubItems.Add(acc.CharacterId?.ToString() ?? "");
                if (group == "All" || (acc.Group != null && acc.Group.Contains(group)))
                {
                    listViewAccounts.Items.Add(item);
                }
            }
        }

        // Sets values of group drop down based on accounts loaded
        private void UpdateGroupView()
        {
            groupSelection.Items.Clear();
            var groups = accountManager.GetGroups();

            groupSelection.Items.Add("All");
            foreach (var group in groups)
            {
                groupSelection.Items.Add(group);
            }
            groupSelection.SelectedIndex = 0;

        }

        // Refreshes the process display and updates the list view with the selected group.
        private void buttonLoadPreviews_Click(object sender, EventArgs e)
        {
            clientService.RefreshProcessDisplay();
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }

        // Launches the selected account in the list view.
        private void buttonLaunchSelected_Click(object sender, EventArgs e)
        {
            if (listViewAccounts.SelectedIndices.Count == 0) return;
            var selectedAccount = listViewAccounts.SelectedItems[0].SubItems[2].Text.ToString();
            // TODO fix BUG: If the id of the character is not unique, this will not work correctly.
            // This is a known issue with the current implementation.
            // This is complex and ugly, but it works for now.
            var acc = accountManager.GetAccountByCharacterId(selectedAccount);
            if (acc != null)
            {

                if (toolStripButtonScale.Checked)
                {
                    clientService.LaunchClient(acc, numericClientScale.Value);
                }
                else
                {
                    clientService.LaunchClient(acc, 0);
                }
            }
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }


        // Handles the item activation in the list view, which is triggered when an item is double-clicked or activated.
        private void listViewAccounts_ItemActivate(object sender, EventArgs e)
        {
            var selectedAccount = listViewAccounts.SelectedItems[0].SubItems[2].Text.ToString();
            // TODO fix BUG: If the id of the character is not unique, this will not work correctly.
            // This is a known issue with the current implementation.
            // This is complex and ugly, but it works for now.
            var pid = int.Parse(accountManager.GetAccountByCharacterId(selectedAccount)?.Pid?.ToString() ?? "0");
            clientService.FocusClient(pid);
        }

        /*
         * Was intended to set the world IDs in the preferences file.
        private void setWorlds_CheckedChanged(object sender, EventArgs e)
        {
            if (setWorlds.Checked)
            {
                ClientProcessService.ReplaceWorldIds("418", "418");
            }
            else
            {
                //preferences file handles the config differently
                ClientProcessService.ReplaceWorldIds("-1", "0");
            }
        }
        */

        // Handles the selection change in the group selection dropdown.
        private void groupSelection_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }

        // Handles the window messages to process hotkeys for navigation.
        protected override void WndProc(ref Message m)
        {
            HotkeyNavigator.Handle(ref m, WM_HOTKEY, listViewAccounts,
                HOTKEY_ID_PGDN, HOTKEY_ID_PGUP, HOTKEY_ID_DEL,
                () => listViewAccounts_ItemActivate(this, EventArgs.Empty));

            base.WndProc(ref m);
        }



        private void flowPanelProcesses_Scroll(object sender, ScrollEventArgs e)
        {
            clientService.UpdateThumbnailPositions();
        }

        private void flowPanelProcesses_Resize(object sender, EventArgs e)
        {
            clientService.UpdateThumbnailPositions();
        }

        // Toggles the topmost state of the main form based on the button state.
        private void toolStripButtonTop_Click(object sender, EventArgs e)
        {
            if (toolStripButtonTop.Checked)
            {
                MainForm.ActiveForm.TopMost = true;
            }
            else
            {
                MainForm.ActiveForm.TopMost = false;
            }
        }

        // Starts or stops watching for new character processes based on the button state.
        private void toolStripButtonImport_Click(object sender, EventArgs e)
        {
            if (watchTokenSource == null)
            {
                // Disable other controls
                groupSelection.Enabled = false;
                toolStripButtonLaunch.Enabled = false;
                toolStripButtonRefresh.Enabled = false;
                listViewAccounts.Enabled = false;
                toolStripButtonScale.Enabled = false;
                numericClientScale.Enabled = false;

                // Start watching
                //toolStripProgressBar1.Visible = true;
                toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                watchTokenSource = new CancellationTokenSource();
                labelLoading.Visible = true;
                //buttonWatchCharacters.Text = "Stop Import Helper";
                // Watching for new client processes
                watchTask = Task.Run(() => clientService.WatchForClientsAsync(watchTokenSource.Token));
            }
            else
            {
                // Enable other controls
                groupSelection.Enabled = true;
                toolStripButtonLaunch.Enabled = true;
                toolStripButtonRefresh.Enabled = true;
                listViewAccounts.Enabled = true;
                toolStripButtonScale.Enabled = true;
                numericClientScale.Enabled = true;
                // Stop watching
                //toolStripProgressBar1.Visible = false;
                toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
                toolStripProgressBar1.Value = 0;
                watchTokenSource.Cancel();
                watchTokenSource = null;
                labelLoading.Visible = false;
                //buttonWatchCharacters.Text = "Start Import Helper";
                UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
            }
        }

        // Launches all accounts in the selected group.
        private async void toolStripButtonLaunch_Click(object sender, EventArgs e)
        {
            listViewAccounts.Enabled = false;
            groupSelection.Enabled = false;
            toolStripButtonLaunch.Enabled = false;
            toolStripButtonScale.Enabled = false;
            numericClientScale.Enabled = false;
            var rand = new Random();
            foreach (var acc in accountManager.Accounts)
            {
                if (acc.Group != null && acc.Group.Contains(groupSelection.SelectedItem?.ToString()))
                {
                    if (toolStripButtonScale.Checked)
                    {
                        clientService.LaunchClient(acc, numericClientScale.Value);
                    }
                    else
                    {
                        clientService.LaunchClient(acc, 0);
                    }

                    // otherwise it causes some clients to fail launch
                    await Task.Delay(2000 + rand.Next(1000));
                }
                else
                {
                    Console.WriteLine(acc.Group?.ToString() + "; " + groupSelection.SelectedValue?.ToString());
                }
            }
            listViewAccounts.Enabled = true;
            groupSelection.Enabled = true;
            toolStripButtonLaunch.Enabled = true;
            toolStripButtonScale.Enabled = true;
            numericClientScale.Enabled = true;
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }

        // Refreshes the process display and updates the list view with the selected group.
        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            clientService.RefreshProcessDisplay();
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }

        private void toolStripButtonHelp_Click(object sender, EventArgs e)
        {
            //TODO open info window for credit
            MessageBox.Show(".steakboy on Discord\r\n" +
                            "Icons are from Those Icons, Chanut and Freepik at FlatIcons\r\n" +
                            "Features from chronic0590",
                            "Credits", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripButtonHot_Click(object sender, EventArgs e)
        {
            if (toolStripButtonHot.Checked)
            {
                NativeMethods.RegisterHotKey(this.Handle, HOTKEY_ID_PGDN, 0, (uint)Keys.PageDown);
                NativeMethods.RegisterHotKey(this.Handle, HOTKEY_ID_PGUP, 0, (uint)Keys.PageUp);
                NativeMethods.RegisterHotKey(this.Handle, HOTKEY_ID_DEL, 0, (uint)Keys.Delete);
            }
            else
            {
                NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_ID_PGDN);
                NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_ID_PGUP);
                NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_ID_DEL);
            }
        }
    }

}
