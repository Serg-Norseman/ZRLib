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
public final class GenevaWheelRoom extends CustomStaticArea
{
    private static final String[] gwArea;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new GenevaWheelRoom(owner, parentMark);
    }

    public GenevaWheelRoom(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(21, 21);
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        Rect other = anArea.getDimensionRect();
        return other.isIntersect(new Rect(this.Left + 7, this.Top, this.Left + 13, this.Top + 6)) || 
                other.isIntersect(new Rect(this.Left + 0, this.Top + 7, this.Left + 20, this.Top + 13)) || 
                other.isIntersect(new Rect(this.Left + 7, this.Top + 14, this.Left + 13, this.Top + 20));
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return GenevaWheelRoom.gwArea[aY].charAt(aX);
    }

    @Override
    public int getDevourArea()
    {
        return this.fDevourArea + 32;
    }

    static {
        gwArea = new String[]{
            "       XXXNXXX       ",
            "       X.....X       ",
            "       X.....X       ",
            "       XX...XX       ",
            "        X...X        ",
            "        X...X        ",
            "        X...X        ",
            "XXXX    XX.XX    XXXX",
            "X..XXXXX X.X XXXXX..X",
            "X......XXX.XXX......X",
            "W...................E",
            "X......XXX.XXX......X",
            "X..XXXXX X.X XXXXX..X",
            "XXXX    XX.XX    XXXX",
            "        X...X        ",
            "        X...X        ",
            "        X...X        ",
            "       XX...XX       ",
            "       X.....X       ",
            "       X.....X       ",
            "       XXXSXXX       "};
    }
}
