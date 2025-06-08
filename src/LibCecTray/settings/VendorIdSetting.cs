// VendorIdSetting.cs
using CecSharp;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ModernCECTray.Settings
{
    public class VendorIdSetting : ModernCECSetting<CecVendorId>
    {
        private static readonly Dictionary<CecVendorId, string> VendorNames =
            new Dictionary<CecVendorId, string>
        {
            { CecVendorId.Unknown, "Unknown" },
            { CecVendorId.Akai, "Akai" },
            { CecVendorId.Aoc, "AOC" },
            { CecVendorId.Benq, "BenQ" },
            { CecVendorId.Daewoo, "Daewoo" },
            { CecVendorId.Grundig, "Grundig" },
            { CecVendorId.Harman, "Harman/Kardon" },
            { CecVendorId.LG, "LG" },
            { CecVendorId.Loewe, "Loewe" },
            { CecVendorId.Marantz, "Marantz" },
            { CecVendorId.Medion, "Medion" },
            { CecVendorId.Onkyo, "Onkyo" },
            { CecVendorId.Oppo, "Oppo" },
            { CecVendorId.Panasonic, "Panasonic" },
            { CecVendorId.Philips, "Philips" },
            { CecVendorId.Pioneer, "Pioneer" },
            { CecVendorId.Pulse8, "Pulse-Eight" },
            { CecVendorId.Samsung, "Samsung" },
            { CecVendorId.Sharp, "Sharp" },
            { CecVendorId.Sony, "Sony" },
            { CecVendorId.Toshiba, "Toshiba" },
            { CecVendorId.Vestel, "Vestel" },
            { CecVendorId.ViewSonic, "ViewSonic" },
            { CecVendorId.Vizio, "Vizio" },
            { CecVendorId.Yamaha, "Yamaha" }
        };

        private readonly bool _allowAutodetect;
        private ComboBox _comboBox;

        public VendorIdSetting(string key, string displayName, CecVendorId defaultValue, bool allowAutodetect = true)
            : base(key, displayName, defaultValue)
        {
            _allowAutodetect = allowAutodetect;
        }

        public override void BindToControl(Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (!(control is ComboBox))
                throw new ArgumentException("Control must be a ComboBox", nameof(control));

            base.BindToControl(control);
            _comboBox = (ComboBox)control;

            try
            {
                PopulateComboBox();
                _comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                UpdateControl();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error binding control: {ex.Message}");
                throw;
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_comboBox.SelectedItem is ComboBoxItem item)
            {
                try
                {
                    SetUserValue(item.VendorId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error setting vendor ID: {ex.Message}");
                    MessageBox.Show("Failed to set vendor ID.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PopulateComboBox()
        {
            if (_comboBox == null) return;

            try
            {
                _comboBox.Items.Clear();
                _comboBox.BeginUpdate();

                if (_allowAutodetect)
                {
                    _comboBox.Items.Add(new ComboBoxItem(CecVendorId.Unknown, "Auto-detect"));
                }

                foreach (var vendor in VendorNames)
                {
                    if (vendor.Key != CecVendorId.Unknown) // Skip Unknown in the main list
                    {
                        _comboBox.Items.Add(new ComboBoxItem(vendor.Key, vendor.Value));
                    }
                }

                _comboBox.EndUpdate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error populating ComboBox: {ex.Message}");
                _comboBox.EndUpdate();
                throw;
            }
        }

        protected override void UpdateControl()
        {
            if (_comboBox == null) return;

            if (_comboBox.InvokeRequired)
            {
                try
                {
                    _comboBox.Invoke(new Action(UpdateControl));
                    return;
                }
                catch (ObjectDisposedException)
                {
                    // Control was disposed, nothing to update
                    return;
                }
            }

            try
            {
                foreach (ComboBoxItem item in _comboBox.Items)
                {
                    if (item.VendorId == Value)
                    {
                        _comboBox.SelectedItem = item;
                        return;
                    }
                }

                // If we didn't find a match, select Auto-detect if allowed, otherwise first item
                if (_allowAutodetect)
                {
                    _comboBox.SelectedIndex = 0; // Auto-detect
                }
                else if (_comboBox.Items.Count > 0)
                {
                    _comboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating control: {ex.Message}");
                // Don't throw here as this is called from multiple contexts
            }
        }

        protected override object ConvertToRegistry(CecVendorId value)
        {
            return (int)value;
        }

        protected override CecVendorId ConvertFromRegistry(object value)
        {
            try
            {
                if (value is int intValue)
                {
                    // Verify this is a valid vendor ID
                    if (VendorNames.ContainsKey((CecVendorId)intValue))
                    {
                        return (CecVendorId)intValue;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting from registry: {ex.Message}");
            }

            return CecVendorId.Unknown;
        }

        public static string GetVendorName(CecVendorId vendorId)
        {
            return VendorNames.TryGetValue(vendorId, out string name) ? name : "Unknown Vendor";
        }

        private class ComboBoxItem
        {
            public CecVendorId VendorId { get; }
            private readonly string _displayName;

            public ComboBoxItem(CecVendorId vendorId, string displayName)
            {
                VendorId = vendorId;
                _displayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            }

            public override string ToString()
            {
                return _displayName;
            }

            public override bool Equals(object obj)
            {
                if (obj is ComboBoxItem other)
                {
                    return VendorId == other.VendorId;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return VendorId.GetHashCode();
            }
        }
    }
}