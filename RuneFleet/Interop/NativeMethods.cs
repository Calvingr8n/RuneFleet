using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace RuneFleet.Interop
{
    public static class NativeMethods
    {
        public const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(nint hWnd, int id);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(nint hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(nint hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern long GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern long SetWindowLong(nint hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public const int GWL_STYLE = -16;
        public const uint WS_CAPTION = 0x00C00000;
        public const uint WS_THICKFRAME = 0x00040000;
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_FRAMECHANGED = 0x0020;

        [DllImport("dwmapi.dll")]
        public static extern int DwmRegisterThumbnail(nint dest, nint src, out nint thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(nint thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUpdateThumbnailProperties(nint hThumb, ref DWM_THUMBNAIL_PROPERTIES props);


        // Used in EnvReader.cs to get process information
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeProcessHandle OpenProcess(ProcessAccessFlags access, bool inheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(SafeProcessHandle hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION64 pbi, int cb, out int returnLength);

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess32(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION32 pbi, int cb, out int returnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

    }
}
