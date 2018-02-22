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
    public sealed class QuakeIIArena : StaticArea
    {
        private static readonly string[] QaArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new QuakeIIArena(owner, parentMark);
        }

        public QuakeIIArena(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(21, 21);
        }

        protected override bool BuildArea()
        {
            bool result = false;

            for (int y = 0; y < 21; y++) {
                for (int x = 0; x < 21; x++) {
                    char c = QaArea[y][x];
                    if (c != '.') {
                        if (c == 'X') {
                            fArea[x, y] = TileType.ttDungeonWall;
                        }
                    } else {
                        fArea[x, y] = TileType.ttUndefined;
                    }
                }
            }

            switch (ParentMark.Direction) {
                case Directions.DtNorth:
                    SetPosition(ParentMark.Location.X - RandomHelper.GetBoundedRnd(1, Width - 2), ParentMark.Location.Y - 20);
                    result = MathHelper.IsValueBetween(ParentMark.Location.X, Left, Left + Width - 1, false);
                    break;
                case Directions.DtSouth:
                    SetPosition(ParentMark.Location.X - RandomHelper.GetBoundedRnd(1, Width - 2), ParentMark.Location.Y);
                    result = MathHelper.IsValueBetween(ParentMark.Location.X, Left, Left + Width - 1, false);
                    break;
                case Directions.DtWest:
                    SetPosition(ParentMark.Location.X - 20, ParentMark.Location.Y - RandomHelper.GetBoundedRnd(1, Height - 2));
                    result = MathHelper.IsValueBetween(ParentMark.Location.Y, Top, Top + Height - 1, false);
                    break;
                case Directions.DtEast:
                    SetPosition(ParentMark.Location.X, ParentMark.Location.Y - RandomHelper.GetBoundedRnd(1, Height - 2));
                    result = MathHelper.IsValueBetween(ParentMark.Location.Y, Top, Top + Height - 1, false);
                    break;
            }

            return result;
        }

        public override void GenerateMarksList()
        {
            try {
                int marksCount = RandomHelper.GetBoundedRnd(1, (int)Owner.RectRoomMarksLimit);
                int failedTries = 0;
                if (marksCount > 0) {
                    do {
                        int markDirection = RandomHelper.GetBoundedRnd(Directions.DtNorth, Directions.DtEast);
                        int tDirectionType = markDirection;
                        int markX = 0;
                        int markY = 0;

                        switch (tDirectionType) {
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

                        if (failedTries > (int)Owner.RightMarkSearchLimit) {
                            marksCount--;
                        }
                    } while (marksCount > 0);
                }
            } catch (Exception ex) {
                Logger.Write("QuakeIIArena.generateMarksList(): " + ex.Message);
            }
        }

        public override bool IsAllowedPointAsMark(int ptX, int ptY)
        {
            return (MathHelper.IsValueBetween(ptX, Left, Left + Width - 1, false) && Top == ptY) || (MathHelper.IsValueBetween(ptX, Left, Left + Width - 1, false) && Top + Height - 1 == ptY) || (Left == ptX && MathHelper.IsValueBetween(ptY, Top, Top + Height - 1, false)) || (Left + Width - 1 == ptX && MathHelper.IsValueBetween(ptY, Top, Top + Height - 1, false));
        }

        public override int DevourArea
        {
            get {
                return Width * Height;
            }
        }

        public override bool IsOwnedPoint(int ptX, int ptY)
        {
            char ch = QaArea[ptY - Top][ptX - Left - 1];
            return ("X+.").IndexOf(ch) >= 0;
        }

        static QuakeIIArena()
        {
            QaArea = new string[] {
                "XXXXXXXXXXXXXXXXXXXXX",
                "X...................X",
                "X.....X.......X.....X",
                "X....XX.......XX....X",
                "X...XX.........XX...X",
                "X...XX.........XX...X",
                "X..XX...........XX..X",
                "X..XX...........XX..X",
                "X..XX.XXXXXXXXX.XX..X",
                "X..XX..XXXXXXX..XX..X",
                "X..XXX..XX.XX..XXX..X",
                "X...XXX.XX.XX.XXX...X",
                "X...XXXXXX.XXXXXX...X",
                "X....XXXXXXXXXXX....X",
                "X.....XXXXXXXXX.....X",
                "X.......XX.XX.......X",
                "X.......XX.XX.......X",
                "X.......XX.XX.......X",
                "X........X.X........X",
                "X...................X",
                "XXXXXXXXXXXXXXXXXXXXX"
            };
        }
    }

}