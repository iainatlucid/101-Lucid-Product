using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Routing
{
    public class Route
    {
        public Guid Guid { get; set; }
        public RouteType RouteType { get; set; }
        public List<Input> Inputs { get; set; }
        public List<Output> Outputs { get; set; }
    }
    public enum RouteType
    {
        Audio,
        Video,
        Control,
        Usb
    }
}
