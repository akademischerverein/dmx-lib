using DmxLib.StatePart;
using DmxLib.Test.Builders;
using DmxLib.Test.Helpers;
using DmxLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLib.Test
{
    [TestClass]
    public sealed class ColorRendererTests
    {
        private const byte Off = 0;
        private const byte Half = 127;
        private const byte Full = 255;

        [DataTestMethod]
        [DataRow(1, 0.0f, 0)]
        [DataRow(7, 0.25f, 63)]
        [DataRow(13, 0.5f, 127)]
        [DataRow(27, 0.75f, 191)]
        [DataRow(101, 1.0f, 255)]
        public void RgbWithoutDimmerScalesColorByBrightness(int startAddress, float brightness, int expectedRed)
        {
            var fixture = new FixtureBuilder().Rgb().Address((ushort)startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<BrightnessState>().Brightness = brightness;
            state.Get<ColorState>().Color = TestColors.Red;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, (byte)expectedRed, Off, Off);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2);
        }

        [TestMethod]
        public void RgbWithDimmerDoesNotScaleColorChannels()
        {
            const int startAddress = 11;
            var fixture = new FixtureBuilder().Rgb().Dimmer().Address((ushort)startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<BrightnessState>().Brightness = 0.5f;
            state.Get<ColorState>().Color = TestColors.Red;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, Full, Off, Off, Half);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2, startAddress + 3);
        }

        [TestMethod]
        public void WhiteRendersAllRgbChannelsAtFullOutput()
        {
            const int startAddress = 3;
            var fixture = new FixtureBuilder().Rgb().Address((ushort)startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<BrightnessState>().Brightness = 1.0f;
            state.Get<ColorState>().Color = TestColors.White;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, Full, Full, Full);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2);
        }

        [TestMethod]
        public void BlackRendersAllRgbChannelsAtZeroOutput()
        {
            const int startAddress = 9;
            var fixture = new FixtureBuilder().Rgb().Address((ushort)startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<BrightnessState>().Brightness = 1.0f;
            state.Get<ColorState>().Color = TestColors.Black;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, Off, Off, Off);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2);
        }
    }
}
