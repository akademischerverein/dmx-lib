using System;
using System.Collections.ObjectModel;

namespace DmxLib
{
    public interface IDevice
    {
        object Get(DeviceProperty property);
        void Set(DeviceProperty property, object value);
        ReadOnlyCollection<object> ValidValues(DeviceProperty property);
        ReadOnlyCollection<DeviceProperty> SupportedProperties { get; }
    }
}