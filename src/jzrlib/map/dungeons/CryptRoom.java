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
public final class CryptRoom extends CustomStaticArea
{
    private static final String[] cryptArea;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new CryptRoom(owner, parentMark);
    }

    public CryptRoom(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(19, 19);
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return CryptRoom.cryptArea[aY].charAt(aX);
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        Rect other = anArea.getDimensionRect();
        return other.isIntersect(new Rect(this.Left + 6, this.Top, this.Left + 12, this.Top + 2)) || 
                other.isIntersect(new Rect(this.Left + 3, this.Top + 3, this.Left + 15, this.Top + 5)) || 
                other.isIntersect(new Rect(this.Left + 0, this.Top + 6, this.Left + 18, this.Top + 12)) || 
                other.isIntersect(new Rect(this.Left + 3, this.Top + 13, this.Left + 15, this.Top + 15)) || 
                other.isIntersect(new Rect(this.Left + 6, this.Top + 16, this.Left + 12, this.Top + 18));
    }

    static {
        cryptArea = new String[]{
            "      XXXNXXX      ",
            "      X.....X      ",
            "      X.....X      ",
            "   XXXX.....XXXX   ",
            "   X..XX...XX..X   ",
            "   X...XX.XX...X   ",
            "XXXXX...X.X...XXXXX",
            "X...XX.......XX...X",
            "X....XX.....XX....X",
            "W.................E",
            "X....XX.....XX....X",
            "X...XX.......XX...X",
            "XXXXX...X.X...XXXXX",
            "   X...XX.XX...X   ",
            "   X..XX...XX..X   ",
            "   XXXX.....XXXX   ",
            "      X.....X      ",
            "      X.....X      ",
            "      XXXSXXX      "};
    }
}
