/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih.
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using BSLib;

namespace ZRLib.External.BSP
{
    public delegate void INodeSplitHandler(BSPNode node, ref int refSplitWidth);

    public delegate void ISplitProc(int x1, int y1, int x2, int y2);

    public sealed class BSPTree
    {
        private const int DEBUG_DEPTH = -1;

        private BSPNode fRoot;

        private readonly ExtRect fArea;
        private readonly int fMinNodeSize;
        private readonly int fMaxNodeSize;
        private readonly bool fDistinctBounds;
        private readonly ISplitProc fSplitProc;
        private readonly INodeSplitHandler fNodeSplitHandler;

        public BSPTree(ExtRect area, int minNodeSize, int maxNodeSize, bool distinctBounds, ISplitProc splitProc, INodeSplitHandler nodeSplitHandler)
        {
            fArea = area;
            fMinNodeSize = minNodeSize;
            fMaxNodeSize = maxNodeSize;
            fDistinctBounds = distinctBounds;
            fSplitProc = splitProc;
            fNodeSplitHandler = nodeSplitHandler;

            Generate();
        }

        public BSPNode Root
        {
            get {
                return fRoot;
            }
        }

        public void Dispose()
        {
            DisposeNode(fRoot);
        }

        private void DisposeNode(BSPNode node)
        {
            if (node != null) {
                if (node.Left != null) {
                    DisposeNode(node.Left);
                    node.Left = null;
                }

                if (node.Right != null) {
                    DisposeNode(node.Right);
                    node.Right = null;
                }
            }
        }

        public IList<BSPNode> Leaves
        {
            get {
                IList<BSPNode> leaves = new List<BSPNode>();
                GetLeaves(leaves, fRoot);
                return leaves;
            }
        }

        private void GetLeaves(IList<BSPNode> vec, BSPNode node)
        {
            if (node != null) {
                if (node.Left == null && node.Right == null) {
                    vec.Add(node);
                } else {
                    GetLeaves(vec, node.Left);
                    GetLeaves(vec, node.Right);
                }
            }
        }

        public void Generate()
        {
            BSPNode.ResetId();
            fRoot = new BSPNode(null, 1, fArea.Left, fArea.Top, fArea.Right, fArea.Bottom);
            Generate(fRoot);
        }

        private void Generate(BSPNode node)
        {
            if (DEBUG_DEPTH > 0 && node.Depth > DEBUG_DEPTH) {
                return;
            }

            // if splitting the node was successful, do the same to the newly
            // generated child nodes, if they're too big or with a 75% chance.
            if (SplitNode(node)) {
                BSPNode leaf;

                leaf = node.Left;
                if (leaf.Width > fMaxNodeSize || leaf.Height > fMaxNodeSize || RandomHelper.GetRandom(100) > 25) {
                    Generate(leaf);
                }

                leaf = node.Right;
                if (leaf.Width > fMaxNodeSize || leaf.Height > fMaxNodeSize || RandomHelper.GetRandom(100) > 25) {
                    Generate(leaf);
                }
            }
        }

        private bool SplitNode(BSPNode node)
        {
            // if the nodes exist, return
            if (node.Left != null || node.Right != null) {
                return false;
            }

            // check which direction we're splitting.
            // if the height and width are similar, pick one randomly.
            bool splitH = (RandomHelper.GetRandom(2) == 1);
            int nW = node.Width;
            int nH = node.Height;
            if (nW > nH && ((float)nH / (float)nW >= 0.05f)) {
                splitH = false;
            } else if (nH > nW && ((float)nW / (float)nH >= 0.05f)) {
                splitH = true;
            }

            int splitWidth = 0;
            if (fNodeSplitHandler != null) {
                fNodeSplitHandler(node, ref splitWidth);
            }

            // check to see if we have enough space to split
            int max = (splitH ? nH : nW) - fMinNodeSize - splitWidth;
            if (max <= fMinNodeSize) {
                return false;
            }

            // get a space to split on
            int split = fMinNodeSize + RandomHelper.GetRandom(max - fMinNodeSize);
            int offset = ((fDistinctBounds) ? 1 : 0) + splitWidth;

            // gen the depth for new leafs
            int subDepth = node.Depth + 1;

            // split the node
            if (splitH) {
                node.SplitterType = SplitterType.stHorz;
                node.Left = new BSPNode(node, subDepth, node.X1, node.Y1, node.X2, node.Y1 + split);
                node.Right = new BSPNode(node, subDepth, node.X1, node.Y1 + split + offset, node.X2, node.Y2);

                if (fDistinctBounds) {
                    ProcessSplit(node.X1, node.Y1 + split + 1, node.X2, node.Y1 + split + offset - 1);
                }
            } else {
                node.SplitterType = SplitterType.stVert;
                node.Left = new BSPNode(node, subDepth, node.X1, node.Y1, node.X1 + split, node.Y2);
                node.Right = new BSPNode(node, subDepth, node.X1 + split + offset, node.Y1, node.X2, node.Y2);

                if (fDistinctBounds) {
                    ProcessSplit(node.X1 + split + 1, node.Y1, node.X1 + split + offset - 1, node.Y2);
                }
            }

            return true;
        }

        private void ProcessSplit(int x1, int y1, int x2, int y2)
        {
            if (fSplitProc != null) {
                fSplitProc(x1, y1, x2, y2);
            }
        }

        private bool HasPoint(BSPNode node, int ax, int ay)
        {
            if (node == null) {
                return false;
            } else {
                return (ax >= node.X1 && ax <= node.X2 && ay >= node.Y1 && ay <= node.Y2);
            }
        }

        private bool HasNode(IList<BSPNode> leaves, int ax, int ay)
        {
            foreach (BSPNode node in leaves) {
                if (HasPoint(node, ax, ay)) {
                    return true;
                }
            }

            return false;
        }

        private void CheckNode(IList<BSPNode> leaves, BSPNode node)
        {
            int cX = node.X1 + (node.Width / 2);
            int cY = node.Y1 + (node.Height / 2);
            int halfMin = fMinNodeSize / 2;

            bool resL, resT, resR, resB;

            resL = HasNode(leaves, node.X1 - halfMin, cY); // left
            node.Sides.Add(new NodeSide(SideType.stLeft, (resL) ? LocationType.ltInner : LocationType.ltOuter));

            resR = HasNode(leaves, node.X2 + halfMin, cY); // right
            node.Sides.Add(new NodeSide(SideType.stRight, (resR) ? LocationType.ltInner : LocationType.ltOuter));

            resT = HasNode(leaves, cX, node.Y1 - halfMin); // top
            node.Sides.Add(new NodeSide(SideType.stTop, (resT) ? LocationType.ltInner : LocationType.ltOuter));

            resB = HasNode(leaves, cX, node.Y2 + halfMin); // bottom
            node.Sides.Add(new NodeSide(SideType.stBottom, (resB) ? LocationType.ltInner : LocationType.ltOuter));


            if (!resL || !resR || !resT || !resB) {
                node.NodeType = LocationType.ltOuter;
            } else {
                node.NodeType = LocationType.ltInner;
            }
        }

        public void CheckLeaves()
        {
            IList<BSPNode> leaves = Leaves;
            foreach (BSPNode node in leaves) {
                CheckNode(leaves, node);
            }
        }
    }

}