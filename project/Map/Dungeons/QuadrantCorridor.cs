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

using System;
using BSLib;
using ZRLib.Core;

namespace ZRLib.Map.Dungeons
{
    public sealed class QuadrantCorridor : DungeonArea
    {
        private enum QuadrantIndex
        {
            qiFirst,
            qiSecond,
            qiThird,
            qiFourth
        }

        public int InRadius;
        public int ExRadius;
        public int CenterX;
        public int CenterY;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new QuadrantCorridor(owner, parentMark);
        }

        public QuadrantCorridor(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
        }

        public int NextDirection
        {
            get {
                int result;
                if (ParentMark.Location.X == CenterX) {
                    if (ParentMark.Location.Y > CenterY) {
                        result = Directions.DtNorth;
                    } else {
                        result = Directions.DtSouth;
                    }
                } else {
                    if (ParentMark.Location.Y != CenterY) {
                        throw new Exception("TQuadrantCorridor: incorrect area definition! Check the buildArea method.");
                    }
                    if (ParentMark.Location.X > CenterX) {
                        result = Directions.DtWest;
                    } else {
                        result = Directions.DtEast;
                    }
                }
                return result;
            }
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            bool Result = DimensionRect.IsIntersect(anArea.DimensionRect);
            if (Result) {
                if (anArea is RectangularRoom) {
                    RectangularRoom rectArea = ((RectangularRoom)anArea);

                    int rLeft = rectArea.Left - CenterX;
                    int rRight = rectArea.Right - CenterX;
                    int rTop = CenterY - rectArea.Top;
                    int rBottom = CenterY - rectArea.Bottom;
                    QuadrantIndex quadrant = Quadrant;
                    bool exIntersectFlag = false;
                    float exDist = 0.0f;
                    float inDist = 0.0f;

                    if (quadrant != QuadrantIndex.qiFirst) {
                        if (quadrant != QuadrantIndex.qiSecond) {
                            if (quadrant != QuadrantIndex.qiThird) {
                                if (quadrant == QuadrantIndex.qiFourth) {
                                    exIntersectFlag = (rLeft > 0 && rRight > 0 && rTop < 0 && rBottom < 0);
                                    if (exIntersectFlag) {
                                        float L2 = (float)(rLeft * rLeft);
                                        exDist = (float)Math.Sqrt(L2 + (float)(rTop * rTop));
                                    } else {
                                        float R2 = (float)(rRight * rRight);
                                        inDist = (float)Math.Sqrt(R2 + (float)(rBottom * rBottom));
                                    }
                                }
                            } else {
                                exIntersectFlag = (rLeft < 0 && rRight < 0 && rTop < 0 && rBottom < 0);
                                if (exIntersectFlag) {
                                    float R2 = (float)(rRight * rRight);
                                    exDist = (float)Math.Sqrt(R2 + (float)(rTop * rTop));
                                } else {
                                    float L2 = (float)(rLeft * rLeft);
                                    inDist = (float)Math.Sqrt(L2 + (float)(rBottom * rBottom));
                                }
                            }
                        } else {
                            exIntersectFlag = (rLeft < 0 && rRight < 0 && rTop > 0 && rBottom > 0);
                            if (exIntersectFlag) {
                                float R2 = (float)(rRight * rRight);
                                exDist = (float)Math.Sqrt(R2 + (float)(rBottom * rBottom));
                            } else {
                                float L2 = (float)(rLeft * rLeft);
                                inDist = (float)Math.Sqrt(L2 + (float)(rTop * rTop));
                            }
                        }
                    } else {
                        exIntersectFlag = (rLeft > 0 && rRight > 0 && rTop > 0 && rBottom > 0);
                        if (exIntersectFlag) {
                            float L2 = (float)(rLeft * rLeft);
                            exDist = (float)Math.Sqrt(L2 + (float)(rBottom * rBottom));
                        } else {
                            float R2 = (float)(rRight * rRight);
                            inDist = (float)Math.Sqrt(R2 + (float)(rTop * rTop));
                        }
                    }

                    if (exIntersectFlag) {
                        Result = (((long)Math.Round(exDist)) <= (long)ExRadius);
                    } else {
                        Result = (((long)Math.Round(inDist)) >= (long)InRadius);
                    }
                } else {
                    if (anArea is CylindricityRoom) {
                        CylindricityRoom cylArea = ((CylindricityRoom)anArea);

                        int rLeft = cylArea.fCenterX - CenterX;
                        int rTop = cylArea.fCenterY - CenterY;

                        float L2 = rLeft * rLeft;
                        float T2 = rTop * rTop;
                        long hyp = ((long)Math.Round(Math.Sqrt((L2 + T2))));
                        if (hyp > (long)ExRadius) {
                            Result = (hyp < (long)(ExRadius + cylArea.fRadius));
                        } else {
                            Result = (hyp + (long)cylArea.fRadius > (long)InRadius);
                        }
                    } else {
                        if (!(anArea is QuadrantCorridor)) {
                            Result = anArea.IsIntersectWithArea(this);
                        }
                    }
                }
            }
            return Result;
        }

