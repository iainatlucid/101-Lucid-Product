using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

namespace WinFormsDesigner
{
	public partial class MainForm : Form
	{
		private enum MouseOp { NOTHING, RECT, LINE };     //What the mouse is drawing
		private MouseOp mouseop = MouseOp.NOTHING;
		private int clickx = -1, clicky = -1;               //where the MouseDown event occurred (-1 on MouseUp)
		private int currentx, currenty;                     //where the mouse is now
		private int lastx, lasty;                           //where the mouse was last event
		private Symbol symbol = new Symbol();               //for testing if symbols overlap, this one is moved but never drawn; also as a holder whilst drawing before committing to the symbol list
		private IEnumerable<Symbol> moving;                 //any symbols being dragged right now
		public static MainForm Instance { get; set; }
		[JsonProperty]
		public static List<Symbol> Symbols = new List<Symbol>();  //The blocks representing devices
		public static void Route(string sourcename, string destname)
		{
			Device dest = (from s in Symbols where s.Name.Equals(destname) select s).FirstOrDefault();
			if (dest == null)
			{
				Console.WriteLine("Bad destination");
				return;
			}
			var route = dest.TraceRoute(sourcename);
			if (route != null)
			{
				route.Enumerate();
				Console.WriteLine("Route: {0}", route.ToString());
			}
			//foreach (var s in Symbols)
			//	s.Mask = true;
			Instance.Invalidate();
		}
		public static void ClearRoute()
		{
			foreach (var s in MainForm.Symbols)
			{
				s.ClearRoute();
				//s.Mask = false; 
			}
			Instance.Invalidate();
		}
		public MainForm()
		{
			InitializeComponent();
		}
		private void MainForm_Load(object sender, EventArgs e)
		{
			DoubleBuffered = true;          //prevents jerky refresh
			Size = new Size(1250, 500);
		}
		private void MainForm_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				foreach (var s in Symbols)
				{
					var contains = s.Contains(e.X, e.Y);
					if (contains is Device)
					{
						s.Name = Interaction.InputBox("Prompt", "Title", "Default", e.X, e.Y);
						break;
					}
				}
			}
		}
		private void MainForm_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				clickx = e.X; clicky = e.Y;                     //a drag has started
				symbol = new Symbol(clickx, clicky, 0, 0);
				foreach (var s in Symbols)
				{
					var contains = s.Contains(clickx, clicky);
					if (contains is Device)
					{

						s.Moving = true;
						return;
					}
					else if (contains is IO)
					{
						symbol = s;
						s.CurrentNode = (IO)contains;
						mouseop = MouseOp.LINE;
						Point p = s.NodeXY(clickx, clicky);
						clickx = p.X;
						clicky = p.Y;
						return;
					}
				}
				mouseop = MouseOp.RECT;
			}
			else
			{
				foreach (var s in Symbols)
					if (s.Contains(e.X, e.Y) is Device)
					{
						s.RemoveConnections();
						Symbols.Remove(s);
						break;
					}
			}
		}
		private void MainForm_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (moving.Any())
					moving.First().Moving = false;
				else if (mouseop == MouseOp.RECT)
				{
					var overlaps = from s in Symbols where s.Overlaps(symbol) select s;
					if (!overlaps.Any() && clickx != currentx && clicky != currenty)
					{
						symbol.Name = string.Format("Device {0}", Symbol.ID);
						Symbols.Add(symbol);
					}
				}
				else if (mouseop == MouseOp.LINE)
				{
					foreach (var s in Symbols)
					{
						var contains = s.Contains(currentx, currenty);
						if (contains is IO)
						{
							((IO)contains).Connect(symbol.CurrentNode);
							mouseop = MouseOp.NOTHING;
							symbol.CurrentNode = null;
						}
					}
				}
			}
			clickx = clicky = -1;   //tell program the drag has finished
			Refresh();
			foreach (var s in Symbols)
				s.Tokenise();
			var json = JsonConvert.SerializeObject(Symbols, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
			if (File.Exists(Directory.GetCurrentDirectory() + @"\..\..\symbols.json"))
			{
				try { File.Delete(Directory.GetCurrentDirectory() + @"\..\..\symbols.json"); }
				catch (Exception x) { Console.WriteLine(">>> EXCEPTION: serialise(): Cannot delete file: \r\n{0}", x.StackTrace); }
			}
			using (var file = File.CreateText(Directory.GetCurrentDirectory() + @"\..\..\symbols.json"))
			{
				file.Write(json);
			}
			foreach (var s in Symbols)
				s.Detokenise(Symbols);
			symbol = null;
			mouseop = MouseOp.NOTHING;
		}
		private void MainForm_MouseMove(object sender, MouseEventArgs e)
		{
			lastx = currentx; lasty = currenty;
			currentx = e.X; currenty = e.Y;                         //make current mouse position available
			moving = from s in Symbols where s.Moving select s;
			if (moving.Any())
			{
				symbol.Left = moving.First().Left + currentx - lastx; symbol.Top = moving.First().Top + currenty - lasty;
				symbol.Width = moving.First().Width; symbol.Height = moving.First().Height;
				var overlaps = from s in Symbols where s != moving.First() && s.Overlaps(symbol) select s;
				if (!overlaps.Any())
					moving.First().SetXY(symbol.Left, symbol.Top);
			}
			else if (mouseop == MouseOp.RECT)
			{
				symbol.Left = clickx < currentx ? clickx : currentx;
				symbol.Top = clicky < currenty ? clicky : currenty;
				symbol.Width = Math.Abs(currentx - clickx);
				symbol.Height = Math.Abs(currenty - clicky);
				symbol.ReconcileNodes();
			}
			Refresh();
		}
		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			foreach (var s in Symbols)
				s.Draw(e.Graphics);                                                                                         //have all symbols draw themselves			
			if ((moving != null && moving.Any()) || clickx < 0) return;                                                     //don't draw new one if moving old one OR not drawing
			if (mouseop == MouseOp.RECT)
			{
				var overlaps = from s in Symbols where s.Overlaps(symbol) select s;                                           //test all symbols for overlap
				if (clickx < currentx)
					symbol.SetOutputsX(currentx);
				else
					symbol.SetInputsX(currentx);
				symbol.ResetNodesY();
				symbol.Draw(e.Graphics, overlaps.Any() ? Color.Red : Color.Green);                                                 //draw new symbol and take care of backwards dragging
			}
			else if (mouseop == MouseOp.LINE)
				e.Graphics.DrawLine(new Pen(Color.Black, 1), clickx, clicky, currentx, currenty);
		}
	}
}
