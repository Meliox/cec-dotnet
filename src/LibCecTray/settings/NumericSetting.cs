// NumericSetting.cs
using System;
using System.Windows.Forms;

namespace ModernCECTray.Settings
{
    public class NumericSetting : ModernCECSetting<int>
    {
        private readonly int _minimum;
        private readonly int _maximum;

        public NumericSetting(string key, string displayName, int defaultValue,
            int minimum = int.MinValue, int maximum = int.MaxValue)
            : base(key, displayName, defaultValue)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        public override void BindToControl(Control control)
        {
            base.BindToControl(control);
            if (control is ComboBox comboBox)
            {
                comboBox.SelectedIndexChanged += (s, e) =>
                {
                    if (comboBox.SelectedItem != null &&
                        int.TryParse(comboBox.SelectedItem.ToString(), out int value))
                    {
                        SetUserValue(value);
                    }
                };
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
                comboBox.SelectedItem = Value.ToString();
            }
        }

        protected override object ConvertToRegistry(int value)
        {
            return value;
        }

        protected override int ConvertFromRegistry(object value)
        {
            if (value is int intValue)
            {
                if (intValue < _minimum) return _minimum;
                if (intValue > _maximum) return _maximum;
                return intValue;
            }
            return _minimum;
        }
    }
}