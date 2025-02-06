using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Color = DmxLib.Util.Color;

namespace DmxLib.Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            ProgramScene.MainScene(args);
            return;
            Console.WriteLine("Hello World!");
            var sink = new AvSink("192.168.0.6", 5120);
            var sinkThread = new Thread(sink.Commit);

            var uni = new Universe(512, sink);
            var devices = new List<Device>();
            var tempGroups = new Dictionary<string, List<Device>>();

            var lines = File.ReadLines(@"../../../../kanalplan.txt");
            foreach (var l in lines)
            {
                if (l.StartsWith("#") || l.Trim().Length == 0)
                    continue;
                var ch = uint.Parse(l.Split(";")[0].Trim());
                var typeInfo = l.Split(";")[1].Trim();
                var group = l.Split(";")[2].Trim();
                var name = l.Split(";")[3].Trim();
                var width = uint.Parse(typeInfo.Split(':')[0]);
                var typ = typeInfo.Split(':')[1];

                if (!tempGroups.ContainsKey(group))
                {
                    tempGroups[group] = new List<Device>();
                }

                switch (typ)
                {
                    case "RGB":
                    case "RGBW":
                    case "DRGB":
                        var devColor = new Device(name, width, ch, new IHandler[]{new RgbHandler(typ.ToLower())});
                        //uni.AddDevice(devColor);
                        devices.Add(devColor);
                        tempGroups[group].Add(devColor);
                        break;
                    case "Dimmer":
                    case "Blinder":
                        var devDimming = new Device(name, width, ch, new IHandler[]{new DimmerHandler(0)});
                        //uni.AddDevice(devDimming);
                        devices.Add(devDimming);
                        devDimming.Set(PropertyDimming, 0.0);
                        tempGroups[group].Add(devDimming);
                        break;
                }
            }

            var groups = new List<DeviceGroup>();
            foreach (var devs in tempGroups)
            {
                if (devs.Value.Count == 0) continue;
                var devGroup = new DeviceGroup(devs.Key, devs.Value);
                groups.Add(devGroup);
                uni.AddDevice(devGroup);
            }

            foreach (var dev in devices)
            {
                Console.WriteLine("Device ch={0} name={3} {1} width={2}", dev.Channels.First(), dev.Name, dev.Channels.Count, tempGroups.Where(l => l.Value.Contains(dev)).First().Key);
            }

            for (var i=0; i<groups.Count; i++)
            {
                var group = groups[i];
                Console.WriteLine("{0}: DeviceGroup size={1} name={2}", i, group.Children.Count(), group.Name);
            }

            sinkThread.Start();

            while (true)
            {
                Console.Write("ch: ");
                var selector = Console.ReadLine();
                IDevice dev;
                if (selector.StartsWith("g"))
                {
                    dev = groups[int.Parse(selector.Substring(1))];
                }
                else
                {
                    var ch = uint.Parse(selector);
                    dev = devices.Find(d => d.Channels.Contains(ch));
                }

                if (dev.SupportedProperties.Contains(PropertyColor))
                {
                    Console.Write("r: ");
                    var r = double.Parse(Console.ReadLine());
                    Console.Write("g: ");
                    var g = double.Parse(Console.ReadLine());
                    Console.Write("b: ");
                    var b = double.Parse(Console.ReadLine());
                    dev.Set(PropertyColor, Color.FromRGB(r, g, b));
                }

                if (dev.SupportedProperties.Contains(PropertyDimming))
                {
                    Console.Write("d: ");
                    var d = double.Parse(Console.ReadLine());
                    dev.Set(PropertyDimming, d);   
                }
            }
        }

        public static readonly DeviceProperty PropertyColor = DeviceProperty.RegisterProperty("color",
            typeof(Color), Color.FromRGB(0, 0, 0), (g, d) => ((Color)g).R != 0 || ((Color)g).G != 0 || ((Color)g).B != 0 ? g : d);
        public static readonly DeviceProperty PropertyDimming = DeviceProperty.RegisterProperty("dimming", typeof(double), 1.0, (g, d) => (double)g * (double)d);
    }
}