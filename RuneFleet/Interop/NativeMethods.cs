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

        [DllImport("dwmapi.dll")]
        public static extern int DwmRegisterThumbnail(nint dest, nint src, out nint thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(nint thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUpdateThumbnailProperties(nint hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

    }
}
