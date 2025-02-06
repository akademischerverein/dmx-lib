using System.Collections.ObjectModel;

namespace DmxLib
{
    public interface IDevice
    {
        T GetProperty<T>(DeviceProperty property);
        void SetProperty<T>(DeviceProperty property, T value);
        ReadOnlyCollection<DeviceProperty> SupportedProperties { get; }
        ReadOnlyCollection<string> SupportedGobos { get; }
    }
}