/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
 *  Copyright (C) 2002-2008, 2014 by Serg V. Zhdanovskih (aka Alchemist).
 *
 *  This file is part of "NorseWorld: Ragnarok".
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
package jzrlib.map.dungeons;

import jzrlib.core.Rect;

/**
 * 
 * @author Serg V. Zhdanovskih (aka Alchemist), 2005, 2014
 */
public final class RoseRoom extends CustomStaticArea
{
    private static final String[] rrArea;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new RoseRoom(owner, parentMark);
    }

    public RoseRoom(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(19, 19);
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return RoseRoom.rrArea[aY].charAt(aX);
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        Rect other = anArea.getDimensionRect();
        return other.isIntersect(new Rect(this.Left + 5, this.Top, this.Left + 13, this.Top + 4)) || 
                other.isIntersect(new Rect(this.Left + 0, this.Top + 5, this.Left + 18, this.Top + 13)) || 
                other.isIntersect(new Rect(this.Left + 5, this.Top + 14, this.Left + 13, this.Top + 18));
    }

    static {
        rrArea = new String[]{
            "      XXXNXXX      ",
            "     XX.....XX     ",
            "     X.......X     ",
            "     X.......X     ",
            "     XX.....XX     ",
            " XXXX X.....X XXXX ",
            "XX..XXXX...XXXX..XX",
            "X.....XXX.XXX.....X",
            "X......X...X......X",
            "W.................E",
            "X......X...X......X",
            "X.....XXX.XXX.....X",
            "XX..XXXX...XXXX..XX",
            " XXXX X.....X XXXX ",
            "     XX.....XX     ",
            "     X.......X     ",
            "     X.......X     ",
            "     XX.....XX     ",
            "      XXXSXXX      "};
    }
}
