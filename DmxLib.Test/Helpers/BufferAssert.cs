using System;
using DmxLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DmxLib.Test.Helpers
{
    internal static class BufferAssert
    {
        public const byte Sentinel = 0x7E;

        public static byte[] CreateSentinelBuffer(int length = 512)
        {
            var buffer = new byte[length];
            Array.Fill(buffer, Sentinel);
            return buffer;
        }

        public static void ContainsSlice(byte[] buffer, int oneBasedStartAddress, params byte[] expected)
        {
            for(var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], buffer[oneBasedStartAddress - 1 + i], $"Unexpected value at DMX channel {oneBasedStartAddress + i}.");
            }
        }

        public static void OnlyChannelsChanged(byte[] buffer, params int[] oneBasedChannels)
        {
            var changed = new bool[buffer.Length];
            foreach(var channel in oneBasedChannels)
            {
                changed[channel - 1] = true;
            }

            for(var i = 0; i < buffer.Length; i++)
            {
                if(!changed[i])
                {
                    Assert.AreEqual(Sentinel, buffer[i], $"DMX channel {i + 1} was changed unexpectedly.");
                }
            }
        }

        public static void ChannelsAreZeroExcept(byte[] buffer, params int[] oneBasedChannels)
        {
            var changed = new bool[buffer.Length];
            foreach(var channel in oneBasedChannels)
            {
                changed[channel - 1] = true;
            }

            for(var i = 0; i < buffer.Length; i++)
            {
                if(!changed[i])
                {
                    Assert.AreEqual((byte)0, buffer[i], $"DMX channel {i + 1} was changed unexpectedly.");
                }
            }
        }
    }
}
