using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DmxLib.Capability;
using DmxLib.StatePart;

namespace DmxLib.Rendering
{
    public class ColorRenderer : ICapabilityRenderer<ColorCapability>
    {
        public void Render(RenderContext ctx, ColorCapability capability)
        {
            var colorState = ctx.State.Get<ColorState>();
            var brightnessState = ctx.State.Get<BrightnessState>();
            var simBrightness = !ctx.Definition.Capabilities.OfType<DimmerCapability>().Any();
            var brightness = 1.0;
            if (simBrightness)
            {
                brightness = brightnessState.Brightness;
            }

            var adjUniverse = ctx.Buffer.Span.Slice((int)(ctx.StartAddress + capability.ChannelOffset));

            for(int i = 0; i<capability.Emitters.Count; i++)
            {
                switch(capability.Emitters[i])
                {
                    case ColorCapability.EmitterColor.Red:
                        adjUniverse[i] = (byte)(Math.Clamp(colorState.Color.R * brightness, 0.0, 1.0) * 255);
                        break;
                    case ColorCapability.EmitterColor.Green:
                        adjUniverse[i] = (byte)(Math.Clamp(colorState.Color.G * brightness, 0.0, 1.0) * 255);
                        break;
                    case ColorCapability.EmitterColor.Blue:
                        adjUniverse[i] = (byte)(Math.Clamp(colorState.Color.B * brightness, 0.0, 1.0) * 255);
                        break;
                    default:
                        throw new NotImplementedException($"EmitterColor {capability.Emitters[i]} not implemented");
                }
            }
        }
    }
}
