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

        [TestMethod]
        public void SameBrightnessScalesRgbOnlyWhenFixtureHasNoDedicatedDimmer()
        {
            const int simulatedDimmerAddress = 41;
            const int dedicatedDimmerAddress = 51;
            var simulatedDimmerFixture = new FixtureBuilder().Rgb().Address(simulatedDimmerAddress).Build();
            var dedicatedDimmerFixture = new FixtureBuilder().Rgb().Dimmer().Address(dedicatedDimmerAddress).Build();
            var simulatedState = simulatedDimmerFixture.Definition.CreateState();
            simulatedState.Get<ColorState>().Color = TestColors.Red;
            simulatedState.Get<BrightnessState>().Brightness = 0.5f;
            var dedicatedState = dedicatedDimmerFixture.Definition.CreateState();
            dedicatedState.Get<ColorState>().Color = TestColors.Red;
            dedicatedState.Get<BrightnessState>().Brightness = 0.5f;
            var buffer = BufferAssert.CreateSentinelBuffer();
            var renderer = RendererTestFactory.CreateFixtureRenderer();

            renderer.Render(simulatedDimmerFixture, simulatedState, buffer);
            renderer.Render(dedicatedDimmerFixture, dedicatedState, buffer);

            BufferAssert.ContainsSlice(buffer, simulatedDimmerAddress, 127, 0, 0);
            BufferAssert.ContainsSlice(buffer, dedicatedDimmerAddress, 255, 0, 0, 127);
            BufferAssert.OnlyChannelsChanged(
                buffer,
                simulatedDimmerAddress,
                simulatedDimmerAddress + 1,
                simulatedDimmerAddress + 2,
                dedicatedDimmerAddress,
                dedicatedDimmerAddress + 1,
                dedicatedDimmerAddress + 2,
                dedicatedDimmerAddress + 3);
        }

        [DataTestMethod]
        [DataRow(nameof(TestColors.Red))]
        [DataRow(nameof(TestColors.Green))]
        [DataRow(nameof(TestColors.Blue))]
        [DataRow(nameof(TestColors.White))]
        [DataRow(nameof(TestColors.Magenta))]
        public void ZeroBrightnessSuppressesRgbOutputWithoutDedicatedDimmer(string colorName)
        {
            const int startAddress = 70;
            var fixture = new FixtureBuilder().Rgb().Address(startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<ColorState>().Color = colorName switch
            {
                nameof(TestColors.Red) => TestColors.Red,
                nameof(TestColors.Green) => TestColors.Green,
                nameof(TestColors.Blue) => TestColors.Blue,
                nameof(TestColors.White) => TestColors.White,
                nameof(TestColors.Magenta) => TestColors.Magenta,
                _ => throw new AssertFailedException($"Unknown test color {colorName}."),
            };
            state.Get<BrightnessState>().Brightness = 0.0f;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, 0, 0, 0);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress, startAddress + 1, startAddress + 2);
        }
    }
}
