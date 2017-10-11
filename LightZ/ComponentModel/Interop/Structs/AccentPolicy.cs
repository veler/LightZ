using LightZ.ComponentModel.Enums;
using System.Runtime.InteropServices;

namespace LightZ.ComponentModel.Interop.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }
}
