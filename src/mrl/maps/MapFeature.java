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
package mrl.maps;

import java.util.ArrayList;
import java.util.List;
import jzrlib.core.GameSpace;
import jzrlib.core.LocatedEntity;
import jzrlib.core.action.IAction;
import jzrlib.core.action.IActor;
import mrl.game.Game;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class MapFeature extends LocatedEntity implements IActor
{
    protected TileID fTileID;
    
    public int Durability;

    public MapFeature(GameSpace space, Object owner)
    {
        super(space, owner);
    }

    public MapFeature(GameSpace space, Object owner, TileID tileId)
    {
        super(space, owner);
        this.fTileID = tileId;
    }

    public MapFeature(GameSpace space, Object owner, int x, int y)
    {
        super(space, owner);
        this.setPos(x, y);
    }

    @Override
    public final Game getSpace()
    {
        return (Game) this.fSpace;
    }

    public TileID getTileID()
    {
        return this.fTileID;
    }

    public void render()
    {
        // dummy
    }
    
    @Override
    public List<IAction> getActionsList()
    {
        return new ArrayList<>();
    }
}
