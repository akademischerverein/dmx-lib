using System;
using DmxLib.StatePart;
using DmxLib.Test.Builders;
using DmxLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLib.Test
{
    [TestClass]
    public sealed class FixtureDefinitionTests
    {
        [TestMethod]
        public void NumChannelsReturnsCurrentChannelSpanCalculation()
        {
            var definition = new FixtureBuilder().Rgb().Dimmer().Relay().BuildDefinition();

            var numChannels = definition.NumChannels;

            Assert.AreEqual((uint)6, numChannels);
        }

        [TestMethod]
        public void CreateStateCreatesEveryRequiredStatePart()
        {
            var definition = new FixtureBuilder().Rgb().Dimmer().Relay().BuildDefinition();

            var state = definition.CreateState();

            Assert.IsNotNull(state.Get<ColorState>());
            Assert.IsNotNull(state.Get<BrightnessState>());
            Assert.IsNotNull(state.Get<RelayState>());
        }

        [TestMethod]
        public void CreateStateCreatesDuplicateRequiredStatePartsOnlyOnce()
        {
            var definition = new FixtureBuilder().Rgb().Dimmer().BuildDefinition();

            var state = definition.CreateState();

            Assert.AreSame(state.Get<BrightnessState>(), state.Get<BrightnessState>());
            Assert.IsNotNull(state.Get<ColorState>());
        }

        [TestMethod]
        public void CreateStateOmitsUnrequiredStateParts()
        {
            var definition = new FixtureBuilder().Relay().BuildDefinition();

            var state = definition.CreateState();

            Assert.IsNotNull(state.Get<RelayState>());
            Assert.ThrowsException<InvalidOperationException>(() => state.Get<BrightnessState>());
            Assert.ThrowsException<InvalidOperationException>(() => state.Get<ColorState>());
        }
    }
}
