using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Routing
{
    public abstract class Component : IRoutable
    {
        public Guid Guid { get; set; }
        public List<Route> Routes { get; set; }
        public ComponentType ComponentType { get; set; }

        public bool MakeRoute<T>(Guid guid, List<Guid> guids)
        {
            try
            {
                //Not implemented.
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public enum ComponentType
    {
        Unknown,
        Display,
        Matrix,
        Source
    }
}
