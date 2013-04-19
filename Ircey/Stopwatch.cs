using System;
using System.Collections;
using System.Collections.Generic;

namespace Ircey
{
	public class StopwatchManager{
		public struct Stopwatch {
			public readonly DateTime time;
			public readonly string name;
			public Stopwatch (string name) {
				this.name = name;
				time = DateTime.Now;
			}
		}
		private Hashtable stHash = new Hashtable();
		public StopwatchManager () {
		}
		public bool LookupStopwatch (string name) {
			return stHash.ContainsKey(name);
		}
		public void StartStopwatch (string name) {
			stHash.Add(name, new Stopwatch(name));
		}
		public TimeSpan StopStopwatch (string name) {
			TimeSpan r = DateTime.Now - ((Stopwatch)stHash[name]).time;
			stHash.Remove(name);
			return r;
		}
	}
}

