/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
 *  Copyright (C) 2003 by Ruslan N. Garipov (aka Brigadir).
 *  Copyright (C) 2002-2008, 2014 by Serg V. Zhdanovskih.
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
    public sealed class CylindricityRoom : DungeonArea
    {
        internal int fCenterX;
        internal int fCenterY;
        internal int fRadius;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new CylindricityRoom(owner, parentMark);
        }

        public CylindricityRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            bool result;

            if (anArea is RectangularRoom) {
                ExtRect rectArea = ((RectangularRoom)anArea).DimensionRect;

                if (!MathHelper.IsValueBetween(fCenterY, rectArea.Top, rectArea.Bottom, true) && !MathHelper.IsValueBetween(fCenterX, rectArea.Left, rectArea.Right, true)) {
                    result = (fRadius > Math.Min(Math.Abs(rectArea.Top - fCenterY), Math.Abs(rectArea.Bottom - fCenterY)) && fRadius > Math.Min(Math.Abs(rectArea.Left - fCenterX), Math.Abs(rectArea.Right - fCenterX)));
                } else {
                    if (MathHelper.IsValueBetween(fCenterY, rectArea.Top, rectArea.Bottom, true) && !MathHelper.IsValueBetween(fCenterX, rectArea.Left, rectArea.Right, true)) {
                        result = (fRadius > Math.Min(Math.Abs(rectArea.Left - fCenterX), Math.Abs(rectArea.Right - fCenterX)));
                    } else {
                        result = (MathHelper.IsValueBetween(fCenterY, rectArea.Top, rectArea.Bottom, true) || !MathHelper.IsValueBetween(fCenterX, rectArea.Left, rectArea.Right, true) || fRadius > Math.Min(Math.Abs(rectArea.Top - fCenterY), Math.Abs(rectArea.Bottom - fCenterY)));
                    }
                }
            } else {
                if (anArea is CylindricityRoom) {
                    CylindricityRoom cylArea = ((CylindricityRoom)anArea);
                    try {
                        int dX = fCenterX - cylArea.fCenterX;
                        int dY = fCenterY - cylArea.fCenterY;
                        float s = (float)Math.Sqrt((float)(dX * dX) + (float)(dY * dY));
                        result = ((float)(fRadius + cylArea.fRadius) > s);
                    } catch (Exception ex) {
                        Logger.Write("CylindricityRoom.isIntersectWithArea(): " + ex.Message);
                        result = true;
                    }
                } else {
                    result = anArea.IsIntersectWithArea(this);
                }
            }

            return result;
        }

        protected override bool BuildArea()
        {
            bool result = base.ParentMark != null && (base.ParentMark.ParentArea == null || base.ParentMark.ParentArea is LinearCorridor);

            if (result) {
                fRadius = RandomHelper.GetBoundedRnd((int)base.Owner.CylindricityRoomRadius, (int)((int)((uint)(int)base.Owner.AreaSizeLimit >> 1)));

                switch (base.ParentMark.Direction) {
                    case Directions.DtNorth:
                        fCenterY = base.ParentMark.Location.Y - fRadius;
                        SetDimension(base.ParentMark.Location.X, fCenterY, fRadius);
                        break;
                    case Directions.DtSouth:
                        fCenterY = base.ParentMark.Location.Y + fRadius;
                        SetDimension(base.ParentMark.Location.X, fCenterY, fRadius);
                        break;
                    case Directions.DtWest:
                        fCenterX = base.ParentMark.Location.X - fRadius;
                        SetDimension(fCenterX, base.ParentMark.Location.Y, fRadius);
                        break;
                    case Directions.DtEast:
                        fCenterX = base.ParentMark.Location.X + fRadius;
                        SetDimension(fCenterX, base.ParentMark.Location.Y, fRadius);
                        break;
                }

                result = (fRadius >= (int)base.Owner.CylindricityRoomRadius);
            }

            return result;
        }

        protected override bool IsWallPoint(int ptX, int ptY)
        {
            bool result;
            if (fRadius < Math.Abs(fCenterX - ptX)) {
                result = false;
            } else {
                float sqrRad = (float)(fRadius * fRadius);
                int dX = fCenterX - ptX;
                result = (((long)Math.Round(Math.Sqrt((sqrRad - (float)(dX * dX))))) == (long)Math.Abs(fCenterY - ptY));
            }
            return result;
        }

        internal void SetDimension(int centerX, int centerY, int radius)
        {
            fCenterX = centerX;
            fCenterY = centerY;
            fRadius = radius;
        }

        public override void FlushToMap()
        {
            try {
                for (int ax = 0; ax <= fRadius; ax++) {
                    int dy = (int)(Math.Round(Math.Sqrt(((fRadius * fRadius) - (float)(ax * ax)))));

                    for (int ay = dy; ay >= -dy; ay--) {
                        int ax1 = ax + 1;
                        int hp = (ax1 * ax1) + (ay * ay);

                        if ((IsWallPoint(fCenterX + ax, fCenterY - ay)) || (hp > fRadius * fRadius)) {
                            base.Owner.SetTile(fCenterX - ax, fCenterY - ay, TileType.ttCylindricityRoomWall);
                            base.Owner.SetTile(fCenterX + ax, fCenterY - ay, TileType.ttCylindricityRoomWall);
                        } else {
                            base.Owner.SetTile(fCenterX - ax, fCenterY - ay, TileType.ttUndefined);
                            base.Owner.SetTile(fCenterX + ax, fCenterY + ay, TileType.ttUndefined);
                        }
                    }
                }

                FlushMarksList();
            } catch (Exception ex) {
                Logger.Write("CylindricityRoom.flushToMap(): " + ex.Message);
                throw ex;
            }
        }

        private static int RotateDirectionToRightAngleCW(int direction)
        {
            int result = direction;

            switch (direction) {
                case Directions.DtNorth:
                    result = Directions.DtEast;
                    break;
                case Directions.DtSouth:
                    result = Directions.DtWest;
                    break;
                case Directions.DtWest:
                    result = Directions.DtNorth;
                    break;
                case Directions.DtEast:
                    result = Directions.DtSouth;
                    break;
            }

            return result;
        }

        private static int RotateDirectionToRightAngleCCW(int direction)
        {
            int result = direction;

            switch (direction) {
                case Directions.DtNorth:
                    result = Directions.DtWest;
                    break;
                case Directions.DtSouth:
                    result = Directions.DtEast;
                    break;
                case Directions.DtWest:
                    result = Directions.DtSouth;
                    break;
                case Directions.DtEast:
                    result = Directions.DtNorth;
                    break;
            }

            return result;
        }

        public override void GenerateMarksList()
        {
            try {
                int marksCount = 3;
                int failedTries = 0;
                do {
                    int num = marksCount;
                    int markDirection = Directions.DtNorth;
                    int markX = 0;
                    int markY = 0;
                    if (num != 1) {
                        if (num != 2) {
                            if (num == 3) {
                                markDirection = RotateDirectionToRightAngleCCW(base.ParentMark.Direction);
                                switch (base.ParentMark.Direction) {
                                    case Directions.DtNorth:
                                        markX = fCenterX - fRadius;
                                        markY = fCenterY;
                                        break;
                                    case Directions.DtSouth:
                                        markX = fCenterX + fRadius;
                                        markY = fCenterY;
                                        break;
                                    case Directions.DtWest:
                                        markX = fCenterX;
                                        markY = fCenterY + fRadius;
                                        break;
                                    case Directions.DtEast:
                                        markX = fCenterX;
                                        markY = fCenterY - fRadius;
                                        break;
                                }
                            }
                        } else {
                            markDirection = base.ParentMark.Direction;
                            switch (base.ParentMark.Direction) {
                                case Directions.DtNorth:
                                    markX = fCenterX;
                                    markY = fCenterY - fRadius;
                                    break;
                                case Directions.DtSouth:
                                    markX = fCenterX;
                                    markY = fCenterY + fRadius;
                                    break;
                                case Directions.DtWest:
                                    markX = fCenterX - fRadius;
                                    markY = fCenterY;
                                    break;
                                case Directions.DtEast:
                                    markX = fCenterX + fRadius;
                                    markY = fCenterY;
                                    break;
                            }
                        }
                    } else {
                        markDirection = RotateDirectionToRightAngleCW(base.ParentMark.Direction);

                        switch (base.ParentMark.Direction) {
                            case Directions.DtNorth:
                                markX = fCenterX + fRadius;
                                markY = fCenterY;
                                break;
                            case Directions.DtSouth:
                                markX = fCenterX - fRadius;
                                markY = fCenterY;
                                break;
                            case Directions.DtWest:
                                markX = fCenterX;
                                markY = fCenterY - fRadius;
                                break;
                            case Directions.DtEast:
                                markX = fCenterX;
                                markY = fCenterY + fRadius;
                                break;
                        }
                    }

                    DungeonMark mark = new DungeonMark(this, markX, markY, markDirection);
                    mark.ForcedAreaCreateProc = LinearCorridor.CreateArea;

                    if (IsAllowedMark(mark)) {
                        MarksList.Add(mark);
                        marksCount--;
                        failedTries = 0;
                    } else {
                        mark.Dispose();
                        failedTries++;
                    }

                    if (failedTries > (int)base.Owner.RightMarkSearchLimit) {
                        marksCount--;
                    }
                } while (marksCount > 0);
            } catch (Exception ex) {
                Logger.Write("CylindricityRoom.generateMarksList(): " + ex.Message);
            }
        }

        public override int DevourArea
        {
            get {
                return (int)Math.Round((float)(Math.PI * fRadius * fRadius));
            }
        }

        public override ExtRect DimensionRect
        {
            get {
                return ExtRect.Create(fCenterX - fRadius, fCenterY - fRadius, fCenterX + fRadius, fCenterY + fRadius);
            }
        }

        public override bool IsAllowedPointAsMark(int ptX, int ptY)
        {
            return (ptX == fCenterX && (ptY == fCenterY - fRadius || ptY == fCenterY + fRadius)) || (ptY == fCenterY && (ptX == fCenterX - fRadius || ptX == fCenterX + fRadius));
        }

        public override bool IsOwnedPoint(int ptX, int ptY)
        {
            bool result;
            if (fRadius < Math.Abs(fCenterX - ptX)) {
                result = false;
            } else {
                float sqrRad = (float)(fRadius * fRadius);
                int dX = fCenterX - ptX;
                result = (((long)Math.Round(Math.Sqrt((sqrRad - (float)(dX * dX))))) <= (long)Math.Abs(fCenterY - ptY));
            }
            return result;
        }
    }
}
