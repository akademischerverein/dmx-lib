using DmxLib.StatePart;
using DmxLib.Test.Builders;
using DmxLib.Test.Helpers;
using DmxLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLib.Test
{
    [TestClass]
    public sealed class RegressionTests
    {
        [TestMethod]
        public void RgbWithoutDimmerSimulatesDimmingInColorChannels()
        {
            const int startAddress = 5;
            var fixture = new FixtureBuilder().Rgb().Address(startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<ColorState>().Color = TestColors.Red;
            state.Get<BrightnessState>().Brightness = 0.5f;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, 127, 0, 0);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2);
        }

        [TestMethod]
        public void RgbWithDimmerDoesNotScaleColorChannels()
        {
            const int startAddress = 5;
            var fixture = new FixtureBuilder().Rgb().Dimmer().Address(startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<ColorState>().Color = TestColors.Red;
            state.Get<BrightnessState>().Brightness = 0.5f;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, 255, 0, 0, 127);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2, startAddress + 3);
        }
    }
}
