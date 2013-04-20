using System;

namespace Ircey
{
	public enum Comparison {Absolute, Comparative, Superlative};
	public static class Adjectives {
		private static Random RND = new Random();
		public static string RNDAbsolute {get{return STD[RND.Next(0,STD.Length/3), 0];}}
		public static string RNDComparative {get{return STD[RND.Next(0,STD.Length/3), 1];}}
		public static string RNDSuperlative {get{return STD[RND.Next(0,STD.Length/3), 2];}}
		public static string RNDAdjective (Comparison C) {
			switch (C) {
			case Comparison.Absolute:
				return RNDAbsolute;
			case Comparison.Comparative:
				return RNDComparative;
			case Comparison.Superlative:
				return RNDSuperlative;
			default:
				throw new ArgumentException();
			}
		}
		//Absolute //Comparative //Superlative
		public static string[,] STD = new string[,] {
			//{"" ,"" ,""},
			{"blue" ,"bluer" ,"bluest"},
			{"green" ,"greener" ,"greenest"},
			{"pretty" ,"prettier" ,"prettiest"},
			{"red", "redder", "reddest"},
		};
	}

	public static class Nouns {
		private static Random RND = new Random();
		//Singular //Plural
		public static string[,] STD = new string[,] {
			//{"" ,""},
			{"cat" ,"cats"},
			{"computer" ,"computers"},
			{"girl" ,"girls"},
			{"potato" ,"potatoes"},
			{"tank" ,"tanks"},
		};
		public struct ProperNoun {
			public string name;
			public bool requiresThe;
			public ProperNoun(string name, bool requiresThe) {
				this.name = name;
				this.requiresThe = requiresThe;
			}
		}
		public static ProperNoun[] People = new ProperNoun[] {
			//new ProperNoun("", false),
			new ProperNoun("Barack Obama", false),
			new ProperNoun("Kim Jong-Un", false),
		};
		public static ProperNoun[] Places = new ProperNoun[] {
			//new ProperNoun("", false),
			new ProperNoun("North Korea", false),
			new ProperNoun("United Kingdom", true),
			new ProperNoun("United States of America", true),
		};
	}
}

