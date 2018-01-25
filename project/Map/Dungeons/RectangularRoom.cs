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
    public class RectangularRoom : DungeonArea
    {
        public int Left;
        public int Top;
        public int Width;
        public int Height;

        public int Right
        {
            get {
                return Left + Width - 1;
            }
        }

        public int Bottom
        {
            get {
                return Top + Height - 1;
            }
        }

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new RectangularRoom(owner, parentMark);
        }

        public RectangularRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            bool result;
            if (anArea is RectangularRoom) {
                result = DimensionRect.IsIntersect(((RectangularRoom)anArea).DimensionRect);
            } else {
                result = anArea.IsIntersectWithArea(this);
            }
            return result;
        }

        protected override bool BuildArea()
        {
            bool result = false;

            int corrWidth = Owner.CorridorWidthBottomLimit;
            int corrLength = Owner.CorridorLengthBottomLimit;
            int areaSize = Owner.AreaSizeLimit;

            switch (ParentMark.Direction) {
                case Directions.DtNorth:
                    Width = RandomHelper.GetBoundedRnd(corrWidth, areaSize);
                    Height = RandomHelper.GetBoundedRnd(corrLength, areaSize);
                    SetDimension(ParentMark.Location.X - RandomHelper.GetBoundedRnd(1, Width - 2), ParentMark.Location.Y - Height + 1, Width, Height);
                    result = MathHelper.IsValueBetween(ParentMark.Location.X, Left, Left + Width - 1, false);
                    break;

                case Directions.DtSouth:
                    Width = RandomHelper.GetBoundedRnd(corrWidth, areaSize);
                    Height = RandomHelper.GetBoundedRnd(corrLength, areaSize);
                    SetDimension(ParentMark.Location.X - RandomHelper.GetBoundedRnd(1, Width - 2), ParentMark.Location.Y, Width, Height);
                    result = MathHelper.IsValueBetween(ParentMark.Location.X, Left, Left + Width - 1, false);
                    break;

                case Directions.DtWest:
                    Width = RandomHelper.GetBoundedRnd(corrLength, areaSize);
                    Height = RandomHelper.GetBoundedRnd(corrWidth, areaSize);
                    SetDimension(ParentMark.Location.X - Width + 1, ParentMark.Location.Y - RandomHelper.GetBoundedRnd(1, Height - 2), Width, Height);
                    result = MathHelper.IsValueBetween(ParentMark.Location.Y, Top, Top + Height - 1, false);
                    break;

                case Directions.DtEast:
                    Width = RandomHelper.GetBoundedRnd(corrLength, areaSize);
                    Height = RandomHelper.GetBoundedRnd(corrWidth, areaSize);
                    SetDimension(ParentMark.Location.X, ParentMark.Location.Y - RandomHelper.GetBoundedRnd(1, Height - 2), Width, Height);
                    result = MathHelper.IsValueBetween(ParentMark.Location.Y, Top, Top + Height - 1, false);
                    break;
            }

            return result;
        }

        protected override bool IsWallPoint(int ptX, int ptY)
        {
            return DimensionRect.IsBorder(ptX, ptY);
        }

        protected virtual void SetDimension(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public override void FlushToMap()
        {
            try {
                int right = Left + Width - 1;
                int bottom = Top + Height - 1;
                for (int ax = Left; ax <= right; ax++) {
                    for (int ay = Top; ay <= bottom; ay++) {
                        if (IsWallPoint(ax, ay)) {
                            if (this is LinearCorridor) {
                                Owner.SetTile(ax, ay, TileType.ttLinearCorridorWall);
                            } else {
                                Owner.SetTile(ax, ay, TileType.ttRectRoomWall);
                            }
                        } else {
                            Owner.SetTile(ax, ay, TileType.ttUndefined);
                        }
                    }
                }

                FlushMarksList();
            } catch (Exception ex) {
                Logger.Write("RectangularRoom.flushToMap(): " + ex.Message);
                throw ex;
            }
        }

        public override void GenerateMarksList()
        {
            try {
                int marksCount = RandomHelper.GetBoundedRnd(1, Owner.RectRoomMarksLimit);
                int failedTries = 0;
                if (marksCount > 0) {
                    do {
                        int markDirection = (RandomHelper.GetBoundedRnd(Directions.DtNorth, Directions.DtEast));
                        int markX = 0;
                        int markY = 0;
                        switch (markDirection) {
                            case Directions.DtNorth:
                                markX = Left + RandomHelper.GetBoundedRnd(1, Width - 2);
                                markY = Top;
                                break;
                            case Directions.DtSouth:
                                markX = Left + RandomHelper.GetBoundedRnd(1, Width - 2);
                                markY = Top + Height - 1;
                                break;
                            case Directions.DtWest:
                                markX = Left;
                                markY = Top + RandomHelper.GetBoundedRnd(1, Height - 2);
                                break;
                            case Directions.DtEast:
                                markX = Left + Width - 1;
                                markY = Top + RandomHelper.GetBoundedRnd(1, Height - 2);
                                break;
                        }

                        DungeonMark mark = new DungeonMark(this, markX, markY, markDirection);

                        if (IsAllowedMark(mark)) {
                            MarksList.Add(mark);
                            marksCount--;
                            failedTries = 0;
                        } else {
                            mark.Dispose();
                            failedTries++;
                        }

                        if (failedTries > Owner.RightMarkSearchLimit) {
                            marksCount--;
                        }
                    } while (marksCount > 0);
                }
            } catch (Exception ex) {
                Logger.Write("RectangularRoom.generateMarksList(): " + ex.Message);
            }
        }

        public override bool IsAllowedPointAsMark(int ptX, int ptY)
        {
            return (MathHelper.IsValueBetween(ptX, Left, Left + Width - 1, false) && Top == ptY) || (MathHelper.IsValueBetween(ptX, Left, Left + Width - 1, false) && Top + Height - 1 == ptY) || (Left == ptX && MathHelper.IsValueBetween(ptY, Top, Top + Height - 1, false)) || (Left + Width - 1 == ptX && MathHelper.IsValueBetween(ptY, Top, Top + Height - 1, false));
        }

        public override int DevourArea
        {
            get { return Width * Height; }
        }

        public override bool IsOwnedPoint(int ptX, int ptY)
        {
            return MathHelper.IsValueBetween(ptX, Left, Left + Width - 1, true) && MathHelper.IsValueBetween(ptY, Top, Top + Height - 1, true);
        }

        public override ExtRect DimensionRect
        {
            get { return ExtRect.Create(Left, Top, Right, Bottom); }
        }
    }
}
