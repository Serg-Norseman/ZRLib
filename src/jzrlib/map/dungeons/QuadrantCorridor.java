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
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.core.Directions;
import jzrlib.map.TileType;

/**
 * 
 * @author Ruslan N. Garipov (aka Brigadir), 2003
 * @author Serg V. Zhdanovskih (aka Alchemist), 2003-2008, 2014
 */
public final class QuadrantCorridor extends DungeonArea
{
    private enum TQuadrantIndex
    {
        qiFirst,
        qiSecond,
        qiThird,
        qiFourth;
    }

    public int InRadius;
    public int ExRadius;
    public int CenterX;
    public int CenterY;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new QuadrantCorridor(owner, parentMark);
    }

    public QuadrantCorridor(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
    }

    public final int getNextDirection()
    {
        int result;
        if (super.ParentMark.Location.X == this.CenterX) {
            if (super.ParentMark.Location.Y > this.CenterY) {
                result = Directions.dtNorth;
            } else {
                result = Directions.dtSouth;
            }
        } else {
            if (super.ParentMark.Location.Y != this.CenterY) {
                throw new RuntimeException("TQuadrantCorridor: incorrect area definition! Check the buildArea method.");
            }
            if (super.ParentMark.Location.X > this.CenterX) {
                result = Directions.dtWest;
            } else {
                result = Directions.dtEast;
            }
        }
        return result;
    }

    @Override
    public boolean isIntersectWithArea(DungeonArea anArea)
    {
        boolean Result = this.getDimensionRect().isIntersect(anArea.getDimensionRect());
        if (Result) {
            if (anArea instanceof RectangularRoom) {
                RectangularRoom rectArea = ((RectangularRoom) anArea);

                int rLeft = rectArea.Left - this.CenterX;
                int rRight = rectArea.getRight() - this.CenterX;
                int rTop = this.CenterY - rectArea.Top;
                int rBottom = this.CenterY - rectArea.getBottom();
                TQuadrantIndex quadrant = this.getQuadrant();
                boolean exIntersectFlag = false;
                float exDist = 0.0f;
                float inDist = 0.0f;

                if (quadrant != TQuadrantIndex.qiFirst) {
                    if (quadrant != TQuadrantIndex.qiSecond) {
                        if (quadrant != TQuadrantIndex.qiThird) {
                            if (quadrant == TQuadrantIndex.qiFourth) {
                                exIntersectFlag = (rLeft > 0 && rRight > 0 && rTop < 0 && rBottom < 0);
                                if (exIntersectFlag) {
                                    float L2 = (float) (rLeft * rLeft);
                                    exDist = (float) Math.sqrt(L2 + (float) (rTop * rTop));
                                } else {
                                    float R2 = (float) (rRight * rRight);
                                    inDist = (float) Math.sqrt(R2 + (float) (rBottom * rBottom));
                                }
                            }
                        } else {
                            exIntersectFlag = (rLeft < 0 && rRight < 0 && rTop < 0 && rBottom < 0);
                            if (exIntersectFlag) {
                                float R2 = (float) (rRight * rRight);
                                exDist = (float) Math.sqrt(R2 + (float) (rTop * rTop));
                            } else {
                                float L2 = (float) (rLeft * rLeft);
                                inDist = (float) Math.sqrt(L2 + (float) (rBottom * rBottom));
                            }
                        }
                    } else {
                        exIntersectFlag = (rLeft < 0 && rRight < 0 && rTop > 0 && rBottom > 0);
                        if (exIntersectFlag) {
                            float R2 = (float) (rRight * rRight);
                            exDist = (float) Math.sqrt(R2 + (float) (rBottom * rBottom));
                        } else {
                            float L2 = (float) (rLeft * rLeft);
                            inDist = (float) Math.sqrt(L2 + (float) (rTop * rTop));
                        }
                    }
                } else {
                    exIntersectFlag = (rLeft > 0 && rRight > 0 && rTop > 0 && rBottom > 0);
                    if (exIntersectFlag) {
                        float L2 = (float) (rLeft * rLeft);
                        exDist = (float) Math.sqrt(L2 + (float) (rBottom * rBottom));
                    } else {
                        float R2 = (float) (rRight * rRight);
                        inDist = (float) Math.sqrt(R2 + (float) (rTop * rTop));
                    }
                }

                if (exIntersectFlag) {
                    Result = (((long) Math.round(exDist)) <= (long) this.ExRadius);
                } else {
                    Result = (((long) Math.round(inDist)) >= (long) this.InRadius);
                }
            } else {
                if (anArea instanceof CylindricityRoom) {
                    CylindricityRoom cylArea = ((CylindricityRoom) anArea);

                    int rLeft = cylArea.fCenterX - this.CenterX;
                    int rTop = cylArea.fCenterY - this.CenterY;

                    float L2 = (float) (rLeft * rLeft);
                    float T2 = (float) (rTop * rTop);
                    long hyp = (Math.round(Math.sqrt((L2 + T2))));
                    if (hyp > (long) this.ExRadius) {
                        Result = (hyp < (long) (this.ExRadius + cylArea.fRadius));
                    } else {
                        Result = (hyp + (long) cylArea.fRadius > (long) this.InRadius);
                    }
                } else {
                    if (!(anArea instanceof QuadrantCorridor)) {
                        Result = anArea.isIntersectWithArea(this);
                    }
                }
            }
        }
        return Result;
    }

    @Override
    protected boolean buildArea()
    {
        boolean result = super.ParentMark != null;
        if (result) {
            this.ExRadius = AuxUtils.getBoundedRnd((int) super.Owner.QuadrantCorridorExRadius, (int) ((int) super.Owner.AreaSizeLimit >>> 1));
            this.InRadius = AuxUtils.getBoundedRnd((int) super.Owner.QuadrantCorridorInRadius, this.ExRadius - ((int) super.Owner.QuadrantCorridorExRadius - (int) super.Owner.QuadrantCorridorInRadius));
            this.ExRadius = this.InRadius + Math.max(4, this.ExRadius - this.InRadius);
            int delta = AuxUtils.getBoundedRnd(1, this.ExRadius - this.InRadius - 1);
            int factor;
            if (AuxUtils.getBoundedRnd(0, 1) > 0) {
                factor = -1;
            } else {
                factor = 1;
            }
            int direction = super.ParentMark.Direction;

            if (direction != Directions.dtNorth && direction != Directions.dtSouth) {
                if (direction == Directions.dtWest || direction == Directions.dtEast) {
                    this.setDimension(super.ParentMark.Location.X, super.ParentMark.Location.Y + factor * (this.InRadius + delta), this.InRadius, this.ExRadius);
                }
            } else {
                this.setDimension(super.ParentMark.Location.X + factor * (this.InRadius + delta), super.ParentMark.Location.Y, this.InRadius, this.ExRadius);
            }

            Rect mapArea = this.Owner.getDungeonArea();
            result = (AuxUtils.isValueBetween(this.CenterX, 0, mapArea.getWidth() - 1, true) && AuxUtils.isValueBetween(this.CenterY, 0, mapArea.getHeight() - 1, true));
        }
        return result;
    }

    @Override
    protected boolean isWallPoint(int ptX, int ptY)
    {
        int direction = super.ParentMark.Direction;

        boolean result;
        if (direction != Directions.dtNorth) {
            if (direction != Directions.dtSouth) {
                if (direction != Directions.dtWest) {
                    result = (direction == Directions.dtEast && ((ptY == this.CenterY && AuxUtils.isValueBetween(ptX, this.CenterX + this.InRadius, this.CenterX + this.ExRadius, true)) || (ptX == this.CenterX && (AuxUtils.isValueBetween(ptY, this.CenterY + this.InRadius, this.CenterY + this.ExRadius, true) || AuxUtils.isValueBetween(ptY, this.CenterY - this.InRadius, this.CenterY - this.ExRadius, true)))));
                } else {
                    result = ((ptY == this.CenterY && AuxUtils.isValueBetween(ptX, this.CenterX - this.InRadius, this.CenterX - this.ExRadius, true)) || (ptX == this.CenterX && (AuxUtils.isValueBetween(ptY, this.CenterY + this.InRadius, this.CenterY + this.ExRadius, true) || AuxUtils.isValueBetween(ptY, this.CenterY - this.InRadius, this.CenterY - this.ExRadius, true))));
                }
            } else {
                result = ((ptX == this.CenterX && AuxUtils.isValueBetween(ptY, this.CenterY + this.ExRadius, this.CenterY + this.InRadius, true)) || (ptY == this.CenterY && (AuxUtils.isValueBetween(ptX, this.CenterX + this.InRadius, this.CenterX + this.ExRadius, true) || AuxUtils.isValueBetween(ptX, this.CenterX - this.InRadius, this.CenterX - this.ExRadius, true))));
            }
        } else {
            result = ((ptX == this.CenterX && AuxUtils.isValueBetween(ptY, this.CenterY - this.InRadius, this.CenterY - this.ExRadius, true)) || (ptY == this.CenterY && (AuxUtils.isValueBetween(ptX, this.CenterX + this.InRadius, this.CenterX + this.ExRadius, true) || AuxUtils.isValueBetween(ptX, this.CenterX - this.InRadius, this.CenterX - this.ExRadius, true))));
        }

        if (!result) {
            if (Math.abs(this.CenterX - ptX) <= this.InRadius) {
                float inrad2 = (float) (this.InRadius * this.InRadius);
                int dX = this.CenterX - ptX;
                result = (((long) Math.round(Math.sqrt((inrad2 - (float) (dX * dX))))) == (long) Math.abs(this.CenterY - ptY));
            }
            if (!result && Math.abs(this.CenterX - ptX) <= this.ExRadius) {
                float exrad2 = (float) (this.ExRadius * this.ExRadius);
                int dX = this.CenterX - ptX;
                result = (((long) Math.round(Math.sqrt((exrad2 - (float) (dX * dX))))) == (long) Math.abs(this.CenterY - ptY));
            }
        }

        return result;
    }

    protected final void setDimension(int centerX, int centerY, int inRadius, int exRadius)
    {
        this.CenterX = centerX;
        this.CenterY = centerY;
        this.InRadius = inRadius;
        this.ExRadius = exRadius;
    }

    protected final Point getXYFactors()
    {
        Point result = Point.Zero();

        if ((super.ParentMark.Direction == Directions.dtNorth && this.getNextDirection() == Directions.dtWest) || (super.ParentMark.Direction == Directions.dtEast && this.getNextDirection() == Directions.dtSouth)) {
            result.X = 1;
            result.Y = -1;
        } else {
            if ((super.ParentMark.Direction == Directions.dtNorth && this.getNextDirection() == Directions.dtEast) || (super.ParentMark.Direction == Directions.dtWest && this.getNextDirection() == Directions.dtSouth)) {
                result.X = -1;
                result.Y = -1;
            } else {
                if ((super.ParentMark.Direction == Directions.dtSouth && this.getNextDirection() == Directions.dtWest) || (super.ParentMark.Direction == Directions.dtEast && this.getNextDirection() == Directions.dtNorth)) {
                    result.X = 1;
                    result.Y = 1;
                } else {
                    if ((super.ParentMark.Direction == Directions.dtSouth && this.getNextDirection() == Directions.dtEast) || (super.ParentMark.Direction == Directions.dtWest && this.getNextDirection() == Directions.dtNorth)) {
                        result.X = -1;
                        result.Y = 1;
                    }
                }
            }
        }

        return result;
    }

    private TQuadrantIndex getQuadrant()
    {
        TQuadrantIndex result = TQuadrantIndex.qiFirst;

        if ((super.ParentMark.Direction == Directions.dtNorth && this.getNextDirection() == Directions.dtWest) || (super.ParentMark.Direction == Directions.dtEast && this.getNextDirection() == Directions.dtSouth)) {
            result = TQuadrantIndex.qiFirst;
        } else {
            if ((super.ParentMark.Direction == Directions.dtNorth && this.getNextDirection() == Directions.dtEast) || (super.ParentMark.Direction == Directions.dtWest && this.getNextDirection() == Directions.dtSouth)) {
                result = TQuadrantIndex.qiSecond;
            } else {
                if ((super.ParentMark.Direction == Directions.dtSouth && this.getNextDirection() == Directions.dtWest) || (super.ParentMark.Direction == Directions.dtEast && this.getNextDirection() == Directions.dtNorth)) {
                    result = TQuadrantIndex.qiFourth;
                } else {
                    if ((super.ParentMark.Direction == Directions.dtSouth && this.getNextDirection() == Directions.dtEast) || (super.ParentMark.Direction == Directions.dtWest && this.getNextDirection() == Directions.dtNorth)) {
                        result = TQuadrantIndex.qiThird;
                    }
                }
            }
        }

        return result;
    }

    @Override
    public void flushToMap()
    {
        try {
            Point factors = this.getXYFactors();

            for (int ax = 0; ax <= this.ExRadius; ax++) {
                int yBottomLimit;
                if (ax < this.InRadius) {
                    yBottomLimit = (int) ((long) Math.round(Math.sqrt(((this.InRadius * this.InRadius) - (float) (ax * ax)))));
                } else {
                    yBottomLimit = 0;
                }

                int arg = (int) ((long) Math.round(Math.sqrt(((this.ExRadius * this.ExRadius) - (float) (ax * ax)))));
                for (int ay = arg; ay <= yBottomLimit; ay++) {
                    int ax1 = ax + 1;
                    int ax2 = ax - 1;
                    if ((this.isWallPoint(this.CenterX + factors.X * ax, this.CenterY + factors.Y * ay)) || ((ax1 * ax1) + (ay * ay) > this.ExRadius * this.ExRadius) || ((ax2 * ax2) + (ay * ay) < this.InRadius * this.InRadius)) {
                        super.Owner.setTile(this.CenterX + factors.X * ax, this.CenterY + factors.Y * ay, TileType.ttDungeonWall);
                    } else {
                        super.Owner.setTile(this.CenterX + factors.X * ax, this.CenterY + factors.Y * ay, TileType.ttUndefined);
                    }
                }
            }

            super.Owner.setTile(this.CenterX + factors.X * (this.ExRadius - 1), this.CenterY + factors.Y, TileType.ttUndefined);
            super.Owner.setTile(this.CenterX + factors.X * this.ExRadius, this.CenterY + (factors.Y << 1), TileType.ttDungeonWall);
            super.Owner.setTile(this.CenterX + factors.X * this.ExRadius, this.CenterY + factors.Y, TileType.ttDungeonWall);

            this.flushMarksList();
        } catch (Exception ex) {
            Logger.write("QuadrantCorridor.flushToMap(): " + ex.getMessage());
            throw ex;
        }
    }

    @Override
    public void generateMarksList()
    {
        try {
            int marksCount = 1;
            int failedTries = 0;
            do {
                int direction = super.ParentMark.Direction;
                int markX = 0;
                int markY = 0;
                if (direction != Directions.dtNorth) {
                    if (direction != Directions.dtSouth) {
                        if (direction != Directions.dtWest) {
                            if (direction == Directions.dtEast) {
                                markX = AuxUtils.getBoundedRnd(this.CenterX + this.InRadius + 1, this.CenterX + this.ExRadius - 1);
                                markY = this.CenterY;
                            }
                        } else {
                            markX = AuxUtils.getBoundedRnd(this.CenterX - this.InRadius - 1, this.CenterX - this.ExRadius + 1);
                            markY = this.CenterY;
                        }
                    } else {
                        markX = this.CenterX;
                        markY = AuxUtils.getBoundedRnd(this.CenterY + this.InRadius + 1, this.CenterY + this.ExRadius - 1);
                    }
                } else {
                    markX = this.CenterX;
                    markY = AuxUtils.getBoundedRnd(this.CenterY - this.InRadius - 1, this.CenterY - this.ExRadius + 1);
                }

                DungeonMark mark = new DungeonMark(this, markX, markY, this.getNextDirection());

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
            Logger.write("QuadrantCorridor.generateMarksList(): " + ex.getMessage());
        }
    }

    @Override
    public int getDevourArea()
    {
        return (int) ((long) Math.round((Math.PI * (float) this.ExRadius * (float) this.ExRadius - Math.PI * (float) this.InRadius * (float) this.InRadius) / 4.0));
    }

    @Override
    public Rect getDimensionRect()
    {
        Rect Result = Rect.Empty();

        if ((super.ParentMark.Direction == Directions.dtNorth && this.getNextDirection() == Directions.dtWest) || (super.ParentMark.Direction == Directions.dtEast && this.getNextDirection() == Directions.dtSouth)) {
            Result = new Rect(this.CenterX, this.CenterY - this.ExRadius, this.CenterX + this.ExRadius, this.CenterY);
        } else {
            if ((super.ParentMark.Direction == Directions.dtNorth && this.getNextDirection() == Directions.dtEast) || (super.ParentMark.Direction == Directions.dtWest && this.getNextDirection() == Directions.dtSouth)) {
                Result = new Rect(this.CenterX - this.ExRadius, this.CenterY - this.ExRadius, this.CenterX, this.CenterY);
            } else {
                if ((super.ParentMark.Direction == Directions.dtSouth && this.getNextDirection() == Directions.dtWest) || (super.ParentMark.Direction == Directions.dtEast && this.getNextDirection() == Directions.dtNorth)) {
                    Result = new Rect(this.CenterX, this.CenterY, this.CenterX + this.ExRadius, this.CenterY + this.ExRadius);
                } else {
                    if ((super.ParentMark.Direction == Directions.dtSouth && this.getNextDirection() == Directions.dtEast) || (super.ParentMark.Direction == Directions.dtWest && this.getNextDirection() == Directions.dtNorth)) {
                        Result = new Rect(this.CenterX - this.ExRadius, this.CenterY, this.CenterX, this.CenterY + this.ExRadius);
                    }
                }
            }
        }

        return Result;
    }

    @Override
    public boolean isAllowedPointAsMark(int ptX, int ptY)
    {
        boolean Result = this.isWallPoint(ptX, ptY);
        if (Result) {
            int direction = super.ParentMark.Direction;
            if (direction != Directions.dtNorth) {
                if (direction != Directions.dtSouth) {
                    if (direction != Directions.dtWest) {
                        if (direction == Directions.dtEast) {
                            Result = ((ptY == this.CenterY && (ptX == this.CenterX + this.InRadius || ptX == this.CenterX + this.ExRadius)) || (ptX == this.CenterX && (ptY == this.CenterY + this.InRadius || ptY == this.CenterY + this.ExRadius || ptY == this.CenterY - this.InRadius || ptY == this.CenterY - this.ExRadius)));
                        }
                    } else {
                        Result = ((ptY == this.CenterY && (ptX == this.CenterX - this.InRadius || ptX == this.CenterX - this.ExRadius)) || (ptX == this.CenterX && (ptY == this.CenterY + this.InRadius || ptY == this.CenterY + this.ExRadius || ptY == this.CenterY - this.InRadius || ptY == this.CenterY - this.ExRadius)));
                    }
                } else {
                    Result = ((ptX == this.CenterX && (ptY == this.CenterY + this.InRadius || ptY == this.CenterY + this.ExRadius)) || (ptY == this.CenterY && (ptX == this.CenterX + this.InRadius || ptX == this.CenterX + this.ExRadius || ptX == this.CenterX - this.InRadius || ptX == this.CenterX - this.ExRadius)));
                }
            } else {
                Result = ((ptX == this.CenterX && (ptY == this.CenterY - this.InRadius || ptY == this.CenterY - this.ExRadius)) || (ptY == this.CenterY && (ptX == this.CenterX + this.InRadius || ptX == this.CenterX + this.ExRadius || ptX == this.CenterX - this.InRadius || ptX == this.CenterX - this.ExRadius)));
            }
            if (Result) {
                Result = false;
            }
        }
        return Result;
    }

    @Override
    public boolean isOwnedPoint(int ptX, int ptY)
    {
        boolean result = this.isWallPoint(ptX, ptY);

        int dX = this.CenterX - ptX;
        if (!result && Math.abs(dX) <= this.InRadius) {
            int dY = Math.abs(this.CenterY - ptY);
            float inrad2 = (float) (this.InRadius * this.InRadius);
            int inrHyp = (int) (Math.round(Math.sqrt((inrad2 - (float) (dX * dX)))));
            float exrad2 = (float) (this.ExRadius * this.ExRadius);
            result = AuxUtils.isValueBetween(dY, inrHyp, (int) (Math.round(Math.sqrt((exrad2 - (float) (dX * dX))))), true);
        } else {
            result = false;
        }

        return result;
    }
}
