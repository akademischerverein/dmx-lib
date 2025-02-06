using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using DmxLib.Util;

namespace DmxLib
{
    public class Device : IDevice
    {
        internal DeviceGroup Group;
        internal readonly IHandler[] Handlers;
        internal readonly Dictionary<DeviceProperty, object> Properties;
        internal Universe Universe;

        public Device(string name, uint width, uint channel, IHandler[] handlers)
        {
            if (width < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than 0");
            }

            if (channel < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(channel), "Channel must be greater than 0");
            }

            Name = name;
            Width = width;
            Channel = channel;
            Properties = new Dictionary<DeviceProperty, object>();
            Handlers = handlers;

            Properties = handlers.SelectMany(h => h.SupportedProperties).Distinct().ToDictionary(p => p, p => p.DefaultValue);
        }

        public readonly string Name;
        public readonly uint Width;
        public readonly uint Channel;
        public ReadOnlyCollection<DeviceProperty> SupportedProperties => new ReadOnlyCollection<DeviceProperty>(Properties.Keys.ToList());

        public object Get(DeviceProperty property)
        {
            return Properties[property];
        }

        public void Set(DeviceProperty property, object value)
        {
            if (!property.Type.IsInstanceOfType(value))
            {
                throw new ArgumentException("Property value of illegal type", nameof(value));
            }

            if (!Properties.ContainsKey(property))
            {
                throw new ArgumentException("Property not supported for this device", nameof(property));
            }

            if (Handlers.Any(h => h.SupportedProperties.Contains(property) && !h.IsValidValue(property, value)))
            {
                throw new ArgumentException("Property value not accepted by handler", nameof(value));
            }

            Properties[property] = value;
            Universe.ApplyProperties(this, Properties);
        }

        public IEnumerable<object> ValidValues(DeviceProperty property)
        {
            var supported = new HashSet<object>();
            foreach (var h in Handlers)
            {
                if (h.SupportedProperties.Contains(property))
                {
                    supported.UnionWith(h.ValidValues(property));
                }
            }

            return supported;
        }

        public bool IsValidValue(DeviceProperty property, object o)
        {
            return Handlers.Where(h => h.SupportedProperties.Contains(property)).Select(h => h.IsValidValue(property, o))
                .Any();
        }
    }
}