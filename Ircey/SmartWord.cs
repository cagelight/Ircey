using System;

namespace Ircey
{
	public interface ISmartWord { char Callsign(); };

	public static class WordFront {
		public static string RandomAdjective () {
			return new SmartAdjective((Comparison) new Random().Next(0,3)).ToString();
		}
	}

	public struct SmartAdjective : ISmartWord {
		public string word;
		public Comparison C;
		public SmartAdjective (Comparison c, string s = "") {
			this.C = c;
			if (s == "") {
				this.word = Adjectives.RNDAdjective(C);
			} else {
				this.word = s;
			}
		}
		public override string ToString () {
			return word;
		}
		public char Callsign() {
			return 'a';
		}
	}
}

