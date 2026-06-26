using System;
using System.Collections.Generic;
using System.Text;
using DmxLib.Capability;

namespace DmxLib.Rendering
{
    public interface ICapabilityRenderer<T> where T : ICapability
    {
        public void Render(RenderContext ctx, T capability);
    }
}
