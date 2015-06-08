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
public final class SpiderRoom extends CustomStaticArea
{
    private static final String[] sprArea;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new SpiderRoom(owner, parentMark);
    }

    public SpiderRoom(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(31, 31);
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return SpiderRoom.sprArea[aY].charAt(aX);
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        Rect other = anArea.getDimensionRect();
        return other.isIntersect(new Rect(this.Left + 7, this.Top, this.Left + 23, this.Top + 6)) || 
                other.isIntersect(new Rect(this.Left + 0, this.Top + 7, this.Left + 30, this.Top + 22)) || 
                other.isIntersect(new Rect(this.Left + 7, this.Top + 23, this.Left + 23, this.Top + 30));
    }

    static {
        sprArea = new String[]{
            "       XXXX   XNX   XXXX       ",
            "       X..X   X.X   X..X       ",
            "       W..X   X.X   X..E       ",
            "       X..XX  X.X  XX..X       ",
            "       XX..X  X.X  X..XX       ",
            "        X..XXXX.XXXX..X        ",
            "        XX...XX.XX...XX        ",
            "XXNXX    XX.........XX    XXNXX",
            "X...XXX   XXX.....XXX   XXX...X",
            "X.....XX    XXX.XXX    XX.....X",
            "XXXX...XX     X.X     XX...XXXX",
            "   XXX..X     X.X     X..XXX   ",
            "     X..XX   XX.XX   XX..X     ",
            "     XX..X  XX...XX  X..XX     ",
            "XXXXXXX..XXXX.....XXXX..XXXXXXX",
            "W.............................E",
            "XXXXXXX..XXXX.....XXXX..XXXXXXX",
            "     XX..X  XX...XX  X..XX     ",
            "     X..XX   XX.XX   XX..X     ",
            "   XXX..X     X.X     X..XXX   ",
            "XXXX...XX     X.X     XX...XXXX",
            "X.....XX    XXX.XXX    XX.....X",
            "X...XXX   XXX.....XXX   XXX...X",
            "XXSXX    XX.........XX    XXSXX",
            "        XX...XX.XX...XX        ",
            "        X..XXXX.XXXX..X        ",
            "       XX..X  X.X  X..XX       ",
            "       X..XX  X.X  XX..X       ",
            "       W..X   X.X   X..E       ",
            "       X..X   X.X   X..X       ",
            "       XXXX   XSX   XXXX       "};
    }
}
