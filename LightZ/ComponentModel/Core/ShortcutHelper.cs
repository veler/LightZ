using LightZ.ComponentModel.Interop.Classes;
using LightZ.ComponentModel.Interop.Interfaces;
using LightZ.ComponentModel.Interop.Structs;
using System;
using System.Diagnostics;
using System.IO;

namespace LightZ.ComponentModel.Core
{
    /// <summary>
    /// Provide some methods to create shortcuts files (.LNK).
    /// </summary>
    internal static class ShortcutHelper
    {
        #region Methods

        /// <summary>
        /// Create a new shortcut.
        /// </summary>
        /// <param name="lnkFilePath">The LNK file path.</param>
        /// <param name="overrides">overrides if already exists.</param>
        internal static void CreateShortcut(string lnkFilePath, bool overrides)
        {
            CreateShortcut(lnkFilePath, Guid.NewGuid(), overrides);
        }

        /// <summary>
        /// Create a new shortcut.
        /// </summary>
        /// <param name="lnkFilePath">The LNK file path.</param>
        /// <param name="guid">The GUID.</param>
        /// <param name="overrides">overrides if already exists.</param>
        internal static void CreateShortcut(string lnkFilePath, Guid guid, bool overrides)
        {
            Requires.NotNullOrWhiteSpace(lnkFilePath, nameof(lnkFilePath));
            Requires.NotNull(guid, nameof(guid));

            if (File.Exists(lnkFilePath))
            {
                if (overrides)
                {
                    File.Delete(lnkFilePath);
                }
                else
                {
                    return;
                }
            }

            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            var newShortcut = (IShellLinkW)new CShellLink();

            Requires.VerifySucceeded((int)newShortcut.SetPath(exePath));
            Requires.VerifySucceeded((int)newShortcut.SetArguments(""));

            var newShortcutProperties = (IPropertyStore)newShortcut;
            var systemPropertiesSystemAppUserModelId = new PropertyKey(guid, 5);

            using (var appId = new PropVariant(CoreHelper.GetApplicationName()))
            {
                Requires.VerifySucceeded(newShortcutProperties.SetValue(systemPropertiesSystemAppUserModelId, appId));
                Requires.VerifySucceeded(newShortcutProperties.Commit());
            }

            var newShortcutSave = (IPersistFile)newShortcut;
            Requires.VerifySucceeded((int)newShortcutSave.Save(lnkFilePath, true));
        }

        #endregion
    }

}
