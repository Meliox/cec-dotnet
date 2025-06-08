// BoolSetting.cs
using System;
using System.Windows.Forms;

namespace ModernCECTray.Settings
{
    public class BoolSetting : ModernCECSetting<bool>
    {
        public BoolSetting(string key, string displayName, bool defaultValue)
            : base(key, displayName, defaultValue)
        {
        }

        public override void BindToControl(Control control)
        {
            base.BindToControl(control);
            if (control is CheckBox checkBox)
            {
                checkBox.CheckedChanged += (s, e) => SetUserValue(checkBox.Checked);
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

            if (AssociatedControl is CheckBox checkBox)
            {
                checkBox.Checked = Value;
            }
        }

        protected override object ConvertToRegistry(bool value)
        {
            return value ? 1 : 0;
        }

        protected override bool ConvertFromRegistry(object value)
        {
            return value is int intValue && intValue == 1;
        }
    }
}