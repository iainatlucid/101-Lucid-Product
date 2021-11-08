using System;
using System.Threading;

namespace WinFormsDesigner
{
	public class Terminal
	{
		private Thread thread;
		private bool quit = false;
		public Terminal()
		{
			thread = new Thread(run);
			thread.Start();
		}
		private void run()
		{
			while (!quit)
			{
				string s = Console.ReadLine();
				string[] split = s.Split('>');
				for (int i = 0; i < split.Length; i++)
					split[i]=split[i].Trim();
				if (split.Length < 2)
				{
					if (split[0].ToUpper().Equals("CLEAR"))
						MainForm.ClearRoute();
					else
						Console.WriteLine("Bad route");
				}
				else
					MainForm.Route(split[0], split[1]);
			}
		}
	}
}
