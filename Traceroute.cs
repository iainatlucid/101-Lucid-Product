using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsDesigner
{
	public class Traceroute
	{
		List<Element> hops;
		public Traceroute(Element e)
		{
			hops = new List<Element>();
			Add(e);
		}
		public Traceroute Add(Element e) { hops.Add(e); return this; }
		public void Enumerate()
		{
			IO lastio = null;
			foreach (var h in hops)
			{
				if (h is IO)
				{
					if (lastio != null)
						lastio.Route = (IO)h;
					lastio = (IO)h;
				}
			}
		}
		public new string ToString()
		{
			StringBuilder route = new StringBuilder();
			foreach (var h in hops)
			{
				if (h is Device)
					route.Append(((Device)h).Name + "/");
				else if (h is Input)
					route.Append(((Input)h).Name);
				else if (h is Output)
					route.Append(((Output)h).Name);
			}
			return route.ToString().Trim(new char[] { '/' });
		}
	}
}
