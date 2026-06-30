using DmxLib.StatePart;
using DmxLib.Test.Builders;
using DmxLib.Test.Helpers;
using DmxLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLib.Test
{
    [TestClass]
    public sealed class RelayRendererTests
    {
        [DataTestMethod]
        [DataRow(1, false, 0)]
        [DataRow(42, true, 255)]
        public void RelayRendersActiveStateToSingleChannel(int startAddress, bool active, int expected)
        {
            var fixture = new FixtureBuilder().Relay().Address((ushort)startAddress).Build();
            var state = fixture.Definition.CreateState();
            state.Get<RelayState>().Active = active;
            var buffer = BufferAssert.CreateSentinelBuffer();

            RendererTestFactory.CreateFixtureRenderer().Render(fixture, state, buffer);

            BufferAssert.ContainsSlice(buffer, startAddress, (byte)expected);
            BufferAssert.OnlyChannelsChanged(buffer, startAddress);
        }
    }
}
