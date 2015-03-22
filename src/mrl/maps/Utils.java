/*
 *  "MysteriesRL", Java Roguelike game.
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
package mrl.maps;

import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.external.bsp.SideType;
import jzrlib.map.BaseTile;
import jzrlib.map.IMap;
import jzrlib.utils.AuxUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class Utils
{
    /**
     * unused
     */
    public static final Axis getTilesLineAxis(IMap map, int x, int y, int tileId)
    {
        if (map.checkTile(x - 1, y, tileId, true) && map.checkTile(x + 1, y, tileId, true)) {
            return Axis.axHorz;
        } else if (map.checkTile(x, y - 1, tileId, true) && map.checkTile(x, y + 1, tileId, true)) {
            return Axis.axVert;
        }

        return Axis.axNone;
    }
    
    public static final Axis getSideAxis(SideType side)
    {
        switch (side) {
            case stTop:
                return Axis.axHorz;
            case stRight:
                return Axis.axVert;
            case stBottom:
                return Axis.axHorz;
            case stLeft:
                return Axis.axVert;
            default:
                return Axis.axNone;
        }
    }
    
    public static Point getCheckPoint(SideType side, Rect area, int delta)
    {
        Point result;
        switch (side) {
            case stTop:
                result = new Point(AuxUtils.getBoundedRnd(area.Left + 1, area.Right - 1), area.Top - delta);
                break;
            case stRight: 
                result = new Point(area.Right + delta, AuxUtils.getBoundedRnd(area.Top + 1, area.Bottom - 1));
                break;
            case stBottom:
                result = new Point(AuxUtils.getBoundedRnd(area.Left + 1, area.Right - 1), area.Bottom + delta);
                break;
            case stLeft:
                result = new Point(area.Left - delta, AuxUtils.getBoundedRnd(area.Top + 1, area.Bottom - 1));
                break;
            default:
                result = null;
        }

        return result;
    }

    public static boolean hasTiles(IMap map, Rect area, int tileId, boolean fg)
    {
        for (int y = area.Top; y <= area.Bottom; y++) {
            for (int x = area.Left; x <= area.Right; x++) {
                BaseTile mtile = map.getTile(x, y);

                if (mtile != null) {
                    int tid = (fg) ? mtile.Foreground : mtile.Background;

                    if (tid == tileId) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    public static void genRareTiles(IMap map, Rect area, float densityFactor, int tileId, boolean fg)
    {
        int num = (int) (area.getSquare() * densityFactor);
        for (int i = 1; i <= num; i++) {
            Point pt = AuxUtils.getRandomPoint(area);
            BaseTile mtile = map.getTile(pt.X, pt.Y);

            if (mtile != null && mtile.getBackBase() != TileID.tid_Water.Value) {
                if (fg) {
                    mtile.Foreground = (short) tileId;
                } else {
                    mtile.Background = (short) tileId;
                }
            }
        }
    }
    
    public static void fillBorder(IMap map, Rect mapRect, int check, int tid)
    {
        int x1 = mapRect.Left; 
        int y1 = mapRect.Top;
        int x2 = mapRect.Right;
        int y2 = mapRect.Bottom;

        for (int xx = x1; xx <= x2; xx++) {
            setTile(map, y1, xx, check, tid);
            setTile(map, y2, xx, check, tid);
        }

        for (int yy = y1; yy <= y2; yy++) {
            setTile(map, yy, x1, check, tid);
            setTile(map, yy, x2, check, tid);
        }
    }
    
    private static void setTile(IMap map, int y, int x, int check, int tid)
    {
        BaseTile tile = map.getTile(x, y);
        if (tile != null && tile.Foreground == check) {
            tile.Foreground = (short) tid;
        } 
    }
}
