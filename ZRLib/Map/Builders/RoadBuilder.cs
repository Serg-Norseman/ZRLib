/* 
 * Written by Kusigrosz in September 2010 (parts much earlier)
 * This program is in public domain, with all its bugs etc.
 * No warranty whatsoever.
 *
 * Generating winding roads/corridors for a roguelike game.
 *
 * The functions that do road generating are:
 *
 * uti_windroad(&road, mpc, x1, y1, x2, y2, pertamt)
 *     Generates a winding road from x1, y1 to x2, y2 without
 *     sharp turns. The road winds regardless of the relative location
 *     of endpoints (unless it is too short). The parameter pertamt
 *     controls the degree of perturbation the initially straight road
 *     is subjected to; typical values of 5-50 give decent results.
 *     mpc is the pointer to the map structure (needed to make sure
 *     the winding road stays within the map.
 *
 *  Java/C# translation.
 *  Copyright (C) 2014 by Serg V. Zhdanovskih.
 */

using System;
using BSLib;

namespace ZRLib.Map.Builders
{
    public sealed class RoadBuilder
    {
        private const int MAX_SEQ = 1024;

        private class SeqCells
        {
            public ExtPoint[] Pts = new ExtPoint[MAX_SEQ];
            public int Count;

            public SeqCells()
            {
                Count = 0;
            }

            public void Add(int x, int y)
            {
                Pts[Count] = new ExtPoint(x, y);
                Count++;
            }
        }

        private readonly int[] Xoff = new int[]{ 1, 1, 0, -1, -1, -1, 0, 1 };
        private readonly int[] Yoff = new int[]{ 0, 1, 1, 1, 0, -1, -1, -1 };

        private readonly IMap fMap;

        private static int Sqr(int x)
        {
            return (x * x);
        }

        /* 
         * The Bresenham line algorithm. Not symmetrical.
         */
        private SeqCells Rline(int x1, int y1, int x2, int y2)
        {
            SeqCells ret = new SeqCells();

            int xstep = Math.Sign(x2 - x1);
            int ystep = Math.Sign(y2 - y1);

            int xc = x1;
            int yc = y1;

            ret.Add(xc, yc);

            if ((x1 == x2) && (y1 == y2)) {
                return ret;
            }

            if (Math.Abs(x2 - x1) >= Math.Abs(y2 - y1)) {
                int acc = Math.Abs(x2 - x1);
                do {
                    xc += xstep;
                    acc += 2 * Math.Abs(y2 - y1);

                    if (acc >= 2 * Math.Abs(x2 - x1)) {
                        acc -= 2 * Math.Abs(x2 - x1);
                        yc += ystep;
                    }
                    ret.Add(xc, yc);
                } while ((xc != x2));
            } else {
                int acc = Math.Abs(y2 - y1);
                do {
                    yc += ystep;
                    acc += 2 * Math.Abs(x2 - x1);

                    if (acc >= 2 * Math.Abs(y2 - y1)) {
                        acc -= 2 * Math.Abs(y2 - y1);
                        xc += xstep;
                    }
                    ret.Add(xc, yc);
                } while ((yc != y2));
            }

            return ret;
        }

        /* 
         * The square of the cosine of the angle between vectors p0p1 and p1p2,
         * with the sign of the cosine, in permil (1.0 = 1000).
         */
        private int Signcos2(int x0, int y0, int x1, int y1, int x2, int y2)
        {
            int sqlen01 = Sqr(x1 - x0) + Sqr(y1 - y0);
            int sqlen12 = Sqr(x2 - x1) + Sqr(y2 - y1);

            int prod = (x1 - x0) * (x2 - x1) + (y1 - y0) * (y2 - y1);
            int val = 1000 * (prod * prod / sqlen01) / sqlen12; // Overflow?

            if (prod < 0) {
                val = -val;
            }

            return (val);
        }

