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
public final class StarRoom extends CustomStaticArea
{
    private static final String[] srArea;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new StarRoom(owner, parentMark);
    }

    public StarRoom(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(23, 23);
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return StarRoom.srArea[aY].charAt(aX);
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        Rect other = anArea.getDimensionRect();
        return other.isIntersect(new Rect(this.Left + 6, this.Top, this.Left + 16, this.Top + 5)) || 
                other.isIntersect(new Rect(this.Left + 0, this.Top + 6, this.Left + 22, this.Top + 16)) || 
                other.isIntersect(new Rect(this.Left + 6, this.Top + 17, this.Left + 16, this.Top + 22));
    }

    static {
        srArea = new String[]{
            "      XXXNX            ",
            "      W...XX           ",
            "      XXX..XX          ",
            "        XX..X          ",
            "         XX.X          ",
            "          X.X          ",
            "         XX.XX      XNX",
            "       XXX...XXX    X.X",
            "       X.......X   XX.X",
            "      XX.......XX XX..E",
            "  XXXXX.........XXX..XX",
            " XX.................XX ",
            "XX..XXX.........XXXXX  ",
            "W..XX XX.......XX      ",
            "X.XX   X.......X       ",
            "X.X    XXX...XXX       ",
            "XSX      XX.XX         ",
            "          X.X          ",
            "          X.XX         ",
            "          X..XX        ",
            "          XX..XXX      ",
            "           XX...E      ",
            "            XSXXX      "};
    }
}
