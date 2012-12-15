using System;
using Tamir .SharpSsh;
using Org.Mentalis.Security.Certificates;
using System.IO;
using Microsoft.CSharp;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections;

namespace nssh
{
	
	
	class MainClass
	{
		class sshServer {
			public class shellResponse {
				public string name;
				public string[] response;
				public bool error;
			}
		private string name;
        private string username;
		SshShell thisServer;

		public shellResponse write (string text)
			{
				if (thisServer.Connected && thisServer.ShellConnected) {
                    if (text.IndexOf("^C") > -1)
                    {
                        thisServer.Write("17");
                    }
                    else if (text.IndexOf("^Z") > -1)
                    {
                        byte[] buffer = new byte[1];
                        buffer[0]=26;
                        thisServer.Write(buffer);
                        thisServer.WriteLine("");
                    }else 
                    {
                        thisServer.WriteLine(text);
                    }

					shellResponse r = new shellResponse ();
					r.name = this.name;
					r.error = false;
					r.response = thisServer.Expect(new Regex(this.username + "@" + ".*?#")).Split ('\n');
					return r;
				} else {
					shellResponse r = new shellResponse ();
					r.name = this.name;
					r.error = true;
					return r;
			}
		}
		
		public sshServer (string name,string host, string username, string password, string prompt)
		{
				this.name = name;
                this.username = username;
			try {
				thisServer = new SshShell (host, username, password);
				
				thisServer.Connect();
				thisServer.Expect(new Regex(username + "@" + ".*?#"));


					thisServer.RemoveTerminalEmulationCharacters = true;
			} catch (Exception e) {
				Console.WriteLine ("[ERROR] " + e.Message);	
			}
		}
	}

		static ArrayList shells = new ArrayList(); 
		public static int Main (string[] args)
		{
			string file; 
			if (args.Length <= 0 || args [0].Length <= 0)
				file = "default";
			else
				file = args [0];
			
			if (File.Exists (file + ".psh") == false) {
				Console.WriteLine ("Configuration file not found. Exiting (-2)");
				return -2;
			}
				
			StreamReader configfile = new StreamReader (file + ".psh");
			while (configfile.Peek() != -1) {
				string[] readl = configfile.ReadLine ().Split (',');
				//	sshServer tempServer = n
				shells.Add (new sshServer (readl [0], readl [1], readl [2], readl [3], readl [4]));
				Console.WriteLine (DateTime.Now.ToString () + " [" + readl [0] + "] Added...");
			}
			
			while (1==1) {
				Console.WriteLine ();
				Console.Write ("[LOCAL] Enter Command: ");
				string comd = Console.ReadLine ();	
   
				ArrayList responses = new ArrayList ();
				
				
			
				
				foreach (sshServer serv in shells) {
					if (responses.Count == 0) {
                        responses.Add(serv.write(comd));
                    }
					else {
						sshServer.shellResponse resp = serv.write (comd);
						if (resp.response.Length > (responses [0] as sshServer.shellResponse).response.Length) {
							responses.Insert (0, resp);	
						} else {
							responses.Add (resp);
						}
					}
				}
                bool exitNow = false;
                switch (comd.ToLower().Trim())
                {
                    case "exit":
                        exitNow = true;
                        break;
                }
                if (exitNow) break;
				for (int i= 0; i<(responses [0] as sshServer.shellResponse).response.Length; i++) {
					string original_line =Regex.Replace( (responses [0] as sshServer.shellResponse).response [i],"^.*?@" + ".*?#","");
					Console.WriteLine ("Line #" + i + ":\n\t[" + (responses [0] as sshServer.shellResponse).name + "] " + original_line); 
					foreach (sshServer.shellResponse response in responses) {
						
						if (i < response.response.Length && original_line != Regex.Replace(response.response [i],"^.*?@" + ".*?#","")) {
							Console.WriteLine ("\t[" +  response.name + "] " + response.response[i]); 
						}
					}
				}
				/*
				foreach (sshServer.shellResponse s in responses) {
					Console.WriteLine ("---[" + s.name + "]---");
					
					if (s.error == true) {
						Console.WriteLine ("not connected!");	
					} else {
						foreach (string line in s.response) {
							Console.WriteLine (line);
						}
					}
				}
				*/
				responses = null;
			}
			return 1;
		}
	}
}
