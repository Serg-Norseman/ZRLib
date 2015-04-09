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

import jzrlib.map.BaseTile;
import jzrlib.map.CustomMap;
import jzrlib.map.Movements;
import jzrlib.map.TileType;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class BaseMap extends CustomMap
{
    public BaseMap(int width, int height)
    {
        super(width, height);
    }

    @Override
    public Movements getTileMovements(short tileID)
    {
        Movements moves = new Movements();
        
        TileID tid = TileID.forValue(tileID);
        if (tid != null && !tid.Flags.contains(TileFlags.tfBarrier)) {
            moves.include(Movements.mkWalk);
        }
        
        return moves;
    }

    @Override
    public short translateTile(TileType defTile)
    {
        switch (defTile) {
            case ttUndefined:
                return (short) 0;
            
            case ttGround:
                return '.';
            case ttWall:
                return 'x';

            case ttCaveFloor:
                return (short) TileID.tid_DungeonFloor.Value;
                
            case ttRock:
            case ttDungeonWall:
                return (short) TileID.tid_DungeonWall.Value;
                
            default:
                return '.';
        }
    }

    @Override
    public void setMetaTile(int x, int y, TileType tile)
    {
        BaseTile baseTile = this.getTile(x, y);
        if (baseTile != null) {
            baseTile.Background = this.translateTile(tile);
        }
    }

    @Override
    public void fillMetaBorder(int x1, int y1, int x2, int y2, TileType tile)
    {
        short defTile = this.translateTile(tile);
        this.fillBorder(x1, y1, x2, y2, defTile, false);
    }

    public char getMetaTile(int x, int y)
    {
        BaseTile baseTile = this.getTile(x, y);
        if (baseTile != null) {
            return (char) baseTile.Background;
        } else {
            return ' ';
        }
    }
}
