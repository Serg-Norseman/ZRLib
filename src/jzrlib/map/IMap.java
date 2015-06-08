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
package jzrlib.map;

import jzrlib.core.CreatureEntity;
import jzrlib.core.EntityList;
import jzrlib.core.LocatedEntity;
import jzrlib.core.Point;
import jzrlib.core.Rect;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public interface IMap
{
    Rect getAreaRect();
    int getHeight();
    int getWidth();

    EntityList getFeatures();
    BaseTile getTile(int x, int y);

    boolean isValid(int x, int y);
    boolean isBlockLOS(int x, int y);
    boolean isBarrier(int x, int y);

    void normalize();
    void resize(int width, int height);
    void setSeen(int x, int y);

    void setTile(int x, int y, int tid, boolean fg);
    boolean checkTile(int x, int y, int checkId, boolean fg);

    void fillArea(Rect area, int tid, boolean fg);
    void fillArea(int x1, int y1, int x2, int y2, int tid, boolean fg);
    void fillBorder(int x1, int y1, int x2, int y2, int tid, boolean fg);
    void fillRadial(int aX, int aY, short boundTile, short fillTile, boolean fg);
    
    void fillBackground(int tid);
    void fillForeground(int tid);
    
    CreatureEntity findCreature(int aX, int aY);
    LocatedEntity findItem(int aX, int aY);
    
    Point searchFreeLocation();
    Point searchFreeLocation(Rect area);
    
    int checkForeAdjacently(int x, int y, TileType defTile);
    
    void fillMetaBorder(int x1, int y1, int x2, int y2, TileType tile);
    void setMetaTile(int x, int y, TileType tile);
    short translateTile(TileType defTile);

    void gen_Path(int px1, int py1, int px2, int py2, 
            Rect area, int tid, boolean wide, boolean bg, ITileChangeHandler tileHandler);
    void gen_Path(int px1, int py1, int px2, int py2, 
            Rect area, int tid, boolean wide, int wide2, boolean bg, ITileChangeHandler tileHandler);
    
    /**
     * Get the cost of moving through the given tile. This can be used to make
     * certain areas more desirable. A simple and valid implementation of this
     * method would be to return 1 in all cases.
     *
     * @param creature
     * @param tx The tile X coordinate
     * @param ty The tile Y coordinate
     * @param tile
     * @return The relative cost of moving across the given tile
     */
    float getPathTileCost(CreatureEntity creature, int tx, int ty, BaseTile tile);
}
