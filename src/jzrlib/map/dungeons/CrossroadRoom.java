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
public final class CrossroadRoom extends CustomStaticArea
{
    private static final String[] crArea;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new CrossroadRoom(owner, parentMark);
    }

    public CrossroadRoom(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(21, 25);
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return CrossroadRoom.crArea[aY].charAt(aX);
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        Rect other = anArea.getDimensionRect();
        return other.isIntersect(new Rect(this.Left, this.Top, this.Left + 20, this.Top + 8)) 
                || other.isIntersect(new Rect(this.Left + 5, this.Top + 9, this.Left + 15, this.Top + 15)) 
                || other.isIntersect(new Rect(this.Left, this.Top + 16, this.Left + 20, this.Top + 24));
    }

    static {
        crArea = new String[]{
            "XXX      XNX      XXX",
            "X.X     XX.XX     X.X",
            "X.XX   XX...XX   XX.X",
            "W..XX  X.....X  XX..E",
            "XX..XXXX.....XXXX..XX",
            " XX...XXX...XXX...XX ",
            "  XXX...........XXX  ",
            "    XXXXX...XXXXX    ",
            "        X...X        ",
            "     XXXXX.XXXXX     ",
            "     X.........X     ",
            "     X.........X     ",
            "     W.........E     ",
            "     X.........X     ",
            "     X.........X     ",
            "     XXXXX.XXXXX     ",
            "        X...X        ",
            "    XXXXX...XXXXX    ",
            "  XXX...........XXX  ",
            " XX...XXX...XXX...XX ",
            "XX..XXXX.....XXXX..XX",
            "W..XX  X.....X  XX..E",
            "X.XX   XX...XX   XX.X",
            "X.X     XX.XX     X.X",
            "XXX      XSX      XXX"};
    }
}
