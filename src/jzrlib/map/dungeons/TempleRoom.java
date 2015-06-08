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
public final class TempleRoom extends CustomStaticArea
{
    private static final String[] tArea;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new TempleRoom(owner, parentMark);
    }

    public TempleRoom(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(39, 39);
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        Rect other = anArea.getDimensionRect();
        return other.isIntersect(new Rect(this.Left + 16, this.Top, this.Left + 22, this.Top + 4)) || 
                other.isIntersect(new Rect(this.Left + 11, this.Top + 4, this.Left + 27, this.Top + 34)) || 
                other.isIntersect(new Rect(this.Left + 4, this.Top + 11, this.Left + 34, this.Top + 27)) || 
                other.isIntersect(new Rect(this.Left, this.Top + 16, this.Left + 4, this.Top + 22)) || 
                other.isIntersect(new Rect(this.Left + 16, this.Top + 34, this.Left + 22, this.Top + 38)) || 
                other.isIntersect(new Rect(this.Left + 34, this.Top + 16, this.Left + 38, this.Top + 22));
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return TempleRoom.tArea[aY].charAt(aX);
    }

    @Override
    public int getDevourArea()
    {
        return this.fDevourArea + 64 + 36;
    }

    static {
        tArea = new String[]{
            "                XXXNXXX                ",
            "                X.....X                ",
            "                W.....E                ",
            "                X.....X                ",
            "                XX...XX                ",
            "                 X...X                 ",
            "                 X...X                 ",
            "                 X...X                 ",
            "                 XX.XX                 ",
            "                  X.X                  ",
            "                  X.X                  ",
            "                XXX.XXX                ",
            "            XXXXX.....XXXXX            ",
            "            X.............X            ",
            "            X.............X            ",
            "            X.............X            ",
            "XXNXX      XX.............XX      XXNXX",
            "X...XXXXX  X...............X  XXXXX...X",
            "X.......XXXX...............XXXX.......X",
            "W.....................................E",
            "X.......XXXX...............XXXX.......X",
            "X...XXXXX  X...............X  XXXXX...X",
            "XXSXX      XX.............XX      XXSXX",
            "            X.............X            ",
            "            X.............X            ",
            "            X.............X            ",
            "            XXXXX.....XXXXX            ",
            "                XXX.XXX                ",
            "                  X.X                  ",
            "                  X.X                  ",
            "                 XX.XX                 ",
            "                 X...X                 ",
            "                 X...X                 ",
            "                 X...X                 ",
            "                XX...XX                ",
            "                X.....X                ",
            "                W.....E                ",
            "                X.....X                ",
            "                XXXSXXX                "};
    }
}
