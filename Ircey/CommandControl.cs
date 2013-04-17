using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Ircey
{
	public delegate string FormatDelegate (string format, params string[] list);
	public class CommandControl {
		public readonly string channel;
		FormatDelegate F = new FormatDelegate(String.Format);
		WebClient CLI = new WebClient();
		public CommandControl (string channel) {
			this.channel = channel;
		}
		public string CommandSwitch (string cmd, string fuseargs, string[] divargs) {
			switch (cmd) {
			case "echo":
				return F("PRIVMSG {0} {1}",channel, divargs[4]);
			case "math":
				return F("PRIVMSG {0} {1}", channel, Calc(fuseargs));
			case "ping":
				return F("PRIVMSG {0} {1}", channel, "pong");
			case "win":
				return F("PRIVMSG {0} {1}", channel, "A strange game. The only winning move is not to play.");
			case "ircey":
				return F("PRIVMSG {0} Fuck You {1}.", channel, divargs[0].Split(new char[]{'!'})[0]);
			case "dice":
				try {
					try {
						return F("PRIVMSG {0} {1} rolls the dice! {2}!", channel, divargs[0].Split(new char[]{'!'})[0], new Random().Next(Convert.ToInt32(divargs[4]),Convert.ToInt32(divargs[5])+1).ToString());
					} catch {
						return F("PRIVMSG {0} {1} rolls the dice! {2}!", channel, divargs[0].Split(new char[]{'!'})[0], new Random().Next(1,Convert.ToInt32(divargs[4])+1).ToString());
					}
				} catch {
					return F("PRIVMSG {0} {1} rolls the dice! {2}!", channel, divargs[0].Split(new char[]{'!'})[0], new Random().Next(1,7).ToString());
				}
			case "coin":
				string coin;
				if (new Random().Next(2) == 0) {
					coin = "Heads";
				} else {
					coin = "Tails";
				}
				return F("PRIVMSG {0} {1}", channel, coin);
			case "google":
				try {
					return 	F("PRIVMSG {0} {1}", channel, Google(fuseargs));
				} catch {
					return 	F("PRIVMSG {0} {1}", channel, "Google Search Failed");
				}
			}
			return F("PRIVMSG {0} {1}", channel, "Unknown Command");
		}

		public string Calc (string args) {
			string[] sorted = Regex.Split(args, "(\\()|(\\))|(\\+)|(-)|(\\*)|(\\/)|(\\^)");
			iNumber result;
			try { result = Calculate.IMathematicalList(null); return result.ToString(); }
			catch { return "Syntax Error"; }
		}

		public string Google (string par) {
			string content = CLI.DownloadString("https://www.google.com/search?q="+par);
			int place = content.IndexOf("<h3 class=\"r\">");
			int href = content.IndexOf("href=\"/url?q=", place) + 13;
			int endref = content.IndexOf("&amp", href);
			return content.Substring(href, endref - href);
		}

		public static bool emptykiller (string s) {
			return (s.Contains(" ") || (s == ""));
		}
	}

}

