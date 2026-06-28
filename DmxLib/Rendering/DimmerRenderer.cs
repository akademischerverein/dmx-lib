using System;
using System.Collections.Generic;
using System.Text;
using DmxLib.Capability;
using DmxLib.StatePart;

namespace DmxLib.Rendering
{
    public class DimmerRenderer : CapabilityRenderer<DimmerCapability>
    {
        public override void Render(RenderContext ctx, DimmerCapability capability)
        {
            var brightnessState = ctx.State.Get<BrightnessState>();

            ctx.Buffer.Span[(int)(ctx.StartAddress + capability.ChannelOffset - 1)] = (byte)(brightnessState.Brightness * 255);
        }
    }
}
