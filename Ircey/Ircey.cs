using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Ircey
{
	struct IRCConfig {
		public string server;
		public short port;
		public string name;
	}

	class MainThread {
		public static void Main (string[] args)
		{
			//MainThread.StandardInit(args);
			MainThread.TestInit(args);
		}
		public static void StandardInit (string[] args) {
			IRCConfig conf = new IRCConfig();
			conf.name = "Ircey";
			if (args.Length > 0) {
				conf.server = args[0];
			} else {
				Console.WriteLine("Enter Server IP:"); conf.server = Console.ReadLine();
			}
			if (args.Length > 1) {
				conf.port = Convert.ToInt16(args[1]);
			} else {
				Console.WriteLine("Enter Server Port:"); conf.port = Convert.ToInt16(Console.ReadLine());
			}
			Ircey I;
			if (args.Length > 2) {
				I = new Ircey(conf, args[2]);
			} else {
				I = new Ircey(conf, null);
			}
			Console.WriteLine("Bot quit/crashed");
		}
		public static void TestInit (string[] args) {
			CommandControl cmd = new CommandControl("#");
			while (true) {
				string todo = Console.ReadLine();
				Console.WriteLine(cmd.CommandSwitch("math", todo, new string[]{}));
			}
		}
	}

	class Ircey {
		TcpClient IRCConnection = null;
		IRCConfig config;
		NetworkStream ns = null;
		StreamReader sr = null;
		StreamWriter sw = null;
		bool shouldRun = true;
		string channel;
		CommandControl cmd;
		public Ircey(IRCConfig config, string defchan = null) {
			this.config = config;
			try {
				IRCConnection = new TcpClient(config.server, config.port);
			}
			catch {
				Console.WriteLine("Connection Error");
			}
			
			try {
				ns = IRCConnection.GetStream();
				sr = new StreamReader(ns);
				sw = new StreamWriter(ns);
				sendData(String.Format("USER {0} snsys.us snsys.us : {1}", config.name, config.name));
				sendData(String.Format("NICK {0}", config.name));
				ConsoleDaemon(defchan);
			}
			catch {
				Console.WriteLine("Communication error");
			}
			finally {
				if (sr != null)
					sr.Close();
				if (sw != null)
					sw.Close();
				if (ns != null)
					ns.Close();
				if (IRCConnection != null)
					IRCConnection.Close();
			}
		}
		public void sendData(string cmd) {
				sw.WriteLine(cmd);
				sw.Flush();
				Console.WriteLine(cmd);
		}

		public void ConsoleDaemon (string defchan) {
			Thread IRCReader = new Thread(IRCWork);
			IRCReader.Start();
			sendData(String.Format("PRIVMSG NickServ identify IRCEYBOT"));
			Console.ReadLine();
			if (defchan == null) {
				Console.WriteLine("\nSPECIFY CHANNEL TO JOIN\n\n");
				channel = Console.ReadLine();
				sendData(String.Format("JOIN {0}", channel));
			} else {
				channel = defchan;
				sendData(String.Format("JOIN {0}", defchan));
			}
			cmd = new CommandControl(channel);
			cmd.Google("Init");
			while(shouldRun) {
				string c = Console.ReadLine();
				if (c.ToLower().Contains("quit")) {
					shouldRun = false;
				}
				sendData(c);
			}
		}

		public void IRCWork() {
			string[] ex;
			string data;
			while (shouldRun)
			{
				try {
					data = sr.ReadLine();
					Console.WriteLine(data);
					char[] charSeparator = new char[] { ' ' };
					ex = data.Split(charSeparator);
					string stringargs = "";
					for(int i=4;i<ex.Length;i++) {
						stringargs += " " + ex[i];
					}
					if (stringargs != "") {stringargs = stringargs.Substring(1);}
					
					if (ex[0] == "PING"){
						sendData(String.Format("PONG {0}", ex[1]));
					} else if (ex.Length > 3 && channel != null) {
						string command = ex[3];
						if (command == ":!fuckyou") {
							sendData(String.Format("PRIVMSG {0} {1}", channel, "What the fuck did you just fucking say about me, "+ex[0].Split(new char[]{'!',})[0].Trim(new char[]{':',' '})+"? I’ll have you know I graduated top of my class in the C# IDE, and I’ve been involved in numerous secret raids on IRC Networks, and I have over 300 confirmed ragequits."));
							sendData(String.Format("PRIVMSG {0} {1}", channel, "I am trained in gorilla warfare and I’m the top spammer in the entire Internet. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Network, mark my fucking words."));
							sendData(String.Format("PRIVMSG {0} {1}", channel, "You think you can get away with saying that shit to me over this network? Think again, fucker. As we speak I am contacting my secret network of IRC bots across the Internet and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life."));
							sendData(String.Format("PRIVMSG {0} {1}", channel, "You’re fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that’s just with my bare code. Not only am I extensively trained in spamming networks, but I have access to the entire arsenal of the Internet Relay Chat Protocol and I will use it to its full extent to wipe your miserable ass off the face of the network, you little shit."));
							sendData(String.Format("PRIVMSG {0} {1}", channel, "If only you could have known what unholy retribution your little “clever” comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn’t, you didn’t, and now you’re paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You’re fucking dead, kiddo."));
						} else if (command.Contains(":!")) {
							sendData(cmd.CommandSwitch(command.Substring(2), stringargs, ex));
						}
					}
				} catch (Exception e) {
					Console.WriteLine(e);
				}
			}
		}
	}
}
