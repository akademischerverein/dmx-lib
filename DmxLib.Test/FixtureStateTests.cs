using System;
using DmxLib.StatePart;
using DmxLib.Test.Builders;
using DmxLib.Test.Helpers;
using DmxLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLib.Test
{
    [TestClass]
    public sealed class FixtureStateTests
    {
        [TestMethod]
        public void GetReturnsRequestedStatePart()
        {
            var state = new FixtureBuilder().Rgb().Dimmer().Relay().BuildDefinition().CreateState();

            var color = state.Get<ColorState>();
            var brightness = state.Get<BrightnessState>();
            var relay = state.Get<RelayState>();

            Assert.IsNotNull(color);
            Assert.IsNotNull(brightness);
            Assert.IsNotNull(relay);
        }

        [TestMethod]
        public void GetThrowsWhenStatePartIsMissing()
        {
            var state = new FixtureBuilder().Relay().BuildDefinition().CreateState();

            Assert.ThrowsException<InvalidOperationException>(() => state.Get<ColorState>());
        }

        [TestMethod]
        public void GetThrowsForWrongSemanticStateType()
        {
            var state = new FixtureBuilder().Rgb().BuildDefinition().CreateState();

            Assert.ThrowsException<InvalidOperationException>(() => state.Get<RelayState>());
        }

        [TestMethod]
        public void DefaultValuesDisableNewFixtureState()
        {
            var state = new FixtureBuilder().Rgb().Dimmer().Relay().BuildDefinition().CreateState();

            Assert.AreEqual(0.0f, state.Get<BrightnessState>().Brightness);
            Assert.AreEqual(TestColors.Black.R, state.Get<ColorState>().Color.R);
            Assert.AreEqual(TestColors.Black.G, state.Get<ColorState>().Color.G);
            Assert.AreEqual(TestColors.Black.B, state.Get<ColorState>().Color.B);
            Assert.IsFalse(state.Get<RelayState>().Active);
        }
    }
}
