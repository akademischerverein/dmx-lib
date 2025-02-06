using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DmxLib
{
    public interface IDevice
    {
        object Get(DeviceProperty property);
        void Set(DeviceProperty property, object value);
        IEnumerable<object> ValidValues(DeviceProperty property);
        bool IsValidValue(DeviceProperty property, object o);
        ReadOnlyCollection<DeviceProperty> SupportedProperties { get; }
    }
}