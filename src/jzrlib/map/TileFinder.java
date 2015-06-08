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

import java.util.ArrayList;
import java.util.List;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.utils.AuxUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class TileFinder
{
    // Tile search flags
    public static final int TSF_SIMPLE = 1;
    public static final int TSF_BORDER = 2;
    public static final int TSF_FREE = 4;
    
    public static final Point findTile(IMap map, Rect area, int flags)
    {
        if ((flags & TSF_SIMPLE) > 0) {
            int tries = 50;
            while (tries > 0) {
                Point pt = AuxUtils.getRandomPoint(area);
                if (pt != null) {
                    if ((flags & TSF_FREE) > 0) {
                        if (!map.isBarrier(pt.X, pt.Y)) {
                            return pt;
                        }
                    } else {
                        return pt;
                    }
                }

                tries--;
            }
        } else {
            List<Point> pts = new ArrayList<>();
            
            int x1 = area.Left;
            int y1 = area.Top;
            int x2 = area.Right;
            int y2 = area.Bottom;

            if ((flags & TSF_BORDER) > 0) {
                for (int xx = x1; xx <= x2; xx++) {
                    checkTile(map, xx, y1, flags, pts);
                    checkTile(map, xx, y2, flags, pts);
                }

                for (int yy = y1; yy <= y2; yy++) {
                    checkTile(map, x1, yy, flags, pts);
                    checkTile(map, x2, yy, flags, pts);
                }
            } else {
                for (int y = y1; y <= y2; y++) {
                    for (int x = x1; x <= x2; x++) {
                        checkTile(map, x, y, flags, pts);
                    }
                }
            }
            
            return AuxUtils.getRandomItem(pts);
        }
        
        return null;
    }
    
    private static void checkTile(IMap map, int x, int y, int flags, List<Point> pts)
    {
        if ((flags & TSF_FREE) > 0) {
            if (!map.isBarrier(x, y)) {
                BaseTile tile = map.getTile(x, y);
                if (!tile.hasState(TileStates.TS_NFM)) {
                    pts.add(new Point(x, y));
                }
            }
        } else {
            pts.add(new Point(x, y));
        }
    }
}
