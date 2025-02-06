using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DmxLib.Util;

namespace DmxLib
{
    public class Device : IDevice
    {
        internal DeviceGroup group;
        internal IHandler[] handlers;
        private readonly Dictionary<DeviceProperty, object> _properties;
        internal Universe universe;

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
            _properties = new Dictionary<DeviceProperty, object>();
            this.handlers = handlers;

            foreach (var h in handlers)
            {
                foreach (var prop in h.SupportedProperties)
                {
                    if (!_properties.ContainsKey(prop))
                    {
                        _properties[prop] = prop.DefaultValue;
                    }
                }
            }
        }

        public readonly string Name;
        public readonly uint Width;
        public readonly uint Channel;
        public ReadOnlyCollection<DeviceProperty> SupportedProperties => new ReadOnlyCollection<DeviceProperty>(_properties.Keys.ToList());

        public object Get(DeviceProperty property)
        {
            return _properties[property];
        }

        public void Set(DeviceProperty property, object value)
        {
            if (!property.Type.IsInstanceOfType(value))
            {
                throw new ArgumentException("Property value of illegal type", nameof(value));
            }

            if (!_properties.ContainsKey(property))
            {
                throw new ArgumentException("Property not supported for this device", nameof(property));
            }

            foreach (var h in handlers)
            {
                if (!h.IsValidValue(property, value))
                {
                    throw new ArgumentException("Property value not accepted by handler", nameof(value));
                }
            }

            _properties[property] = value;
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<object> ValidValues(DeviceProperty property)
        {
            var supported = new HashSet<object>();
            foreach (var h in handlers)
            {
                supported.UnionWith(h.ValidValues(property));
            }

            return new ReadOnlyCollection<object>(supported.ToList());
        }
    }
}