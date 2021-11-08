using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Routing
{
    public class Device : Component
    {
        public string Name { get;  set; }
        public Device(ComponentType componentType, string name)
        {
            ComponentType = componentType;
            Name = name;
            Guid = Guid.NewGuid();
        }
    }
}
