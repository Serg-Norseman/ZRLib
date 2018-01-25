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
    public abstract class StaticArea : DungeonArea
    {
        protected TileType[,] fArea;

        public int Left;
        public int Top;
        public int Height;
        public int Width;

        protected StaticArea(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            Left = -1;
            Top = -1;
            Width = -1;
            Height = -1;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                fArea = null;
            }
            base.Dispose(disposing);
        }

        protected void SetPosition(int left, int top)
        {
            Left = left;
            Top = top;
        }

        protected void SetDimension(int width, int height)
        {
            fArea = null;

            Width = width;
            Height = height;

            fArea = new TileType[width, height];

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    fArea[x, y] = TileType.ttRock;
                }
            }
        }

        public override bool IsIntersectWithArea(DungeonArea area)
        {
            return DimensionRect.IsIntersect(area.DimensionRect);
        }

        protected override bool IsWallPoint(int ptX, int ptY)
        {
            return fArea[ptX - Left, ptY - Top] == TileType.ttDungeonWall;
        }

        public override void FlushToMap()
        {
            try {
                DungeonBuilder builder = base.Owner;

                int num = Left + Width - 1;
                for (int ax = Left; ax <= num; ax++) {
                    int num2 = Top + Height - 1;
                    for (int ay = Top; ay <= num2; ay++) {
                        if (IsWallPoint(ax, ay)) {
                            builder.SetTile(ax, ay, TileType.ttDungeonWall);
                        } else {
                            if (IsOwnedPoint(ax, ay)) {
                                builder.SetTile(ax, ay, TileType.ttUndefined);
                            }
                        }
                    }
                }

                FlushMarksList();
            } catch (Exception ex) {
                Logger.Write("StaticArea.flushToMap(): " + ex.Message);
                throw ex;
            }
        }

        public override ExtRect DimensionRect
        {
            get {
                return ExtRect.Create(Left, Top, Left + Width - 1, Top + Height - 1);
            }
        }

        public override bool IsAllowedPointAsMark(int ptX, int ptY)
        {
            foreach (DungeonMark mark in MarksList) {
                if (mark.Location.Equals(ptX, ptY)) {
                    return true;
                }
            }
            return false;
        }
    }
}
