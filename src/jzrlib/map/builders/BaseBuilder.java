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
package jzrlib.map.builders;

import jzrlib.core.Rect;
import jzrlib.map.IMap;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class BaseBuilder
{
    protected final IMap fMap;
    protected final Rect fArea;

    public BaseBuilder(IMap map)
    {
        this.fMap = map;
        this.fArea = map.getAreaRect();
    }

    public BaseBuilder(IMap map, Rect area)
    {
        this.fMap = map;

        if (area != null) {
            this.fArea = area;
        } else {
            this.fArea = map.getAreaRect();
        }
    }
}
