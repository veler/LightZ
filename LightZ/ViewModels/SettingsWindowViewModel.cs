using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LightZ.ComponentModel.Core;
using LightZ.ComponentModel.Enums;
using LightZ.ComponentModel.Services;
using LightZ.ComponentModel.Services.Base;
using LightZ.ComponentModel.UI.Controls;
using LightZ.Models;
using LightZ.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LightZ.ViewModels
{
    /// <summary>
    /// Interaction logic for <see cref="SettingsWindow"/>
    /// </summary>
    internal sealed class SettingsWindowViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether the app must starts with Windows or not.
        /// </summary>
        public bool AutoStartup
        {
            get
            {
                return File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"/LightZ.lnk");
            }
            set
            {
                if (value)
                {
                    ShortcutHelper.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"/LightZ.lnk", true);
                }
                else if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"/LightZ.lnk"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"/LightZ.lnk");
                }
            }
        }

        /// <summary>
        /// Gets the list of available Bluetooth or USB devices.
        /// </summary>
        public List<SerialDevice> SerialDevices => ServiceLocator.GetService<SerialService>().GetSerialDevices();

        /// <summary>
        /// Gets or sets the selected LightZ Bluetooth or USB device.
        /// </summary>
        public string LightZDevice
        {
            get { return Settings.Default.SerialDevice; }
            set
            {
                Settings.Default.SerialDevice = value;
                Settings.Default.Save();
                RaisePropertyChanged();
                ServiceLocator.GetService<SerialService>().CurrentSerialDevice = SerialDevices.Single(device => device.DeviceName == value);
            }
        }

        /// <summary>
        /// Gets a descriptive status of the connection to the LightZ device.
        /// </summary>
        public string LightZDeviceStatus
        {
            get
            {
                var service = ServiceLocator.GetService<SerialService>();
                if (service.Connected)
                {
                    return "Connected";
                }
                return "Disconnected";
            }
        }

        /// <summary>
        /// Gets or sets the led strip mode
        /// </summary>
        public LedStripMode Mode
        {
            get { return (LedStripMode)Settings.Default.LedStripMode; }
            set
            {
                Settings.Default.LedStripMode = (int)value;
                Settings.Default.Save();
                RaisePropertyChanged();
                ServiceLocator.GetService<LedStripService>().Mode = value;
            }
        }

        /// <summary>
        /// Gets or sets the led brightness
        /// </summary>
        public byte ManualBrightness
        {
            get { return Settings.Default.ManualBrightness; }
            set
            {
                Settings.Default.ManualBrightness = value;
                Settings.Default.Save();
                RaisePropertyChanged();
                ServiceLocator.GetService<LedStripService>().ManualBrightness = value;
            }
        }

        /// <summary>
        /// Gets or sets the led color
        /// </summary>
        public Color ManualColor
        {
            get { return JsonConvert.DeserializeObject<Color>(Settings.Default.ManualColor); }
            set
            {
                Settings.Default.ManualColor = JsonConvert.SerializeObject(value);
                Settings.Default.Save();
                RaisePropertyChanged();
                ServiceLocator.GetService<LedStripService>().ManualColor = value;
            }
        }

        /// <summary>
        /// Gets the list of available sound cards.
        /// </summary>
        public List<AudioDevice> AudioDevices => ServiceLocator.GetService<AudioService>().GetAudioDevices();

        /// <summary>
        /// Gets or sets the selected sound card.
        /// </summary>
        public string AudioDevice
        {
            get { return Settings.Default.AudioDevice; }
            set
            {
                Settings.Default.AudioDevice = value;
                Settings.Default.Save();
                RaisePropertyChanged();
                ServiceLocator.GetService<AudioService>().CurrentAudioDevice = AudioDevices.FirstOrDefault(device => device.DeviceName == value);
            }
        }

        /// <summary>
        /// Gets the list of available monitors.
        /// </summary>
        public List<ScreenInfo> Monitors => SystemInfoHelper.GetAllScreenInfos().ToList();

        /// <summary>
        /// Gets or sets the selected monitor.
        /// </summary>
        public string Monitor
        {
            get { return Settings.Default.Monitor; }
            set
            {
                Settings.Default.Monitor = value;
                Settings.Default.Save();
                RaisePropertyChanged();
                ServiceLocator.GetService<DirectXScreenService>().CurrentMonitor = Monitors.FirstOrDefault(device => device.DeviceId == value);
            }
        }

        /// <summary>
        /// Gets or sets the led strip position on the monitor
        /// </summary>
        public LedStripMonitorPosition LedStripMonitorPosition
        {
            get { return (LedStripMonitorPosition)Settings.Default.LedStripMonitorPosition; }
            set
            {
                Settings.Default.LedStripMonitorPosition = (int)value;
                Settings.Default.Save();
                RaisePropertyChanged();
                ServiceLocator.GetService<DirectXScreenService>().LedStripMonitorPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the total number of horizontal Leds (top and bottom)
        /// </summary>
        public string HorizontalLeds
        {
            get { return Settings.Default.HorizontalLeds.ToString(); }
            set
            {
                int newInt;
                if (int.TryParse(value, out newInt))
                {
                    if (newInt < 2)
                    {
                        newInt = 2;
                    }
                    else if (newInt > 100)
                    {
                        newInt = 100;
                    }
                    else if (newInt % 2 != 0) // even number expected
                    {
                        newInt = 2;
                    }

                    Settings.Default.HorizontalLeds = newInt;
                    Settings.Default.Save();
                    ServiceLocator.GetService<DirectXScreenService>().HorizontalLedCount = newInt;
                }

                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the total number of vertical Leds (left and right)
        /// </summary>
        public string VerticalLeds
        {
            get { return Settings.Default.VerticalLeds.ToString(); }
            set
            {
                int newInt;
                if (int.TryParse(value, out newInt))
                {
                    if (newInt < 2)
                    {
                        newInt = 2;
                    }
                    else if (newInt > 100)
                    {
                        newInt = 100;
                    }
                    else if (newInt % 2 != 0) // even number expected
                    {
                        newInt = 2;
                    }

                    Settings.Default.VerticalLeds = newInt;
                    Settings.Default.Save();
                    ServiceLocator.GetService<DirectXScreenService>().VerticalLedCount = newInt;
                }

                RaisePropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="SettingsWindowViewModel"/> class.
        /// </summary>
        internal SettingsWindowViewModel()
        {
            InitializeCommands();

            ServiceLocator.GetService<SerialService>().ConnectionStateChanged += SettingsWindowViewModel_ConnectionStateChanged;
        }

        #endregion

        #region Commands

        /// <summary>
        /// Initialize the commands of the View Model
        /// </summary>
        private void InitializeCommands()
        {
            CloseButtonCommand = new RelayCommand<BlurredWindow>(ExecuteCloseButtonCommand);
        }

        #region CloseButton

        /// <summary>
        /// Gets or sets a <see cref="RelayCommand"/> executed when click on the close button
        /// </summary>
        public RelayCommand<BlurredWindow> CloseButtonCommand { get; private set; }

        private void ExecuteCloseButtonCommand(BlurredWindow window)
        {
            window.Close();
        }

        #endregion

        #endregion

        #region Handled Methods

        private void SettingsWindowViewModel_ConnectionStateChanged(object sender, System.EventArgs e)
        {
            RaisePropertyChanged(nameof(LightZDeviceStatus));
        }

        #endregion
    }
}
