using DmxLib.Rendering;
using DmxLib.StatePart;
using DmxLib.Util;
using DmxLib;

namespace DmxLib.Test.Builders
{
    internal sealed class RenderStateBuilder
    {
        private readonly RenderFrame _frame;

        public RenderStateBuilder(Universe universe)
        {
            _frame = universe.CreateState();
        }

        public RenderStateBuilder Brightness(Fixture fixture, float brightness)
        {
            _frame.Get(fixture).Get<BrightnessState>().Brightness = brightness;
            return this;
        }

        public RenderStateBuilder Color(Fixture fixture, Color color)
        {
            _frame.Get(fixture).Get<ColorState>().Color = color;
            return this;
        }

        public RenderStateBuilder Relay(Fixture fixture, bool active)
        {
            _frame.Get(fixture).Get<RelayState>().Active = active;
            return this;
        }

        public RenderFrame Build()
        {
            return _frame;
        }
    }
}
