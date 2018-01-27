/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
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
using System.Collections.Generic;
using BSLib;
using ZRLib.Core;

namespace ZRLib.Map.Dungeons
{
    public abstract class CustomStaticArea : StaticArea
    {
        private sealed class CustomMark
        {
            public int X;
            public int Y;
            public int Dir;

            public CustomMark(int x, int y, int dir)
            {
                X = x;
                Y = y;
                Dir = dir;
            }
        }

        private const string DoorSym = "NSWE";
        private const string CustomSym = "X.NSWE";

        private List<CustomMark> fAvailMarks;
        protected int fDevourArea;

        protected CustomStaticArea(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            fAvailMarks = new List<CustomMark>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                fAvailMarks.Clear();
                fAvailMarks = null;
            }
            base.Dispose(disposing);
        }

        private static void AddDoor(List<CustomMark> doorsList, int aX, int aY, char aSym)
        {
            int dir;

            switch (aSym) {
                case 'E':
                    dir = Directions.DtEast;
                    break;
                case 'N':
                    dir = Directions.DtNorth;
                    break;
                case 'S':
                    dir = Directions.DtSouth;
                    break;
                case 'W':
                    dir = Directions.DtWest;
                    break;
                default:
                    return;
            }

            doorsList.Add(new CustomMark(aX, aY, dir));
        }

        protected override bool BuildArea()
        {
            bool result = false;

            try {
                fAvailMarks.Clear();
                fDevourArea = 0;

                List<CustomMark> doors = new List<CustomMark>();

                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        char sym = GetCustomCell(x, y);
                        if (sym == '.') {
                            fDevourArea++;
                            fArea[x, y] = TileType.ttUndefined;
                        } else {
                            if (sym == 'E' || sym == 'N' || sym == 'S' || sym == 'W' || sym == 'X') {
                                fDevourArea++;
                                fArea[x, y] = TileType.ttDungeonWall;

                                if (DoorSym.IndexOf(sym) >= 0) {
                                    AddDoor(fAvailMarks, x, y, sym);
                                    if (sym == Directions.Data[ParentMark.Direction].SymOpposite) {
                                        AddDoor(doors, x, y, sym);
                                    }
                                }
                            }
                        }
                    }
                }

                CustomMark door = RandomHelper.GetRandomItem(doors);
                if (door != null) {
                    SetPosition(ParentMark.Location.X - door.X, ParentMark.Location.Y - door.Y);
                    result = true;
                }
            } catch (Exception ex) {
                Logger.Write("CustomStaticArea.buildArea(): " + ex.Message);
                throw ex;
            }

            return result;
        }

        protected abstract char GetCustomCell(int aX, int aY);

        public override void GenerateMarksList()
        {
            try {
                foreach (CustomMark mark in fAvailMarks) {
                    TryInsertMark(Left + mark.X, Top + mark.Y, mark.Dir);
                }
            } catch (Exception ex) {
                Logger.Write("CustomStaticArea.generateMarksList(): " + ex.Message);
                throw ex;
            }
        }

        public override int DevourArea
        {
            get { return fDevourArea; }
        }

        public override bool IsOwnedPoint(int ptX, int ptY)
        {
            int ax = ptX - Left;
            int ay = ptY - Top;

            bool result;
            if (ax < 0 || ax >= Width || ay < 0 || ay >= Height) {
                result = false;
            } else {
                char sym = GetCustomCell(ax, ay);
                result = CustomSym.IndexOf(sym) >= 0;
            }
            return result;
        }
    }
}
