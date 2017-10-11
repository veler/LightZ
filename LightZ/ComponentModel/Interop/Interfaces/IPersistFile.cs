using LightZ.ComponentModel.Enums;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LightZ.ComponentModel.Interop.Interfaces
{
    [ComImport, Guid(Consts.PersistFile), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersistFile
    {
        uint GetCurFile([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile);

        uint IsDirty();

        uint Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.U4)] Stgm dwMode);

        uint Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, bool fRemember);

        uint SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
    }
}
