using System.Diagnostics;
using System.Management;

namespace RuneFleet.Services
{
    public static class RuneLiteProcessHelper
    {
        // Runelite spawns a child process which is the pid we need for preview management
        // Asynchronously wait for the child RuneLite process to start
        public static async Task<int?> WaitForChildRuneLiteAsync(int parentPid, long timeoutMs = 60000, int pollIntervalMs = 200)
        {
            int waited = 0;

            while (waited < timeoutMs)
            {
                var childPid = GetChildRuneLitePid(parentPid);
                if (childPid.HasValue)
                    return childPid;

                await Task.Delay(pollIntervalMs);
                waited += pollIntervalMs;
            }

            return null; // timeout
        }

        // Synchronously get the child RuneLite process ID
        private static int? GetChildRuneLitePid(int parentPid)
        {
            // Query WMI for child RuneLite.exe processes
            string query = $"SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = {parentPid} AND Name = 'RuneLite.exe'";
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
                catch { }
            }
            return null;
        }

    }
}
