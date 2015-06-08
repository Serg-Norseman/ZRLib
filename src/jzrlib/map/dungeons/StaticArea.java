/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
 *  Copyright (C) 2003 by Ruslan N. Garipov (aka Brigadir).
 *  Copyright (C) 2002-2008, 2014 by Serg V. Zhdanovskih (aka Alchemist).
 *
 *  This file is part of "NorseWorld: Ragnarok".
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
package jzrlib.map.dungeons;

import jzrlib.core.Rect;
import jzrlib.map.TileType;
import jzrlib.utils.Logger;

/**
 * 
 * @author Ruslan N. Garipov (aka Brigadir), 2003
 * @author Serg V. Zhdanovskih (aka Alchemist), 2003-2008, 2014
 */
public abstract class StaticArea extends DungeonArea
{
    protected TileType[][] fArea;

    public int Left;
    public int Top;
    public int Height;
    public int Width;

    public StaticArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);

        this.Left = -1;
        this.Top = -1;
        this.Width = -1;
        this.Height = -1;
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.fArea = null;
        }
        super.dispose(disposing);
    }

    protected final void setPosition(int left, int top)
    {
        this.Left = left;
        this.Top = top;
    }

    protected final void setDimension(int width, int height)
    {
        this.fArea = null;

        this.Width = width;
        this.Height = height;

        this.fArea = new TileType[width][height];

        for (int x = 0; x < this.Width; x++) {
            for (int y = 0; y < this.Height; y++) {
                this.fArea[x][y] = TileType.ttRock;
            }
        }
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea area)
    {
        return this.getDimensionRect().isIntersect(area.getDimensionRect());
    }

    @Override
    protected boolean isWallPoint(int ptX, int ptY)
    {
        return this.fArea[ptX - this.Left][ptY - this.Top] == TileType.ttDungeonWall;
    }

    @Override
    public void flushToMap()
    {
        try {
            DungeonBuilder builder = super.Owner;

            int num = this.Left + this.Width - 1;
            for (int ax = this.Left; ax <= num; ax++) {
                int num2 = this.Top + this.Height - 1;
                for (int ay = this.Top; ay <= num2; ay++) {
                    if (this.isWallPoint(ax, ay)) {
                        builder.setTile(ax, ay, TileType.ttDungeonWall);
                    } else {
                        if (this.isOwnedPoint(ax, ay)) {
                            builder.setTile(ax, ay, TileType.ttUndefined);
                        }
                    }
                }
            }

            this.flushMarksList();
        } catch (Exception ex) {
            Logger.write("StaticArea.flushToMap(): " + ex.getMessage());
            throw ex;
        }
    }

    @Override
    public Rect getDimensionRect()
    {
        return new Rect(this.Left, this.Top, this.Left + this.Width - 1, this.Top + this.Height - 1);
    }

    @Override
    public boolean isAllowedPointAsMark(int ptX, int ptY)
    {
        for (DungeonMark mark : this.MarksList) {
            if (mark.Location.equals(ptX, ptY)) {
                return true;
            }
        }
        return false;
    }
}
