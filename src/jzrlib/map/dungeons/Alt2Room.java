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
public final class Alt2Room extends CustomStaticArea
{
    private static final String[] a2Area;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new Alt2Room(owner, parentMark);
    }

    public Alt2Room(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(9, 7);
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return Alt2Room.a2Area[aY].charAt(aX);
    }

    static {
        a2Area = new String[]{
            "XXXXNXXXX",
            "X.......X",
            "X..X.X..X",
            "W..X.X..E",
            "X..XXX..X",
            "X.......X",
            "XXXXSXXXX"};
    }
}
