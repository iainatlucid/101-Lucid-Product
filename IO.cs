using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinFormsDesigner
{
	public class IO : Element
	{
		public int ID;
		[JsonIgnore]
		public object Device;
		public object Connection;
		[JsonIgnore]
		public IO Route;
		public virtual Traceroute TraceRoute(string source) { return null; }
		public void Connect(IO to)
		{
			if (GetType().IsAssignableFrom(to.GetType()) && to.GetType().IsAssignableFrom(this.GetType()))
				return;
			Connection = to;
			to.Connection = this;
		}
		public virtual void PrintRoute() { }
		public void Tokenise()
		{
			if (Connection == null)
				return;
			Device d = (Device)((IO)Connection).Device;
			if (d == null)
				return;
			Connection = string.Format("{0}:{1}", d.Name, ((IO)Connection).ID);
		}
		public void Detokenise(List<Symbol> devices, Device d)
		{
			if (Connection == null) return;
			Device = d;
			string[] split = Connection.ToString().Split(':');
			if (split.Length < 2) return;
			Device far = (from dev in devices where dev.Name.Equals(split[0]) select dev).FirstOrDefault();
			int i = int.Parse(split[1]);
			if (this is Input)
				Connection = (IO)far.outputs[i];
			else
				Connection = (IO)far.inputs[i];
		}
	}
	public class Input : IO
	{
		public string Name { get { return "Input " + (ID + 1).ToString() + "/"; } }
		public override void PrintRoute()
		{
			if (Route == null || Device == null) return;
			Console.WriteLine("Hop: {0}", ((Device)Device).Name + "/" + Name);
			Route.PrintRoute();
		}
		public override Traceroute TraceRoute(string source)
		{
			if (Connection == null) return null;
			Traceroute route;
			route = ((IO)Connection).TraceRoute(source);
			if (route != null)
				return route.Add(this);
			return null;
		}
	}
	public class Output : IO
	{
		public string Name { get { return "Output " + (ID + 1).ToString() + " > "; } }
		public override void PrintRoute()
		{
			if (Route == null || Device == null) return;
			Console.WriteLine("Hop: {0}", ((Device)Device).Name + "/" + Name);
			Route.PrintRoute();
		}
		public override Traceroute TraceRoute(string source)
		{
			if (Device == null) return null;
			Traceroute route = ((Device)Device).TraceRoute(source);
			if (route != null)
				return route.Add(this);
			return null;
		}
	}
}