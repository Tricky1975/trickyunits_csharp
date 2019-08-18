using System;
using System.Collections.Generic;

/*
 * This is an experimental project to calculate lines using the Bressenham algorithm
 * Please note, Bressenham can only draw lines from left to right and only when the angle between horizontal and the line is 45 degrees or smaller
 * All else must be faked with clever maths. Now I could already tackle a few issues, like drawing from right to left, but the beyond 45 degrees issue is one I'm still trying to get a good
 * calculation for!
 */ 

namespace TrickyUnits.Bressenham {

    class Bressenham {

        public class Node {
            public int x { get; private set; }
            public int y { get; private set; }
            public Node(int sx, int sy) { x = sx; y = sy; }
        }

        Node[] Nodes;

        private Bressenham() { }
        
        static List<Node> basicline(int x0, int y0, int x1, int y1) {
            var ret = new List<Node>();
            void putpixel(int ax, int ay, int whatever = 0) => ret.Add(new Node(ax, ay));

            int dx, dy, p, x, y;

            dx = x1 - x0;
            dy = y1 - y0;

            x = x0;
            y = y0;

            p = 2 * dy - dx;

            while (x < x1) {
                if (p >= 0) {
                    putpixel(x, y, 7);
                    y = y + 1;
                    p = p + 2 * dy - 2 * dx;
                } else {
                    putpixel(x, y, 7);
                    p = p + 2 * dy;
                }
                x = x + 1;
            }
            return ret;
        }

        public Node this[int idx] {
            get {
                if (idx < 0 || idx >= Nodes.Length) throw new Exception($"Bressenham node index out of bounds. Got {idx}, range is 0-{Nodes.Length}");
                return Nodes[idx];
            }
        }

        public int Length => Nodes.Length;
        public int Count => Nodes.Length;


        static public Bressenham GenerateLine(int x1, int y1, int x2, int y2) {
            var ret = new Bressenham();
            // Simple version
            if (x1 == x2 && y1 != y2) {
                var N = new List<Node>();
                if (y1 < y2) for (int i = y1; i <= y2; i++) N.Add(new Node(x1, i));
                if (y1 > y2) for (int i = y1; i >= y2; i--) N.Add(new Node(x1, i));
                ret.Nodes = N.ToArray();
            } else if (x1 <= x2 && y1 <= y2)
                ret.Nodes = basicline(x1, y1, x2, y2).ToArray();
            else if (x1 >= x2 && y1 >= y2) {
                ret.Nodes = basicline(x2, y2, x1, y1).ToArray();
                Array.Reverse(ret.Nodes);
            } else if (x1 < x2 && y1 > y2) {
                ret.Nodes = basicline(x1, y2, x2, y1).ToArray();
                var N2 = new List<Node>();
                var l = ret.Nodes.Length - 1;
                for (int i = 0; i < ret.Nodes.Length; i++)
                    N2.Add(new Node(ret[i].x, ret[l - i].y));
                ret.Nodes = N2.ToArray();
            } else if (x1 > x2 && y1 < y2) {
                ret.Nodes = basicline(x1, y2, x2, y1).ToArray();
                var N2 = new List<Node>();
                var l = ret.Nodes.Length - 1;
                for (int i = 0; i < ret.Nodes.Length; i++)
                    N2.Add(new Node(ret[l - i].x, ret[l].y));
                ret.Nodes = N2.ToArray();
            } else
                throw new Exception("Request done that Bressenham doesn't support by itself, and for which no alternate support has yet been set up"); // Shouldn't be possible, but just in case!
            return ret;
        }

    }

}