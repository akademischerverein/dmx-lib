using DmxLib.StatePart;
using DmxLib.Test.Builders;
using DmxLib.Test.Helpers;
using DmxLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLib.Test
{
    [TestClass]
    public sealed class UniverseRendererTests
    {
        [TestMethod]
        public void RendersMultipleFixturesAtDifferentAddressesWithoutOverlappingChannels()
        {
            var rgbFixture = new FixtureBuilder().Rgb().Address(1).Build();
            var dimmerFixture = new FixtureBuilder().Dimmer().Address(20).Build();
            var relayFixture = new FixtureBuilder().Relay().Address(50).Build();
            var universe = new UniverseBuilder()
                .WithFixture(rgbFixture)
                .WithFixture(dimmerFixture)
                .WithFixture(relayFixture)
                .Build();
            var frame = universe.CreateState();
            frame.Get(rgbFixture).Get<BrightnessState>().Brightness = 1.0f;
            frame.Get(rgbFixture).Get<ColorState>().Color = TestColors.White;
            frame.Get(dimmerFixture).Get<BrightnessState>().Brightness = 0.5f;
            frame.Get(relayFixture).Get<RelayState>().Active = true;

            var buffer = RendererTestFactory.CreateUniverseRenderer().Render(frame).ToArray();

            Assert.AreEqual(512, buffer.Length);
            BufferAssert.ContainsSlice(buffer, 1, 255, 255, 255);
            BufferAssert.ContainsSlice(buffer, 20, 127);
            BufferAssert.ContainsSlice(buffer, 50, 255);
            BufferAssert.ChannelsAreZeroExcept(buffer, 1, 2, 3, 20, 50);
        }

        [TestMethod]
        public void NewlyCreatedRenderStateRendersAllFixturesInactiveByDefault()
        {
            var rgbFixture = new FixtureBuilder().Rgb().Address(10).Build();
            var dimmerFixture = new FixtureBuilder().Dimmer().Address(30).Build();
            var relayFixture = new FixtureBuilder().Relay().Address(60).Build();
            var universe = new UniverseBuilder()
                .WithFixture(rgbFixture)
                .WithFixture(dimmerFixture)
                .WithFixture(relayFixture)
                .Build();
            var frame = universe.CreateState();

            var buffer = RendererTestFactory.CreateUniverseRenderer().Render(frame).ToArray();

            Assert.AreEqual(512, buffer.Length);
            BufferAssert.ContainsSlice(buffer, 10, 0, 0, 0);
            BufferAssert.ContainsSlice(buffer, 30, 0);
            BufferAssert.ContainsSlice(buffer, 60, 0);
            BufferAssert.ChannelsAreZeroExcept(buffer, 10, 11, 12, 30, 60);
        }

        [TestMethod]
        public void RenderStateUsesFixtureObjectIdentityAsKey()
        {
            var firstFixture = new FixtureBuilder().Dimmer().Address(1).Build();
            var secondFixture = new Fixture
            {
                Definition = firstFixture.Definition,
                Address = firstFixture.Address,
            };
            var universe = new UniverseBuilder()
                .WithFixture(firstFixture)
                .WithFixture(secondFixture)
                .Build();
            var frame = universe.CreateState();

            frame.Get(firstFixture).Get<BrightnessState>().Brightness = 0.5f;
            frame.Get(secondFixture).Get<BrightnessState>().Brightness = 1.0f;

            Assert.AreNotSame(frame.Get(firstFixture), frame.Get(secondFixture));
        }
    }
}
