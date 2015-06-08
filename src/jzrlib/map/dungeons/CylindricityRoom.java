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
public final class CylindricityRoom extends DungeonArea
{
    protected int fCenterX;
    protected int fCenterY;
    protected int fRadius;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new CylindricityRoom(owner, parentMark);
    }

    public CylindricityRoom(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        boolean result;

        if (anArea instanceof RectangularRoom) {
            Rect rectArea = ((RectangularRoom) anArea).getDimensionRect();

            if (!AuxUtils.isValueBetween(this.fCenterY, rectArea.Top, rectArea.Bottom, true) && !AuxUtils.isValueBetween(this.fCenterX, rectArea.Left, rectArea.Right, true)) {
                result = (this.fRadius > Math.min(Math.abs(rectArea.Top - this.fCenterY), Math.abs(rectArea.Bottom - this.fCenterY)) && this.fRadius > Math.min(Math.abs(rectArea.Left - this.fCenterX), Math.abs(rectArea.Right - this.fCenterX)));
            } else {
                if (AuxUtils.isValueBetween(this.fCenterY, rectArea.Top, rectArea.Bottom, true) && !AuxUtils.isValueBetween(this.fCenterX, rectArea.Left, rectArea.Right, true)) {
                    result = (this.fRadius > Math.min(Math.abs(rectArea.Left - this.fCenterX), Math.abs(rectArea.Right - this.fCenterX)));
                } else {
                    result = (AuxUtils.isValueBetween(this.fCenterY, rectArea.Top, rectArea.Bottom, true) || !AuxUtils.isValueBetween(this.fCenterX, rectArea.Left, rectArea.Right, true) || this.fRadius > Math.min(Math.abs(rectArea.Top - this.fCenterY), Math.abs(rectArea.Bottom - this.fCenterY)));
                }
            }
        } else {
            if (anArea instanceof CylindricityRoom) {
                CylindricityRoom cylArea = ((CylindricityRoom) anArea);
                try {
                    int dX = this.fCenterX - cylArea.fCenterX;
                    int dY = this.fCenterY - cylArea.fCenterY;
                    float s = (float) Math.sqrt((float) (dX * dX) + (float) (dY * dY));
                    result = ((float) (this.fRadius + cylArea.fRadius) > s);
                } catch (Exception ex) {
                    Logger.write("CylindricityRoom.isIntersectWithArea(): " + ex.getMessage());
                    result = true;
                }
            } else {
                result = anArea.isIntersectWithArea(this);
            }
        }

        return result;
    }

    @Override
    protected boolean buildArea()
    {
        boolean result = super.ParentMark != null && (super.ParentMark.ParentArea == null || super.ParentMark.ParentArea instanceof LinearCorridor);

        if (result) {
            this.fRadius = AuxUtils.getBoundedRnd((int) super.Owner.CylindricityRoomRadius, (int) ((int) super.Owner.AreaSizeLimit >>> 1));

            switch (super.ParentMark.Direction) {
                case Directions.dtNorth:
                    this.fCenterY = super.ParentMark.Location.Y - this.fRadius;
                    this.setDimension(super.ParentMark.Location.X, this.fCenterY, this.fRadius);
                    break;
                case Directions.dtSouth:
                    this.fCenterY = super.ParentMark.Location.Y + this.fRadius;
                    this.setDimension(super.ParentMark.Location.X, this.fCenterY, this.fRadius);
                    break;
                case Directions.dtWest:
                    this.fCenterX = super.ParentMark.Location.X - this.fRadius;
                    this.setDimension(this.fCenterX, super.ParentMark.Location.Y, this.fRadius);
                    break;
                case Directions.dtEast:
                    this.fCenterX = super.ParentMark.Location.X + this.fRadius;
                    this.setDimension(this.fCenterX, super.ParentMark.Location.Y, this.fRadius);
                    break;
            }

            result = (this.fRadius >= (int) super.Owner.CylindricityRoomRadius);
        }

        return result;
    }

    @Override
    protected boolean isWallPoint(int ptX, int ptY)
    {
        boolean result;
        if (this.fRadius < Math.abs(this.fCenterX - ptX)) {
            result = false;
        } else {
            float sqrRad = (float) (this.fRadius * this.fRadius);
            int dX = this.fCenterX - ptX;
            result = (((long) Math.round(Math.sqrt((sqrRad - (float) (dX * dX))))) == (long) Math.abs(this.fCenterY - ptY));
        }
        return result;
    }

    protected final void setDimension(int centerX, int centerY, int radius)
    {
        this.fCenterX = centerX;
        this.fCenterY = centerY;
        this.fRadius = radius;
    }

    @Override
    public void flushToMap()
    {
        try {
            for (int ax = 0; ax <= this.fRadius; ax++) {
                int dy = (int) (Math.round(Math.sqrt(((this.fRadius * this.fRadius) - (float) (ax * ax)))));

                for (int ay = dy; ay >= -dy; ay--) {
                    int ax1 = ax + 1;
                    int hp = (ax1 * ax1) + (ay * ay);

                    if ((this.isWallPoint(this.fCenterX + ax, this.fCenterY - ay)) || (hp > this.fRadius * this.fRadius)) {
                        super.Owner.setTile(this.fCenterX - ax, this.fCenterY - ay, TileType.ttCylindricityRoomWall);
                        super.Owner.setTile(this.fCenterX + ax, this.fCenterY - ay, TileType.ttCylindricityRoomWall);
                    } else {
                        super.Owner.setTile(this.fCenterX - ax, this.fCenterY - ay, TileType.ttUndefined);
                        super.Owner.setTile(this.fCenterX + ax, this.fCenterY + ay, TileType.ttUndefined);
                    }
                }
            }

            this.flushMarksList();
        } catch (Exception ex) {
            Logger.write("CylindricityRoom.flushToMap(): " + ex.getMessage());
            throw ex;
        }
    }

    private static int rotateDirectionToRightAngleCW(int direction)
    {
        int result = direction;

        switch (direction) {
            case Directions.dtNorth:
                result = Directions.dtEast;
                break;
            case Directions.dtSouth:
                result = Directions.dtWest;
                break;
            case Directions.dtWest:
                result = Directions.dtNorth;
                break;
            case Directions.dtEast:
                result = Directions.dtSouth;
                break;
        }
        
        return result;
    }

    private static int rotateDirectionToRightAngleCCW(int direction)
    {
        int result = direction;

        switch (direction) {
            case Directions.dtNorth:
                result = Directions.dtWest;
                break;
            case Directions.dtSouth:
                result = Directions.dtEast;
                break;
            case Directions.dtWest:
                result = Directions.dtSouth;
                break;
            case Directions.dtEast:
                result = Directions.dtNorth;
                break;
        }
        
        return result;
    }

    @Override
    public void generateMarksList()
    {
        try {
            int marksCount = 3;
            int failedTries = 0;
            do {
                int num = marksCount;
                int markDirection = Directions.dtNorth;
                int markX = 0;
                int markY = 0;
                if (num != 1) {
                    if (num != 2) {
                        if (num == 3) {
                            markDirection = rotateDirectionToRightAngleCCW(super.ParentMark.Direction);
                            switch (super.ParentMark.Direction) {
                                case Directions.dtNorth:
                                    markX = this.fCenterX - this.fRadius;
                                    markY = this.fCenterY;
                                    break;
                                case Directions.dtSouth:
                                    markX = this.fCenterX + this.fRadius;
                                    markY = this.fCenterY;
                                    break;
                                case Directions.dtWest:
                                    markX = this.fCenterX;
                                    markY = this.fCenterY + this.fRadius;
                                    break;
                                case Directions.dtEast:
                                    markX = this.fCenterX;
                                    markY = this.fCenterY - this.fRadius;
                                    break;
                            }
                        }
                    } else {
                        markDirection = super.ParentMark.Direction;
                        switch (super.ParentMark.Direction) {
                            case Directions.dtNorth:
                                markX = this.fCenterX;
                                markY = this.fCenterY - this.fRadius;
                                break;
                            case Directions.dtSouth:
                                markX = this.fCenterX;
                                markY = this.fCenterY + this.fRadius;
                                break;
                            case Directions.dtWest:
                                markX = this.fCenterX - this.fRadius;
                                markY = this.fCenterY;
                                break;
                            case Directions.dtEast:
                                markX = this.fCenterX + this.fRadius;
                                markY = this.fCenterY;
                                break;
                        }
                    }
                } else {
                    markDirection = rotateDirectionToRightAngleCW(super.ParentMark.Direction);

                    switch (super.ParentMark.Direction) {
                        case Directions.dtNorth:
                            markX = this.fCenterX + this.fRadius;
                            markY = this.fCenterY;
                            break;
                        case Directions.dtSouth:
                            markX = this.fCenterX - this.fRadius;
                            markY = this.fCenterY;
                            break;
                        case Directions.dtWest:
                            markX = this.fCenterX;
                            markY = this.fCenterY - this.fRadius;
                            break;
                        case Directions.dtEast:
                            markX = this.fCenterX;
                            markY = this.fCenterY + this.fRadius;
                            break;
                    }
                }

                DungeonMark mark = new DungeonMark(this, markX, markY, markDirection);
                mark.ForcedAreaCreateProc = LinearCorridor::createArea;

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
        } catch (Exception ex) {
            Logger.write("CylindricityRoom.generateMarksList(): " + ex.getMessage());
        }
    }

    @Override
    public int getDevourArea()
    {
        return Math.round((float) (Math.PI * this.fRadius * this.fRadius));
    }

    @Override
    public Rect getDimensionRect()
    {
        return new Rect(this.fCenterX - this.fRadius, this.fCenterY - this.fRadius, this.fCenterX + this.fRadius, this.fCenterY + this.fRadius);
    }

    @Override
    public boolean isAllowedPointAsMark(int ptX, int ptY)
    {
        return (ptX == this.fCenterX && (ptY == this.fCenterY - this.fRadius || ptY == this.fCenterY + this.fRadius)) || (ptY == this.fCenterY && (ptX == this.fCenterX - this.fRadius || ptX == this.fCenterX + this.fRadius));
    }

    @Override
    public boolean isOwnedPoint(int ptX, int ptY)
    {
        boolean result;
        if (this.fRadius < Math.abs(this.fCenterX - ptX)) {
            result = false;
        } else {
            float sqrRad = (float) (this.fRadius * this.fRadius);
            int dX = this.fCenterX - ptX;
            result = (((long) Math.round(Math.sqrt((sqrRad - (float) (dX * dX))))) <= (long) Math.abs(this.fCenterY - ptY));
        }
        return result;
    }
}
