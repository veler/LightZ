namespace LightZ.ComponentModel
{
    /// <summary>
    /// Provides constants
    /// </summary>
    internal static class Consts
    {
        // Native methods
        internal const string Kernel32 = "kernel32.dll";
        internal const string Ole32 = "ole32.dll";
        internal const string Shcore = "Shcore.dll";
        internal const string User32 = "user32.dll";
        internal const string UxTheme = "uxtheme.dll";

        // Interop
        internal const int MONITOR_DEFAULTTONEAREST = 2;
        internal const uint SPI_GETSCREENREADER = 0x0046;

        // Audio
        internal const int MaxAudioSpectrumLine = 16;
        internal static readonly int[] AudioSpectrumBassLines = { 0, 4 }; // The samples corresponding to the bass are the first 4.

        // Serial
        internal const int BandwidthRate = 115200;
        internal const int WriteTimeout = 200;
        internal const string StandardSerialPortSignature = "{4d36e978-e325-11ce-bfc1-08002be10318}";

        // DirectX monitor borders
        internal const int BytePerPixel = 4;
        internal const int Margin = 40;
        internal const int Thickness = 20;

        // COM
        internal const string CShellLink = "00021401-0000-0000-C000-000000000046";
        internal const string PersistFile = "0000010b-0000-0000-C000-000000000046";
        internal const string PropertyStore = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99";
        internal const string ShellLinkW = "000214F9-0000-0000-C000-000000000046";

        // Other
        internal const int SingleInstanceProcessExitCode = 334534;
    }
}
