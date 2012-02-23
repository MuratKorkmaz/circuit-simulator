using System.Collections;

namespace JavaToSharp
{
    public class CircuitNode
    {
        internal int x, y;
        internal readonly ArrayList links;
        internal bool @internal;
        internal CircuitNode()
        {
            links = new ArrayList();
        }
    }
}
