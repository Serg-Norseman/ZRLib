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
import jzrlib.core.CreatureEntity;
import jzrlib.core.Point;
import jzrlib.utils.Logger;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class PathSearch
{
    // PathSearch tile statuses
    public static final byte tps_Unvisited = 0;
    public static final byte tps_Path = 1;
    public static final byte tps_Bound = 2;
    public static final byte tps_Passed = 3;
    public static final byte tps_Start = 4;
    public static final byte tps_Finish = 5;
    public static final byte tps_Barrier = 6;

    public static final float BARRIER_COST = 1000.0f;
    
    public final static class PSResult
    {
        public final int Dist;
        public final Point Step;
        public final List<Point> Path;
        
        private PSResult(int dist, Point step, List<Point> path)
        {
            this.Dist = dist;
            this.Step = step;
            this.Path = path;
        }
    }

    public static PSResult search(IMap map, Point src, Point dst, CreatureEntity creature)
    {
        return search(map, src, dst, creature, false);
    }

    public static PSResult search(IMap map, Point src, Point dst, CreatureEntity creature, boolean needpath)
    {
        PSResult result = null;

        try {
            PathSearch.clear(map);

            AStar ast = new AStar(map, creature, src.X, src.Y, dst.X, dst.Y);

            if (ast.search()) {
                List<Point> path = null;
                if (needpath) {
                    path = new ArrayList<>();
                    path.add(new Point(dst.X, dst.Y));
                }
                
                int len = 1;
                Point prv = map.getTile(dst.X, dst.Y).pf_prev;
                while (prv.X != src.X || prv.Y != src.Y) {
                    len++;
                    if (path != null) {
                        path.add(0, prv);
                    }

                    BaseTile m_tile = map.getTile(prv.X, prv.Y);
                    m_tile.pf_status = PathSearch.tps_Path;
                    prv = m_tile.pf_prev;

                    if (len > map.getWidth() << 1) {
                        return null;
                    }
                }

                result = new PSResult(len, prv.clone(), path);
            }
        } catch (Exception ex) {
            Logger.write("PathSearch.search(): " + ex.getMessage());
            result = null;
        }

        return result;
    }

    public static void clear(IMap map)
    {
        int height = map.getHeight();
        int width = map.getWidth();
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                BaseTile m_tile = map.getTile(x, y);
                m_tile.pf_status = PathSearch.tps_Unvisited;
                m_tile.pf_prev = null;
            }
        }
    }
}
