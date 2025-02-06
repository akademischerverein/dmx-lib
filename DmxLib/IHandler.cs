using System;
using System.Collections.ObjectModel;

namespace DmxLib
{
    public interface IHandler
    {
        void Update(Device device, ReadOnlyDictionary<DeviceProperty, Object> properties, byte[] values);
        ReadOnlyCollection<DeviceProperty> SupportedProperties { get; }
        ReadOnlyCollection<string> SupportedGobos { get; }
    }
}