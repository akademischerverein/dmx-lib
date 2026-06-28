using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DmxLib.Capability;

namespace DmxLib.Rendering
{
    public class RenderingResolver
    {
        public IEnumerable<ICapabilityRenderer> Renderer { get; init; }

        public ICapabilityRenderer Resolve<T>() where T : ICapability
        {
            return Resolve(typeof(T));
        }

        public ICapabilityRenderer Resolve(Type capabilityType)
        {
            return Renderer.First(
                renderer => renderer.GetType().GetInterfaces().Any(
                    rendererInf => rendererInf.IsGenericType &&
                    rendererInf.GetGenericTypeDefinition() == typeof(ICapabilityRenderer<>) &&
                    rendererInf.GenericTypeArguments.Any(args => args == capabilityType)));
        }
    }
}
