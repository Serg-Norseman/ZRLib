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

import jzrlib.core.Rect;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class MapUtils
{
    private static final TileType[] dtWalls;
    private static final TileType[] dtWaters;
    private static final TileType[][] dtDoors;
    private static final TileType[] dtBuilding;

    static {
        dtWalls = new TileType[]{TileType.ttBWallN, TileType.ttBWallS, TileType.ttBWallW, TileType.ttBWallE, TileType.ttBWallNW, TileType.ttBWallNE, TileType.ttBWallSW, TileType.ttBWallSE, TileType.ttWall, TileType.ttDoor, TileType.ttDoorXX_Closed, TileType.ttDoorYY_Closed, TileType.ttDoorXX_Opened, TileType.ttDoorYY_Opened};
        dtWaters = new TileType[]{TileType.ttWater, TileType.ttDeepWater, TileType.ttDeeperWater};

        dtDoors = new TileType[2][2];
        dtDoors[0][0] = TileType.ttDoorXX_Opened;
        dtDoors[0][1] = TileType.ttDoorXX_Closed;
        dtDoors[1][0] = TileType.ttDoorYY_Opened;
        dtDoors[1][1] = TileType.ttDoorYY_Closed;

        dtBuilding = new TileType[]{TileType.ttWall, TileType.ttFloor, TileType.ttDoor, TileType.ttDoorXX_Closed, TileType.ttDoorYY_Closed, TileType.ttBWallN, TileType.ttBWallS, TileType.ttBWallW, TileType.ttBWallE, TileType.ttBWallNW, TileType.ttBWallNE, TileType.ttBWallSW, TileType.ttBWallSE, TileType.ttDoorXX_Opened, TileType.ttDoorYY_Opened};
    }

    public static TileType getWallKind(int x, int y, Rect r)
    {
        TileType result = TileType.ttFloor;

        if (x == r.Left && y == r.Top) {
            result = TileType.ttBWallNW;
        } else if (x == r.Right && y == r.Top) {
            result = TileType.ttBWallNE;
        } else if (x == r.Left && y == r.Bottom) {
            result = TileType.ttBWallSW;
        } else if (x == r.Right && y == r.Bottom) {
            result = TileType.ttBWallSE;
        } else if (x > r.Left && x < r.Right && y == r.Top) {
            result = TileType.ttBWallN;
        } else if (x > r.Left && x < r.Right && y == r.Bottom) {
            result = TileType.ttBWallS;
        } else if (y > r.Top && y < r.Bottom && x == r.Left) {
            result = TileType.ttBWallW;
        } else if (y > r.Top && y < r.Bottom && x == r.Right) {
            result = TileType.ttBWallE;
        }

        return result;
    }

    public static final boolean isBuilding(AbstractMap map, short tile)
    {
        for (TileType buildingTile : dtBuilding) {
            if (map.translateTile(buildingTile) == tile) {
                return true;
            }
        }
        return false;
    }

    public static final boolean isWalls(AbstractMap map, short tile)
    {
        for (TileType wallTile : dtWalls) {
            if (map.translateTile(wallTile) == tile) {
                return true;
            }
        }
        return false;
    }

    public static final boolean isWaters(AbstractMap map, short tile)
    {
        for (TileType waterTile : dtWaters) {
            if (map.translateTile(waterTile) == tile) {
                return true;
            }
        }
        return false;
    }

    public static final void normalizeDoors(AbstractMap map, Rect r, boolean DefClosed)
    {
        for (int x = r.Left; x <= r.Right; x++) {
            for (int y = r.Top; y <= r.Bottom; y++) {
                BaseTile t = map.getTile(x, y);
                if (t != null && t.Foreground == map.translateTile(TileType.ttDoor)) {
                    if (isWalls(map, map.getTile(x, y + 1).Foreground) && isWalls(map, map.getTile(x, y - 1).Foreground)) {
                        t.Foreground = map.translateTile(dtDoors[0][DefClosed ? 1 : 0]);
                    }
                    if (isWalls(map, map.getTile(x + 1, y).Foreground) && isWalls(map, map.getTile(x - 1, y).Foreground)) {
                        t.Foreground = map.translateTile(dtDoors[1][DefClosed ? 1 : 0]);
                    }
                }
            }
        }
    }
    
}
