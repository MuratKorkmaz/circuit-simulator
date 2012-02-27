using System;

namespace JavaToSharp
{
    class FindPathInfo
    {
        private const int INDUCT = 1;
        private const int VOLTAGE = 2;
        private const int SHORT = 3;
        private const int CAP_V = 4;
        private bool[] used;
        private int dest;
        private CircuitElm firstElm;
        private CircuitElm[] elmList;
        private int type;
        private readonly CirSim _simController;

        internal FindPathInfo(CirSim cirSim)
        {
            _simController = cirSim;
        }

        public void TryFindPath(CircuitElm[] elms)
        {
            elmList = elms;
            int size = _simController.nodeList.Count;
            used = new bool[size];
            
            for (int i = 0; i != elmList.Length; i++)
            {
                CircuitElm ce = elms[i];
                dest = ce.getNode(1);
                firstElm = ce;
                if (ce is InductorElm)
                {
                    FindInductPath(ce);
                }
                if (ce is CurrentElm)
                {
                    if (TryFindCurrentSourcesPath(ce)) return;
                }
                // look for voltage source loops
                if ((ce is VoltageElm && ce.PostCount == 2) || ce is WireElm)
                {
                    if (TryFindVoltagePath(ce)) return;
                }
                // look for shorted caps, or caps w/ voltage but no R
                if (ce is CapacitorElm)
                {
                    if (TryFindShortOrCapsVPath(ce)) return;
                }
            }
        }

        private bool TryFindShortOrCapsVPath(CircuitElm ce)
        {
            type = SHORT;
            if (findPath(ce.getNode(0)))
            {
                Console.WriteLine(ce + " shorted");
                ce.reset();
            }
            else
            {
                if (findPath(ce.getNode(0)))
                {
                    _simController.stop("Короткое замыкание конденсатора!", ce);
                    return true;
                }
            }
            return false;
        }

        private bool TryFindVoltagePath(CircuitElm ce)
        {
            if (findPath(ce.getNode(0)))
            {
                _simController.stop("Короткое замыкание источника напряжения!", ce);
                return true;
            }
            return false;
        }

        private bool TryFindCurrentSourcesPath(CircuitElm ce)
        {
            if (!findPath(ce.getNode(0)))
            {
                _simController.stop("Нет пути для источника тока!", ce);
                return true;
            }
            return false;
        }

        private void FindInductPath(CircuitElm ce)
        {
            // first try findPath with maximum depth of 5, to avoid slowdowns
            if (!findPath(ce.getNode(0), 5) && !findPath(ce.getNode(0)))
            {
                Console.WriteLine(ce + " нет пути");
                ce.reset();
            }
        }

        private bool findPath(int n1)
        {
            return findPath(n1, -1);
        }

        private bool findPath(int n1, int depth)
        {
            if (n1 == dest)
                return true;
            if (depth-- == 0)
                return false;
            if (used[n1])
            {
                //System.out.println("used " + n1);
                return false;
            }
            used[n1] = true;
            int i;
            for (i = 0; i != elmList.Length; i++)
            {
                CircuitElm ce = elmList[i];
                if (ce == firstElm)
                    continue;
                if (type == INDUCT)
                {
                    if (ce is CurrentElm)
                        continue;
                }
                if (type == VOLTAGE)
                {
                    if (!(ce.isWire || ce is VoltageElm))
                        continue;
                }
                if (type == SHORT && !ce.isWire)
                    continue;
                if (type == CAP_V)
                {
                    if (!(ce.isWire || ce is CapacitorElm || ce is VoltageElm))
                        continue;
                }
                if (n1 == 0)
                {
                    // look for posts which have a ground connection;
                    // our path can go through ground

                    for (int l = 0; l != ce.PostCount; l++)
                    {
                        if (ce.hasGroundConnection(l) && findPath(ce.getNode(l), depth))
                        {
                            used[n1] = false;
                            return true;
                        }
                    }
                }
                int j;
                for (j = 0; j != ce.PostCount; j++)
                {
                    if (ce.getNode(j) == n1)
                        break;
                }
                if (j == ce.PostCount)
                    continue;
                if (ce.hasGroundConnection(j) && findPath(0, depth))
                {
                    used[n1] = false;
                    return true;
                }
                if (type == INDUCT && ce is InductorElm)
                {
                    double c = ce.Current;
                    if (j == 0)
                        c = -c;
                    if (Math.Abs(c - firstElm.Current) > 1e-10)
                        continue;
                }
                int k;
                for (k = 0; k != ce.PostCount; k++)
                {
                    if (j == k)
                        continue;
                    //System.out.println(ce + " " + ce.getNode(j) + "-" + ce.getNode(k));
                    if (ce.getConnection(j, k) && findPath(ce.getNode(k), depth))
                    {
                        //System.out.println("got findpath " + n1);
                        used[n1] = false;
                        return true;
                    }
                    //System.out.println("back on findpath " + n1);
                }
            }
            used[n1] = false;
            //System.out.println(n1 + " failed");
            return false;
        }
    }
}
