using Microsoft.Win32.SafeHandles;
using RuneFleet.Interop;
using RuneFleet.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace RuneFleet.Services
{
    /// <summary>
    /// Provides client process operations such as launching, focusing,
    /// thumbnail management and environment capture.
    /// </summary>
internal class ClientProcessService : IDisposable
    {
        private readonly Form form;
        private readonly FlowLayoutPanel panel;
        private readonly List<Account> accounts;
        private readonly Action refreshListView;
        private readonly Dictionary<IntPtr, IntPtr> thumbnailMap = new();
        private readonly Dictionary<IntPtr, PictureBox> pictureMap = new();

        public ClientProcessService(Form form,
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
        /// Launch a RuneScape client for the given account.
        /// </summary>
        public async void LaunchClient(Account acc, decimal scale)
        {
            var filename = acc.Client;
            if (string.IsNullOrEmpty(filename))
            {
                filename = @"C:\\Program Files (x86)\\Jagex Launcher\\Games\\Old School RuneScape\\Client\\osclient.exe";
            }

            string args = acc.Arguments ?? string.Empty;
            if (scale != 0)
            {
                string scaleArg = Regex.Replace(args, @"--scale=\d+(\.\d+)?", string.Empty);
                args = scaleArg + " --scale=" + scale.ToString();
            }

            var psi = new ProcessStartInfo
            {
                // Workaround to not open as child of RuneFleet
                FileName = "cmd.exe",
                Arguments = $"/C start \"\" \"{filename}\" {args}",
                CreateNoWindow = true,
                UseShellExecute = false
                //FileName = filename,
            };
            psi.EnvironmentVariables["JX_ACCESS_TOKEN"] = acc.AccessToken;
            psi.EnvironmentVariables["JX_REFRESH_TOKEN"] = acc.RefreshToken;
            psi.EnvironmentVariables["JX_SESSION_ID"] = acc.SessionId;
            psi.EnvironmentVariables["JX_CHARACTER_ID"] = acc.CharacterId;
            psi.EnvironmentVariables["JX_DISPLAY_NAME"] = acc.DisplayName;
            
            
            var proc = Process.Start(psi);

            
            if (filename.Contains("osclient", StringComparison.OrdinalIgnoreCase))
            {
                var childPid = await WaitForChildRuneLiteAsync(proc.Id, 1);
                acc.Pid = childPid;
            }
            else
            {
                var childPid = await WaitForChildRuneLiteAsync(proc.Id, 2);
                acc.Pid = childPid;
            }
        }

        /// <summary>
        /// Bring the client with the specified PID to the foreground.
        /// </summary>
        public void FocusClient(int pid)
        {
            try
            {
                var proc = Process.GetProcessById(pid);
                NativeMethods.ShowWindow(proc.MainWindowHandle, NativeMethods.SW_RESTORE);
                NativeMethods.SetForegroundWindow(proc.MainWindowHandle);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"FocusClient failed: {ex}");
            }
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
                catch (Exception ex)
                {
                    Trace.TraceWarning($"Unable to read process {acc.Pid}: {ex.Message}");
                    acc.Pid = null;
                    continue;
                }

                AddProcessThumbnail(acc, proc);
            }
        }

        private void CleanupThumbnailsAndControls()
        {
            foreach (var thumb in thumbnailMap.Values)
                NativeMethods.DwmUnregisterThumbnail(thumb);

            thumbnailMap.Clear();
            pictureMap.Clear();
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
                pictureMap[hwnd] = pictureBox;
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
                    FocusClient(acc.Pid!.Value);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError($"Failed to kill process {proc.Id}: {ex.Message}");
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

        public void UpdateThumbnailPositions()
        {
            foreach (var kvp in thumbnailMap)
            {
                if (pictureMap.TryGetValue(kvp.Key, out var box))
                {
                    SetThumbnailProperties(box, kvp.Value);
                }
            }
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
                        var env = ReadEnvironmentVariablesFromProcess(proc.Id);
                        string? sessionId = env.ContainsKey("JX_SESSION_ID") ? env["JX_SESSION_ID"] : null;
                        string? displayName = env.ContainsKey("JX_DISPLAY_NAME") ? env["JX_DISPLAY_NAME"] : null;
                        string? characterId = env.ContainsKey("JX_CHARACTER_ID") ? env["JX_CHARACTER_ID"] : null;
                        string clientPath = proc.MainModule?.FileName ?? string.Empty;

                        if (!string.IsNullOrWhiteSpace(sessionId))
                        {
                            var fetched = await FetchAccountsFromSessionAsync(sessionId);
                            foreach (var f in fetched)
                            {
                                if (string.IsNullOrWhiteSpace(f.DisplayName))
                                    continue;
                                if (accounts.Any(a => a.DisplayName == f.DisplayName))
                                    continue;

                                var acc = new Account
                                {
                                    SessionId = sessionId,
                                    DisplayName = f.DisplayName,
                                    CharacterId = f.AccountId,
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
                        else if (!string.IsNullOrWhiteSpace(displayName) && !accounts.Any(a => a.DisplayName == displayName))
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
                    catch (Exception ex)
                    {
                        Trace.TraceError($"Failed to capture process {proc.Id}: {ex}");
                    }
                }

                await Task.Delay(2000, token);
            }
        }

        private static async Task<List<ApiAccount>> FetchAccountsFromSessionAsync(string sessionId)
        {
            var list = new List<ApiAccount>();
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sessionId}");
                using var resp = await client.GetAsync("https://auth.runescape.com/game-session/v1/accounts?fetchMembership=true");
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<ApiAccount>>(json, opts);
                if (result != null)
                    list = result;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to fetch accounts for session {sessionId}: {ex}");
            }

            return list;
        }

        private class ApiAccount
        {
            public string AccountId { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
        }

        private static async Task<int?> WaitForChildRuneLiteAsync(int parentPid, int depth, long timeoutMs = 60000, int pollIntervalMs = 200)
        {
            int waited = 0;
            int foundDepth = 0;
            while (waited < timeoutMs)
            {
                var childPid = GetChildRuneLitePid(parentPid);
                if (childPid.HasValue)
                {
                    foundDepth++;
                    if (foundDepth == depth)
                    {
                        return childPid;
                    }
                    else
                    {
                        parentPid = childPid.Value; // Continue searching deeper
                    }
                }

                await Task.Delay(pollIntervalMs);
                waited += pollIntervalMs;
            }
            return null;
        }

        private static int? GetChildRuneLitePid(int parentPid)
        {
            string query = $"SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = {parentPid} AND (Name = 'RuneLite.exe' OR Name = 'osclient.exe')";
            using var searcher = new ManagementObjectSearcher(query);
            using var results = searcher.Get();

            foreach (ManagementObject proc in results)
            {
                int childPid = Convert.ToInt32(proc["ProcessId"]);
                try
                {
                    var childProc = Process.GetProcessById(childPid);
                    if (!childProc.HasExited)
                        return childPid;
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning($"Failed to inspect child process {childPid}: {ex.Message}");
                }
            }
            return null;
        }

        public static void ReplaceWorldIds(string defaltWorld, string lastWorld)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filePath = Path.Combine(userProfile, @"Jagex\Old School RuneScape\preferences_client.dat");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            var lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("DefaultWorldId"))
                {
                    lines[i] = $"DefaultWorldId {defaltWorld}";
                }
                else if (lines[i].StartsWith("LastWorldId"))
                {
                    lines[i] = $"LastWorldId {lastWorld}";
                }
            }

            File.WriteAllLines(filePath, lines);
        }

        private static Dictionary<string, string> ReadEnvironmentVariablesFromProcess(int pid)
        {
            var envVars = new Dictionary<string, string>();

            using var hProcess = NativeMethods.OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead, false, pid);
            if (hProcess.IsInvalid)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to open process.");

            bool isWow64;
            if (!NativeMethods.IsWow64Process(hProcess.DangerousGetHandle(), out isWow64))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to check process bitness.");

            if (Environment.Is64BitOperatingSystem && !isWow64)
            {
                return ReadEnvironment64(hProcess);
            }
            else
            {
                return ReadEnvironment32(hProcess);
            }
        }

        private static Dictionary<string, string> ReadEnvironment64(SafeProcessHandle hProcess)
        {
            var envVars = new Dictionary<string, string>();

            PROCESS_BASIC_INFORMATION64 pbi = new();
            int returnLength;
            int status = NativeMethods.NtQueryInformationProcess(hProcess.DangerousGetHandle(), 0, ref pbi, Marshal.SizeOf<PROCESS_BASIC_INFORMATION64>(), out returnLength);
            if (status != 0)
                throw new Win32Exception(status, "Failed to query process information.");

            IntPtr rtlUserProcParamsAddress = ReadPointer64(hProcess, new IntPtr((long)pbi.PebBaseAddress + 0x20));
            IntPtr environmentAddress = ReadPointer64(hProcess, rtlUserProcParamsAddress + 0x80);

            byte[] buffer = new byte[0x10000];
            if (!NativeMethods.ReadProcessMemory(hProcess, environmentAddress, buffer, buffer.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to read environment block.");

            string envBlock = System.Text.Encoding.Unicode.GetString(buffer);
            var entries = envBlock.Split('\0', StringSplitOptions.RemoveEmptyEntries);
            foreach (var entry in entries)
            {
                int idx = entry.IndexOf('=');
                if (idx > 0)
                {
                    string key = entry.Substring(0, idx);
                    string val = entry.Substring(idx + 1);
                    envVars[key] = val;
                }
            }

            return envVars;
        }

        private static Dictionary<string, string> ReadEnvironment32(SafeProcessHandle hProcess)
        {
            var envVars = new Dictionary<string, string>();

            PROCESS_BASIC_INFORMATION32 pbi = new();
            int returnLength;
            int status = NativeMethods.NtQueryInformationProcess32(hProcess.DangerousGetHandle(), 0, ref pbi, Marshal.SizeOf<PROCESS_BASIC_INFORMATION32>(), out returnLength);
            if (status != 0)
                throw new Win32Exception(status, "Failed to query process information.");

            IntPtr rtlUserProcParamsAddress = ReadPointer32(hProcess, new IntPtr(pbi.PebBaseAddress.ToInt32() + 0x10));
            IntPtr environmentAddress = ReadPointer32(hProcess, new IntPtr(rtlUserProcParamsAddress.ToInt32() + 0x48));

            byte[] buffer = new byte[0x10000];
            if (!NativeMethods.ReadProcessMemory(hProcess, environmentAddress, buffer, buffer.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to read environment block.");

            string envBlock = System.Text.Encoding.Unicode.GetString(buffer);
            var entries = envBlock.Split('\0', StringSplitOptions.RemoveEmptyEntries);
            foreach (var entry in entries)
            {
                int idx = entry.IndexOf('=');
                if (idx > 0)
                {
                    string key = entry.Substring(0, idx);
                    string val = entry.Substring(idx + 1);
                    envVars[key] = val;
                }
            }

            return envVars;
        }

        private static IntPtr ReadPointer64(SafeProcessHandle hProcess, IntPtr address)
        {
            byte[] buffer = new byte[8];
            if (!NativeMethods.ReadProcessMemory(hProcess, address, buffer, buffer.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to read pointer.");

            return new IntPtr(BitConverter.ToInt64(buffer, 0));
        }

        private static IntPtr ReadPointer32(SafeProcessHandle hProcess, IntPtr address)
        {
            byte[] buffer = new byte[4];
            if (!NativeMethods.ReadProcessMemory(hProcess, address, buffer, buffer.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to read pointer.");

            return new IntPtr(BitConverter.ToInt32(buffer, 0));
        }

        public void Dispose()
        {
            CleanupThumbnailsAndControls();
        }
    }
}
