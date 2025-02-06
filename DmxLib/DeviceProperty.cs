using System;
using System.Collections.Generic;

namespace DmxLib
{
    public class DeviceProperty
    {
        private DeviceProperty(string name, Type type, object defaultValue)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
        }

        public readonly string Name;
        public readonly Type Type;
        public readonly object DefaultValue;

        private static readonly Dictionary<string, DeviceProperty> Registry = new Dictionary<string, DeviceProperty>();

        public static DeviceProperty GetProperty(string name)
        {
            return Registry[name];
        }

        public static DeviceProperty RegisterProperty(string name, Type type, Object defaultValue)
        {
            if (Registry.ContainsKey(name))
            {
                if (Registry[name].Type == type)
                {
                    return Registry[name];
                }
                else
                {
                    throw new NotSupportedException(string.Format("The name \"%s\" is already taken by a different type", name));
                }
            }
            else
            {
                DeviceProperty property = new DeviceProperty(name, type, defaultValue);
                Registry[name] = property;
                return property;
            }
        }

        public static bool IsNameTaken(string name)
        {
            return Registry.ContainsKey(name);
        }
    }
}