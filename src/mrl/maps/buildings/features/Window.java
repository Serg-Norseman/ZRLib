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
import mrl.creatures.Creature;
import mrl.game.events.VandalismEvent;
import mrl.maps.Axis;
import mrl.maps.MapFeature;
import mrl.maps.TileID;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Window extends BuildingFeature
{
    private final Axis fAxis;
    private boolean fIsBroken;

    public Window(GameSpace space, Object owner, Axis axis)
    {
        super(space, owner);
        this.fAxis = axis;
    }

    @Override
    public final TileID getTileID()
    {
        if (this.fIsBroken) {
            return TileID.tid_HouseWindowB;
        }
        
        switch (this.fAxis) {
            case axHorz:
                return TileID.tid_HouseWindowH;
            case axVert:
                return TileID.tid_HouseWindowV;
            default:
                return TileID.tid_HouseWindow;
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
        class ActionBreak extends BaseEntityAction
        {
            @Override
            public void execute(Object invoker)
            {
                fIsBroken = true;

                VandalismEvent evt = new VandalismEvent(getLocation().clone(), (Creature) invoker);
                evt.post();

                render();
            }
        }

        if (!fIsBroken) {
            ActionList<MapFeature> list = new ActionList();
            list.setOwner(this);
            list.addAction(new ActionBreak(), "Break window");

            return list.getList();
        } else {
            return super.getActionsList();
        }
    }
}
