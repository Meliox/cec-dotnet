// ModernCECSetting.cs
using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ModernCECTray.Settings
{
    public abstract class ModernCECSetting<T>
    {
        private T _value;
        private bool _isOverridden = false;
        private const string RegistryBase = @"Software\Pulse-Eight\libCECTray";

        public string Key { get; }
        public string DisplayName { get; }
        public Control AssociatedControl { get; private set; }

        protected ModernCECSetting(string key, string displayName, T defaultValue)
        {
            Key = key;
            DisplayName = displayName;
            _value = defaultValue;
            LoadFromRegistry();
        }

        public T Value
        {
            get => _value;
            protected set
            {
                var oldValue = _value;
                _value = value;
                if (!Equals(oldValue, value))
                {
                    SaveToRegistry();
                    UpdateControl();
                }
            }
        }

        public bool IsOverridden
        {
            get => _isOverridden;
            private set
            {
                _isOverridden = value;
                SaveOverrideState();
            }
        }

        public void SetUserValue(T value)
        {
            IsOverridden = true;
            Value = value;
        }

        public void UpdateFromDevice(T value)
        {
            if (!IsOverridden)
            {
                Value = value;
            }
        }

        public virtual void BindToControl(Control control)
        {
            AssociatedControl = control;
            UpdateControl();
        }

        protected abstract void UpdateControl();

        private void LoadFromRegistry()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryBase))
            {
                if (key != null)
                {
                    var value = key.GetValue(Key);
                    if (value != null)
                    {
                        Value = ConvertFromRegistry(value);
                    }

                    var overrideValue = key.GetValue($"{Key}_Override");
                    if (overrideValue is int intValue)
                    {
                        _isOverridden = intValue == 1;
                    }
                }
            }
        }

        private void SaveToRegistry()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(RegistryBase))
            {
                key.SetValue(Key, ConvertToRegistry(Value));
            }
        }

        private void SaveOverrideState()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(RegistryBase))
            {
                key.SetValue($"{Key}_Override", IsOverridden ? 1 : 0);
            }
        }

        protected abstract object ConvertToRegistry(T value);
        protected abstract T ConvertFromRegistry(object value);
    }
}