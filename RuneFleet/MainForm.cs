using RuneFleet.Interop;
using RuneFleet.Models;
using RuneFleet.Services;
using System.Diagnostics;

namespace RuneFleet
{
    public partial class MainForm : Form
    {
        private List<Account> accounts = new();
        // Thumbnails
        private Dictionary<IntPtr, IntPtr> thumbnailMap = new();
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
            accounts = AccountLoader.LoadFromCsv("accounts.csv");
            UpdateGroupView();
            UpdateListView("All");

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
        }

        // Modifies visible account list based on group
        private void UpdateListView(string group)
        {
            //
            listViewAccounts.Items.Clear();
            foreach (var acc in accounts)
            {
                var item = new ListViewItem(acc.DisplayName);
                item.SubItems.Add(acc.Pid?.ToString() ?? "");
                if (group == "All" || acc.Group.Contains(group))
                {
                    listViewAccounts.Items.Add(item);
                }
            }
        }

        // Sets values of group drop down based on accounts loaded
        private void UpdateGroupView()
        {
            groupSelection.Items.Clear();
            var groups = accounts
                .Where(p => p.Group != null)
                .SelectMany(p => p.Group)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct();

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
            RefreshProcessDisplay();
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }

        // Launches the selected account in the list view.
        private void buttonLaunchSelected_Click(object sender, EventArgs e)
        {
            if (listViewAccounts.SelectedIndices.Count == 0) return;
            //var acc = accounts[listViewAccounts.SelectedIndices[0]];
            var selectedAccount = listViewAccounts.SelectedItems[0].Text.ToString();
            // TODO fix BUG: If the name of the character is not unique, this will not work correctly.
            // This is a known issue with the current implementation.
            // This is complex and ugly, but it works for now.
            var acc = accounts.FirstOrDefault(a => a.DisplayName == selectedAccount);
            ClientHelper.LaunchClient(acc);
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }

        // Launches all accounts in the selected group.
        private async void buttonLaunchAll_Click(object sender, EventArgs e)
        {
            listViewAccounts.Enabled = false;
            groupSelection.Enabled = false;
            var rand = new Random();
            foreach (var acc in accounts)
            {
                if (acc.Group.Contains(groupSelection.SelectedItem?.ToString()))
                {
                    ClientHelper.LaunchClient(acc);
                    // otherwise it causes some clients to fail launch
                    await Task.Delay(1000 + rand.Next(1000));
                }
                else
                {
                    Console.WriteLine(acc.Group?.ToString() + "; " + groupSelection.SelectedValue?.ToString());
                }
            }
            listViewAccounts.Enabled = true;
            groupSelection.Enabled = true;
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }

        // Handles the item activation in the list view, which is triggered when an item is double-clicked or activated.
        private void listViewAccounts_ItemActivate(object sender, EventArgs e)
        {
            var selectedAccount = listViewAccounts.SelectedItems[0].Text.ToString();
            // TODO fix BUG: If the name of the character is not unique, this will not work correctly.
            // This is a known issue with the current implementation.
            // This is complex and ugly, but it works for now.
            var pid = int.Parse(accounts.FirstOrDefault(acc => acc.DisplayName == selectedAccount)?.Pid?.ToString() ?? "0");
            ClientHelper.FocusWindowByPid(pid);
        }

