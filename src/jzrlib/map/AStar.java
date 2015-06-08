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

import jzrlib.core.CreatureEntity;
import jzrlib.core.Point;
import jzrlib.java.SmartPriorityQueue;
import jzrlib.utils.Logger;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class AStar
{
    private static final class PSBound implements Comparable<PSBound>
    {
        public short px;
        public short py;
        public float gval;
        public float fval;

        public PSBound(int px, int py, float gval, float fval)
        {
            this.px = (short) px;
            this.py = (short) py;
            this.gval = gval;
            this.fval = fval;
        }
        
        @Override
        public int compareTo(PSBound obj)
        {
            if (this.fval < obj.fval) {
                return -1;
            }
            if (this.fval > obj.fval) {
                return 1;
            }
            return 0;
        }
    }

    private static final int MaxBoundSize = 500;
    private static final float[] kk;
    private static final int[] CoursesX, CoursesY;

    static {
        kk = new float[] {1.42f, 1.0f}; // 1.42f
        CoursesX = new int[] {  0,  1,  1,  1,  0, -1, -1, -1};
        CoursesY = new int[] { -1, -1,  0,  1,  1,  1,  0, -1};
    }

    private final IMap map;
    private final CreatureEntity creature;
    private final int src_x;
    private final int src_y;
    private final int dst_x;
    private final int dst_y;
    private final int dtx;
    private final int dty;

    public AStar(IMap map, CreatureEntity creature, int src_x, int src_y, int dst_x, int dst_y)
    {
        this.map = map;
        this.creature = creature;
        this.src_x = src_x;
        this.src_y = src_y;
        this.dst_x = dst_x;
        this.dst_y = dst_y;

        this.dtx = src_x - dst_x;
        this.dty = src_y - dst_y;
    }

    private float hest(int ax, int ay)
    {
        int dx = ax - this.dst_x;
        int dy = ay - this.dst_y;
        int cross = dx * this.dty - this.dtx * dy;
        if (cross < 0) {
            cross = -cross;
        }
        return (float) (Math.max(Math.abs(dx), Math.abs(dy)) + (float) cross * 0.001f);
    }

    public final boolean search()
    {
        try {
            SmartPriorityQueue<PSBound> bounds = new SmartPriorityQueue<>(MaxBoundSize);

            map.getTile(this.dst_x, this.dst_y).pf_status = PathSearch.tps_Finish;

            float gval = 0.0f;
            bounds.add(new PSBound(this.src_x, this.src_y, gval, (gval + hest(this.src_x, this.src_y))));

            while (!bounds.isEmpty()) {
                PSBound bnd = bounds.poll();
                int bx = bnd.px;
                int by = bnd.py;
                float b_gval = bnd.gval;

                BaseTile b_tile = map.getTile(bx, by);
                b_tile.pf_status = PathSearch.tps_Passed;

                for (int i = 1; i <= 8; i++) {
                    int ax = bx + CoursesX[i - 1];
                    int ay = by + CoursesY[i - 1];

                    BaseTile a_tile = map.getTile(ax, ay);
                    if (a_tile != null) {
                        float cost = map.getPathTileCost(this.creature, ax, ay, a_tile);
                        
                        byte pf_status;
                        if (cost >= PathSearch.BARRIER_COST) {
                            pf_status = PathSearch.tps_Barrier;
                        } else {
                            pf_status = a_tile.pf_status;
                        }

                        switch (pf_status) {
                            case PathSearch.tps_Unvisited:
                                gval = (b_gval + cost * kk[i % 2]);
                                a_tile.pf_prev = new Point(bx, by);
                                a_tile.pf_status = PathSearch.tps_Bound;
                                bounds.add(new PSBound(ax, ay, gval, (gval + hest(ax, ay))));
                                break;

                            case PathSearch.tps_Finish:
                                a_tile.pf_prev = new Point(bx, by);
                                map.getTile(this.src_x, this.src_y).pf_status = PathSearch.tps_Start;
                                return true;
                        }
                    }
                }
            }

            map.getTile(this.src_x, this.src_y).pf_status = PathSearch.tps_Start;
        } catch (Exception ex) {
            Logger.write("AStar.search(): " + ex.getMessage());
        }

        return false;
    }

}
