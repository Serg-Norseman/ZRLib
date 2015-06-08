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
 *  Java translation.
 *  Copyright (C) 2014 by Serg V. Zhdanovskih (aka Alchemist).
 */
package jzrlib.map.builders;

import jzrlib.core.Point;
import jzrlib.map.IMap;
import jzrlib.map.ITileChangeHandler;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.RefObject;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class RoadBuilder
{
    private static final int MAX_SEQ = 1024;

    private static class SeqCells
    {
        public Point[] pts = new Point[MAX_SEQ];
        public int count;

        public SeqCells()
        {
            this.count = 0;
        }

        public final void add(int x, int y)
        {
            this.pts[this.count] = new Point(x, y);
            this.count++;
        }
    }

    private final int[] Xoff = new int[]{1, 1, 0, -1, -1, -1, 0, 1};
    private final int[] Yoff = new int[]{0, 1, 1, 1, 0, -1, -1, -1};

    private final IMap fMap;

    private static int sqr(int x)
    {
        return (x * x);
    }

    /* 
     * The Bresenham line algorithm. Not symmetrical.
     */
    private SeqCells rline(int x1, int y1, int x2, int y2)
    {
        SeqCells ret = new SeqCells();

        int xstep = Integer.signum(x2 - x1);
        int ystep = Integer.signum(y2 - y1);

        int xc = x1;
        int yc = y1;

        ret.add(xc, yc);

        if ((x1 == x2) && (y1 == y2)) {
            return ret;
        }

        if (Math.abs(x2 - x1) >= Math.abs(y2 - y1)) {
            int acc = Math.abs(x2 - x1);
            do {
                xc += xstep;
                acc += 2 * Math.abs(y2 - y1);

                if (acc >= 2 * Math.abs(x2 - x1)) {
                    acc -= 2 * Math.abs(x2 - x1);
                    yc += ystep;
                }
                ret.add(xc, yc);
            } while ((xc != x2));
        } else {
            int acc = Math.abs(y2 - y1);
            do {
                yc += ystep;
                acc += 2 * Math.abs(x2 - x1);

                if (acc >= 2 * Math.abs(y2 - y1)) {
                    acc -= 2 * Math.abs(y2 - y1);
                    xc += xstep;
                }
                ret.add(xc, yc);
            } while ((yc != y2));
        }

        return ret;
    }

    /* 
     * The square of the cosine of the angle between vectors p0p1 and p1p2,
     * with the sign of the cosine, in permil (1.0 = 1000).
     */
    private int signcos2(int x0, int y0, int x1, int y1, int x2, int y2)
    {
        int sqlen01 = sqr(x1 - x0) + sqr(y1 - y0);
        int sqlen12 = sqr(x2 - x1) + sqr(y2 - y1);

        int prod = (x1 - x0) * (x2 - x1) + (y1 - y0) * (y2 - y1);
        int val = 1000 * (prod * prod / sqlen01) / sqlen12; /* Overflow? */

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
    private int perturb(SeqCells way, int mindist, int maxdist, int pertamt)
    {
        int mincos2 = 500; /* cos^2 in 1/1000, for angles < 45 degrees */

        if (way.count < 3) {
            /* nothing to do */
            return (0);
        }

        int mind2 = sqr(mindist);
        int maxd2 = sqr(maxdist);
        for (int i = 0; i < pertamt * way.count; i++) {
            int ri = 1 + AuxUtils.getRandom(way.count - 2);
            int rdir = AuxUtils.getRandom(8);
            int nx = way.pts[ri].X + Xoff[rdir];
            int ny = way.pts[ri].Y + Yoff[rdir];
            int lox = way.pts[ri - 1].X;
            int loy = way.pts[ri - 1].Y;
            int hix = way.pts[ri + 1].X;
            int hiy = way.pts[ri + 1].Y;
            int lod2 = sqr(nx - lox) + sqr(ny - loy);
            int hid2 = sqr(nx - hix) + sqr(ny - hiy);

            if ((!fMap.isValid(nx, ny))
                    || (lod2 < mind2) || (lod2 > maxd2)
                    || (hid2 < mind2) || (hid2 > maxd2)) {
                continue;
            }
            /* Check the angle at ri (vertex at nx, ny) */
            if (signcos2(lox, loy, nx, ny, hix, hiy) < mincos2) {
                continue;
            }
            /* Check the angle at ri - 1 (vertex at lox, loy) */
            if ((ri > 1) && (signcos2(way.pts[ri - 2].X, way.pts[ri - 2].Y, lox, loy, nx, ny) < mincos2)) {
                continue;
            }
            /* Check the angle at ri + 1 (vertex at hix, hiy) */
            if ((ri < way.count - 2) && (signcos2(nx, ny, hix, hiy, way.pts[ri + 2].X, way.pts[ri + 2].Y) < mincos2)) {
                continue;
            }

            way.pts[ri] = new Point(nx, ny);
        }
        return (1);
    }

    /* 
     * Connect the waypoints in way with straight lines, putting the result
     * in the returned structure.
     */
    private SeqCells connectWayPoints(SeqCells waypts)
    {
        SeqCells result = new SeqCells();

        result.add(waypts.pts[0].X, waypts.pts[0].Y);
        for (int i = 0; i < waypts.count - 1; i++) {
            SeqCells segment = rline(waypts.pts[i].X, waypts.pts[i].Y, waypts.pts[i + 1].X, waypts.pts[i + 1].Y);
            for (int j = 1; j < segment.count; j++) {
                result.add(segment.pts[j].X, segment.pts[j].Y);
            }
        }

        return result;
    }

    private SeqCells cutCorners(SeqCells seq)
    {
        SeqCells result = new SeqCells();
        result.add(seq.pts[0].X, seq.pts[0].Y);

        for (int i = 1; i < seq.count - 1; i++) {
            Point old_pt = seq.pts[i - 1];
            Point new_pt = seq.pts[i];
            int dx = Integer.signum(new_pt.X - old_pt.X);
            int dy = Integer.signum(new_pt.Y - old_pt.Y);
            if (dx != 0 && dy != 0) {
                Point cor_pt = new_pt.clone();

                switch (AuxUtils.getRandom(2)) {
                    case 0:
                        cor_pt.X = seq.pts[i - 1].X;
                        break;
                    case 1:
                        cor_pt.Y = seq.pts[i - 1].Y;
                        break;
                }

                result.add(cor_pt.X, cor_pt.Y);
            }

            result.add(new_pt.X, new_pt.Y);
        }

        return result;
    }

    private SeqCells splitWay(SeqCells waypts)
    {
        SeqCells result = new SeqCells();

        /* Copy one cell in two/three, making sure the ends are copied */
        for (int i = 0; i < waypts.count;) {
            result.add(waypts.pts[i].X, waypts.pts[i].Y);

            if ((i < waypts.count - 5) || (i >= waypts.count - 1)) {
                i += 2 + AuxUtils.getRandom(2);
            } else if (i == waypts.count - 5) {
                i += 2;
            } else {
                i = waypts.count - 1;
            }
        }

        return result;
    }

    private SeqCells windroad(int x1, int y1, int x2, int y2, int pertamt)
    {
        SeqCells waypts = rline(x1, y1, x2, y2);
        if (waypts.count < 5) {
            /* Too short to wind, just copy the straight line */
            return waypts;
        }

        waypts = splitWay(waypts);

        perturb(waypts, 2, 5, pertamt); /* waypoint dist min, max, 2, 5 */

        waypts = connectWayPoints(waypts);

        waypts = cutCorners(waypts);

        return waypts;
    }

    public RoadBuilder(IMap map)
    {
        this.fMap = map;
    }
    
    public final void generate(int x1, int y1, int x2, int y2, int pertamt, int roadTile, ITileChangeHandler tileHandler)
    {
        SeqCells road = windroad(x1, y1, x2, y2, pertamt);

        RefObject<Boolean> refContinue = new RefObject<>(true);
        for (int i = 0; i < road.count; i++) {
            int cx = road.pts[i].X;
            int cy = road.pts[i].Y;

            if (fMap.isValid(cx, cy)) {
                tileHandler.invoke(this.fMap, cx, cy, roadTile, refContinue);
                if (!refContinue.argValue) {
                    break;
                }
                //fMap.getTile(cx, cy).foreGround = (short) roadTile;
            }
        }
    }
}
