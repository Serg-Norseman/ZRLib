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

import jzrlib.utils.AuxUtils;

/**
 * 
 * @author Serg V. Zhdanovskih (aka Alchemist), 2015
 */
public final class Alt5Room extends CustomStaticArea
{
    private static final String[] AREA1;
    private static final String[] AREA2;
    private static final String[] AREA3;
    
    private String[] fArea;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new Alt5Room(owner, parentMark);
    }

    public Alt5Room(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(21, 11);
        
        int idx = AuxUtils.getRandom(3);
        if (idx == 0) {
            this.fArea = AREA1;
        } else if (idx == 1) {
            this.fArea = AREA2;
        } else {
            this.fArea = AREA3;
        }
    }

    @Override
    protected char getCustomCell(int aX, int aY)
    {
        return this.fArea[aY].charAt(aX);
    }

    static {
        AREA1 = new String[]{
            "XXXXNXXXXXNXXXXXNXXXX",
            "XX.................XX",
            "X...................X",
            "X...................X",
            "X...................X",
            "W...................E",
            "X...................X",
            "X...................X",
            "X...................X",
            "XX.................XX",
            "XXXXSXXXXXSXXXXXSXXXX",
        };

        AREA2 = new String[]{
            "XXXXNXXXXXNXXXXXNXXXX",
            "XX.................XX",
            "X...XXXXX...XXXXX...X",
            "X...X...........X...X",
            "X...X..XXX.XXX..X...X",
            "W...................E",
            "X...X..XXX.XXX..X...X",
            "X...X...........X...X",
            "X...XXXXX...XXXXX...X",
            "XX.................XX",
            "XXXXSXXXXXSXXXXXSXXXX",
        };

        AREA3 = new String[]{
            "XXXXNXXXXXNXXXXXNXXXX",
            "XX.................XX",
            "X...................X",
            "X..XXXXXX...XXXXXX..X",
            "X..X.............X..X",
            "W..X..XXXXXXXXX..X..E",
            "X..X.............X..X",
            "X..XXXXXX...XXXXXX..X",
            "X...................X",
            "XX.................XX",
            "XXXXSXXXXXSXXXXXSXXXX",
        };
    }
}
