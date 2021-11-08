using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Routing
{
    public interface IRoutable
    {
        Guid Guid { get; set; }
        List<Route> Routes { get; set; }

        bool MakeRoute<T>(Guid guid, List<Guid> guids);
    }
}
