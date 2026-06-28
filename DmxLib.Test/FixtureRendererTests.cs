using DmxLib.Capability;
using DmxLib.Rendering;
using DmxLib.StatePart;
using DmxLib.Test.Builders;
using DmxLib.Test.Helpers;
using DmxLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLib.Test
{
    [TestClass]
    public sealed class FixtureRendererTests
    {
        [TestMethod]
        public void RendersAllFixtureCapabilitiesWithResolvedRenderers()
        {
            const int startAddress = 17;
            var fixture = new FixtureBuilder().Rgb().Dimmer().Relay().Address(startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<ColorState>().Color = TestColors.Red;
            state.Get<BrightnessState>().Brightness = 0.5f;
            state.Get<RelayState>().Active = true;
            var buffer = BufferAssert.CreateSentinelBuffer();
            var renderer = RendererTestFactory.CreateFixtureRenderer();

            renderer.Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, 255, 0, 0, 127, 255);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2, startAddress + 3, startAddress + 4);
        }

        [TestMethod]
        public void ResolverFindsRendererForEachSupportedCapability()
        {
            var resolver = RendererTestFactory.CreateResolver();

            var colorRenderer = resolver.Resolve<ColorCapability>();
            var dimmerRenderer = resolver.Resolve<DimmerCapability>();
            var relayRenderer = resolver.Resolve<RelayCapability>();

            Assert.IsInstanceOfType(colorRenderer, typeof(ColorRenderer));
            Assert.IsInstanceOfType(dimmerRenderer, typeof(DimmerRenderer));
            Assert.IsInstanceOfType(relayRenderer, typeof(RelayRenderer));
        }
    }
}
