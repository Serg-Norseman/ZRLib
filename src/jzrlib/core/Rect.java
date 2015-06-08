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
public final class Rect implements Cloneable
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public Rect()
    {
    }

    public Rect(int ALeft, int ATop, int ARight, int ABottom)
    {
        this.Left = ALeft;
        this.Top = ATop;
        this.Right = ARight;
        this.Bottom = ABottom;
    }

    public final void setBounds(int ALeft, int ATop, int ARight, int ABottom)
    {
        this.Left = ALeft;
        this.Top = ATop;
        this.Right = ARight;
        this.Bottom = ABottom;
    }

    @Override
    public String toString()
    {
        return "[X=" + String.valueOf(this.Left) + ",Y=" + String.valueOf(this.Top) + 
                ",Width=" + String.valueOf(this.getWidth()) + ",Height=" + String.valueOf(this.getHeight()) + "]";
    }

    public final int getWidth()
    {
        return (this.Right - this.Left) + 1;
    }

    public final int getHeight()
    {
        return (this.Bottom - this.Top) + 1;
    }

    public static Rect Empty()
    {
        return new Rect(0, 0, 0, 0);
    }

    public final boolean isEmpty()
    {
        return this.Right <= this.Left || this.Bottom <= this.Top;
    }

    public final boolean equals(Rect value)
    {
        return this.Left == value.Left && this.Top == value.Top && this.Right == value.Right && this.Bottom == value.Bottom;
    }

    public final boolean contains(int ptX, int ptY)
    {
        return ptX >= this.Left && ptY >= this.Top && ptX <= this.Right && ptY <= this.Bottom;
    }

    public final boolean contains(Point pt)
    {
        return this.contains(pt.X, pt.Y);
    }

    public final boolean contains(Rect rt)
    {
        return this.contains(rt.Left, rt.Top) && this.contains(rt.Right, rt.Top) 
                && this.contains(rt.Left, rt.Bottom) && this.contains(rt.Right, rt.Bottom);
    }

    /*public final boolean Intersects(TRect other)
    {
        return this.Left < other.Right && this.Right > other.Left && this.Top < other.Bottom && this.Bottom > other.Top;
    }*/

    public final Rect offset(int dX, int dY)
    {
        return new Rect(this.Left + dX, this.Top + dY, this.Right + dX, this.Bottom + dY);
    }

    public final Rect offset(Point value)
    {
        return this.offset(value.X, value.Y);
    }

    public final void inflate(int dX, int dY)
    {
        this.Left -= dX;
        this.Top -= dY;
        this.Right += dX;
        this.Bottom += dY;
    }

    public final boolean isBorder(int x, int y)
    {
        return y == this.Top || y == this.Bottom || x == this.Left || x == this.Right;
    }

    public final boolean isInside(Rect checkedRect)
    {
        return checkedRect.Left >= this.Left && checkedRect.Top >= this.Top && 
                checkedRect.Right <= this.Right && checkedRect.Bottom <= this.Bottom;
    }

    public final boolean isIntersect(Rect other)
    {
        Rect r = this.clone();

        if (other.Left > this.Left) {
            r.Left = other.Left;
        }
        if (other.Top > this.Top) {
            r.Top = other.Top;
        }
        if (other.Right < this.Right) {
            r.Right = other.Right;
        }
        if (other.Bottom < this.Bottom) {
            r.Bottom = other.Bottom;
        }

        return !r.isEmpty();
    }

    @Override
    public Rect clone()
    {
        Rect varCopy = new Rect(this.Left, this.Top, this.Right, this.Bottom);
        return varCopy;
    }
    
    public final int getSquare()
    {
        return this.getWidth() * this.getHeight();
    }
    
    public final Point getCenter()
    {
        int cx = this.Left + this.getWidth() / 2;
        int cy = this.Top + this.getHeight() / 2;
        return new Point(cx, cy);
    }
}
