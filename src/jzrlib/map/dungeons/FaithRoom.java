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
public final class FaithRoom extends CustomStaticArea
{
    private static final String[] frArea;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new FaithRoom(owner, parentMark);
    }

    public FaithRoom(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(31, 31);
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return FaithRoom.frArea[aY].charAt(aX);
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        Rect other = anArea.getDimensionRect();
        return other.isIntersect(new Rect(this.Left + 11, this.Top, this.Left + 19, this.Top + 10)) || 
                other.isIntersect(new Rect(this.Left + 0, this.Top + 11, this.Left + 30, this.Top + 19)) || 
                other.isIntersect(new Rect(this.Left + 11, this.Top + 20, this.Left + 19, this.Top + 30));
    }

    static {
        frArea = new String[]{
            "             XXNXX             ",
            "             X...X             ",
            "            XX...XX            ",
            "           XX.....XX           ",
            "           X.......X           ",
            "           W.......E           ",
            "           X.......X           ",
            "           XX.....XX           ",
            "            XX...XX            ",
            "             X...X             ",
            "           XXX...XXX           ",
            "   XXNXX  XX.......XX  XXNXX   ",
            "  XX...XX X.........X XX...XX  ",
            "XXX.....XXX.........XXX.....XXX",
            "X.............................X",
            "W.............................E",
            "X.............................X",
            "XXX.....XXX.........XXX.....XXX",
            "  XX...XX X.........X XX...XX  ",
            "   XXSXX  XX.......XX  XXSXX   ",
            "           XXX...XXX           ",
            "             X...X             ",
            "            XX...XX            ",
            "           XX.....XX           ",
            "           X.......X           ",
            "           W.......E           ",
            "           X.......X           ",
            "           XX.....XX           ",
            "            XX...XX            ",
            "             X...X             ",
            "             XXSXX             "};
    }
}