        /*
         * Was intended to set the world IDs in the preferences file.
        private void setWorlds_CheckedChanged(object sender, EventArgs e)
        {
            if (setWorlds.Checked)
            {
                ClientHelper.ReplaceWorldIds("418", "418");
            }
            else
            {
                //preferences file handles the config differently
                ClientHelper.ReplaceWorldIds("-1", "0");
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

        // TODO: Restructure this to a different folder
        // Refactored: Extracted logic into helper methods, improved process validation, and clarified event handling.
        // Refreshes the process display by checking running processes and updating thumbnails.
        private void RefreshProcessDisplay()
        {
            CleanupThumbnailsAndControls();

            foreach (var acc in accounts)
            {
                if (!acc.Pid.HasValue)
                    continue;

                Process? proc = null;
                try
                {
                    proc = Process.GetProcessById(acc.Pid.Value);
                    if (proc.HasExited || !IsOsrsClient(proc))
                    {
                        acc.Pid = null;
                        continue;
                    }
                }
                catch (Exception)
                {
                    acc.Pid = null;
                    continue;
                }

                AddProcessThumbnail(acc, proc);
            }
        }

        // Cleans up thumbnails and controls to prevent memory leaks and UI clutter.
        // TODO: Refactor this to a different folder
        private void CleanupThumbnailsAndControls()
        {
            foreach (var thumb in thumbnailMap.Values)
            {
                NativeMethods.DwmUnregisterThumbnail(thumb);
            }
            thumbnailMap.Clear();
            flowPanelProcesses.Controls.Clear();
        }

        // Checks if the process is an OSRS client based on its name.
        // TODO: Refactor this to a different folder
        // This method can be extended to include other clients if needed.
        // TODO: Use this in other functions that check for OSRS clients ex. WatchForClientsAsync.
        private bool IsOsrsClient(Process proc)
        {
            return (proc.ProcessName.Contains("osclient", StringComparison.OrdinalIgnoreCase) ||
                proc.ProcessName.Contains("runelite", StringComparison.OrdinalIgnoreCase));
        }

        // Adds a thumbnail for the process in the flow panel.
        // TODO: Refactor this to a different folder
        private void AddProcessThumbnail(Account acc, Process proc)
        {
            var hwnd = proc.MainWindowHandle;
            if (hwnd == IntPtr.Zero)
                return;

            var pictureBox = CreateProcessPictureBox(acc, proc);

            flowPanelProcesses.Controls.Add(pictureBox);

            if (NativeMethods.DwmRegisterThumbnail(this.Handle, hwnd, out IntPtr thumb) == 0)
            {
                SetThumbnailProperties(pictureBox, thumb);
                thumbnailMap[hwnd] = thumb;
            }
        }

        // Creates a PictureBox for the process thumbnail with event handling for clicks.
        // TODO: Refactor this to a different folder
        private PictureBox CreateProcessPictureBox(Account acc, Process proc)
        {
            var pictureBox = new PictureBox
            {
                Width = 99,
                Height = 65,
                BackColor = Color.Black,
                Margin = new Padding(1)
            };

            pictureBox.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ClientHelper.FocusWindowByPid(acc.Pid.Value);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch
                    {
                        // Optionally log or handle process kill exceptions
                    }
                    acc.Pid = null;
                    RefreshProcessDisplay();
                    UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
                }
            };

            return pictureBox;
        }

        // Sets the properties for the thumbnail, including position and visibility.
        // TODO: Refactor this to a different folder
        private void SetThumbnailProperties(PictureBox pictureBox, IntPtr thumb)
        {
            Point screenPos = pictureBox.PointToScreen(Point.Empty);
            Point formPos = this.PointToClient(screenPos);

            var props = new DWM_THUMBNAIL_PROPERTIES
            {
                dwFlags = DwmFlags.DWM_TNP_RECTDESTINATION | DwmFlags.DWM_TNP_VISIBLE | DwmFlags.DWM_TNP_OPACITY,
                fVisible = true,
                opacity = 255,
                rcDestination = new RECT
                {
                    Left = formPos.X,
                    Top = formPos.Y,
                    Right = formPos.X + pictureBox.Width,
                    Bottom = formPos.Y + pictureBox.Height
                }
            };

            NativeMethods.DwmUpdateThumbnailProperties(thumb, ref props);
        }

        // Starts or stops watching for new character processes based on the button state.
        private void buttonWatchCharacters_Click(object sender, EventArgs e)
        {
            if (watchTokenSource == null)
            {
                // Disable other controls
                groupSelection.Enabled = false;
                buttonLaunchAll.Enabled = false;
                refreshPane.Enabled = false;
                listViewAccounts.Enabled = false;

                // Start watching
                watchTokenSource = new CancellationTokenSource();
                pictureLoading.Visible = true;
                labelLoading.Visible = true;
                buttonWatchCharacters.Text = "Stop Import Helper";
                // Watching for new client processes
                watchTask = Task.Run(() => WatchForClientsAsync(watchTokenSource.Token));
            }
            else
            {
                // Enable other controls
                groupSelection.Enabled = true;
                buttonLaunchAll.Enabled = true;
                refreshPane.Enabled = true;
                listViewAccounts.Enabled = true;
                // Stop watching
                watchTokenSource.Cancel();
                watchTokenSource = null;
                pictureLoading.Visible = false;
                labelLoading.Visible = false;
                buttonWatchCharacters.Text = "Start Import Helper";
                UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
            }
        }

        // Watches for new RuneLite or OSRS client processes and updates the accounts list.
        // This method runs in a separate task and checks for new processes every 2 seconds.
        // TODO: Refactor this to a different folder
        private async Task WatchForClientsAsync(CancellationToken token)
        {
            var knownPids = Process.GetProcesses()
                .Where(p => p.ProcessName.Equals("RuneLite", StringComparison.OrdinalIgnoreCase) ||
                            p.ProcessName.Equals("osclient", StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Id)
                .ToHashSet();

            while (!token.IsCancellationRequested)
            {
                var newClients = Process.GetProcesses()
                    .Where(p => (p.ProcessName.Equals("RuneLite", StringComparison.OrdinalIgnoreCase) ||
                                 p.ProcessName.Equals("osclient", StringComparison.OrdinalIgnoreCase)) &&
                                 !knownPids.Contains(p.Id))
                    .ToList();

                foreach (var proc in newClients)
                {
                    knownPids.Add(proc.Id);

                    try
                    {
                        var env = EnvReader.ReadEnvironmentVariablesFromProcess(proc.Id);
                        string sessionId = env.ContainsKey("JX_SESSION_ID") ? env["JX_SESSION_ID"] : null;
                        string displayName = env.ContainsKey("JX_DISPLAY_NAME") ? env["JX_DISPLAY_NAME"] : null;
                        string characterId = env.ContainsKey("JX_CHARACTER_ID") ? env["JX_CHARACTER_ID"] : null;
                        string clientPath = proc.MainModule?.FileName ?? "";

                        if (!string.IsNullOrWhiteSpace(displayName) && !accounts.Any(a => a.DisplayName == displayName))
                        {
                            var acc = new Account
                            {
                                SessionId = sessionId,
                                DisplayName = displayName,
                                CharacterId = characterId,
                                AccessToken = "",
                                RefreshToken = "",
                                Client = clientPath,
                                Group = [],
                                Arguments = ""
                            };

                            accounts.Add(acc);
                            File.AppendAllText("accounts.csv", $"{acc.AccessToken},{acc.RefreshToken},{acc.SessionId},{acc.DisplayName},{acc.CharacterId},Captured;,{acc.Client},{acc.Arguments}\r\n");
                        }
                    }
                    catch { }
                }
                await Task.Delay(2000, token);
            }
        }

        private void checkTopMost_CheckedChanged(object sender, EventArgs e)
        {
            if (checkTopMost.Checked)
            {
                MainForm.ActiveForm.TopMost = true;
            }
            else
            {
                MainForm.ActiveForm.TopMost = false;
            }
        }
    }

}
