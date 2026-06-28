using System;
using System.Collections.Generic;
using System.Text;
using DmxLib.Capability;
using DmxLib.StatePart;

namespace DmxLib.Rendering
{
    public class RelayRenderer : CapabilityRenderer<RelayCapability>
    {
        public override void Render(RenderContext ctx, RelayCapability capability)
        {
            var relayState = ctx.State.Get<RelayState>();

            ctx.Buffer.Span[(int)(ctx.StartAddress + capability.ChannelOffset - 1)] = (byte)(relayState.Active ? 255 : 0);
        }
    }
}
