using System;
using System.Drawing;

namespace WinFormsDesigner
{
	public class InputNode : Input
	{
		public int X, Y;
		public bool Mask;
		public InputNode(Device d, int i, int x, int y)
		{
			Device = d; ID = i; X = x; Y = y;
		}
		public void Draw(Graphics g, Pen p)
		{
			p.Color = Color.Black;
			g.DrawEllipse(p, X, Y, Symbol.NODE_DIAMETER, Symbol.NODE_DIAMETER);
			if (Route != null)
			{
				p.Color = Color.Red;
				g.DrawLine(p, X + Symbol.NODE_DIAMETER / 2, Y + Symbol.NODE_DIAMETER / 2
					, ((OutputNode)Route).X + Symbol.NODE_DIAMETER / 2, ((OutputNode)Route).Y + Symbol.NODE_DIAMETER / 2);
			}
			else if (Connection != null && !Mask)
			{
				if (((IO)Connection).Route == null)
					g.DrawLine(p, X + Symbol.NODE_DIAMETER / 2, Y + Symbol.NODE_DIAMETER / 2
						, ((OutputNode)Connection).X + Symbol.NODE_DIAMETER / 2, ((OutputNode)Connection).Y + Symbol.NODE_DIAMETER / 2);
			}
			p.Color = Color.Black;
		}
	}
	public class OutputNode : Output
	{
		public int X, Y;
		public OutputNode(Device d, int i, int x, int y)
		{
			Device = d; ID = i; X = x; Y = y;
		}
		public void Draw(Graphics g, Pen p)
		{
			p.Color = Color.Black;
			g.DrawEllipse(p, X, Y, Symbol.NODE_DIAMETER, Symbol.NODE_DIAMETER);
			p.Color = Color.Red;
			if (Route != null)
				g.DrawLine(p, X + Symbol.NODE_DIAMETER / 2, Y + Symbol.NODE_DIAMETER / 2
					, ((InputNode)Route).X + Symbol.NODE_DIAMETER / 2, ((InputNode)Route).Y + Symbol.NODE_DIAMETER / 2);
			p.Color = Color.Black;
		}
	}
}
