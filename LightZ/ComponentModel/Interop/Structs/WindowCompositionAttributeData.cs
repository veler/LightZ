using LightZ.ComponentModel.Enums;
using System;
using System.Runtime.InteropServices;

namespace LightZ.ComponentModel.Interop.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }
}
