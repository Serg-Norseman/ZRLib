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
package jzrlib.core;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Point implements Cloneable
{
    public int X;
    public int Y;

    public Point()
    {
    }

    public Point(int ax, int ay)
    {
        this.X = ax;
        this.Y = ay;
    }

    @Override
    public String toString()
    {
        return "[X=" + Integer.toString(this.X) + ",Y=" + Integer.toString(this.Y) + "]";
    }

    public final void setValues(int ax, int ay)
    {
        this.X = ax;
        this.Y = ay;
    }

    public static Point Zero()
    {
        return new Point(0, 0);
    }

    public boolean isZero()
    {
        return (this.X == 0 && this.Y == 0);
    }

    public final boolean equals(int ax, int ay)
    {
        return this.X == ax && this.Y == ay;
    }

    public final boolean equals(Point value)
    {
        if (value == null) {
            return false;
        }
        return (this.X == value.X && this.Y == value.Y);
    }

    @Override
    public Point clone()
    {
        return new Point(this.X, this.Y);
    }
}
