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

import java.util.List;
import jzrlib.core.GameSpace;
import jzrlib.core.action.ActionList;
import jzrlib.core.action.IAction;
import jzrlib.map.IMap;
import mrl.core.BaseEntityAction;
import mrl.maps.Axis;
import mrl.maps.MapFeature;
import mrl.maps.TileID;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Door extends BuildingFeature
{
    private final Axis fAxis;

    public boolean IsMain = false;
    public boolean Opened = true;

    public Door(GameSpace space, Object owner, Axis axis)
    {
        super(space, owner);
        this.fAxis = axis;
    }

    public final Axis getSideAxis()
    {
        return this.fAxis;
    }
    
    @Override
    public final TileID getTileID()
    {
        switch (this.fAxis) {
            case axHorz:
                if (this.Opened) {
                    return TileID.tid_HouseDoorHO;
                } else {
                    return TileID.tid_HouseDoorHC;
                }
            case axVert:
                if (this.Opened) {
                    return TileID.tid_HouseDoorVO;
                } else {
                    return TileID.tid_HouseDoorVC;
                }
            default:
                return TileID.tid_HouseDoor;
        }
    }

    @Override
    public void render()
    {
        IMap map = this.getMap();
        map.getTile(this.getPosX(), this.getPosY()).Foreground = (short) this.getTileID().Value;
    }

    @Override
    public List<IAction> getActionsList()
    {
        class ActionOpen extends BaseEntityAction
        {
            @Override
            public void execute(Object invoker)
            {
                Opened = true;
                render();
            }
        }

        class ActionClose extends BaseEntityAction
        {
            @Override
            public void execute(Object invoker)
            {
                Opened = false;
                render();
            }
        }

        class ActionBreak extends BaseEntityAction
        {
            @Override
            public void execute(Object invoker)
            {
                // dummy
            }
        }

        ActionList<MapFeature> list = new ActionList();
        list.setOwner(this);
        if (!Opened) {
            list.addAction(new ActionOpen(), "Open door");
        } else {
            list.addAction(new ActionClose(), "Close door");
        }

        list.addAction(new ActionBreak(), "Break door");

        return list.getList();
    }
}
