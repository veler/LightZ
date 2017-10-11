using LightZ.ComponentModel.Enums;
using LightZ.ComponentModel.Interop.Classes;
using LightZ.ComponentModel.Interop.Structs;
using System;
using System.Runtime.InteropServices;
using static LightZ.ComponentModel.Core.Delegates;

namespace LightZ.ComponentModel.Interop
{
    /// <summary>
    /// Provides a set of native methods
    /// </summary>
    internal static class NativeMethods
    {
        #region Kernel32

        /// <summary>
        /// Retrieves a module handle for the specified module. The module must have been loaded by the calling process.
        /// </summary>
        /// <param name="lpModuleName">The name of the loaded module (either a .dll or .exe file).</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified module. If the function fails, the return value is NULL.</returns>
        [DllImport(Consts.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// Enables an application to inform the system that it is in use, thereby preventing the system from entering sleep or turning off the display while the application is running.
        /// </summary>
        /// <param name="esFlags">The thread's execution requirements.</param>
        /// <returns>If the function succeeds, the return value is the previous thread execution state. If the function fails, the return value is NULL.</returns>
        [DllImport(Consts.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        #endregion

        #region Ole32

        /// <summary>
        /// The PropVariantClear function frees all elements that can be freed in a given <see cref="PropVariant"/> structure. For complex elements with known element pointers, the underlying elements are freed prior to freeing the containing element.
        /// </summary>
        /// <param name="pvar">A pointer to an initialized <see cref="PropVariant"/> structure for which any deallocatable elements are to be freed. On return, all zeroes are written to the <see cref="PropVariant"/> structure.</param>
        [DllImport(Consts.Ole32, SetLastError = true, PreserveSig = false)]
        internal static extern void PropVariantClear([In, Out] PropVariant pvar);

        #endregion

        #region Shcore

        /// <summary>
        /// Gets the scale factor of a specific monitor. This function replaces GetScaleFactorForDevice.
        /// </summary>
        /// <param name="hmonitor">The monitor's handle.</param>
        /// <param name="deviceScaleFactor">When this function returns successfully, this value points to one of the DEVICE_SCALE_FACTOR values that specify the scale factor of the specified monitor. </param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(Consts.Shcore, SetLastError = true)]
        internal static extern IntPtr GetScaleFactorForMonitor([In]IntPtr hmonitor, [Out]out int deviceScaleFactor);

        #endregion

        #region User32

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="hhk">This parameter is ignored.</param>
        /// <param name="nCode">The hook code passed to the current hook procedure. The next hook procedure uses this code to determine how to process the hook information.</param>
        /// <param name="wParam">The wParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
        /// <param name="lParam">The lParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
        /// <returns>This value is returned by the next hook procedure in the chain. The current hook procedure must also return this value. The meaning of the return value depends on the hook type. For more information, see the descriptions of the individual hook procedures.</returns>
        [DllImport(Consts.User32, SetLastError = true)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Moves the cursor to the specified screen coordinates. If the new coordinates are not within the screen rectangle set by the most recent ClipCursor function call, the system automatically adjusts the coordinates so that the cursor stays within the rectangle.
        /// </summary>
        /// <param name="pt">The <see cref="Point"/> that corresponds to the cursor's coordonates.</param>
        /// <returns>Returns nonzero if successful or zero otherwise.</returns>
        [DllImport(Consts.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Point pt);

        /// <summary>
        /// Retrieves the status of the specified virtual key. The status specifies whether the key is up, down, or toggled (on, off—alternating each time the key is pressed). 
        /// </summary>
        /// <param name="keyCode">A virtual key. If the desired virtual key is a letter or digit (A through Z, a through z, or 0 through 9), nVirtKey must be set to the ASCII value of that character. For other keys, it must be a virtual-key code.</param>
        /// <returns>The return value specifies the status of the specified virtual key, as follows: If the high-order bit is 1, the key is down; otherwise, it is up. If the low-order bit is 1, the key is toggled.A key, such as the CAPS LOCK key, is toggled if it is turned on.The key is off and untoggled if the low-order bit is 0. A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled, and off when the key is untoggled.</returns>
        [DllImport(Consts.User32, SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern short GetKeyState(int keyCode);

        /// <summary>
        /// The MonitorFromPoint function retrieves a handle to the display monitor that contains a specified point.
        /// </summary>
        /// <param name="pt">A <see cref="System.Drawing.Point"/> structure that specifies the point of interest in virtual-screen coordinates.</param>
        /// <param name="dwFlags">Determines the function's return value if the point is not contained within any display monitor.</param>
        /// <returns>If the point is contained by a display monitor, the return value is an HMONITOR handle to that display monitor.</returns>
        [DllImport(Consts.User32, SetLastError = true)]
        internal static extern IntPtr MonitorFromPoint([In]System.Drawing.Point pt, [In]uint dwFlags);

        /// <summary>
        /// Brings the thread that created the specified window into the foreground and activates the window. Keyboard input is
        /// directed to the window, and various visual cues are changed for the user. The system assigns a slightly higher
        /// priority to the thread that created the foreground window than it does to other threads.
        /// <para>See for https://msdn.microsoft.com/en-us/library/windows/desktop/ms633539%28v=vs.85%29.aspx more information.</para>
        /// </summary>
        /// <returns><c>true</c> or nonzero if the window was brought to the foreground, <c>false</c> or zero If the window was not brought to the foreground.</returns>
        [DllImport(Consts.User32, SetLastError = true)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Sets various information regarding DWM window attributes
        /// </summary>
        /// <param name="hwnd">The window handle whose information is to be changed</param>
        /// <param name="data">Pointer to a structure which both specifies and delivers the attribute data</param>
        /// <returns>Nonzero on success, zero otherwise.</returns>
        [DllImport(Consts.User32)]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        /// <summary>
        /// Retrieves or sets the value of one of the system-wide parameters. This function can also update the user profile while setting a parameter.
        /// </summary>
        /// <param name="uiAction">The system-wide parameter to be retrieved or set.</param>
        /// <param name="uiParam">A parameter whose usage and format depends on the system parameter being queried or set. For more information about system-wide parameters, see the uiAction parameter. If not otherwise indicated, you must specify zero for this parameter.</param>
        /// <param name="pvParam">A parameter whose usage and format depends on the system parameter being queried or set.</param>
        /// <param name="fWinIni">If a system parameter is being set, specifies whether the user profile is to be updated, and if so, whether the WM_SETTINGCHANGE message is to be broadcast to all top-level windows to notify them of the change.</param>
        /// <returns>If the function succeeds, the return value is a nonzero value.</returns>
        [DllImport(Consts.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref bool pvParam, uint fWinIni);

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain. You would install a hook procedure to monitor the system for certain types of events. These events are associated either with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="hookType">The type of hook procedure to be installed.</param>
        /// <param name="lpfn">A pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a thread created by a different process, the lpfn parameter must point to a hook procedure in a DLL. Otherwise, lpfn can point to a hook procedure in the code associated with the current process.</param>
        /// <param name="hMod">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure is within the code associated with the current process.</param>
        /// <param name="dwThreadId">The identifier of the thread with which the hook procedure is to be associated. For desktop apps, if this parameter is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread. For Windows Store apps, see the Remarks section.</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
        [DllImport(Consts.User32, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(HookType hookType, HookProcCallback lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
        /// </summary>
        /// <param name="hhk">A handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport(Consts.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        #endregion

        #region UxTheme

        /// <summary>
        /// Get the immersive user color set preference.
        /// </summary>
        /// <param name="forceCheckRegistry"></param>
        /// <param name="skipCheckOnFail"></param>
        /// <returns></returns>
        [DllImport(Consts.UxTheme, SetLastError = true, EntryPoint = "#98", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        internal static extern uint GetImmersiveUserColorSetPreference(bool forceCheckRegistry, bool skipCheckOnFail);

        /// <summary>
        /// Get the immersive color set count.
        /// </summary>
        /// <returns></returns>
        [DllImport(Consts.UxTheme, SetLastError = true, EntryPoint = "#94", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        internal static extern uint GetImmersiveColorSetCount();

        /// <summary>
        /// Get the immersive color from color set.
        /// </summary>
        /// <param name="immersiveColorSet"></param>
        /// <param name="immersiveColorType"></param>
        /// <param name="ignoreHighContrast"></param>
        /// <param name="highContrastCacheMode"></param>
        /// <returns></returns>
        [DllImport(Consts.UxTheme, SetLastError = true, EntryPoint = "#95", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        internal static extern uint GetImmersiveColorFromColorSetEx(uint immersiveColorSet, uint immersiveColorType, bool ignoreHighContrast, uint highContrastCacheMode);

        /// <summary>
        /// Get the immersive color type from name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport(Consts.UxTheme, SetLastError = true, EntryPoint = "#96", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        internal static extern uint GetImmersiveColorTypeFromName(IntPtr name);

        /// <summary>
        /// Get the immersive color named type by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [DllImport(Consts.UxTheme, SetLastError = true, EntryPoint = "#100", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetImmersiveColorNamedTypeByIndex(uint index);

        #endregion
    }
}
