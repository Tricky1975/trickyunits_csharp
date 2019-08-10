// Lic:
// Dijkstra.cs
// Dijkstra Pathfinding Algorithm
// version: 19.08.10
// Copyright (C)  Jeroen P. Broks
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 1. The origin of this software must not be misrepresented; you must not
// claim that you wrote the original software. If you use this software
// in a product, an acknowledgment in the product documentation would be
// appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
// misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
// EndLic

/* This is a kind of experimental pathfinding module
 * based on the Dijkstra algorithm. 
 * It's set up to be very very simplistic, but also to
 * be kind of flexible... At least flexible enough for *my* needs.
 * 
 * The Dijkstra algorithm was invented by Edgser Dijkstra, a famous
 * Dutch computer scientist. Thanks to his work navigational 
 * system work. so all hail Dijkstra ;)
 *  
 */
using System;
using System.Collections.Generic;

namespace TrickyUnits.Dijkstra {

    delegate bool TPassible (int x, int y);

    class Node {
        public int x = 0, y = 0;
        public bool visited = false;
        public bool passible = true;
        internal Node parent;
        public int Distance {
            get {
                int ret = 1;
                Node Current = this;
                while (Current.parent != null) {
                    ret++;
                    Current = Current.parent;
                }
                return ret;
            }
        }
    }

    class NodeMap {
        readonly public int maxwidth;
        readonly public int maxheight;
        readonly TMap<int, Node> Nodes = new TMap<int, Node>();

        public NodeMap(int width,int height) { maxwidth = width;maxheight = height; }

        public Node this[int x,int y] {
            get {
                if (x < 0 || y < 0 || x >= maxwidth || y >= maxheight)
                    throw new Exception($"NodeMap index out of bounds. [{x},{y}] Format is {maxwidth}x{maxheight}! ");
                var idx = (x * maxheight) + y;
                var ret = Nodes[idx];
                if (ret==null) {
                    ret = new Node();
                    Nodes[idx] = ret;
                    ret.x = x;
                    ret.y = y;
                }
                return ret;
            }
        }
    }

    class Path {
        public bool Success;
        public Node[] Nodes;
    }

    class Dijkstra {

        static public bool Config_AllowDiagonally = false;

        readonly TPassible Passible;
        readonly bool[,] BlockMap; // Only needed when default passible is used;
        readonly NodeMap Nodes;

        readonly int StartX, EndX, StartY, EndY;

        bool DefaultPassible(int x,int y) {
            if (x < 0 || y < 0) return false;
            if (x >= Nodes.maxwidth || y >= Nodes.maxheight) return false;
            return BlockMap[x,y];
        }

        /// <summary>
        /// Create a Dijkstra pathfinder
        /// </summary>
        public Dijkstra(TPassible Passible,int width,int height, int startx, int starty, int endx, int endy) {
            StartX = startx;
            StartY = starty;
            EndX = endx;
            EndY = endy;
            Nodes = new NodeMap(width, height);
            this.Passible = Passible;
        }

        public Dijkstra(bool[,] BlockMap, int startx, int starty, int endx, int endy) {
            StartX = startx;
            StartY = starty;
            EndX = endx;
            EndY = endy;
            Nodes = new NodeMap(BlockMap.GetLength(0), BlockMap.GetLength(1));
            this.BlockMap = BlockMap;
            Passible = DefaultPassible;
        }

        /// <summary>
        /// Start finding a path, and return the result
        /// </summary>
        /// <returns></returns>
        public Path Start() {
            bool Check(int x, int y) => Passible(x, y) && (!Nodes[x, y].visited);
            void Catch(List<Node> NodeList,Node ParentNode, int x,int y) { if (Check(x, y)) { NodeList.Add(Nodes[x, y]); Nodes[x, y].parent = ParentNode; } }
                     
            Nodes[EndX, EndY].visited = true;
            var Latest = new List<Node>();
            Latest.Add(Nodes[EndX, EndY]);
            while (true) { // Once the path has been found, we'll need to break out of the loop anyway!
                var Caught = new List<Node>();
                var Any = false;
                foreach (Node CNode in Latest) {
                    if (Config_AllowDiagonally) {
                        // When testing diagonally, test the entire way round. This WILL result into a self-check too, but as the self is already visited, this should always result into false anyway!
                        for (int ix = CNode.x - 1; ix <= CNode.x + 1; ix++) for (int iy = CNode.y - 1; iy <= CNode.y + 1; iy++) Catch(Caught,CNode, ix, iy);
                    } else {
                        Catch(Caught, CNode, CNode.x + 1, CNode.y); // East
                        Catch(Caught, CNode, CNode.x - 1, CNode.y); // West
                        Catch(Caught, CNode, CNode.x, CNode.y + 1); // South
                        Catch(Caught, CNode, CNode.x, CNode.y - 1); // North
                    }
                    foreach (Node N in Caught) {
                        Any = true;
                        N.visited = true;
                        //N.parent = CNode;
                        if (N.x==StartX && N.y == StartY) { // BINGO!!!
                            var ret = new Path();
                            ret.Nodes = new Node[N.Distance];
                            var i = 0;
                            var iNode = N;
                            while (iNode != null) {
                                ret.Nodes[i] = iNode;
                                iNode = iNode.parent;
                                i++;
                                //if (i >= ret.Nodes.Length) System.Diagnostics.Debug.WriteLine($"HUH? {i}/{ret.Nodes.Length}");
                            }
                            ret.Success = true;
                            return ret;
                        }
                    }
                }
                if (!Any) { // Failed!
                    var failret = new Path();
                    failret.Nodes = new Node[0]; // Prevent any (needless) issues
                    failret.Success = false;
                    return failret;
                }
                Latest = Caught;
            } // Infinite loop, rememner!
        }

        static public Path QuickPath(bool[,] BlockMap, int startx, int starty, int endx, int endy) {
            var a = new Dijkstra(BlockMap, startx, starty, endx, endy);
            return a.Start();
        }

        static public Path QuickPath(TPassible Passible, int width, int height, int startx, int starty, int endx, int endy) {
            var a = new Dijkstra(Passible, width, height, startx, starty, endx, endy);
            return a.Start();
        }

    }

}
