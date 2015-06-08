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

/**
 * 
 * @author Serg V. Zhdanovskih (aka Alchemist), 2006, 2014
 */
public final class Alt4Room extends CustomStaticArea
{
    private static final String[] a4Area;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new Alt4Room(owner, parentMark);
    }

    public Alt4Room(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(17, 9);
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return Alt4Room.a4Area[aY].charAt(aX);
    }

    static {
        a4Area = new String[]{
            "XXXXXNXXXXXNXXXXX",
            "X...............X",
            "X.X.XXXXX.XXXXX.X",
            "X.X..X..X..X..X.X",
            "W.X..X..X..X..X.E",
            "X.X..X..X..X..X.X",
            "X.XXXX.XXXXX.XX.X",
            "X...............X",
            "XXXXXSXXXXXSXXXXX"
        };
    }
}
