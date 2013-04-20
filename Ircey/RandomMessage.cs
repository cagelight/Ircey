using System;

namespace Ircey
{
	public static class RandomMessage
	{
		private static Random wordRND = new Random(DateTime.Now.Millisecond);
		public static string[] adjectiveiest = new string[]{"Prettiest", "Shittiest", "Coolest", "Hottest", "Fastest", "Slowest", "Ugliest", "Gayest", "Hardest", "Softest"};
		public static string[] adjective = new string[]{"Red", "Blue", "Green", "Soft", "Hard", "Ugly", "Pretty", "Stupid", "Smart", "Dumb"};
		public static string[] kineticverbs = new string[]{"Force", "Kick", "Push", "Throw", "Toss", "Place", "Set"};
		public static string[] nonkineticverbs = new string[]{"Kill", "Punch", "Kick", "Destroy", "Decimate", "Obliterate", "Annihilate", "Smash"};
		public static string[] nouns = new string[]{"Computer", "Box", "Fish tank", "Cup", "Laptop", "House", "Hole", "Human body", "Car", "Cabinet", "Apartment", "Orbit", "Sword", "Fan", "Bag", "Girl"};
		public static string[] pluralnouns = new string[]{"Potatoes", "Vacuum tubes", "Chicken nuggets", "Motherboards", "Processors", "C# programs", "Meteors", "Bananas", "Dogs", "Cats", "Lizards", "Swords", "Knives", "Dicks", "Girls", "Boxes"};
		public static string[] propernounslocations = new string[] {"North Korea", "The United States of America", "France", "Germany", "Spain", "The United Kingdom", "Russia", "The Czech Republic", "Japan", "Belarus"};
		public static string[] propernounsnames = new string[] {"Bill Gates", "Steve Jobs", "George Washington", "Kim Il-Sung", "Kim Jong-Un", "Vladimir Putin", "Jesus Christ", "God", "Barack Obama", "Adolf Hitler", "Fidel Castro"};
		public static string New () {
			int responseindex = wordRND.Next(0,7);
			switch (responseindex) {
			case 0:
				double starweight = wordRND.NextDouble();
				if (starweight < 0.10) {
					return String.Format("I am currently orbiting a {0}, are these supposed to exist?", GenerateStar.GenerateFictional());
				} else {
					return String.Format("I am currently orbiting a {0}.", GenerateStar.GenerateWeightedRandom(0.9));
				}
			case 1:
				return String.Format("{0} are not something you can simply {1} into {2}.", GetPluralNoun(false), GetKineticVerb(true), GetNounAndAccessor(true));
			case 2:
				return String.Format("Did you know that {0} are in fact the {1} objects in the known universe?", GetPluralNoun(true), GetAdjectiveIest(true));
			case 3:
				return String.Format("I will {0} you and your {1} {2}.", GetNonKineticVerb(true), GetAdjective(true), GetNoun(true));
			case 4:
				return String.Format("Last I heard, {0} was the leader of {1}, correct?", GetProperName(), GetProperLocation());
			case 5:
				return String.Format("Do you think {0} likes to {1} {2} {3}?", GetProperName(), GetNonKineticVerb(true), GetAdjective(true), GetPluralNoun(true));
			default:
				return "Hello my baby, Hello my honey, Hello my ragtime gal.";
			}
		}
		public static string GetPluralNoun(bool lowercase) {
			string r = pluralnouns[wordRND.Next(0, pluralnouns.Length)];
			if (lowercase) {r = r.ToLower();}
			return r;
		}
		public static string GetKineticVerb(bool lowercase) {
			string r = kineticverbs[wordRND.Next(0, kineticverbs.Length)];
			if (lowercase) {r = r.ToLower();}
			return r;
		}
		public static string GetNoun(bool lowercase) {
			string r = nouns[wordRND.Next(0, nouns.Length)];
			if (lowercase) {r = r.ToLower();}
			return r;
		}
		public static string GetAdjectiveIest(bool lowercase) {
			string r = adjectiveiest[wordRND.Next(0, adjectiveiest.Length)];
			if (lowercase) {r = r.ToLower();}
			return r;
		}
		public static string GetAdjective(bool lowercase) {
			string r = adjective[wordRND.Next(0, adjective.Length)];
			if (lowercase) {r = r.ToLower();}
			return r;
		}
		public static string GetNonKineticVerb(bool lowercase) {
			string r = nonkineticverbs[wordRND.Next(0, nonkineticverbs.Length)];
			if (lowercase) {r = r.ToLower();}
			return r;
		}
		public static string GetProperLocation() {
			return propernounslocations[wordRND.Next(0, propernounslocations.Length)];
		}
		public static string GetProperName() {
			return propernounsnames[wordRND.Next(0, propernounsnames.Length)];
		}
		public static string GetNounAndAccessor (bool lowercase) {
			string newnoun = GetNoun(true);
			char k = newnoun[0];
			if(k=='a'||k=='e'||k=='i'||k=='o'||k=='u'){
				if(lowercase){
					return "an "+newnoun;
				} else {
					return "An "+newnoun;
				}
			} else {
				if(lowercase){
					return "a "+newnoun;
				} else {
					return "A "+newnoun;
				}
			}
		}
	}
	public static class GenerateStar {
		private static Random starRND = new Random(DateTime.Now.Millisecond+5);
		private static string[] specialStars = new string[]{"Neutron Star", "Black Hole", "Brown Dwarf", "Large Asteroid"};
		private static string[] starSubClasses = new string[]{"Hypergiant", "Luminous Supergiant", "Bright Giant", "Giant", "Subgiant", "Dwarf", "Subdwarf"};
		private static string[,] starClasses = new string[,] {
			{"O", "Blue"},
			{"B", "Blue-White"},
			{"A", "White"},
			{"F", "Yellow-White"},
			{"G", "Yellow"},
			{"K", "Orange"},
			{"M", "Red"},
		};

		private static string[,] starClassesFictional = new string[,] {
			{"X", "Rainbow"},
			{"P", "Magenta"},
			{"C", "Green"},
			{"N", "Green-Yellow"},
			{"V", "Violet"},
			{"R", "Pink"},
		};
		private static string[] starPreSubClassesFictional = new string[]{"Triangular", "Cubic", "Inverted", "Cold", "Flat"};

		public static string GenerateStandard () {
			int classindex = starRND.Next(0, starClasses.Length/2);
			int dec =  starRND.Next(0,10);
			return String.Format("Class {0}{1}{2} {3} {4}", starClasses[classindex, 0], starRND.Next(0,10).ToString(), dec==0?"":"."+dec, starClasses[classindex, 1], starSubClasses[starRND.Next(0, starSubClasses.Length)]);
		}
		public static string GenerateSpecial() {
			return specialStars[starRND.Next(0, specialStars.Length)];
		}
		public static string GenerateFictional() {
			int classindex = starRND.Next(0, starClassesFictional.Length/2);
			int dec =  starRND.Next(0,10);
			int preclass =  starRND.Next(0,3);
			return String.Format("Class {0}{1}{2} {3} {4}{5}", starClassesFictional[classindex, 0], starRND.Next(0,10).ToString(), dec==0?"":"."+dec, starClassesFictional[classindex, 1], preclass==1?starPreSubClassesFictional[starRND.Next(0, starPreSubClassesFictional.Length)]+" ":"", starSubClasses[starRND.Next(0, starSubClasses.Length)]);
		}
		public static string GenerateWeightedRandom(double stdtospecialratio) {
			return starRND.NextDouble()>stdtospecialratio?GenerateSpecial():GenerateStandard();
		}
	}
}

