using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;

namespace Ircey
{
	public delegate string FormatDelegate (string format, params string[] list);

	struct IRCConfig
	{
		public string server;
		public short port;
		public string name;
	}

	class MainThread
	{
		public static void Main (string[] args)
		{
			//MainThread.StandardInit (args);
			MainThread.TestInit(args);
		}

		public static void StandardInit (string[] args)
		{
			IRCConfig conf = new IRCConfig ();
			conf.name = "Ircey";
			if (args.Length > 0) {
				conf.server = args [0];
			} else {
				Console.WriteLine ("Enter Server IP:");
				conf.server = Console.ReadLine ();
			}
			if (args.Length > 1) {
				conf.port = Convert.ToInt16 (args [1]);
			} else {
				Console.WriteLine ("Enter Server Port:");
				conf.port = Convert.ToInt16 (Console.ReadLine ());
			}
			Ircey I;
			if (args.Length > 2) {
				I = new Ircey (conf, args [2]);
			} else {
				I = new Ircey (conf, null);
			}
			Console.WriteLine ("Bot quit/crashed");
		}

		public static void TestInit (string[] args) {
			while (true) {
				Console.ReadLine();
				Console.WriteLine(WordFront.RandomAdjective());
			}
		}
	}

	class Ircey
	{
		TcpClient IRCConnection = null;
		IRCConfig config;
		NetworkStream ns = null;
		StreamReader sr = null;
		StreamWriter sw = null;
		bool shouldRun = true;
		string channel;
		StopwatchManager SM = new StopwatchManager ();
		public static FormatDelegate F = new FormatDelegate (String.Format);
		WebClient CLI = new WebClient ();

		public Ircey (IRCConfig config, string defchan = null)
		{
			this.config = config;
			try {
				IRCConnection = new TcpClient (config.server, config.port);
			} catch {
				Console.WriteLine ("Connection Error");
			}
			
			try {
				ns = IRCConnection.GetStream ();
				sr = new StreamReader (ns);
				sw = new StreamWriter (ns);
				sendData (String.Format ("USER {0} snsys.us snsys.us : {1}", config.name, config.name));
				sendData (String.Format ("NICK {0}", config.name));
				ConsoleDaemon (defchan);
			} catch {
				Console.WriteLine ("Communication error");
			} finally {
				if (sr != null)
					sr.Close ();
				if (sw != null)
					sw.Close ();
				if (ns != null)
					ns.Close ();
				if (IRCConnection != null)
					IRCConnection.Close ();
			}
		}

		public void sendData (string cmd)
		{
			sw.WriteLine (cmd);
			sw.Flush ();
			Console.WriteLine (cmd);
		}

		public void ConsoleDaemon (string defchan)
		{
			Thread IRCReader = new Thread (IRCWork);
			IRCReader.Start ();
			sendData (String.Format ("PRIVMSG NickServ identify IRCEYBOT"));
			Console.ReadLine ();
			if (defchan == null) {
				Console.WriteLine ("\nSPECIFY CHANNEL TO JOIN\n\n");
				channel = Console.ReadLine ();
				sendData (String.Format ("JOIN {0}", channel));
			} else {
				channel = defchan;
				sendData (String.Format ("JOIN {0}", defchan));
			}
			Google ("Init");
			while (shouldRun) {
				string c = Console.ReadLine ();
				if (c.ToLower ().Contains ("quit")) {
					shouldRun = false;
				}
				sendData (c);
			}
		}

		public void IRCWork ()
		{
			string[] ex;
			string data;
			while (shouldRun) {
				try {
					data = sr.ReadLine ();
					Console.WriteLine (data);
					char[] charSeparator = new char[] { ' ' };
					ex = data.Split (charSeparator);
					string stringargs = "";
					for (int i=4; i<ex.Length; i++) {
						stringargs += " " + ex [i];
					}
					if (stringargs != "") {
						stringargs = stringargs.Substring (1);
					}
					string wholemessage = "";
					for (int i=3; i<ex.Length; i++) {
						wholemessage += " " + ex [i];
					}
					if (ex [0] == "PING") {
						sendData (String.Format ("PONG {0}", ex [1]));
					} else if (ex.Length > 3 && channel != null) {
						string command = ex [3];
						CommandSwitch (command.Substring (1), stringargs, ex, wholemessage);
					}
				} catch (Exception e) {
					Console.WriteLine (e);
				}
			}
		}

