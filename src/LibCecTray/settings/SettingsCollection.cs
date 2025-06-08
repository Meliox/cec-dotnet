// SettingsCollection.cs
using System.Collections.Generic;

namespace ModernCECTray.Settings
{
    public class SettingsCollection
    {
        private readonly Dictionary<string, ModernCECSetting<object>> _settings =
            new Dictionary<string, ModernCECSetting<object>>();

        public T GetSetting<T>(string key) where T : ModernCECSetting<object>
        {
            return _settings.TryGetValue(key, out var setting) ? (T)setting : null;
        }

        public void RegisterSetting<T>(ModernCECSetting<T> setting)
        {
            _settings[setting.Key] = setting as ModernCECSetting<object>;
        }

        public void UpdateFromDevice<T>(string key, T value)
        {
            if (_settings.TryGetValue(key, out var setting))
            {
                (setting as ModernCECSetting<T>)?.UpdateFromDevice(value);
            }
        }
    }
}