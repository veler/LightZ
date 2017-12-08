using LightZ.ComponentModel.Interop;
using LightZ.ComponentModel.Interop.Structs;
using LightZ.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace LightZ.ComponentModel.Core
{
    /// <summary>
    /// Provides a set of functions used to retrieve information about the operating system.
    /// </summary>
    internal static class SystemInfoHelper
    {
        /// <summary>
        /// Determines whether a screen reader is running on the system.
        /// </summary>
        /// <returns>Returns True if a screen reader is running.</returns>
        internal static bool IsScreenReaderRunning()
        {
            if (Debugger.IsAttached)
            {
                return true;
            }

            var result = false;
            NativeMethods.SystemParametersInfo(Consts.SPI_GETSCREENREADER, 0, ref result, 0);
            return result;
        }

        /// <summary>
        /// Retrieves information about all monitors.
        /// </summary>
        /// <returns>An array of <see cref="ScreenInfo"/></returns>
        internal static ScreenInfo[] GetAllScreenInfos()
        {
            var result = new List<ScreenInfo>();
            var allScreens = Screen.AllScreens.ToList();

            var primaryScreenIndex = allScreens.FindIndex(s => s.Primary);
            var primaryScreen = allScreens[primaryScreenIndex];
            var screenScale = GetMonitorScaleFactor(primaryScreen);
            var primaryScreenScaleFactor = screenScale / 100.0;

            result.Add(new ScreenInfo
            {
                Index = primaryScreenIndex,
                DeviceName = $"Primary screen ({primaryScreen.Bounds.Width}x{primaryScreen.Bounds.Height})",
                DeviceId = primaryScreen.DeviceName,
                Scale = screenScale,
                Primary = true,
                Bounds = new Rect(primaryScreen.Bounds.Left, primaryScreen.Bounds.Top, primaryScreen.Bounds.Width, primaryScreen.Bounds.Height),
                OriginalBounds = new Rect(primaryScreen.Bounds.Left, primaryScreen.Bounds.Top, primaryScreen.Bounds.Width, primaryScreen.Bounds.Height)
            });

            for (var i = 0; i < allScreens.Count; i++)
            {
                if (i == primaryScreenIndex)
                {
                    continue;
                }

                var screen = allScreens[i];

                screenScale = GetMonitorScaleFactor(screen);

                double left = screen.Bounds.Left;
                if (screen.Bounds.Left != 0 && primaryScreenScaleFactor > screenScale / 100.0)
                {
                    left = screen.Bounds.Left / primaryScreenScaleFactor * screenScale / 100;
                }

                double top = screen.Bounds.Top;
                if (screen.Bounds.Top != 0 && primaryScreenScaleFactor > screenScale / 100.0)
                {
                    top = screen.Bounds.Top / primaryScreenScaleFactor * screenScale / 100;
                }

                // Since I changed the API to target DirectX 11 instead of 9, there is no scale problem anymore. But let's just comment this part in case.
                //var width = screen.Bounds.Width / primaryScreenScaleFactor * screenScale / 100;
                //var height = screen.Bounds.Height / primaryScreenScaleFactor * screenScale / 100;

                var width = screen.Bounds.Width;
                var height = screen.Bounds.Height;

                result.Add(new ScreenInfo
                {
                    Index = i,
                    DeviceName = $"External screen ({(int)width}x{(int)height})",
                    DeviceId = screen.DeviceName,
                    Scale = screenScale,
                    Bounds = new Rect((int)left, (int)top, (int)width, (int)height),
                    OriginalBounds = new Rect(screen.Bounds.Left, screen.Bounds.Top, screen.Bounds.Width, screen.Bounds.Height),
                    Primary = false
                });
            }

            return result.ToArray();
        }

        /// <summary>
        /// Retrieve the scale factor of the specified screen
        /// </summary>
        /// <param name="screen">The screen</param>
        /// <returns>Return a number between 100 and 300. The value is in percent.</returns>
        private static int GetMonitorScaleFactor(Screen screen)
        {
            var point = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var hMonitor = NativeMethods.MonitorFromPoint(point, Consts.MONITOR_DEFAULTTONEAREST);
            int screenScale;
            NativeMethods.GetScaleFactorForMonitor(hMonitor, out screenScale);
            return screenScale;
        }
    }
}
