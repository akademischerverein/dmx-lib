using System;
using System.Collections.Generic;
using System.Text;
using DmxLib.Capability;

namespace DmxLib.Rendering
{
    public interface ICapabilityRenderer<T> : ICapabilityRenderer
        where T : ICapability
    {
        public void Render(RenderContext ctx, T capability);
    }

    public interface ICapabilityRenderer
    {
        public void Render(RenderContext ctx, ICapability capability);
    }

    public abstract class CapabilityRenderer<T> : ICapabilityRenderer<T> where T : ICapability
    {
        public void Render(RenderContext ctx, ICapability capability)
        {
            Render(ctx, (T)capability);
        }

        public abstract void Render(RenderContext ctx, T capability);
    }
}
