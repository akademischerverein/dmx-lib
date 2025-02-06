using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DmxLib
{
    public interface IHandler
    {
        void Update(Device device, ReadOnlyDictionary<DeviceProperty, object> properties, byte[] values);
        bool IsValidValue(DeviceProperty property, object o);
        IEnumerable<object> ValidValues(DeviceProperty property);
        ReadOnlyCollection<DeviceProperty> SupportedProperties { get; }
    }
}