// ModernCECTray.cs
using System;
using System.Windows.Forms;
using ModernCECTray.Controller;
using ModernCECTray.Settings;
using CecSharp;

namespace ModernCECTray.UI
{
    public partial class ModernCECTray : Form
    {
        private readonly ModernCECController _controller;
        private readonly SettingsCollection _settings;
        private readonly NotifyIcon _trayIcon;

        public ModernCECTray()
        {
            InitializeComponent();

            _settings = InitializeSettings();
            _controller = new ModernCECController(_settings);
            _trayIcon = InitializeTrayIcon();

            BindSettings();
            InitializeController();
        }

        private SettingsCollection InitializeSettings()
        {
            var settings = new SettingsCollection();

            // Initialize all settings
            settings.RegisterSetting(new BoolSetting("global_start_hidden",
                "Start Minimized", false));

            settings.RegisterSetting(new BoolSetting("global_override_physical_address",
                "Override Physical Address", false));

            settings.RegisterSetting(new NumericSetting("global_hdmi_port",
                "HDMI Port", 1, 1, 15));

            // Device type setting with allowed types
            var allowedTypes = new CecDeviceTypeList();
            allowedTypes.Types[0] = CecDeviceType.RecordingDevice;
            allowedTypes.Types[1] = CecDeviceType.PlaybackDevice;
            settings.RegisterSetting(new DeviceTypeSetting("global_device_type",
                "Device Type", CecDeviceType.RecordingDevice, allowedTypes));

            // Add other settings as needed

            return settings;
        }

        private void BindSettings()
        {
            // Bind controls to settings
            // Note: Control names are examples - adjust to match your actual control names
            _settings.GetSetting<BoolSetting>("global_start_hidden")
                ?.BindToControl(checkBoxStartMinimized);

            _settings.GetSetting<BoolSetting>("global_override_physical_address")
                ?.BindToControl(checkBoxOverrideAddress);

            _settings.GetSetting<NumericSetting>("global_hdmi_port")
                ?.BindToControl(comboBoxHDMIPort);

            _settings.GetSetting<DeviceTypeSetting>("global_device_type")
                ?.BindToControl(comboBoxDeviceType);
        }

        private NotifyIcon InitializeTrayIcon()
        {
            var icon = new NotifyIcon
            {
                Icon = Properties.Resources.TrayIcon, // Ensure you have this resource
                Visible = true,
                ContextMenuStrip = CreateTrayContextMenu()
            };

            icon.MouseClick += TrayIcon_MouseClick;
            return icon;
        }

        private ContextMenuStrip CreateTrayContextMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("Show/Hide", null, (s, e) => ToggleVisibility());
            menu.Items.Add("Exit", null, (s, e) => Application.Exit());
            return menu;
        }

        private void InitializeController()
        {
            if (!_controller.Initialize())
            {
                MessageBox.Show("Failed to initialize CEC controller", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleVisibility()
        {
            if (Visible)
            {
                Hide();
            }
            else
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            }
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleVisibility();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                _controller.Close();
                _trayIcon.Dispose();
            }
            base.OnFormClosing(e);
        }

        // Add other necessary form event handlers and methods
    }
}