using System.ComponentModel;

namespace LightZ.ComponentModel.Enums
{
    public enum LedStripMode
    {
        [Description("Off")]
        Off = 0,

        [Description("Manual full color")]
        Manual = 1,

        [Description("Audio spectrum")]
        AudioSpectrum = 2,

        [Description("Monitor's colors")]
        MonitorColors = 3
    }
}
