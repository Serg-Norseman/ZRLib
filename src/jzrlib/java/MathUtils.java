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
package jzrlib.java;

import jzrlib.core.Rect;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class MathUtils
{
    public static MathPoint intersectLine2Line(MathPoint p1, MathPoint p2, MathPoint p3, MathPoint p4)
    {
        double denom = ((p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y));
        if (denom == 0) {
            return null; // lines are parallel
        }
        double ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denom;
        double ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denom;

        if (ua < 0 || ua > 1 || ub < 0 || ub > 1) {
            return null;
        }

        return new MathPoint(p1.x + ua * (p2.x - p1.x), p1.y + ua * (p2.y - p1.y));
    }

    public static MathPoint intersectLine2Rect(MathPoint p1, MathPoint p2, Rect rt)
    {
        double bx1 = rt.Left;
        double by1 = rt.Top;
        double bx2 = rt.Right;
        double by2 = rt.Bottom;

        MathPoint tl = new MathPoint(bx1, by1);
        MathPoint tr = new MathPoint(bx2, by1);
        MathPoint bl = new MathPoint(bx1, by2);
        MathPoint br = new MathPoint(bx2, by2);

        MathPoint pt;

        pt = intersectLine2Line(p1, p2, tl, tr);
        if (pt != null) {
            return pt;
        }

        pt = intersectLine2Line(p1, p2, tr, br);
        if (pt != null) {
            return pt;
        }

        pt = intersectLine2Line(p1, p2, br, bl);
        if (pt != null) {
            return pt;
        }

        pt = intersectLine2Line(p1, p2, bl, tl);
        if (pt != null) {
            return pt;
        }

        return null;
    }
}
