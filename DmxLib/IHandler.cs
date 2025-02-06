using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DmxLib
{
    public interface IHandler
    {
        void Update(Device device, ReadOnlyDictionary<DeviceProperty, object> properties, Dictionary<uint, byte> values);
        bool IsValidValue(DeviceProperty property, object o);
        ReadOnlyCollection<object> ValidValues(DeviceProperty property);
        ReadOnlyCollection<DeviceProperty> SupportedProperties { get; }
    }
}