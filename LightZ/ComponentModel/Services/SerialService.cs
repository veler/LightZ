using LightZ.ComponentModel.Core;
using LightZ.ComponentModel.Services.Base;
using LightZ.Models;
using LightZ.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static LightZ.ComponentModel.Core.Delegates;

namespace LightZ.ComponentModel.Services
{
    /// <summary>
    /// Provides a set of functions designed to interact with a Bluetooth or USB device.
    /// </summary>
    internal sealed class SerialService : IService
    {
        #region Fields

        private SerialPort _serialPort;
        private SerialDevice _currentSerialDevice;
        private bool _doNotConnectCurrentSerialDevice;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current Bluetooth or USB device to connect to.
        /// </summary>
        internal SerialDevice CurrentSerialDevice
        {
            get
            {
                return _currentSerialDevice;
            }
            set
            {
                _currentSerialDevice = value;

                Disconnect();
                if (_currentSerialDevice != null && !_doNotConnectCurrentSerialDevice)
                {
                    Connect(_currentSerialDevice.ComPortNumber);
                }
            }
        }

        /// <summary>
        /// Returns True if it is currently connected to the device
        /// </summary>
        internal bool Connected => _serialPort.IsOpen;

        #endregion

        #region Events

        /// <summary>
        /// Event raised when data from the Bluetooth or USB device is received.
        /// </summary>
        internal event SerialDataReceivedEventArgsEventHandler DataReceived;

        /// <summary>
        /// Event raised when the connection state changed.
        /// </summary>
        internal event EventHandler ConnectionStateChanged;

        #endregion

        #region Handled Methods

        /// <summary>
        /// Method called when the serial port receives data.
        /// </summary>
        /// <param name="sender">The serial port</param>
        /// <param name="e">The data</param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var data = _serialPort.ReadLine().ToUpper();

                if (string.IsNullOrWhiteSpace(data))
                {
                    return;
                }

                Task.Run(() =>
                {
                    DataReceived?.Invoke(this, new Events.SerialDataReceivedEventArgs(data));
                });
            }
            catch { }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public void Initialize()
        {
            _serialPort = new SerialPort();

            if (!string.IsNullOrWhiteSpace(Settings.Default.SerialDevice))
            {
                _doNotConnectCurrentSerialDevice = true;
                CurrentSerialDevice = GetSerialDevices().FirstOrDefault(d => d.DeviceName == Settings.Default.SerialDevice);
                _doNotConnectCurrentSerialDevice = false;
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            Disconnect();
        }

        /// <summary>
        /// Connect to the device.
        /// </summary>
        /// <param name="comPortNumber">COM port number. i.e : COM1</param>
        internal void Connect(string comPortNumber)
        {
            Requires.NotNullOrWhiteSpace(comPortNumber, nameof(comPortNumber));

            var delayer = new Delayer<object>(TimeSpan.FromMilliseconds(100), true);
            delayer.Action += (sender, e) =>
            {
                _serialPort.PortName = comPortNumber;
                _serialPort.BaudRate = Consts.BandwidthRate;
                _serialPort.WriteTimeout = Consts.WriteTimeout;

                var i = 0;
                while (i < 10 && !_serialPort.IsOpen)
                {
                    try
                    {
                        _serialPort.Open();
                    }
                    catch (Exception ex)
                    {
                        _serialPort.Close();
                        Debug.WriteLine($"Unable to connect to Bluetooth or USB (tentative {i}/10) : {ex.Message}");
                        Task.Delay(1000).Wait();
                    }
                }

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
                    _serialPort = new SerialPort();
                    CurrentSerialDevice = null;
                    Settings.Default.SerialDevice = null;
                    Settings.Default.Save();
                    Debug.WriteLine("Unable to connect to Bluetooth or USB : 10 try all failed");
                }
                else
                {
                    _serialPort.DataReceived += SerialPort_DataReceived;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ConnectionStateChanged?.Invoke(this, new EventArgs());
                }, DispatcherPriority.ApplicationIdle);
            };

            delayer.ResetAndTick();
        }

        /// <summary>
        /// Disconnect from the device.
        /// </summary>
        internal void Disconnect()
        {
            if (Connected)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.Close();
                ConnectionStateChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Send a data to the device by Bluetooth or USB
        /// </summary>
        /// <param name="data">The data to send</param>
        internal void Send(byte[] data)
        {
            if (!Connected)
            {
                return;
            }

            try
            {
                _serialPort.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Disconnect();
                if (_currentSerialDevice != null)
                {
                    Connect(_currentSerialDevice.ComPortNumber);
                }
            }
        }

        /// <summary>
        /// Retrieves a list of <see cref="SerialDevice"/> that corresponds to the list of compatible Bluetooth or USB devices that we can interact with via <see cref="SerialPort"/> (COM port).
        /// </summary>
        /// <returns>A list of <see cref="SerialDevice"/>.</returns>
        internal List<SerialDevice> GetSerialDevices()
        {
            var result = new List<SerialDevice>();
            var regexPortName = new Regex(@"(COM\d+)");
            var searchSerial = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");

            foreach (var obj in searchSerial.Get())
            {
                var name = obj["Name"] as string;
                var classGuid = obj["ClassGuid"] as string;
                var deviceID = obj["DeviceID"] as string;

                if (classGuid != null && deviceID != null)
                {
                    if (String.Equals(classGuid, Consts.StandardSerialPortSignature, StringComparison.InvariantCulture))
                    {
                        var tokens = deviceID.Split('&');

                        if (tokens.Length >= 4)
                        {
                            var addressToken = tokens[4].Split('_');
                            var serialAddress = addressToken[0];

                            var match = regexPortName.Match(name);
                            var comPortNumber = "";
                            if (match.Success)
                            {
                                comPortNumber = match.Groups[1].ToString();
                            }

                            if (Convert.ToUInt64(serialAddress, 16) > 0)
                            {
                                var deviceName = GetBluetoothRegistryName(serialAddress);
                                if (string.IsNullOrEmpty(deviceName))
                                {
                                    deviceName = name.Replace($" ({comPortNumber})", string.Empty);
                                }
                                result.Add(new SerialDevice(comPortNumber, deviceName));
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves the friendly name of a Bluetooth device from its addess in the registry.
        /// </summary>
        /// <param name="address">The address of the Bluetooth device.</param>
        /// <returns>The friendly name of the device.</returns>
        private string GetBluetoothRegistryName(string address)
        {
            var deviceName = "";
            var registryPath = @"SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices";
            var devicePath = $"{registryPath}\\{address}";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(devicePath))
            {
                if (key != null)
                {
                    var value = key.GetValue("Name");
                    var raw = value as byte[];

                    if (raw != null)
                    {
                        deviceName = Encoding.ASCII.GetString(raw).Replace("\0", string.Empty); ;
                    }
                }
            }

            return deviceName;
        }

        #endregion
    }
}
