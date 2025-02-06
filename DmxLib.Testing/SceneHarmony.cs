using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DmxLib.Util;

namespace DmxLib.Testing
{
    public class SceneHarmony
    {
        private readonly IDevice[] _a;
        private const uint Fade = 120000;
        private readonly List<Cue> _cues = new List<Cue>();
        private readonly List<Color> _colors = new List<Color>(new Color[]{Color.FromHSV(0, 1, 1), Color.FromHSV(120, 1, 1),
            Color.FromHSV(240, 1, 1)}.ToList());

        public SceneHarmony(IDevice[] devs)
        {
            _a = devs;
        }

        private static bool _isDone(Cue cue, DateTime start, DateTime now)
        {
            return (now - start).TotalMilliseconds >= (cue.Fade + cue.Delay);
        }
        
        public void Update(Universe universe, List<Cue> cues, DateTime now, ReadOnlyDictionary<Cue, DateTime?> starts)
        {
            if (_cues.Count == 0 || _cues.Select(c => starts[c].HasValue && _isDone(c, starts[c].Value, now)).All(b => b))
            {
                Console.WriteLine("Rotating colors");
                var color = _colors[0];
                _colors.RemoveAt(0);
                _colors.Add(color);
                _cues.Clear();

                for(var i = 0; i < _a.Length; i++)
                {
                    var cue = new Cue
                    {
                        Device = _a[i],
                        Property = Program.PropertyColor,
                        Value = _colors[i],
                        Delay = 0,
                        Fade = Fade
                    };
                    _cues.Add(cue);
                }
            }
            cues.AddRange(_cues);
        }
    }
}