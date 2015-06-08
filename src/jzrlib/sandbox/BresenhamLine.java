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
package jzrlib.sandbox;

import java.util.ArrayList;
import java.util.List;
import jzrlib.core.Point;

/**
 * Bresenham's line drawing algorithm.
 * 
 * @author Serg V. Zhdanovskih
 */
public final class BresenhamLine
{
    /**
     * Used for calculation
     */
    private final int x_inc, y_inc, length;
    private int dx, dy, error, xx, yy, count;

    /**
     * Construct a Bresenham algorithm. Plot a line between (x1,y1) and (x2,y2).
     * To step through the line use next().
     *
     * @return the length of the line (which will be 1 more than you are expecting).
     */
    public BresenhamLine(int x1, int y1, int x2, int y2)
    {
        // compute horizontal and vertical deltas
        dx = x2 - x1;
        dy = y2 - y1;

        // test which direction the line is going in i.e. slope angle
        if (dx >= 0) {
            x_inc = 1;
        } else {
            x_inc = -1;
            dx = -dx; // need absolute value
        }

        // test y component of slope
        if (dy >= 0) {
            y_inc = 1;
        } else {
            y_inc = -1;
            dy = -dy; // need absolute value
        }

        xx = x1;
        yy = y1;

        if (dx > dy) {
            error = dx >> 1;
        } else {
            error = dy >> 1;
        }

        count = 0;
        length = Math.max(dx, dy) + 1;
    }

    /**
     * Get the next point in the line. You must not call next() if the previous
     * invocation of next() returned false.
     *
     * Retrieve the X and Y coordinates of the line with getX() and getY().
     *
     * @return true if there is another point to come.
     */
    public boolean next()
    {
        // now based on which delta is greater we can draw the line
        if (dx > dy) {
            // adjust the error term
            error += dy;

            // test if error has overflowed
            if (error >= dx) {
                error -= dx;

                // move to next line
                yy += y_inc;
            }

            // move to the next pixel
            xx += x_inc;
        } else {
            // adjust the error term
            error += dx;

            // test if error overflowed
            if (error >= dy) {
                error -= dy;

                // move to next line
                xx += x_inc;
            }

            // move to the next pixel
            yy += y_inc;
        }

        count++;
        return count < length;
    }

    /**
     * @return the current X coordinate
     */
    public int getX()
    {
        return xx;
    }

    /**
     * @return the current Y coordinate
     */
    public int getY()
    {
        return yy;
    }

    /**
     * Plot a line between (x1,y1) and (x2,y2).
     *
     * @return the length of the line
     */
    public static final List<Point> plot(int x1, int y1, int x2, int y2)
    {
        List<Point> result = new ArrayList<>();

        BresenhamLine bresenham = new BresenhamLine(x1, y1, x2, y2);
        for (int i = 0; i < bresenham.length; i++) {
            result.add(new Point(bresenham.getX(), bresenham.getY()));
            bresenham.next();
        }

        return result;
    }

    /**
     * Implements Bresenham's line drawing algorithm for finding straight paths.
     * Note that the integer only optimization is not included as using floats
     * makes for more intuitive and readable code. Hopefully the performance
     * issues of using a float are not what they were back in the days of
     * Bresenham.
     */
    public static List<Point> calcPath(Point start, Point end)
    {
        //Guard.argumentsAreNotNull(world, start, end);
        //Guard.argumentsInsideBounds(start.x(), start.y(), world.width(), world.height());

        List<Point> path = new ArrayList<>();
        int dx = end.X - start.X;
        int dy = end.Y - start.Y;

        if (Math.abs(dx) > Math.abs(dy)) {
            float slope = (float) dy / dx;
            int ix = (start.X < end.X) ? 1 : -1;
            int x = start.X;
            while (x != end.X) {
                x += ix;
                int y = (int) (slope * (x - start.X) + start.Y + .5f);
                path.add(new Point(x, y));
                /*if (!world.passableAt(x, y)) {
                 break;
                 }*/
            }
        } else {
            float slope = (float) dx / dy;
            int iy = (start.Y < end.Y) ? 1 : -1;
            int y = start.Y;
            while (y != end.Y) {
                y += iy;
                int x = (int) (slope * (y - start.Y) + start.X + .5f);
                path.add(new Point(x, y));
                /*if (!world.passableAt(x, y)) {
                 break;
                 }*/
            }
        }
        return path;
    }
}
