using System.Collections;

namespace JavaToSharp
{
    public class CircuitNode
    {
        internal int x, y;
        internal readonly ArrayList links;
        internal bool Internal;
        internal CircuitNode()
        {
            links = new ArrayList();
        }
    }
}
