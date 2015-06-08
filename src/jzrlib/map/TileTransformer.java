/*
 *  "JZRLib", Java Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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
package jzrlib.map;

import jzrlib.core.BytesSet;
import jzrlib.utils.TypeUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class TileTransformer
{
    public final static class MagicRec
    {
        public BytesSet main;
        public BytesSet ext;

        public MagicRec(BytesSet main, BytesSet ext)
        {
            this.main = main;
            this.ext = ext;
        }
    }

    // Tile select levels
    public static final int TSL_BACK = 0;
    public static final int TSL_FORE = 1;
    public static final int TSL_BACK_EXT = 2;
    public static final int TSL_FORE_EXT = 3;
    public static final int TSL_FOG = 4;
    
    public static int getAdjacentMagic(IMap map, int x, int y, int def, int check, int select)
    {
        int magic = 0;
        for (int i = 0; i < 8; i++) {
            int ax = x + AbstractMap.CoursesX[i];
            int ay = y + AbstractMap.CoursesY[i];

            int tid;
            BaseTile tile = map.getTile(ax, ay);
            if (tile == null) {
                tid = def;
            } else {
                switch (select) {
                    case TSL_BACK:
                        tid = TypeUtils.getShortLo(tile.Background);
                        break;
                    case TSL_FORE:
                        tid = TypeUtils.getShortLo(tile.Foreground);
                        break;
                    case TSL_BACK_EXT:
                        tid = TypeUtils.getShortLo(tile.BackgroundExt);
                        break;
                    case TSL_FORE_EXT:
                        tid = TypeUtils.getShortLo(tile.ForegroundExt);
                        break;
                    /*case TSL_FOG:
                        tid = TypeUtils.getShortLo(tile.FogID);
                        break;*/
                    default:
                        tid = 0;
                        break;
                }
            }

            if (tid == check) {
                magic = TypeUtils.setBit(magic, i, 1);
            }
        }

        return magic;
    }
}