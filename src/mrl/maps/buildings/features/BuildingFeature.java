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
package mrl.maps.buildings.features;

import jzrlib.core.GameSpace;
import jzrlib.map.IMap;
import mrl.maps.MapFeature;
import mrl.maps.buildings.Building;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class BuildingFeature extends MapFeature
{
    public BuildingFeature(GameSpace space, Object owner)
    {
        super(space, owner);
    }

    public BuildingFeature(GameSpace space, Object owner, int x, int y)
    {
        super(space, owner, x, y);
    }

    public final IMap getMap()
    {
        if (this.Owner instanceof IMap) {
            return (IMap) this.Owner;
        } else {
            Building bld = (Building) this.Owner;
            return bld.getMap();
        }
    }
}
