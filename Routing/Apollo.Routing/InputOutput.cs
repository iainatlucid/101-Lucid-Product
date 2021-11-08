using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Routing
{
    public abstract class InputOutput
    {
        public Guid Guid { get; set; }
        public List<uint> ConnectionNodes { get; set; }
    }
}
