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

import jzrlib.utils.AuxUtils;
import jzrlib.utils.Logger;
import jzrlib.core.Rect;
import jzrlib.core.Directions;
import jzrlib.map.TileType;

/**
 * 
 * @author Ruslan N. Garipov (aka Brigadir), 2003
 * @author Serg V. Zhdanovskih (aka Alchemist), 2003-2008, 2014
 */
public class RectangularRoom extends DungeonArea
{
    public int Left;
    public int Top;
    public int Width;
    public int Height;

    public final int getRight()
    {
        return this.Left + this.Width - 1;
    }

    public final int getBottom()
    {
        return this.Top + this.Height - 1;
    }

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new RectangularRoom(owner, parentMark);
    }

    public RectangularRoom(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        boolean result;
        if (anArea instanceof RectangularRoom) {
            result = this.getDimensionRect().isIntersect(((RectangularRoom) anArea).getDimensionRect());
        } else {
            result = anArea.isIntersectWithArea(this);
        }
        return result;
    }

    @Override
    protected boolean buildArea()
    {
        boolean result = false;

        int corrWidth = (int) super.Owner.CorridorWidthBottomLimit;
        int corrLength = (int) super.Owner.CorridorLengthBottomLimit;
        int areaSize = (int) super.Owner.AreaSizeLimit;
        
        switch (super.ParentMark.Direction) {
            case Directions.dtNorth:
                this.Width = AuxUtils.getBoundedRnd(corrWidth, areaSize);
                this.Height = AuxUtils.getBoundedRnd(corrLength, areaSize);
                this.setDimension(super.ParentMark.Location.X - AuxUtils.getBoundedRnd(1, this.Width - 2), super.ParentMark.Location.Y - this.Height + 1, this.Width, this.Height);
                result = AuxUtils.isValueBetween(super.ParentMark.Location.X, this.Left, this.Left + this.Width - 1, false);
                break;

            case Directions.dtSouth:
                this.Width = AuxUtils.getBoundedRnd(corrWidth, areaSize);
                this.Height = AuxUtils.getBoundedRnd(corrLength, areaSize);
                this.setDimension(super.ParentMark.Location.X - AuxUtils.getBoundedRnd(1, this.Width - 2), super.ParentMark.Location.Y, this.Width, this.Height);
                result = AuxUtils.isValueBetween(super.ParentMark.Location.X, this.Left, this.Left + this.Width - 1, false);
                break;

            case Directions.dtWest:
                this.Width = AuxUtils.getBoundedRnd(corrLength, areaSize);
                this.Height = AuxUtils.getBoundedRnd(corrWidth, areaSize);
                this.setDimension(super.ParentMark.Location.X - this.Width + 1, super.ParentMark.Location.Y - AuxUtils.getBoundedRnd(1, this.Height - 2), this.Width, this.Height);
                result = AuxUtils.isValueBetween(super.ParentMark.Location.Y, this.Top, this.Top + this.Height - 1, false);
                break;

            case Directions.dtEast:
                this.Width = AuxUtils.getBoundedRnd(corrLength, areaSize);
                this.Height = AuxUtils.getBoundedRnd(corrWidth, areaSize);
                this.setDimension(super.ParentMark.Location.X, super.ParentMark.Location.Y - AuxUtils.getBoundedRnd(1, this.Height - 2), this.Width, this.Height);
                result = AuxUtils.isValueBetween(super.ParentMark.Location.Y, this.Top, this.Top + this.Height - 1, false);
                break;
        }

        return result;
    }

    @Override
    protected boolean isWallPoint(int ptX, int ptY)
    {
        return this.getDimensionRect().isBorder(ptX, ptY);
    }

    protected void setDimension(int left, int top, int width, int height)
    {
        this.Left = left;
        this.Top = top;
        this.Width = width;
        this.Height = height;
    }

    @Override
    public void flushToMap()
    {
        try {
            int right = this.Left + this.Width - 1;
            int bottom = this.Top + this.Height - 1;
            for (int ax = this.Left; ax <= right; ax++) {
                for (int ay = this.Top; ay <= bottom; ay++) {
                    if (this.isWallPoint(ax, ay)) {
                        if (this instanceof LinearCorridor) {
                            super.Owner.setTile(ax, ay, TileType.ttLinearCorridorWall);
                        } else {
                            super.Owner.setTile(ax, ay, TileType.ttRectRoomWall);
                        }
                    } else {
                        super.Owner.setTile(ax, ay, TileType.ttUndefined);
                    }
                }
            }

            this.flushMarksList();
        } catch (Exception ex) {
            Logger.write("RectangularRoom.flushToMap(): " + ex.getMessage());
            throw ex;
        }
    }

    @Override
    public void generateMarksList()
    {
        try {
            int marksCount = AuxUtils.getBoundedRnd(1, (int) super.Owner.RectRoomMarksLimit);
            int failedTries = 0;
            if (marksCount > 0) {
                do {
                    int markDirection = (AuxUtils.getBoundedRnd(Directions.dtNorth, Directions.dtEast));
                    int markX = 0;
                    int markY = 0;
                    switch (markDirection) {
                        case Directions.dtNorth:
                            markX = this.Left + AuxUtils.getBoundedRnd(1, this.Width - 2);
                            markY = this.Top;
                            break;
                        case Directions.dtSouth:
                            markX = this.Left + AuxUtils.getBoundedRnd(1, this.Width - 2);
                            markY = this.Top + this.Height - 1;
                            break;
                        case Directions.dtWest:
                            markX = this.Left;
                            markY = this.Top + AuxUtils.getBoundedRnd(1, this.Height - 2);
                            break;
                        case Directions.dtEast:
                            markX = this.Left + this.Width - 1;
                            markY = this.Top + AuxUtils.getBoundedRnd(1, this.Height - 2);
                            break;
                    }

                    DungeonMark mark = new DungeonMark(this, markX, markY, markDirection);

                    if (this.isAllowedMark(mark)) {
                        this.MarksList.add(mark);
                        marksCount--;
                        failedTries = 0;
                    } else {
                        mark.dispose();
                        failedTries++;
                    }

                    if (failedTries > (int) super.Owner.RightMarkSearchLimit) {
                        marksCount--;
                    }
                } while (marksCount > 0);
            }
        } catch (Exception ex) {
            Logger.write("RectangularRoom.generateMarksList(): " + ex.getMessage());
        }
    }

    @Override
    public boolean isAllowedPointAsMark(int ptX, int ptY)
    {
        return (AuxUtils.isValueBetween(ptX, this.Left, this.Left + this.Width - 1, false) && this.Top == ptY) || 
                (AuxUtils.isValueBetween(ptX, this.Left, this.Left + this.Width - 1, false) && this.Top + this.Height - 1 == ptY) || 
                (this.Left == ptX && AuxUtils.isValueBetween(ptY, this.Top, this.Top + this.Height - 1, false)) || 
                (this.Left + this.Width - 1 == ptX && AuxUtils.isValueBetween(ptY, this.Top, this.Top + this.Height - 1, false));
    }

    @Override
    public int getDevourArea()
    {
        return this.Width * this.Height;
    }

    @Override
    public boolean isOwnedPoint(int ptX, int ptY)
    {
        return AuxUtils.isValueBetween(ptX, this.Left, this.Left + this.Width - 1, true) && 
                AuxUtils.isValueBetween(ptY, this.Top, this.Top + this.Height - 1, true);
    }

    @Override
    public Rect getDimensionRect()
    {
        return new Rect(this.Left, this.Top, this.getRight(), this.getBottom());
    }
}