        protected override bool BuildArea()
        {
            bool result = ParentMark != null;
            if (result) {
                ExRadius = RandomHelper.GetBoundedRnd((int)Owner.QuadrantCorridorExRadius, (int)((int)((uint)(int)Owner.AreaSizeLimit >> 1)));
                InRadius = RandomHelper.GetBoundedRnd((int)Owner.QuadrantCorridorInRadius, ExRadius - ((int)Owner.QuadrantCorridorExRadius - (int)Owner.QuadrantCorridorInRadius));
                ExRadius = InRadius + Math.Max(4, ExRadius - InRadius);
                int delta = RandomHelper.GetBoundedRnd(1, ExRadius - InRadius - 1);
                int factor;
                if (RandomHelper.GetBoundedRnd(0, 1) > 0) {
                    factor = -1;
                } else {
                    factor = 1;
                }
                int direction = ParentMark.Direction;

                if (direction != Directions.DtNorth && direction != Directions.DtSouth) {
                    if (direction == Directions.DtWest || direction == Directions.DtEast) {
                        SetDimension(ParentMark.Location.X, ParentMark.Location.Y + factor * (InRadius + delta), InRadius, ExRadius);
                    }
                } else {
                    SetDimension(ParentMark.Location.X + factor * (InRadius + delta), ParentMark.Location.Y, InRadius, ExRadius);
                }

                ExtRect mapArea = Owner.DungeonArea;
                result = (MathHelper.IsValueBetween(CenterX, 0, mapArea.GetWidth() - 1, true) && MathHelper.IsValueBetween(CenterY, 0, mapArea.GetHeight() - 1, true));
            }
            return result;
        }

