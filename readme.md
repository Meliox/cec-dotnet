# CEC .NET Enhanced Fork

This is a fork of a fork of [Pulse-Eight/cec-dotnet](https://github.com/Pulse-Eight/cec-dotnet), the official .NET bindings for libCEC. It builds on the original work with additional functionality for advanced use cases and testing.

## 🔧 Key Enhancements

- **Device Name Assignment**  
  Allows setting a custom logical device name for clearer identification on the HDMI bus.

- **Kodi Detection Disable Option**  
  Enables this library to run concurrently with Kodi.  
  > ⚠️ *Note:* You must disable CEC within Kodi to prevent conflicts.

- **.NET Framework 4.8 Support**  
  Updated for modern Windows systems with .NET 4.8 compatibility.

- **Raw CEC Byte Command Interface**  
  Provides a way to send raw CEC messages for advanced users and debugging.
