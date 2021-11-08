using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WinFormsDesigner
{
	static class Program
	{
		///Make a console available for debug output
		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();
		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		const int SW_HIDE = 0;
		const int SW_SHOW = 5;
		static Terminal terminal;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			ConsoleWindow(true);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			string s;
			using (var file = new StreamReader(new FileStream(Directory.GetCurrentDirectory() + @"\..\..\symbols.json", FileMode.Open)))
				s = file.ReadToEnd();
			var form = new MainForm();
			MainForm.Symbols = JsonConvert.DeserializeObject<List<Symbol>>(s, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
			foreach (var y in MainForm.Symbols)
				y.Detokenise(MainForm.Symbols);
			terminal = new Terminal();
			MainForm.Instance = form;
			Application.Run(form);
		}
		public static void ConsoleWindow(bool b)
		{
			var handle = GetConsoleWindow();
			ShowWindow(handle, b ? SW_SHOW : SW_HIDE);
		}
	}
}
