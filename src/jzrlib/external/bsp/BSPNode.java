/*
 *  "JZRLib", Java Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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
package jzrlib.external.bsp;

import java.util.ArrayList;
import java.util.List;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class BSPNode
{
    private static int lastId = 0;

    public final int id;
    public final BSPNode parent;
    public final int depth;
    public final int x1, y1, x2, y2;

    public BSPNode left;
    public BSPNode right;

    public LocationType nodeType;
    public SplitterType splitterType;

    public final List<NodeSide> sides;

    public BSPNode(BSPNode parent, int depth, int x1, int y1, int x2, int y2)
    {
        this.id = ++lastId;
        this.parent = parent;
        this.depth = depth;
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;

        this.left = null;
        this.right = null;

        this.nodeType = LocationType.ltNone;
        this.splitterType = SplitterType.stNone;

        this.sides = new ArrayList<>();
    }

    public final int getWidth()
    {
        return (this.x2 - this.x1) + 1;
    }

    public final int getHeight()
    {
        return (this.y2 - this.y1) + 1;
    }

    public static void resetId()
    {
        lastId = 0;
    }
}
