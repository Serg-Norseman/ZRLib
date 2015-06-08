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
package jzrlib.map.builders;

import java.util.ArrayList;
import jzrlib.core.Rect;
import jzrlib.map.BaseTile;
import jzrlib.map.IMap;
import jzrlib.map.TileType;
import jzrlib.utils.AuxUtils;

/**
 * Generating caves.
 * Newsgroups: rec.games.roguelike.development
 * @author su <steve_ued@yahoo.com>, 28.08.2004
 * @author Serg V. Zhdanovskih
 */
public final class CaveBuilder extends BaseBuilder
{
    private static final class CavePoint
    {
        public int x;
        public int y;
        public int room;

        public CavePoint(int x, int y, int room)
        {
            this.x = x;
            this.y = y;
            this.room = room;
        }
    }

    private int cm_height, cm_width;
    private int[][] connects_map;
    private int room_counter;
    private ArrayList<CavePoint> points;

    public CaveBuilder(IMap map)
    {
        super(map);
    }

    public CaveBuilder(IMap map, Rect area)
    {
        super(map, area);
    }

    public final void build()
    {
        Rect rt = this.fArea;
        this.cm_height = rt.getHeight();
        this.cm_width = rt.getWidth();

        for (int y = rt.Top; y <= rt.Bottom; y++) {
            for (int x = rt.Left; x <= rt.Right; x++) {
                BaseTile tile = fMap.getTile(x, y);
                tile.Background = fMap.translateTile(TileType.ttCaveFloor);
                tile.Foreground = fMap.translateTile(TileType.ttCaveWall);
            }
        }

        AuxUtils.randomize();

        for (int y = rt.Top + 1; y <= rt.Bottom - 1; y++) {
            for (int x = rt.Left + 1; x <= rt.Right - 1; x++) {
                int tmp = AuxUtils.getRandom(10);
                if (tmp % 2 == 1) {
                    fMap.getTile(x, y).Foreground = 0;
                }
            }
        }

        // 1
        int removed_counter = 0;
        for (int y = rt.Top + 2; y <= rt.Bottom - 2; y++) {
            for (int x = rt.Left + 2; x <= rt.Right - 2; x++) {
                BaseTile tile = fMap.getTile(x, y);

                if (tile.Foreground == fMap.translateTile(TileType.ttCaveWall)) {
                    int adjacent_floor_count = 0;
                    //adjacent_floor_count = checkAdjacentlyTile(map, x, y, TileType.ttUndefined);
                    if (fMap.getTile(x, y - 1).Foreground == 0) {
                        adjacent_floor_count++;
                    }
                    if (fMap.getTile(x, y + 1).Foreground == 0) {
                        adjacent_floor_count++;
                    }
                    if (fMap.getTile(x - 1, y).Foreground == 0) {
                        adjacent_floor_count++;
                    }
                    if (fMap.getTile(x + 1, y).Foreground == 0) {
                        adjacent_floor_count++;
                    }

                    if (adjacent_floor_count >= 3) {
                        if (removed_counter + 1 == 10) {
                            removed_counter = 0;
                        } else {
                            tile.Foreground = 0;
                        }
                    }
                }
            }
        }

        // 2
        for (int y = rt.Top + 1; y <= rt.Bottom - 1; y++) {
            for (int x = rt.Left + 1; x <= rt.Right - 1; x++) {
                BaseTile tile = fMap.getTile(x, y);

                if (tile.Foreground == 0) {
                    int adjacents = fMap.checkForeAdjacently(x, y, TileType.ttUndefined);

                    if (adjacents < 2) {
                        tile.Foreground = fMap.translateTile(TileType.ttCaveWall);
                    }
                }
            }
        }

        this.connects_map = new int[cm_height][cm_width];
        this.room_counter = 0;
        this.points = new ArrayList<>();

        do {
            this.processConnects();
        } while (room_counter > 1);

        // only for debug
        /*int id = fMap.translateTile(TileType.ttGrass);
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (connects_map[y][x] > 0) {
                    BaseTile tile = fMap.getTile(x, y);
                    tile.backGround = (short) (id + connects_map[y][x]);
                }
            }
        }*/
    }
    
    private void processConnects()
    {
        Rect rt = this.fArea;
        for (int y = 0; y < cm_height; y++) {
            for (int x = 0; x < cm_width; x++) {
                int mx = rt.Left + x;
                int my = rt.Top + y;
                this.connects_map[y][x] = (this.fMap.getTile(mx, my).Foreground == 0) ? 0 : -1;
            }
        }

        this.room_counter = 0;
        for (int y = 0; y < cm_height; y++) {
            for (int x = 0; x < cm_width; x++) {
                if (this.connects_map[y][x] == 0) {
                    this.room_counter++;
                    searchFloodfill(x, y);
                }
            }
        }

        if (this.room_counter <= 1) {
            return;
        }

        this.points.clear();
        for (int y = 1; y < cm_height - 2; y++) {
            for (int x = 1; x < cm_width - 2; x++) {
                int room = this.connects_map[y][x];
                if (room > 0 && this.checkAdjacently(x, y)) {
                    this.points.add(new CavePoint(x, y, room));
                }
            }
        }

        if (this.points.size() < 2) {
            throw new RuntimeException("gen_Cave(): no connect points");
        }
        
        CavePoint tmp = this.points.get(0);
        final int max = cm_width * cm_height;
        // find near point of other room
        int pt_dist = max;
        CavePoint ptx1 = null;
        for (int k = 1; k < this.points.size(); k++) {
            CavePoint pt2 = this.points.get(k);

            if (tmp.room != pt2.room) {
                int d = AuxUtils.distance(tmp.x, tmp.y, pt2.x, pt2.y);
                if (d < pt_dist) {
                    pt_dist = d;
                    ptx1 = pt2;
                }
            }
        }

        if (ptx1 != null) {
            CavePoint ptx2 = findNearPoint(ptx1.x, ptx1.y, tmp.room, max);

            if (ptx2 != null) {
                fMap.gen_Path(rt.Left + ptx1.x, rt.Top + ptx1.y, rt.Left + ptx2.x, rt.Top + ptx2.y, this.fArea, 0, false, false, null);
            }
        }
    }
    
    private boolean isValidCM(int x, int y)
    {
        return (x >= 0 && x < cm_width && y >= 0 && y < cm_height);
    }
    
    private void searchFloodfill(int cx, int cy)
    {
        if (!this.isValidCM(cx, cy) || this.connects_map[cy][cx] != 0) {
            return;
        }

        this.connects_map[cy][cx] = this.room_counter;

        for (int yy = cy - 1; yy <= cy + 1; yy++) {
            for (int xx = cx - 1; xx <= cx + 1; xx++) {
                searchFloodfill(xx, yy);
            }
        }
    }

    private boolean checkAdjacently(int cx, int cy)
    {
        for (int yy = cy - 1; yy <= cy + 1; yy++) {
            for (int xx = cx - 1; xx <= cx + 1; xx++) {
                if ((xx != cx) || (yy != cy)) {
                    if (this.connects_map[yy][xx] == -1) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private CavePoint findNearPoint(int xx, int yy, int needRoom, int max)
    {
        CavePoint result = null;
        int resDist = max;

        for (CavePoint cp : this.points) {
            if (cp.room == needRoom) {
                int d = AuxUtils.distance(xx, yy, cp.x, cp.y);
                if (d < resDist) {
                    result = cp;
                    resDist = d;
                }
            }
        }

        return result;
    }
}
