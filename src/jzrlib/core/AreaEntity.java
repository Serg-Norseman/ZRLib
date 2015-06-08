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
public abstract class AreaEntity extends GameEntity
{
    private Rect fArea;

    public AreaEntity(GameSpace space, Object owner)
    {
        super(space, owner);
        this.fArea = null;
    }

    public final Rect getArea()
    {
        return this.fArea;
    }

    public void setArea(Rect area)
    {
        this.fArea = area;
    }

    public final void setArea(int left, int top, int right, int bottom)
    {
        this.setArea(new Rect(left, top, right, bottom));
    }

    public final boolean inArea(int x, int y)
    {
        return x >= this.fArea.Left && x <= this.fArea.Right && y >= this.fArea.Top && y <= this.fArea.Bottom;
    }
}
