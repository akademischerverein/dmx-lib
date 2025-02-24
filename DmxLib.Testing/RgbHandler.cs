using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DmxLib.Util;

namespace DmxLib.Testing
{
    public class RgbHandler : IHandler
    {
        private readonly string _layout;

        public RgbHandler(string layout)
        {
            _layout = layout;
        }

        public void Update(Device device, ReadOnlyDictionary<DeviceProperty, object> properties, Dictionary<uint, byte> values)
        {
            var color = (Color) properties[Program.PropertyColor];
            var dimming = (double) properties[Program.PropertyDimming];

            if (!_layout.Contains('d', StringComparison.Ordinal))
            {
                color = Color.FromRGB(color.R * dimming, color.G * dimming, color.B * dimming);
            }

            for (var i = 0; i < _layout.Length; i++)
            {
                switch (_layout[i])
                {
                    case 'r':
                        values[device.Channels[i]] = (byte) (color.R * 255);
                        break;
                    case 'g':
                        values[device.Channels[i]] = (byte) (color.G * 255);
                        break;
                    case 'b':
                        values[device.Channels[i]] = (byte) (color.B * 255);
                        break;
                    case 'd':
                        values[device.Channels[i]] = (byte) (dimming * 255);
                        break;
                }
            }
        }

        public bool IsValidValue(DeviceProperty property, object value)
        {
            return value is Color || value is double;
        }

        public ReadOnlyCollection<object> ValidValues(DeviceProperty property)
        {
            throw new System.ArgumentException();
        }

        public ReadOnlyCollection<DeviceProperty> SupportedProperties => new ReadOnlyCollection<DeviceProperty>(new DeviceProperty[]{Program.PropertyColor, Program.PropertyDimming});
    }
}
