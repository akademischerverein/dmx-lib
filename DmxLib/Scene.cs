using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DmxLib
{
    public class Scene
    {
        private readonly Universe _universe;
        private DateTime _lastUpdate;
        private readonly Dictionary<Cue, DateTime?> _cues;
        private readonly ReadOnlyDictionary<DeviceProperty, Func<Cue, DateTime, TimeSpan, DateTime, object>> _transitions;
        
        public Scene(Universe universe, ReadOnlyDictionary<DeviceProperty, Func<Cue, DateTime, TimeSpan, DateTime, object>> transitions)
        {
            _universe = universe;
            _lastUpdate = DateTime.Now;
            _cues = new Dictionary<Cue, DateTime?>();
            _transitions = transitions;
        }

        private bool _isCueDone(Cue cue, DateTime now)
        {
            if (!_cues[cue].HasValue) return false;

            return (now - _cues[cue].Value).TotalMilliseconds >= (cue.Fade + cue.Delay);
        }

        private void _applyCue(Cue cue, DateTime now, TimeSpan delta)
        {
            if (!_cues[cue].HasValue)
            {
                _cues[cue] = now;
            }

            if (_isCueDone(cue, now))
            {
                cue.Device.Set(cue.Property, cue.Value);
            } else if ((now - _cues[cue].Value).TotalMilliseconds > cue.Delay)
            {
                var newValue = _transitions[cue.Property](cue, now, delta, _cues[cue].Value);
                cue.Device.Set(cue.Property, newValue);
            }
        }
        
        public void Update()
        {
            var now = DateTime.Now;
            var diff = now - _lastUpdate;

            foreach (var cue in new List<Cue>(_cues.Keys))
            {
                _applyCue(cue, now, diff);
            }

            _lastUpdate = now;
            var newCues = new List<Cue>();
            SceneUpdateEvent?.Invoke(_universe, newCues, now, new ReadOnlyDictionary<Cue, DateTime?>(_cues));

            var delete = _cues.Keys.Except(newCues).Distinct();
            var add = newCues.Except(_cues.Keys).Distinct();

            foreach (var c in delete)
            {
                _cues.Remove(c);
            }

            foreach (var c in add)
            {
                _cues[c] = null;
            }
        }

        public delegate void SceneUpdate(Universe universe, List<Cue> cues, DateTime now, ReadOnlyDictionary<Cue, DateTime?> starts);

        public SceneUpdate SceneUpdateEvent;
    }
}