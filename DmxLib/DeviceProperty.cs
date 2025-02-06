using System;
using System.Collections.Generic;

namespace DmxLib
{
    public class DeviceProperty
    {
        private DeviceProperty(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public readonly string Name;
        public readonly Type Type;

        private static Dictionary<string, DeviceProperty> registry = new Dictionary<string, DeviceProperty>();

        public static DeviceProperty GetProperty(string name)
        {
            return registry[name];
        }

        public static DeviceProperty RegisterProperty(string name, Type type)
        {
            if (registry.ContainsKey(name))
            {
                if (registry[name].Type == type)
                {
                    return registry[name];
                }
                else
                {
                    throw new NotSupportedException(string.Format("The name \"%s\" is already taken by a different type", name));
                }
            }
            else
            {
                DeviceProperty property = new DeviceProperty(name, type);
                registry[name] = property;
                return property;
            }
        }

        public static bool IsNameTaken(string name)
        {
            return registry.ContainsKey(name);
        }
    }
}