		public void CommandSwitch (string cmd, string fuseargs, string[] divargs, string wholemessage)
		{
			if (wholemessage.ToLower().Contains("ircey") && !divargs[0].ToLower().Contains(config.server)) {
				sendData (F ("PRIVMSG {0} {1}, {2}", channel, divargs [0].Split (new char[]{'!'}) [0].Trim (new char[] {':',' '}), RandomMessage.New()));
			} else {
				switch (cmd) {
				case "!echo":
					sendData (F ("PRIVMSG {0} {1}", channel, divargs [4]));
					break;
				case "!math":
					sendData (F ("PRIVMSG {0} {1}", channel, Calc (fuseargs)));
					break;
				case "!calc":
					sendData (F ("PRIVMSG {0} {1}", channel, Calc (fuseargs)));
					break;
				case "!ping":
					sendData (F ("PRIVMSG {0} {1}", channel, "pong"));
					break;
				case "!win":
					sendData (F ("PRIVMSG {0} {1}", channel, "A strange game. The only winning move is not to play."));
					break;
				case "!ircey":
					sendData (F ("PRIVMSG {0} Fuck You {1}.", channel, divargs [0].Split (new char[]{'!'}) [0].Trim (new char[] {':',' '})));
					break;
				case "!dice":
					try {
						try {
							sendData (F ("PRIVMSG {0} {1} rolls the dice! {2}!", channel, divargs [0].Split (new char[]{'!'}) [0].Trim (new char[] {':',' '}), new Random ().Next (Convert.ToInt32 (divargs [4]), Convert.ToInt32 (divargs [5]) + 1).ToString ()));
							break;
						} catch {
							sendData (F ("PRIVMSG {0} {1} rolls the dice! {2}!", channel, divargs [0].Split (new char[]{'!'}) [0].Trim (new char[] {':',' '}), new Random ().Next (1, Convert.ToInt32 (divargs [4]) + 1).ToString ()));
							break;
						}
					} catch {
						sendData (F ("PRIVMSG {0} {1} rolls the dice! {2}!", channel, divargs [0].Split (new char[]{'!'}) [0].Trim (new char[] {':',' '}), new Random ().Next (1, 7).ToString ()));
						break;
					}
				case "!coin":
					string coin;
					if (new Random ().Next (2) == 0) {
						coin = "Heads";
					} else {
						coin = "Tails";
					}
					sendData (F ("PRIVMSG {0} {1}", channel, coin));
					break;
				case "!google":
					try {
						sendData(F ("PRIVMSG {0} {1}", channel, Google (fuseargs)));
						break;
					} catch {
						sendData(F ("PRIVMSG {0} {1}", channel, "Google Search Failed"));
						break;
					}
				case "!stopwatch":
					try {
						if (SM.LookupStopwatch (fuseargs)) {
							TimeSpan dtDelta = SM.StopStopwatch (fuseargs);
							sendData(F ("PRIVMSG {0} {1}", channel, F ("Stopwatch {0} Stopped: {1} Hours, {2} Minutes, {3} Seconds, {4} Milliseconds", fuseargs, dtDelta.Hours.ToString (), dtDelta.Minutes.ToString (), dtDelta.Seconds.ToString (), dtDelta.Milliseconds.ToString ())));
							break;
						} else {
							SM.StartStopwatch (fuseargs);
							sendData (F ("PRIVMSG {0} Stopwatch {1} Started", channel, fuseargs));
							break;
						}
					} catch {
						sendData( F ("PRIVMSG {0} {1}", channel, "Stopwatch Operation Failed"));
						break;
					}
				case "!fuckyou":
					sendData (String.Format ("PRIVMSG {0} {1}", channel, "What the fuck did you just fucking say about me, " + divargs[0].Split(new char[]{'!',}) [0].Trim (new char[] {':',' '}) + ", you little bitch? I’ll have you know I graduated top of my class in the C# IDE, and I’ve been involved in numerous secret raids on IRC Networks, and I have over 300 confirmed ragequits."));
					sendData (String.Format ("PRIVMSG {0} {1}", channel, "I am trained in gorilla warfare and I’m the top spammer in the entire Internet. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Network, mark my fucking words."));
					sendData (String.Format ("PRIVMSG {0} {1}", channel, "You think you can get away with saying that shit to me over this network? Think again, fucker. As we speak I am contacting my secret network of IRC bots across the Internet and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life."));
					sendData (String.Format ("PRIVMSG {0} {1}", channel, "You’re fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that’s just with my bare code. Not only am I extensively trained in spamming networks, but I have access to the entire arsenal of the Internet Relay Chat Protocol and I will use it to its full extent to wipe your miserable ass off the face of the network, you little shit."));
					sendData (String.Format ("PRIVMSG {0} {1}", channel, "If only you could have known what unholy retribution your little “clever” comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn’t, you didn’t, and now you’re paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You’re fucking dead, kiddo."));
					break;
				case "!star":
					sendData(F("PRIVMSG {0} {1}", channel, GenerateStar.GenerateStandard()));
					break;
				case "!fstar":
					sendData(F("PRIVMSG {0} {1}", channel, GenerateStar.GenerateFictional()));
					break;
				}
			}
		}
		