        /* 
         * Select random points in the provided trajectory and displace them
         * provided no sharp angles are created, and the new location isn't
         * too close or too far from the neighbours.
         */
        private int Perturb(SeqCells way, int mindist, int maxdist, int pertamt)
        {
            int mincos2 = 500; // cos^2 in 1/1000, for angles < 45 degrees

            if (way.Count < 3) {
                /* nothing to do */
                return (0);
            }

            int mind2 = Sqr(mindist);
            int maxd2 = Sqr(maxdist);
            for (int i = 0; i < pertamt * way.Count; i++) {
                int ri = 1 + RandomHelper.GetRandom(way.Count - 2);
                int rdir = RandomHelper.GetRandom(8);
                int nx = way.Pts[ri].X + Xoff[rdir];
                int ny = way.Pts[ri].Y + Yoff[rdir];
                int lox = way.Pts[ri - 1].X;
                int loy = way.Pts[ri - 1].Y;
                int hix = way.Pts[ri + 1].X;
                int hiy = way.Pts[ri + 1].Y;
                int lod2 = Sqr(nx - lox) + Sqr(ny - loy);
                int hid2 = Sqr(nx - hix) + Sqr(ny - hiy);

                if ((!fMap.IsValid(nx, ny)) || (lod2 < mind2) || (lod2 > maxd2) || (hid2 < mind2) || (hid2 > maxd2)) {
                    continue;
                }
                /* Check the angle at ri (vertex at nx, ny) */
                if (Signcos2(lox, loy, nx, ny, hix, hiy) < mincos2) {
                    continue;
                }
                /* Check the angle at ri - 1 (vertex at lox, loy) */
                if ((ri > 1) && (Signcos2(way.Pts[ri - 2].X, way.Pts[ri - 2].Y, lox, loy, nx, ny) < mincos2)) {
                    continue;
                }
                /* Check the angle at ri + 1 (vertex at hix, hiy) */
                if ((ri < way.Count - 2) && (Signcos2(nx, ny, hix, hiy, way.Pts[ri + 2].X, way.Pts[ri + 2].Y) < mincos2)) {
                    continue;
                }

                way.Pts[ri] = new ExtPoint(nx, ny);
            }
            return (1);
        }

        /* 
         * Connect the waypoints in way with straight lines, putting the result
         * in the returned structure.
         */
        private SeqCells ConnectWayPoints(SeqCells waypts)
        {
            SeqCells result = new SeqCells();

            result.Add(waypts.Pts[0].X, waypts.Pts[0].Y);
            for (int i = 0; i < waypts.Count - 1; i++) {
                SeqCells segment = Rline(waypts.Pts[i].X, waypts.Pts[i].Y, waypts.Pts[i + 1].X, waypts.Pts[i + 1].Y);
                for (int j = 1; j < segment.Count; j++) {
                    result.Add(segment.Pts[j].X, segment.Pts[j].Y);
                }
            }

            return result;
        }

        private SeqCells CutCorners(SeqCells seq)
        {
            SeqCells result = new SeqCells();
            result.Add(seq.Pts[0].X, seq.Pts[0].Y);

            for (int i = 1; i < seq.Count - 1; i++) {
                ExtPoint old_pt = seq.Pts[i - 1];
                ExtPoint new_pt = seq.Pts[i];
                int dx = Math.Sign(new_pt.X - old_pt.X);
                int dy = Math.Sign(new_pt.Y - old_pt.Y);
                if (dx != 0 && dy != 0) {
                    ExtPoint cor_pt = new_pt;

                    switch (RandomHelper.GetRandom(2)) {
                        case 0:
                            cor_pt.X = seq.Pts[i - 1].X;
                            break;
                        case 1:
                            cor_pt.Y = seq.Pts[i - 1].Y;
                            break;
                    }

                    result.Add(cor_pt.X, cor_pt.Y);
                }

                result.Add(new_pt.X, new_pt.Y);
            }

            return result;
        }

        private SeqCells SplitWay(SeqCells waypts)
        {
            SeqCells result = new SeqCells();

            /* Copy one cell in two/three, making sure the ends are copied */
            for (int i = 0; i < waypts.Count;) {
                result.Add(waypts.Pts[i].X, waypts.Pts[i].Y);

                if ((i < waypts.Count - 5) || (i >= waypts.Count - 1)) {
                    i += 2 + RandomHelper.GetRandom(2);
                } else if (i == waypts.Count - 5) {
                    i += 2;
                } else {
                    i = waypts.Count - 1;
                }
            }

            return result;
        }

        private SeqCells Windroad(int x1, int y1, int x2, int y2, int pertamt)
        {
            SeqCells waypts = Rline(x1, y1, x2, y2);
            if (waypts.Count < 5) {
                /* Too short to wind, just copy the straight line */
                return waypts;
            }

            waypts = SplitWay(waypts);

            Perturb(waypts, 2, 5, pertamt); // waypoint dist min, max, 2, 5

            waypts = ConnectWayPoints(waypts);

            waypts = CutCorners(waypts);

            return waypts;
        }

        public RoadBuilder(IMap map)
        {
            fMap = map;
        }

        public void Generate(int x1, int y1, int x2, int y2, int pertamt, int roadTile, ITileChangeHandler tileHandler)
        {
            SeqCells road = Windroad(x1, y1, x2, y2, pertamt);

            bool refContinue = true;
            for (int i = 0; i < road.Count; i++) {
                int cx = road.Pts[i].X;
                int cy = road.Pts[i].Y;

                if (fMap.IsValid(cx, cy)) {
                    tileHandler(fMap, cx, cy, roadTile, ref refContinue);
                    if (!refContinue) {
                        break;
                    }
                    //fMap.getTile(cx, cy).foreGround = (short) roadTile;
                }
            }
        }
    }
}
