using System.Collections.Generic;
using System.Text;

namespace WinFormsDesigner
{
	public class Device:Element
	{
		public string Name;
		public List<object> inputs = new List<object>();
		public List<object> outputs = new List<object>();
		public Traceroute TraceRoute(string source)
		{
			if (source.Equals(Name))
				return new Traceroute(this);
			foreach (var i in inputs)
			{
				if (i == null) continue;
				Traceroute route = ((Input)i).TraceRoute(source);
				if (route != null)
					return route.Add(this);
			}
			return null;
		}
	}
}
