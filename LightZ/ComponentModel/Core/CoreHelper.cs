using LightZ.ComponentModel.Interop;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace LightZ.ComponentModel.Core
{
    /// <summary>
    /// Provides a set of methods used to get information about the application.
    /// </summary>
    internal static class CoreHelper
    {

        /// <summary>
        /// Get a value that defines whether the current execution is from a unit test
        /// </summary>
        /// <returns>True if a unit test is running</returns>
        internal static bool IsUnitTesting()
        {
            return Application.Current == null;
        }

        /// <summary>
        /// Retrieves the handle of the current module.
        /// </summary>
        /// <returns>The handle of the current process's main module</returns>
        internal static IntPtr GetCurrentModuleHandle()
        {
            var currentProcess = Process.GetCurrentProcess();
            var module = currentProcess.MainModule;
            return NativeMethods.GetModuleHandle(module.ModuleName);
        }

        /// <summary>
        /// Returns the version of the executable
        /// </summary>
        /// <returns>A <see cref="Version"/> corresponding to the one of the executable.</returns>
        internal static Version GetApplicationVersion()
        {
            Assembly assembly;

            if (IsUnitTesting())
            {
                assembly = Assembly.GetExecutingAssembly();
            }
            else
            {
                assembly = Assembly.GetEntryAssembly();
            }

            return assembly.GetName().Version;
        }

        /// <summary>
        /// Gets application name.
        /// </summary>
        /// <returns>The application name.</returns>
        internal static string GetApplicationName()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                return assembly.GetName().Name;
            }

            return "UnitTestApp";
        }

        /// <summary>
        /// Throw an exception if the current thread is not <see cref="ApartmentState.STA"/>
        /// </summary>
        internal static void ThrowIfNotStaThread()
        {
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                throw new ThreadStateException("STA thread required");
            }
        }
    }
}
