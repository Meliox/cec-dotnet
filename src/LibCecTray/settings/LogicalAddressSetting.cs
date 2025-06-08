// LogicalAddressSetting.cs
using CecSharp;
using System;
using System.Windows.Forms;

namespace ModernCECTray.Settings
{
    public class LogicalAddressSetting : ModernCECSetting<CecLogicalAddress>
    {
        private readonly CecLogicalAddresses _allowedAddresses;
        private readonly string[] _vendorNames;

        public LogicalAddressSetting(string key, string displayName,
            CecLogicalAddress defaultValue, CecLogicalAddresses allowedAddresses,
            string[] vendorNames)
            : base(key, displayName, defaultValue)
        {
            _allowedAddresses = allowedAddresses;
            _vendorNames = vendorNames;
        }

        public override void BindToControl(Control control)
        {
            base.BindToControl(control);
            if (control is ComboBox comboBox)
            {
                PopulateComboBox(comboBox);
                comboBox.SelectedIndexChanged += (s, e) =>
                {
                    if (comboBox.SelectedItem is ComboBoxItem item)
                    {
                        SetUserValue(item.Address);
                    }
                };
            }
        }

        private void PopulateComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            for (int i = 0; i < 16; i++)
            {
                if (_allowedAddresses.IsSet((CecLogicalAddress)i))
                {
                    var address = (CecLogicalAddress)i;
                    var name = FormatDeviceName(address, i);
                    comboBox.Items.Add(new ComboBoxItem(address, name));
                }
            }
        }

        private string FormatDeviceName(CecLogicalAddress address, int index)
        {
            var vendorName = (_vendorNames != null && index < _vendorNames.Length)
                ? _vendorNames[index]
                : string.Empty;

            return string.IsNullOrEmpty(vendorName)
                ? address.ToString()
                : $"{vendorName} ({address})";
        }

        protected override void UpdateControl()
        {
            if (AssociatedControl == null) return;

            if (AssociatedControl.InvokeRequired)
            {
                AssociatedControl.Invoke(new Action(UpdateControl));
                return;
            }

            if (AssociatedControl is ComboBox comboBox)
            {
                foreach (ComboBoxItem item in comboBox.Items)
                {
                    if (item.Address == Value)
                    {
                        comboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        protected override object ConvertToRegistry(CecLogicalAddress value)
        {
            return (int)value;
        }

        protected override CecLogicalAddress ConvertFromRegistry(object value)
        {
            if (value is int intValue && intValue >= 0 && intValue <= 15)
            {
                return (CecLogicalAddress)intValue;
            }
            return CecLogicalAddress.Unknown;
        }

        private class ComboBoxItem
        {
            public CecLogicalAddress Address { get; }
            private string DisplayName { get; }

            public ComboBoxItem(CecLogicalAddress address, string displayName)
            {
                Address = address;
                DisplayName = displayName;
            }

            public override string ToString()
            {
                return DisplayName;
            }
        }
    }
}