using DmxLib.StatePart;
using DmxLib.Test.Builders;
using DmxLib.Test.Helpers;
using DmxLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLib.Test
{
    [TestClass]
    public sealed class DimmerRendererTests
    {
        [DataTestMethod]
        [DataRow(1, 0.0f, 0)]
        [DataRow(64, 0.5f, 127)]
        [DataRow(200, 1.0f, 255)]
        public void DimmerRendersBrightnessToSingleChannel(int startAddress, float brightness, int expected)
        {
            var fixture = new FixtureBuilder().Dimmer().Address((ushort)startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<BrightnessState>().Brightness = brightness;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, (byte)expected);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress);
        }
    }
}
