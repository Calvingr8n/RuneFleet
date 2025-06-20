using OSRSClientManager.Interop;
using OSRSClientManager.Models;
using OSRSClientManager.Services;
using System.Diagnostics;

namespace OSRSClientManager
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
            listViewAccounts.Items.Clear();
            foreach (var acc in accounts)
            {
                var item = new ListViewItem(acc.DisplayName);
                item.SubItems.Add(acc.Pid?.ToString() ?? "");
                if (group == "All" || group == acc.Group?.ToString())
                {
                    listViewAccounts.Items.Add(item);
                }
            }
        }

        // Sets values of group drop down based on accounts loaded
        private void UpdateGroupView()
        {
            groupSelection.Items.Clear();
            var groups = accounts.Select(p => p.Group).Distinct();
            groupSelection.Items.Add("All");
            foreach (var group in groups)
            {
                groupSelection.Items.Add(group?.ToString() ?? "");
            }
            groupSelection.SelectedIndex = 0;

        }

        private void buttonLoadPreviews_Click(object sender, EventArgs e)
        {
            RefreshProcessDisplay();
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }

        private void buttonLaunchSelected_Click(object sender, EventArgs e)
        {
            if (listViewAccounts.SelectedIndices.Count == 0) return;
            var acc = accounts[listViewAccounts.SelectedIndices[0]];
            ClientHelpers.LaunchClient(acc);
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }

        private async void buttonLaunchAll_Click(object sender, EventArgs e)
        {
            listViewAccounts.Enabled = false;
            groupSelection.Enabled = false;
            var rand = new Random();
            foreach (var acc in accounts)
            {
                if (acc.Group?.ToString() == groupSelection.SelectedItem?.ToString())
                {
                    ClientHelpers.LaunchClient(acc);
                    await Task.Delay(3000 + rand.Next(2000));
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

        private void listViewAccounts_ItemActivate(object sender, EventArgs e)
        {
            var acc = accounts[listViewAccounts.SelectedIndices[0]];
            if (acc.Pid.HasValue)
            {
                ClientHelpers.FocusWindowByPid(acc.Pid.Value);
            }
        }

        private void setWorlds_CheckedChanged(object sender, EventArgs e)
        {
            if (setWorlds.Checked)
            {
                ClientHelpers.ReplaceWorldIds("418", "418");
            }
            else
            {
                //preferences file handles the config differently
                ClientHelpers.ReplaceWorldIds("-1", "0");
            }
        }

        private void groupSelection_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateListView(groupSelection.SelectedItem?.ToString() ?? "All");
        }



        // TODO: Refactor this to a different folder?
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();

                if (listViewAccounts.Items.Count == 0)
                    return;

                int currentIndex;
                if (listViewAccounts.SelectedIndices.Count > 0)
                {
                    currentIndex = listViewAccounts.SelectedIndices[0];
                    listViewAccounts.Items[currentIndex].Selected = false;
                    listViewAccounts.Items[currentIndex].Focused = false;
                }
                else
                {
                    currentIndex = -1;
                }

                int newIndex = 0;

                if (id == HOTKEY_ID_PGDN)
                {
                    newIndex = (currentIndex + 1) % listViewAccounts.Items.Count;
                }
                else if (id == HOTKEY_ID_PGUP)
                {
                    newIndex = (currentIndex - 1 + listViewAccounts.Items.Count) % listViewAccounts.Items.Count;
                }
                else if (id == HOTKEY_ID_DEL)
                {
                    newIndex = 0;
                }

                // Simulate click
                listViewAccounts.Items[newIndex].Selected = true;
                listViewAccounts.Select();
                listViewAccounts.Focus();


                listViewAccounts.Items[newIndex].Focused = true;
                listViewAccounts_ItemActivate(this, EventArgs.Empty); // triggers "click"
            }

            base.WndProc(ref m);
        }

        // TODO: Restructure this to a different folder
        // Refactored: Extracted logic into helper methods, improved process validation, and clarified event handling.
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

        private void CleanupThumbnailsAndControls()
        {
            foreach (var thumb in thumbnailMap.Values)
            {
                NativeMethods.DwmUnregisterThumbnail(thumb);
            }
            thumbnailMap.Clear();
            flowPanelProcesses.Controls.Clear();
        }

        
        private bool IsOsrsClient(Process proc)
        {
            // Adjust process name check as needed for your client
            return (proc.ProcessName.Contains("osclient", StringComparison.OrdinalIgnoreCase) ||
                proc.ProcessName.Contains("runelite", StringComparison.OrdinalIgnoreCase));
        }

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
                    ClientHelpers.FocusWindowByPid(acc.Pid.Value);
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

    }

}