        protected override bool IsWallPoint(int ptX, int ptY)
        {
            int direction = ParentMark.Direction;

            bool result;
            if (direction != Directions.DtNorth) {
                if (direction != Directions.DtSouth) {
                    if (direction != Directions.DtWest) {
                        result = (direction == Directions.DtEast && ((ptY == CenterY && MathHelper.IsValueBetween(ptX, CenterX + InRadius, CenterX + ExRadius, true)) || (ptX == CenterX && (MathHelper.IsValueBetween(ptY, CenterY + InRadius, CenterY + ExRadius, true) || MathHelper.IsValueBetween(ptY, CenterY - InRadius, CenterY - ExRadius, true)))));
                    } else {
                        result = ((ptY == CenterY && MathHelper.IsValueBetween(ptX, CenterX - InRadius, CenterX - ExRadius, true)) || (ptX == CenterX && (MathHelper.IsValueBetween(ptY, CenterY + InRadius, CenterY + ExRadius, true) || MathHelper.IsValueBetween(ptY, CenterY - InRadius, CenterY - ExRadius, true))));
                    }
                } else {
                    result = ((ptX == CenterX && MathHelper.IsValueBetween(ptY, CenterY + ExRadius, CenterY + InRadius, true)) || (ptY == CenterY && (MathHelper.IsValueBetween(ptX, CenterX + InRadius, CenterX + ExRadius, true) || MathHelper.IsValueBetween(ptX, CenterX - InRadius, CenterX - ExRadius, true))));
                }
            } else {
                result = ((ptX == CenterX && MathHelper.IsValueBetween(ptY, CenterY - InRadius, CenterY - ExRadius, true)) || (ptY == CenterY && (MathHelper.IsValueBetween(ptX, CenterX + InRadius, CenterX + ExRadius, true) || MathHelper.IsValueBetween(ptX, CenterX - InRadius, CenterX - ExRadius, true))));
            }

            if (!result) {
                if (Math.Abs(CenterX - ptX) <= InRadius) {
                    float inrad2 = (float)(InRadius * InRadius);
                    int dX = CenterX - ptX;
                    result = (((long)Math.Round(Math.Sqrt((inrad2 - (float)(dX * dX))))) == (long)Math.Abs(CenterY - ptY));
                }
                if (!result && Math.Abs(CenterX - ptX) <= ExRadius) {
                    float exrad2 = (float)(ExRadius * ExRadius);
                    int dX = CenterX - ptX;
                    result = (((long)Math.Round(Math.Sqrt((exrad2 - (float)(dX * dX))))) == (long)Math.Abs(CenterY - ptY));
                }
            }

            return result;
        }

        internal void SetDimension(int centerX, int centerY, int inRadius, int exRadius)
        {
            CenterX = centerX;
            CenterY = centerY;
            InRadius = inRadius;
            ExRadius = exRadius;
        }

        internal ExtPoint XYFactors
        {
            get {
                ExtPoint result = ExtPoint.Empty;
    
                if ((ParentMark.Direction == Directions.DtNorth && NextDirection == Directions.DtWest) || (ParentMark.Direction == Directions.DtEast && NextDirection == Directions.DtSouth)) {
                    result.X = 1;
                    result.Y = -1;
                } else {
                    if ((ParentMark.Direction == Directions.DtNorth && NextDirection == Directions.DtEast) || (ParentMark.Direction == Directions.DtWest && NextDirection == Directions.DtSouth)) {
                        result.X = -1;
                        result.Y = -1;
                    } else {
                        if ((ParentMark.Direction == Directions.DtSouth && NextDirection == Directions.DtWest) || (ParentMark.Direction == Directions.DtEast && NextDirection == Directions.DtNorth)) {
                            result.X = 1;
                            result.Y = 1;
                        } else {
                            if ((ParentMark.Direction == Directions.DtSouth && NextDirection == Directions.DtEast) || (ParentMark.Direction == Directions.DtWest && NextDirection == Directions.DtNorth)) {
                                result.X = -1;
                                result.Y = 1;
                            }
                        }
                    }
                }
    
                return result;
            }
        }

        private QuadrantIndex Quadrant
        {
            get {
                QuadrantIndex result = QuadrantIndex.qiFirst;
    
                if ((ParentMark.Direction == Directions.DtNorth && NextDirection == Directions.DtWest) || (ParentMark.Direction == Directions.DtEast && NextDirection == Directions.DtSouth)) {
                    result = QuadrantIndex.qiFirst;
                } else {
                    if ((ParentMark.Direction == Directions.DtNorth && NextDirection == Directions.DtEast) || (ParentMark.Direction == Directions.DtWest && NextDirection == Directions.DtSouth)) {
                        result = QuadrantIndex.qiSecond;
                    } else {
                        if ((ParentMark.Direction == Directions.DtSouth && NextDirection == Directions.DtWest) || (ParentMark.Direction == Directions.DtEast && NextDirection == Directions.DtNorth)) {
                            result = QuadrantIndex.qiFourth;
                        } else {
                            if ((ParentMark.Direction == Directions.DtSouth && NextDirection == Directions.DtEast) || (ParentMark.Direction == Directions.DtWest && NextDirection == Directions.DtNorth)) {
                                result = QuadrantIndex.qiThird;
                            }
                        }
                    }
                }
    
                return result;
            }
        }

