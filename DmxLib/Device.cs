using System;
using System.Collections.Generic;
using DmxLib.Util;

namespace DmxLib
{
    public class Device : IDevice
    {
        internal DeviceGroup group;
        internal IHandler[] handlers;
        private Dictionary<DeviceProperty, Object> properties;
        private HashSet<string> gobos;
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
            properties = new Dictionary<DeviceProperty, object>();
            gobos = new HashSet<string>();
            this.handlers = handlers;

            foreach (var h in handlers)
            {
                foreach (var prop in h.SupportedProperties)
                {
                    if (!properties.ContainsKey(prop))
                    {
                        switch (prop)
                        {
                            case DeviceProperty.Color:
                                properties[DeviceProperty.Color] = Color.FromRGB(0, 0, 0);
                                break;
                            case DeviceProperty.Dimming:
                                properties[DeviceProperty.Dimming] = (double) 0.0;
                                break;
                            case DeviceProperty.Gobo:
                                properties[DeviceProperty.Gobo] = null;
                                break;
                            case DeviceProperty.Shutter:
                                properties[DeviceProperty.Shutter] = false;
                                break;
                            case DeviceProperty.PosXY:
                                properties[DeviceProperty.PosXY] = new Vector2f(0, 0);
                                break;
                        }
                    }
                }

                if (h.SupportedProperties.Contains(DeviceProperty.Gobo))
                {
                    gobos.UnionWith(h.SupportedGobos);
                }
            }
        }
        
        public string Name { get; }
        public uint Width { get; }
        public uint Channel { get; }

        public Color Color
        {
            get;
            set
            {
                
            }
        }

        public T GetProperty<T>(DeviceProperty property)
        {
            return (T)properties[property];
        }

        public void SetProperty<T>(DeviceProperty property, T value)
        {
            if (property == DeviceProperty.Color && typeof(T) != typeof(Color))
            {
                throw new ArgumentException("Color must be of type DmxLib.Util.Color", nameof(value));
            }
            if (property == DeviceProperty.Dimming && typeof(T) != typeof(double))
            {
                throw new ArgumentException("Dimming must be of type double", nameof(value));
            }
            if (property == DeviceProperty.Gobo && (typeof(T) != typeof(string) && value != null))
            {
                throw new ArgumentException("Dimming must be of type double", nameof(value));
            }
            if (property == DeviceProperty.Dimming && typeof(T) != typeof(double))
            {
                throw new ArgumentException("Dimming must be of type double", nameof(value));
            }
            if (property == DeviceProperty.Dimming && typeof(T) != typeof(double))
            {
                throw new ArgumentException("Dimming must be of type double", nameof(value));
            }
            throw new NotImplementedException();
        }
    }
}