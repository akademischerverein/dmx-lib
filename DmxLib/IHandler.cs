using System;
using System.Collections.ObjectModel;

namespace DmxLib
{
    public interface IHandler
    {
        void Update(Device device, ReadOnlyDictionary<DeviceProperty, object> properties, byte[] values);
        bool IsValidValue(DeviceProperty property, object o);
        ReadOnlyCollection<object> ValidValues(DeviceProperty property);
        ReadOnlyCollection<DeviceProperty> SupportedProperties { get; }
    }
}