        public override void FlushToMap()
        {
            try {
                ExtPoint factors = XYFactors;

                for (int ax = 0; ax <= ExRadius; ax++) {
                    int yBottomLimit;
                    if (ax < InRadius) {
                        yBottomLimit = (int)((long)Math.Round(Math.Sqrt(((InRadius * InRadius) - (float)(ax * ax)))));
                    } else {
                        yBottomLimit = 0;
                    }

                    int arg = (int)((long)Math.Round(Math.Sqrt(((ExRadius * ExRadius) - (float)(ax * ax)))));
                    for (int ay = arg; ay <= yBottomLimit; ay++) {
                        int ax1 = ax + 1;
                        int ax2 = ax - 1;
                        if ((IsWallPoint(CenterX + factors.X * ax, CenterY + factors.Y * ay)) || ((ax1 * ax1) + (ay * ay) > ExRadius * ExRadius) || ((ax2 * ax2) + (ay * ay) < InRadius * InRadius)) {
                            Owner.SetTile(CenterX + factors.X * ax, CenterY + factors.Y * ay, TileType.ttDungeonWall);
                        } else {
                            Owner.SetTile(CenterX + factors.X * ax, CenterY + factors.Y * ay, TileType.ttUndefined);
                        }
                    }
                }

                Owner.SetTile(CenterX + factors.X * (ExRadius - 1), CenterY + factors.Y, TileType.ttUndefined);
                Owner.SetTile(CenterX + factors.X * ExRadius, CenterY + (factors.Y << 1), TileType.ttDungeonWall);
                Owner.SetTile(CenterX + factors.X * ExRadius, CenterY + factors.Y, TileType.ttDungeonWall);

                FlushMarksList();
            } catch (Exception ex) {
                Logger.Write("QuadrantCorridor.flushToMap(): " + ex.Message);
                throw ex;
            }
        }

        public override void GenerateMarksList()
        {
            try {
                int marksCount = 1;
                int failedTries = 0;
                do {
                    int direction = ParentMark.Direction;
                    int markX = 0;
                    int markY = 0;
                    if (direction != Directions.DtNorth) {
                        if (direction != Directions.DtSouth) {
                            if (direction != Directions.DtWest) {
                                if (direction == Directions.DtEast) {
                                    markX = RandomHelper.GetBoundedRnd(CenterX + InRadius + 1, CenterX + ExRadius - 1);
                                    markY = CenterY;
                                }
                            } else {
                                markX = RandomHelper.GetBoundedRnd(CenterX - InRadius - 1, CenterX - ExRadius + 1);
                                markY = CenterY;
                            }
                        } else {
                            markX = CenterX;
                            markY = RandomHelper.GetBoundedRnd(CenterY + InRadius + 1, CenterY + ExRadius - 1);
                        }
                    } else {
                        markX = CenterX;
                        markY = RandomHelper.GetBoundedRnd(CenterY - InRadius - 1, CenterY - ExRadius + 1);
                    }

                    DungeonMark mark = new DungeonMark(this, markX, markY, NextDirection);

                    if (IsAllowedMark(mark)) {
                        MarksList.Add(mark);
                        marksCount--;
                        failedTries = 0;
                    } else {
                        mark.Dispose();
                        failedTries++;
                    }
                    if (failedTries > (int)Owner.RightMarkSearchLimit) {
                        marksCount--;
                    }
                } while (marksCount > 0);
            } catch (Exception ex) {
                Logger.Write("QuadrantCorridor.generateMarksList(): " + ex.Message);
            }
        }

        public override int DevourArea
        {
            get {
                return (int)((long)Math.Round((Math.PI * (float)ExRadius * (float)ExRadius - Math.PI * (float)InRadius * (float)InRadius) / 4.0));
            }
        }

