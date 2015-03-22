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
import mrl.creatures.Human;
import mrl.maps.MapFeature;
import mrl.maps.TileID;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Stairs extends BuildingFeature
{
    private boolean fIsDesc = false;

    public Stairs(GameSpace space, Object owner, int x, int y)
    {
        super(space, owner, x, y);
    }

    @Override
    public final TileID getTileID()
    {
        if (this.fIsDesc) {
            return TileID.tid_StairsD;
        } else {
            return TileID.tid_StairsA;
        }
    }

    @Override
    public void render()
    {
        IMap map = this.getMap();
        map.getTile(this.getPosX(), this.getPosY()).Foreground = (short) this.getTileID().Value;
    }

    public final void setDescending(boolean value)
    {
        this.fIsDesc = value;
    }

    @Override
    public List<IAction> getActionsList()
    {
        class ActionChangeLayer extends BaseEntityAction
        {
            protected void changeLayer(int layerFromID, int layerToID)
            {
            }
        }

        class ActionDescend extends ActionChangeLayer
        {
            @Override
            public void execute(Object invoker)
            {
                ((Human) invoker).descend();
            }
        }

        class ActionAscend extends ActionChangeLayer
        {
            @Override
            public void execute(Object invoker)
            {
                ((Human) invoker).ascend();
            }
        }
 
        ActionList<MapFeature> list = new ActionList();
        list.setOwner(this);
        if (fIsDesc) {
            list.addAction(new ActionDescend(), "Go down");
        } else {
            list.addAction(new ActionAscend(), "Go up");
        }

        return list.getList();
    }
}
