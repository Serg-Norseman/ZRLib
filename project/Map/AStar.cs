/*
 *  "ZRLib", Roguelike games development Library.
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

using System;
using BSLib;
using ZRLib.Core;

namespace ZRLib.Map
{
    public sealed class AStar
    {
        private sealed class PSBound : IComparable<PSBound>
        {
            public short Px;
            public short Py;
            public float Gval;
            public float Fval;

            public PSBound(int px, int py, float gval, float fval)
            {
                Px = (short)px;
                Py = (short)py;
                Gval = gval;
                Fval = fval;
            }

            public int CompareTo(PSBound obj)
            {
                if (Fval < obj.Fval) {
                    return -1;
                }
                if (Fval > obj.Fval) {
                    return 1;
                }
                return 0;
            }
        }

        private const int MaxBoundSize = 500;
        private static readonly float[] Kk;
        private static readonly int[] CoursesX, CoursesY;

        static AStar()
        {
            Kk = new float[] { 1.42f, 1.0f }; // 1.42f
            CoursesX = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };
            CoursesY = new int[] { -1, -1, 0, 1, 1, 1, 0, -1 };
        }

        private readonly IMap Map;
        private readonly CreatureEntity Creature;
        private readonly int Src_x;
        private readonly int Src_y;
        private readonly int Dst_x;
        private readonly int Dst_y;
        private readonly int Dtx;
        private readonly int Dty;

        public AStar(IMap map, CreatureEntity creature, int src_x, int src_y, int dst_x, int dst_y)
        {
            Map = map;
            Creature = creature;
            Src_x = src_x;
            Src_y = src_y;
            Dst_x = dst_x;
            Dst_y = dst_y;

            Dtx = src_x - dst_x;
            Dty = src_y - dst_y;
        }

        private float Hest(int ax, int ay)
        {
            int dx = ax - Dst_x;
            int dy = ay - Dst_y;
            int cross = dx * Dty - Dtx * dy;
            if (cross < 0) {
                cross = -cross;
            }
            return (float)(Math.Max(Math.Abs(dx), Math.Abs(dy)) + (float)cross * 0.001f);
        }

        public bool Search()
        {
            try {
                PriorityQueue<PSBound> bounds = new PriorityQueue<PSBound>(MaxBoundSize);

                Map.GetTile(Dst_x, Dst_y).Pf_status = PathSearch.tps_Finish;

                float gval = 0.0f;
                bounds.Add(new PSBound(Src_x, Src_y, gval, (gval + Hest(Src_x, Src_y))));

                while (!bounds.Empty) {
                    PSBound bnd = bounds.Poll();
                    int bx = bnd.Px;
                    int by = bnd.Py;
                    float b_gval = bnd.Gval;

                    BaseTile b_tile = Map.GetTile(bx, by);
                    b_tile.Pf_status = PathSearch.tps_Passed;

                    for (int i = 1; i <= 8; i++) {
                        int ax = bx + CoursesX[i - 1];
                        int ay = by + CoursesY[i - 1];

                        BaseTile a_tile = Map.GetTile(ax, ay);
                        if (a_tile != null) {
                            float cost = Map.GetPathTileCost(Creature, ax, ay, a_tile);

                            byte pf_status;
                            if (cost >= PathSearch.BARRIER_COST) {
                                pf_status = PathSearch.tps_Barrier;
                            } else {
                                pf_status = a_tile.Pf_status;
                            }

                            switch (pf_status) {
                                case PathSearch.tps_Unvisited:
                                    gval = (b_gval + cost * Kk[i % 2]);
                                    a_tile.Pf_prev = new ExtPoint(bx, by);
                                    a_tile.Pf_status = PathSearch.tps_Bound;
                                    bounds.Add(new PSBound(ax, ay, gval, (gval + Hest(ax, ay))));
                                    break;

                                case PathSearch.tps_Finish:
                                    a_tile.Pf_prev = new ExtPoint(bx, by);
                                    Map.GetTile(Src_x, Src_y).Pf_status = PathSearch.tps_Start;
                                    return true;
                            }
                        }
                    }
                }

                Map.GetTile(Src_x, Src_y).Pf_status = PathSearch.tps_Start;
            } catch (Exception ex) {
                Logger.Write("AStar.search(): " + ex.Message);
            }

            return false;
        }
    }
}
