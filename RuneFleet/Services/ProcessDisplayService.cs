using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using RuneFleet.Models;
using RuneFleet.Interop;

namespace RuneFleet.Services
{
    /// <summary>
    /// Handles process thumbnail management and client watching logic.
    /// </summary>
    internal class ProcessDisplayService
    {
        private readonly Form form;
        private readonly FlowLayoutPanel panel;
        private readonly List<Account> accounts;
        private readonly Action refreshListView;
        private readonly Dictionary<IntPtr, IntPtr> thumbnailMap = new();

        public ProcessDisplayService(Form form,
                                     FlowLayoutPanel panel,
                                     List<Account> accounts,
                                     Action refreshListView)
        {
            this.form = form;
            this.panel = panel;
            this.accounts = accounts;
            this.refreshListView = refreshListView;
        }

        /// <summary>
        /// Refresh thumbnails for all running account processes.
        /// </summary>
        public void RefreshProcessDisplay()
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
                catch
                {
                    acc.Pid = null;
                    continue;
                }

                AddProcessThumbnail(acc, proc);
            }
        }

        /// <summary>
        /// Unregister thumbnails and clear controls.
        /// </summary>
        private void CleanupThumbnailsAndControls()
        {
            foreach (var thumb in thumbnailMap.Values)
                NativeMethods.DwmUnregisterThumbnail(thumb);

            thumbnailMap.Clear();
            panel.Controls.Clear();
        }

        private static bool IsOsrsClient(Process proc)
        {
            return proc.ProcessName.Contains("osclient", StringComparison.OrdinalIgnoreCase) ||
                   proc.ProcessName.Contains("runelite", StringComparison.OrdinalIgnoreCase);
        }

        private void AddProcessThumbnail(Account acc, Process proc)
        {
            var hwnd = proc.MainWindowHandle;
            if (hwnd == IntPtr.Zero)
                return;

            var pictureBox = CreateProcessPictureBox(acc, proc);

            panel.Controls.Add(pictureBox);

            if (NativeMethods.DwmRegisterThumbnail(form.Handle, hwnd, out IntPtr thumb) == 0)
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
                    }
                    acc.Pid = null;
                    RefreshProcessDisplay();
                    refreshListView();
                }
            };

            return pictureBox;
        }

        private void SetThumbnailProperties(PictureBox pictureBox, IntPtr thumb)
        {
            Point screenPos = pictureBox.PointToScreen(Point.Empty);
            Point formPos = form.PointToClient(screenPos);

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

        /// <summary>
        /// Continuously watch for new client processes and capture their data.
        /// </summary>
        public async Task WatchForClientsAsync(CancellationToken token)
        {
            var knownPids = Process.GetProcesses()
                .Where(IsOsrsClient)
                .Select(p => p.Id)
                .ToHashSet();

            while (!token.IsCancellationRequested)
            {
                var newClients = Process.GetProcesses()
                    .Where(p => IsOsrsClient(p) && !knownPids.Contains(p.Id))
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
                        string clientPath = proc.MainModule?.FileName ?? string.Empty;

                        if (!string.IsNullOrWhiteSpace(displayName) && !accounts.Any(a => a.DisplayName == displayName))
                        {
                            var acc = new Account
                            {
                                SessionId = sessionId,
                                DisplayName = displayName,
                                CharacterId = characterId,
                                AccessToken = string.Empty,
                                RefreshToken = string.Empty,
                                Client = clientPath,
                                Group = [],
                                Arguments = string.Empty
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
    }
}
