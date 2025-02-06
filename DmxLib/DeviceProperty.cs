using System;
using System.Collections.Generic;

namespace DmxLib
{
    public class DeviceProperty
    {
        private DeviceProperty(string name, Type type, object defaultValue, Func<object, object, object> projection)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
            Project = projection;
        }

        public readonly string Name;
        public readonly Type Type;
        public readonly object DefaultValue;
        public readonly Func<object, object, object> Project;

        private static readonly Dictionary<string, DeviceProperty> Registry = new Dictionary<string, DeviceProperty>();

        public static DeviceProperty GetProperty(string name)
        {
            return Registry[name];
        }

        public static DeviceProperty RegisterProperty(string name, Type type, object defaultValue)
        {
            return RegisterProperty(name, type, defaultValue, (g, d) => g);
        }

        public static DeviceProperty RegisterProperty(string name, Type type, object defaultValue, Func<object, object, object> projection)
        {
            if (Registry.ContainsKey(name))
            {
                if (Registry[name].Type == type && Registry[name].DefaultValue == defaultValue) //cant compare lambdas
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
                var property = new DeviceProperty(name, type, defaultValue, projection);
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