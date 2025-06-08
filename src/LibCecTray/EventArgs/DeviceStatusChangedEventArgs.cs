// EventArgs/DeviceStatusChangedEventArgs.cs
using CecSharp;

namespace ModernCECTray.EventArgs
{
    public class DeviceStatusChangedEventArgs : System.EventArgs
    {
        public CecLogicalAddress Address { get; }
        public CecPowerStatus PowerStatus { get; }

        public DeviceStatusChangedEventArgs(CecLogicalAddress address, CecPowerStatus status)
        {
            Address = address;
            PowerStatus = status;
        }
    }
}