using System;
using System.Collections.Generic;
using System.Text;
using DmxLib.Capability;
using DmxLib.StatePart;

namespace DmxLib.Rendering
{
    public class DimmerRenderer : ICapabilityRenderer<DimmerCapability>
    {
        public void Render(RenderContext ctx, DimmerCapability capability)
        {
            var brightnessState = ctx.State.Get<BrightnessState>();

            ctx.Buffer.Span[(int)(ctx.StartAddress + capability.ChannelOffset)] = (byte)(brightnessState.Brightness * 255);
        }
    }
}