        public override ExtRect DimensionRect
        {
            get {
                ExtRect Result = ExtRect.Empty;
    
                if ((ParentMark.Direction == Directions.DtNorth && NextDirection == Directions.DtWest) || (ParentMark.Direction == Directions.DtEast && NextDirection == Directions.DtSouth)) {
                    Result = ExtRect.Create(CenterX, CenterY - ExRadius, CenterX + ExRadius, CenterY);
                } else {
                    if ((ParentMark.Direction == Directions.DtNorth && NextDirection == Directions.DtEast) || (ParentMark.Direction == Directions.DtWest && NextDirection == Directions.DtSouth)) {
                        Result = ExtRect.Create(CenterX - ExRadius, CenterY - ExRadius, CenterX, CenterY);
                    } else {
                        if ((ParentMark.Direction == Directions.DtSouth && NextDirection == Directions.DtWest) || (ParentMark.Direction == Directions.DtEast && NextDirection == Directions.DtNorth)) {
                            Result = ExtRect.Create(CenterX, CenterY, CenterX + ExRadius, CenterY + ExRadius);
                        } else {
                            if ((ParentMark.Direction == Directions.DtSouth && NextDirection == Directions.DtEast) || (ParentMark.Direction == Directions.DtWest && NextDirection == Directions.DtNorth)) {
                                Result = ExtRect.Create(CenterX - ExRadius, CenterY, CenterX, CenterY + ExRadius);
                            }
                        }
                    }
                }
    
                return Result;
            }
        }

        public override bool IsAllowedPointAsMark(int ptX, int ptY)
        {
            bool Result = IsWallPoint(ptX, ptY);
            if (Result) {
                int direction = ParentMark.Direction;
                if (direction != Directions.DtNorth) {
                    if (direction != Directions.DtSouth) {
                        if (direction != Directions.DtWest) {
                            if (direction == Directions.DtEast) {
                                Result = ((ptY == CenterY && (ptX == CenterX + InRadius || ptX == CenterX + ExRadius)) || (ptX == CenterX && (ptY == CenterY + InRadius || ptY == CenterY + ExRadius || ptY == CenterY - InRadius || ptY == CenterY - ExRadius)));
                            }
                        } else {
                            Result = ((ptY == CenterY && (ptX == CenterX - InRadius || ptX == CenterX - ExRadius)) || (ptX == CenterX && (ptY == CenterY + InRadius || ptY == CenterY + ExRadius || ptY == CenterY - InRadius || ptY == CenterY - ExRadius)));
                        }
                    } else {
                        Result = ((ptX == CenterX && (ptY == CenterY + InRadius || ptY == CenterY + ExRadius)) || (ptY == CenterY && (ptX == CenterX + InRadius || ptX == CenterX + ExRadius || ptX == CenterX - InRadius || ptX == CenterX - ExRadius)));
                    }
                } else {
                    Result = ((ptX == CenterX && (ptY == CenterY - InRadius || ptY == CenterY - ExRadius)) || (ptY == CenterY && (ptX == CenterX + InRadius || ptX == CenterX + ExRadius || ptX == CenterX - InRadius || ptX == CenterX - ExRadius)));
                }
                if (Result) {
                    Result = false;
                }
            }
            return Result;
        }

        public override bool IsOwnedPoint(int ptX, int ptY)
        {
            bool result = IsWallPoint(ptX, ptY);

            int dX = CenterX - ptX;
            if (!result && Math.Abs(dX) <= InRadius) {
                int dY = Math.Abs(CenterY - ptY);
                float inrad2 = (float)(InRadius * InRadius);
                int inrHyp = (int)(Math.Round(Math.Sqrt((inrad2 - (float)(dX * dX)))));
                float exrad2 = (float)(ExRadius * ExRadius);
                result = MathHelper.IsValueBetween(dY, inrHyp, (int)(Math.Round(Math.Sqrt((exrad2 - (float)(dX * dX))))), true);
            } else {
                result = false;
            }

            return result;
        }
    }
}
