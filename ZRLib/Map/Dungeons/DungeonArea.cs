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
using System.Collections.Generic;
using BSLib;
using ZRLib.Core;

namespace ZRLib.Map.Dungeons
{
    public abstract class DungeonArea : BaseObject
    {
        public readonly IList<DungeonMark> MarksList;
        public readonly DungeonBuilder Owner;
        public readonly DungeonMark ParentMark;

        protected DungeonArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            Owner = owner;
            ParentMark = parentMark;
            MarksList = new List<DungeonMark>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                //MarksList.dispose();
            }
            base.Dispose(disposing);
        }

        protected bool IsAllowedMark(DungeonMark mark)
        {
            bool result;

            try {
                result = !mark.Location.Equals(ParentMark.Location);

                if (result) {
                    int markIterator = 0;
                    while (markIterator < MarksList.Count & result) {
                        result = !mark.Location.Equals((MarksList[markIterator]).Location);
                        markIterator++;
                    }
                }
            } catch (Exception ex) {
                Logger.Write("DungeonArea.isAllowedMark(): " + ex.Message);
                result = false;
            }

            return result;
        }

        protected void FlushMarksList()
        {
            try {
                foreach (DungeonMark mark in MarksList) {
                    int state = mark.State;
                    TileType markType = TileType.ttUndefinedMark;

                    switch (state) {
                        case DungeonMark.Ms_Undefined:
                            markType = TileType.ttUndefinedMark;
                            break;
                        case DungeonMark.Ms_AreaGenerator:
                            markType = TileType.ttAreaGeneratorMark;
                            break;
                        case DungeonMark.Ms_RetriesExhaust:
                            markType = TileType.ttRetriesExhaustMark;
                            break;
                        case DungeonMark.Ms_PointToOtherArea:
                            markType = TileType.ttPointToOtherAreaMark;
                            break;
                    }

                    Owner.SetTile(mark.Location.X, mark.Location.Y, markType);
                }
            } catch (Exception ex) {
                Logger.Write("DungeonArea.flushMarksList(): " + ex.Message);
                throw ex;
            }
        }

        protected void TryInsertMark(int aX, int aY, int direction)
        {
            DungeonMark mark = new DungeonMark(this, aX, aY, direction);
            if (IsAllowedMark(mark)) {
                MarksList.Add(mark);
            } else {
                mark.Dispose();
            }
        }

        public bool TryApplyThisArea()
        {
            try {
                bool result = BuildArea();
                bool kill = false;

                ParentMark.RetriesLeft = ParentMark.RetriesLeft - 1;

                if (result) {
                    kill = (ParentMark != null && ParentMark.PointsToOtherArea);

                    if (kill) {
                        result = false;
                        ParentMark.State = DungeonMark.Ms_PointToOtherArea;
                    }
                }

                if (!kill) {
                    if (result) {
                        result = Owner.IsFitAreaDimension(this);
                    }

                    if (!result) {
                        if (ParentMark.RetriesLeft > 0) {
                            result = TryApplyThisArea();
                        } else {
                            ParentMark.State = DungeonMark.Ms_RetriesExhaust;
                        }
                    }

                    if (result) {
                        ParentMark.State = DungeonMark.Ms_AreaGenerator;
                    }
                }

                return result;
            } catch (Exception ex) {
                Logger.Write("DungeonArea.tryApplyThisArea(): " + ex.Message);
                throw ex;
            }
        }

        protected abstract bool BuildArea();

        protected abstract bool IsWallPoint(int ptX, int ptY);

        public abstract bool IsOwnedPoint(int ptX, int ptY);

        public abstract bool IsIntersectWithArea(DungeonArea area);

        public abstract void FlushToMap();

        public abstract void GenerateMarksList();

        public abstract bool IsAllowedPointAsMark(int ptX, int ptY);

        public abstract int DevourArea { get; }

        public abstract ExtRect DimensionRect { get; }
    }
}
