using System.Runtime.InteropServices;

namespace OSRSClientManager.Interop
{

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_THUMBNAIL_PROPERTIES
    {
        public uint dwFlags;
        public RECT rcDestination;
        public RECT rcSource;
        public byte opacity;
        public bool fVisible;
        public bool fSourceClientAreaOnly;
    }


    public static class DwmFlags
    {
        public const uint DWM_TNP_RECTDESTINATION = 0x00000001;
        public const uint DWM_TNP_VISIBLE = 0x00000008;
        public const uint DWM_TNP_OPACITY = 0x00000004;
    }
}
