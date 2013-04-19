using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Ircey
{
	public delegate string FormatDelegate (string format, params string[] list);
	public class CommandControl {
		StopwatchManager SM = new StopwatchManager();
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
				return F("PRIVMSG {0} Fuck You {1}.", channel, divargs[0].Split(new char[]{'!'})[0].Trim(new char[]{':',' '}));
			case "dice":
				try {
					try {
						return F("PRIVMSG {0} {1} rolls the dice! {2}!", channel, divargs[0].Split(new char[]{'!'})[0].Trim(new char[]{':',' '}), new Random().Next(Convert.ToInt32(divargs[4]),Convert.ToInt32(divargs[5])+1).ToString());
					} catch {
						return F("PRIVMSG {0} {1} rolls the dice! {2}!", channel, divargs[0].Split(new char[]{'!'})[0].Trim(new char[]{':',' '}), new Random().Next(1,Convert.ToInt32(divargs[4])+1).ToString());
					}
				} catch {
					return F("PRIVMSG {0} {1} rolls the dice! {2}!", channel, divargs[0].Split(new char[]{'!'})[0].Trim(new char[]{':',' '}), new Random().Next(1,7).ToString());
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
			case "stopwatch":
				try {
					if (SM.LookupStopwatch(fuseargs)) {
						TimeSpan dtDelta = SM.StopStopwatch(fuseargs);
						return F("PRIVMSG {0} {1}", channel, F("Stopwatch {0} Stopped: {1} Hours, {2} Minutes, {3} Seconds, {4} Milliseconds", fuseargs,  dtDelta.Hours.ToString(), dtDelta.Minutes.ToString(), dtDelta.Seconds.ToString(), dtDelta.Milliseconds.ToString()));
					} else {
						SM.StartStopwatch(fuseargs);
						return F("PRIVMSG {0} Stopwatch {1} Started", channel, fuseargs);
					}
				} catch {
					return F("PRIVMSG {0} {1}", channel, "Stopwatch Operation Failed");
				}
			}
			return F("PRIVMSG {0} {1}", channel, "Unknown Command");
		}

		public string Calc (string args) {
			List<string> sorted = new List<string>(); sorted.AddRange(Regex.Split(args.Replace(" ",""), "(\\()|(\\))|(\\+)|(-)|(\\*)|(\\/)|(\\^)"));
			sorted.RemoveAll(emptykiller);
			List<IMathematical> eq = new List<IMathematical>();
			for (int i=0;i<sorted.Count;i++) {
				switch (sorted[i]) {
				case "-":
					try { if ((eq[i-1].Callsign() == 'c' && ((iContainer)eq[i-1]).open == true) || i == 0) {eq.Add(iFunction.Negative);} else {eq.Add(iOperator.Subtraction);} } catch { if (i == 0) {eq.Add(iFunction.Negative);} else {eq.Add(iOperator.Subtraction);} }
					break;
				default:
					try {
						eq.Add(new iNumber(Convert.ToDouble(sorted[i])));
					} catch {
						try {
							bool q = false;
							foreach (iContainer co in iContainer.StandardContainers) {
								if (sorted[i] == co.sign) {
									eq.Add(co);
									q = true;
									break;
								}
							} if(q){continue;}
							foreach (iFunction fu in iFunction.StandardFunctions) {
								if (sorted[i] == fu.sign) {
									eq.Add(fu);
									q = true;
									break;
								}
							} if(q){continue;}

							foreach (iOperator op in iOperator.StandardOperators) {
								if (sorted[i] == op.sign) {
									eq.Add(op);
									q = true;
									break;
								}
							} if(q){continue;}
							if (!q) {throw new Exception();}
						} catch (Exception e) {
							Console.WriteLine(e);
							return "Syntax Error";
						} 
					}
					break;
				}
			}
#if DEBUG
			foreach (IMathematical m in eq) {
				Console.WriteLine(m.ToString() + " " + m.Callsign());
			}
#endif
			iNumber result;
			try { result = Calculate.IMathematicalList(eq); return result.ToString(); }
			catch (Exception e) {Console.WriteLine(e); return "Syntax Error"; }
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

