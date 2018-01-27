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

namespace ZRLib.External.BSP
{
    public enum SplitterType
    {
        stNone,
        stHorz,
        stVert
    }

    public enum LocationType
    {
        ltNone,
        ltInner,
        ltOuter
    }

    public sealed class BSPNode
    {
        private static int LastId = 0;

        public readonly int Id;
        public readonly BSPNode Parent;
        public readonly int Depth;
        public readonly int X1, Y1, X2, Y2;

        public BSPNode Left;
        public BSPNode Right;

        public LocationType NodeType;
        public SplitterType SplitterType;

        public readonly IList<NodeSide> Sides;

        public BSPNode(BSPNode parent, int depth, int x1, int y1, int x2, int y2)
        {
            Id = ++LastId;
            Parent = parent;
            Depth = depth;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;

            Left = null;
            Right = null;

            NodeType = LocationType.ltNone;
            SplitterType = SplitterType.stNone;

            Sides = new List<NodeSide>();
        }

        public int Width
        {
            get { return (X2 - X1) + 1; }
        }

        public int Height
        {
            get { return (Y2 - Y1) + 1; }
        }

        public static void ResetId()
        {
            LastId = 0;
        }
    }
}
