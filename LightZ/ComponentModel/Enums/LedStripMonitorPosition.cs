using System.ComponentModel;

namespace LightZ.ComponentModel.Enums
{
    public enum LedStripMonitorPosition
    {
        [Description("Bottom right hand corner; make a loop by the top, then left")]
        BottomRight = 0,

        [Description("Top right hand corner; make a loop by the bottom, then left")]
        TopRight = 1,

        [Description("Top left hand corner; make a loop by the bottom, then right")]
        TopLeft = 2,

        [Description("Bottom left hand corner; make a loop by the top, then right")]
        BottomLeft = 3
    }
}
