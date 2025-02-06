using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using DmxLib.Util;

namespace DmxLib.Testing
{
    public class ProgramScene
    {
        private static Scene _scene;
        public static object HsvTransition(Cue cue, DateTime now, TimeSpan delta, DateTime start)
        {
            var timeSince = (now - start).TotalMilliseconds - cue.Delay;
            var targetModifier = delta.TotalMilliseconds / (cue.Fade - timeSince);

            if (cue.Property.Equals(Program.PropertyColor))
            {
                var currentColor = (Color)cue.Device.Get(Program.PropertyColor);
                var targetColor = (Color)cue.Value;

                var h = targetColor.H - currentColor.H;
                if (h < 0.0)
                {
                    h += 360;
                } else if (h > 360.0)
                {
                    h -= 360;
                }

                return Color.FromHSV((currentColor.H + h * targetModifier) % 360,
                    Math.Min(Math.Max(0, currentColor.S + (targetColor.S - currentColor.S) * targetModifier), 1.0),
                    Math.Min(Math.Max(0, currentColor.V + (targetColor.V - currentColor.V) * targetModifier), 1.0));
            } else if (cue.Property.Equals(Program.PropertyDimming))
            {
                var currentDimming = (double)cue.Device.Get(Program.PropertyDimming);
                var targetDimming = (double) cue.Value;
                return Math.Min(Math.Max(0, currentDimming + (targetDimming - currentDimming) * targetModifier), 1.0);
            }
            throw new ArgumentException(string.Format("Unknown transition for property {0}", cue.Property.Name));
        }

        public static void SceneUpdater()
        {
            while (true)
            {
                _scene.Update();
                Thread.Sleep(1);
            }
        }
        
        public static void MainScene(string[] args)
        {
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
                        var devDimming = new Device(name, width, ch, new IHandler[]{new DimmerHandler()});
                        //uni.AddDevice(devDimming);
                        devices.Add(devDimming);
                        devDimming.Set(Program.PropertyDimming, 0.0);
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
            
            var transitions = new Dictionary<DeviceProperty, Func<Cue, DateTime, TimeSpan, DateTime, object>>();
            transitions[Program.PropertyColor] = HsvTransition;
            transitions[Program.PropertyDimming] = HsvTransition;
            _scene = new Scene(uni, new ReadOnlyDictionary<DeviceProperty, Func<Cue, DateTime, TimeSpan, DateTime, object>>(transitions));
            _scene.SceneUpdateEvent +=
                new SceneHarmony(new IDevice[] {groups[1].Children.First(d => d.Channels.Contains(17)), groups[1].Children.First(d => d.Channels.Contains(25)), groups[1].Children.First(d => d.Channels.Contains(29))}).Update;
            var sceneThread = new Thread(SceneUpdater);

            sinkThread.Start();
            sceneThread.Start();

            while (true)
            {
                
            }
        }
    }
}