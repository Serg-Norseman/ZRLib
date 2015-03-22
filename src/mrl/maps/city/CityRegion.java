/*
 *  "MysteriesRL", Java Roguelike game.
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
package mrl.maps.city;

import jzrlib.core.AreaEntity;
import jzrlib.core.GameSpace;
import jzrlib.core.Rect;
import jzrlib.map.IMap;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class CityRegion extends AreaEntity
{
    protected final IMap fMap;

    public CityRegion(GameSpace space, Object owner, IMap map, Rect area)
    {
        super(space, owner);

        this.fMap = map;
        this.setArea(area);
    }

    public final City getCity()
    {
        return (City) this.Owner;
    }

    public final IMap getMap()
    {
        return this.fMap;
    }
}
