// ModernCECController.cs
using CecSharp;
using ModernCECTray.Controller;
using ModernCECTray.Settings;
using System;
using System.Windows.Forms;

namespace ModernCECTray.Controller
{
    public class ModernCECController
    {
        private LibCecSharp _libCec;
        private readonly SettingsCollection _settings;
        private bool _suppressUpdates;

        public ModernCECController(SettingsCollection settings)
        {
            _settings = settings;
        }

        public bool Initialize()
        {
            try
            {
                var callbacks = new CecCallbackMethods();
                var config = CreateDefaultConfiguration();

                _libCec = new LibCecSharp(callbacks, config);
                _libCec.EnableCallbacks();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Close()
        {
            _suppressUpdates = true;
            _libCec?.Close();
            _libCec = null;
        }

        public bool SendCommand(string rawCommand)
        {
            if (_libCec == null || _suppressUpdates)
                return false;

            try
            {
                // Parse the raw command format "xx:xx:xx:xx"
                var parts = rawCommand.Split(':');
                if (parts.Length != 4)
                    return false;

                var command = new CecCommand();
                // Set command parameters based on the raw input
                // This is a simplified example - actual implementation would need proper parsing
                command.Parameters.PushBack(byte.Parse(parts[0], System.Globalization.NumberStyles.HexNumber));
                command.Parameters.PushBack(byte.Parse(parts[1], System.Globalization.NumberStyles.HexNumber));
                command.Parameters.PushBack(byte.Parse(parts[2], System.Globalization.NumberStyles.HexNumber));
                command.Parameters.PushBack(byte.Parse(parts[3], System.Globalization.NumberStyles.HexNumber));

                return _libCec.Transmit(command);
            }
            catch
            {
                return false;
            }
        }

        public void SendStandby(CecLogicalAddress address)
        {
            if (_libCec != null && !_suppressUpdates)
            {
                _libCec.StandbyDevices(address);
            }
        }

        public void ActivateSource()
        {
            if (_libCec != null && !_suppressUpdates)
            {
                _libCec.SetActiveSource(CecDeviceType.PlaybackDevice);
            }
        }

        private LibCECConfiguration CreateDefaultConfiguration()
        {
            var config = new LibCECConfiguration
            {
                DeviceName = "CECTray",
                ClientVersion = LibCECConfiguration.CurrentVersion,
                ServerVersion = 0, // Will be set by the adapter
                DeviceTypes = new CecDeviceTypeList()
            };

            // Set default device type
            config.DeviceTypes.Types[0] = CecDeviceType.RecordingDevice;

            return config;
        }
    }
}