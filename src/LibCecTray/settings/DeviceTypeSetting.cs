// DeviceTypeSetting.cs
using CecSharp;
using System;
using System.Windows.Forms;

namespace ModernCECTray.Settings
{
    public class DeviceTypeSetting : ModernCECSetting<CecDeviceType>
    {
        private readonly CecDeviceTypeList _allowedTypes;

        public DeviceTypeSetting(string key, string displayName,
            CecDeviceType defaultValue, CecDeviceTypeList allowedTypes)
            : base(key, displayName, defaultValue)
        {
            _allowedTypes = allowedTypes;
        }

        public override void BindToControl(Control control)
        {
            base.BindToControl(control);
            if (control is ComboBox comboBox)
            {
                PopulateComboBox(comboBox);
                comboBox.SelectedIndexChanged += (s, e) =>
                {
                    if (comboBox.SelectedItem is CecDeviceType deviceType)
                    {
                        SetUserValue(deviceType);
                    }
                };
            }
        }

        private void PopulateComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (var type in _allowedTypes.Types)
            {
                if (type != CecDeviceType.Reserved)
                {
                    comboBox.Items.Add(type);
                }
            }
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
                comboBox.SelectedItem = Value;
            }
        }

        protected override object ConvertToRegistry(CecDeviceType value)
        {
            return (int)value;
        }

        protected override CecDeviceType ConvertFromRegistry(object value)
        {
            if (value is int intValue)
            {
                return (CecDeviceType)intValue;
            }
            return CecDeviceType.Reserved;
        }
    }
}