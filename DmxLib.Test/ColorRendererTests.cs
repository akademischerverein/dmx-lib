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

        [DataTestMethod]
        [DataRow(-0.25f, 0)]
        [DataRow(1.25f, 255)]
        public void RgbWithoutDimmerClampsBrightnessOutsideNormalizedRange(float brightness, int expectedRed)
        {
            const int startAddress = 21;
            var fixture = new FixtureBuilder().Rgb().Address(startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<BrightnessState>().Brightness = brightness;
            state.Get<ColorState>().Color = TestColors.Red;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, (byte)expectedRed, Off, Off);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2);
        }

        [TestMethod]
        public void RgbRendersOnLastThreeDmxChannels()
        {
            const int startAddress = 510;
            var fixture = new FixtureBuilder().Rgb().Address(startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<BrightnessState>().Brightness = 1.0f;
            state.Get<ColorState>().Color = TestColors.Cyan;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, Off, Full, Full);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2);
        }

        [DataTestMethod]
        [DataRow(nameof(TestColors.Green), 0, 255, 0)]
        [DataRow(nameof(TestColors.Blue), 0, 0, 255)]
        [DataRow(nameof(TestColors.Yellow), 255, 255, 0)]
        [DataRow(nameof(TestColors.Magenta), 255, 0, 255)]
        public void RgbRendersAdditionalColors(string colorName, int expectedRed, int expectedGreen, int expectedBlue)
        {
            const int startAddress = 33;
            var fixture = new FixtureBuilder().Rgb().Address(startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<BrightnessState>().Brightness = 1.0f;
            state.Get<ColorState>().Color = colorName switch
            {
                nameof(TestColors.Green) => TestColors.Green,
                nameof(TestColors.Blue) => TestColors.Blue,
                nameof(TestColors.Yellow) => TestColors.Yellow,
                nameof(TestColors.Magenta) => TestColors.Magenta,
                _ => throw new AssertFailedException($"Unknown test color {colorName}."),
            };
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, (byte)expectedRed, (byte)expectedGreen, (byte)expectedBlue);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2);
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
