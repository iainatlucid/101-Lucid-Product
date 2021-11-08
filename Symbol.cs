using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WinFormsDesigner
{
	public class Symbol : Device
	{
		public static readonly int NODE_SPACING = 15, NODE_DIAMETER = 12;
		public bool Mask { set { foreach (var i in inputs) ((InputNode)i).Mask = value; } }
		private static int id = 1;
		public static int ID { get { return id++; } }
		[JsonIgnore]
		public bool Moving { get; set; }
		public int Left, Top, Width, Height;
		[JsonIgnore]
		public IO CurrentNode = null;
		public Symbol() { }
		public Symbol(int l, int t, int w, int h) { Left = l; Top = t; Width = w; Height = h; }
		public void Tokenise()
		{
			foreach (var i in inputs)
				((Input)i).Tokenise();
			foreach (var o in outputs)
				((Output)o).Tokenise();
		}
		public void Detokenise(List<Symbol> symbols)
		{
			foreach (var i in inputs)
				((Input)i).Detokenise(symbols, this);
			foreach (var o in outputs)
				((Output)o).Detokenise(symbols, this);
		}
		public void RemoveConnections()
		{
			for (int i = 0; i < inputs.Count; i++)
			{
				if (inputs[i] != null && ((Input)inputs[i]).Connection != null && ((IO)((Input)inputs[i]).Connection).Connection != null)
					inputs[i] = ((Input)inputs[i]).Connection = ((IO)((Input)inputs[i]).Connection).Connection = null;
				if (outputs[i] != null && ((Output)outputs[i]).Connection != null && ((IO)((Output)outputs[i]).Connection).Connection != null)
					outputs[i] = ((Output)outputs[i]).Connection = ((IO)((Output)outputs[i]).Connection).Connection = null;
			}
		}
		public void ReconcileNodes()
		{
			int diff = Height / NODE_SPACING - inputs.Count;
			for (int i = 0; i < diff; i++)
			{
				inputs.Add(new InputNode(this, inputs.Count, Left - NODE_DIAMETER / 2, NODE_SPACING / 2 + Top + NODE_SPACING * inputs.Count));
				outputs.Add(new OutputNode(this, outputs.Count, Left + Width - NODE_DIAMETER / 2, NODE_SPACING / 2 + Top + NODE_SPACING * outputs.Count));
			}
			for (int i = 0; i > diff; i--)
			{
				inputs.RemoveAt(inputs.Count - 1);
				outputs.RemoveAt(outputs.Count - 1);
			}
		}
		public void ClearRoute()
		{
			foreach (var i in inputs)
				((IO)i).Route = null;
			foreach (var o in outputs)
				((IO)o).Route = null;
		}
		public void ResetNodesY()
		{
			for(int i=0;i<inputs.Count;i++)
			{
				((InputNode)inputs[i]).Y = NODE_SPACING / 2 + Top + NODE_SPACING * i;
				((OutputNode)outputs[i]).Y = NODE_SPACING / 2 + Top + NODE_SPACING * i;
			}
		}
		private int Distance(int x1, int y1, int x2, int y2)
		{
			return (int)Math.Sqrt(Math.Abs(x2 - x1) * Math.Abs(x2 - x1) + Math.Abs(y2 - y1) * Math.Abs(y2 - y1));
		}
		public Point NodeXY(int x, int y)
		{
			for (int i = 0; i < inputs.Count; i++)
			{
				int nodey = Top + NODE_SPACING / 2 + NODE_DIAMETER / 2 + NODE_SPACING * i;
				if (Distance(x, y, Left, nodey) < NODE_DIAMETER / 2)
					return new Point(Left, nodey);
				if (Distance(x, y, Left + Width, nodey) < NODE_DIAMETER / 2)
					return new Point(Left + Width, nodey);
			}
			return new Point();
		}
		public void Draw(params object[] args)
		{
			var g = (Graphics)args[0];
			var p = new Pen(args.Length > 1 ? ((Color)args[1]) : Color.Black, 1);
			g.DrawRectangle(p, new Rectangle(Left, Top, Width, Height));
			System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
			g.DrawString(Name, new Font("Arial", 12), new SolidBrush(Color.Black), Left + 20, Top + 3, drawFormat);
			foreach (var i in inputs)
				((InputNode)i).Draw(g, p);
			foreach (var o in outputs)
				((OutputNode)o).Draw(g, p);
		}
		public void SetXY(int left, int top)
		{
			Left = left; Top = top;
			SetInputsX(Left);
			SetOutputsX(Left + Width);
			for (int i = 0; i < inputs.Count; i++)
				((InputNode)inputs[i]).Y = ((OutputNode)outputs[i]).Y = NODE_SPACING / 2 + Top + NODE_SPACING * i;
		}
		public void SetOutputsX(int x)
		{
			foreach (var o in outputs)
				((OutputNode)o).X = x - Symbol.NODE_DIAMETER / 2;
		}
		public void SetInputsX(int x)
		{
			foreach (var i in inputs)
				((InputNode)i).X = x - Symbol.NODE_DIAMETER / 2;
		}
		public object Contains(int x, int y)
		{
			int dist;
			for (int n = 0; n < inputs.Count; n++)
			{
				int nodey = Symbol.NODE_SPACING / 2 + n * Symbol.NODE_SPACING + Top + Symbol.NODE_DIAMETER / 2;
				dist = Distance(x, y, Left, nodey);
				if (dist < Symbol.NODE_DIAMETER / 2)
					return (from i in inputs where ((Input)i).ID == n select i).FirstOrDefault();
				dist = Distance(x, y, Left + Width, nodey);
				if (dist < Symbol.NODE_DIAMETER / 2)
					return (from o in outputs where ((Output)o).ID == n select o).FirstOrDefault();
			}
			return (x > Left && x < (Left + Width) && y > Top && y < (Top + Height)) ? this : null;
		}
		public bool Overlaps(Symbol s)
		{
			return
			(Contains(s.Left, s.Top) is Device) ||
			(Contains(s.Left + s.Width, s.Top) is Device) ||
			(Contains(s.Left, s.Top + s.Height) is Device) ||
			(Contains(s.Left + s.Width, s.Top + s.Height) is Device) ||
			(s.Contains(Left, Top) is Device) ||
			(s.Contains(Left + Width, Top) is Device) ||
			(s.Contains(Left, Top + Height) is Device) ||
			(s.Contains(Left + Width, Top + Height) is Device) ||
			(s.Top < Top && s.Left > Left && (s.Left + s.Width < Left + Width) && (s.Top + s.Height > Top + Height)) ||
			(s.Top > Top && s.Left < Left && (s.Left + s.Width > Left + Width) && (s.Top + s.Height < Top + Height))
			;
		}
	}
}
