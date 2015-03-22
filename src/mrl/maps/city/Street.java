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

import jzrlib.core.GameSpace;
import jzrlib.core.Rect;
import jzrlib.map.IMap;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Street extends CityRegion
{
    private static int LastStreetNum = 0;

    private int fLastHouseNum;
    public final int Num;
    
    public Street(GameSpace space, Object owner, IMap map, Rect area)
    {
        super(space, owner, map, area);

        this.Num = ++LastStreetNum;
        this.fLastHouseNum = 0;
    }
    
    public final int getNextHouseNumber()
    {
        return ++this.fLastHouseNum;
    }
    
    public static final void resetNum()
    {
        LastStreetNum = 0;
    }
}
