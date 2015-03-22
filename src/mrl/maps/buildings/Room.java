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
package mrl.maps.buildings;

import java.util.ArrayList;
import java.util.List;
import jzrlib.core.AreaEntity;
import jzrlib.core.GameSpace;
import jzrlib.core.Rect;
import jzrlib.external.bsp.NodeSide;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Room extends AreaEntity
{
    private Rect fInnerArea;
    private final RoomTypes fTypes;

    public final List<NodeSide> OuterSides = new ArrayList<>();
    public RoomLocationType LocationType = RoomLocationType.rltInner;

    public Room(GameSpace space, Object owner)
    {
        super(space, owner);
        this.fInnerArea = null;
        this.fTypes = new RoomTypes();
    }

    @Override
    public final void setArea(Rect area)
    {
        super.setArea(area);

        Rect inner = area.clone();
        inner.inflate(-1, -1);
        this.fInnerArea = inner;
    }

    public final Rect getInnerArea()
    {
        return this.fInnerArea;
    }
    
    public final RoomTypes getTypes()
    {
        return this.fTypes;
    }
    
    @Override
    public final String getDesc()
    {
        StringBuilder result = new StringBuilder();
        
        RoomType[] types = RoomType.values();
        for (RoomType rt : types) {
            if (this.fTypes.contains(rt)) {
                if (result.length() != 0) {
                    result.append(", ");
                }
                result.append(rt.name());
            }
        }
        
        return result.toString();
    }
}