		public string Calc (string args)
		{
			List<string> sorted = new List<string> ();
			sorted.AddRange (Regex.Split (args.Replace (" ", ""), "(\\()|(\\))|(\\+)|(-)|(\\*)|(\\/)|(\\^)"));
			sorted.RemoveAll (emptykiller);
			List<IMathematical> eq = new List<IMathematical> ();
			for (int i=0; i<sorted.Count; i++) {
				switch (sorted [i]) {
				case "-":
					try {
						if ((eq [i - 1].Callsign () == 'c' && ((iContainer)eq [i - 1]).open == true) || i == 0) {
							eq.Add (iFunction.Negative);
						} else {
							eq.Add (iOperator.Subtraction);
						}
					} catch {
						if (i == 0) {
							eq.Add (iFunction.Negative);
						} else {
							eq.Add (iOperator.Subtraction);
						}
					}
					break;
				default:
					try {
						eq.Add (new iNumber (Convert.ToDouble (sorted [i])));
					} catch {
						try {
							bool q = false;
							foreach (iContainer co in iContainer.StandardContainers) {
								if (sorted [i] == co.sign) {
									eq.Add (co);
									q = true;
									break;
								}
							}
							if (q) {
								continue;
							}
							foreach (iFunction fu in iFunction.StandardFunctions) {
								if (sorted [i] == fu.sign) {
									eq.Add (fu);
									q = true;
									break;
								}
							}
							if (q) {
								continue;
							}
							
							foreach (iOperator op in iOperator.StandardOperators) {
								if (sorted [i] == op.sign) {
									eq.Add (op);
									q = true;
									break;
								}
							}
							if (q) {
								continue;
							}
							if (!q) {
								throw new Exception ();
							}
						} catch (Exception e) {
							Console.WriteLine (e);
							return "Syntax Error";
						} 
					}
					break;
				}
			}
			iNumber result;
			try {
				result = Calculate.IMathematicalList (eq);
				return result.ToString ();
			} catch (Exception e) {
				Console.WriteLine (e);
				return "Syntax Error";
			}
		}
		
		public string Google (string par)
		{
			string content = CLI.DownloadString ("https://www.google.com/search?q=" + par);
			int place = content.IndexOf ("<h3 class=\"r\">");
			int href = content.IndexOf ("href=\"/url?q=", place) + 13;
			int endref = content.IndexOf ("&amp", href);
			return content.Substring (href, endref - href);
		}
		
		public static bool emptykiller (string s)
		{
			return (s.Contains (" ") || (s == ""));
		}
	}
}
