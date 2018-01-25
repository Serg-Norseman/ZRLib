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
    public delegate DungeonArea IDungeonAreaCreateProc(DungeonBuilder owner, DungeonMark parentMark);

    public sealed class DungeonMark : BaseObject
    {
        // Marker State
        public const byte Ms_Undefined = 0;
        public const byte Ms_AreaGenerator = 1;
        public const byte Ms_RetriesExhaust = 2;
        public const byte Ms_PointToOtherArea = 3;

        private byte fState;

        public bool AtCorridorEnd;
        public int Direction;
        public IDungeonAreaCreateProc ForcedAreaCreateProc;
        public ExtPoint Location;
        public readonly DungeonArea ParentArea;
        public int RetriesLeft;

        /// 
        /// <returns> see <seealso cref="DungeonMark"/> Marker states </returns>
        public byte State
        {
            get {
                byte result;
                if (fState == DungeonMark.Ms_RetriesExhaust) {
                    if (RetriesLeft > 0) {
                        result = DungeonMark.Ms_Undefined;
                    } else {
                        result = DungeonMark.Ms_RetriesExhaust;
                    }
                } else {
                    result = fState;
                }
                return result;
            }
            set {
                if (fState != value) {
                    if (value == DungeonMark.Ms_RetriesExhaust) {
                        if (RetriesLeft > 0) {
                            fState = DungeonMark.Ms_Undefined;
                        } else {
                            fState = DungeonMark.Ms_RetriesExhaust;
                        }
                    } else {
                        fState = value;
                    }
                }
            }
        }


        public DungeonMark(DungeonArea parentArea, int aX, int aY, int direction)
        {
            ParentArea = parentArea;
            Location = new ExtPoint(aX, aY);
            Direction = direction;
            ForcedAreaCreateProc = null;
            AtCorridorEnd = false;
            fState = DungeonMark.Ms_Undefined;

            if (ParentArea != null) {
                RetriesLeft = ParentArea.Owner.MarkRetriesLimit;
            } else {
                RetriesLeft = DungeonBuilder.DefaultMarkRetriesLimit;
            }
        }

        public bool PointsToOtherArea
        {
            get {
                try {
                    if (ParentArea != null && ParentArea.Owner != null) {
                        return ParentArea.Owner.IsPointsToOtherArea(Location.X, Location.Y, ParentArea);
                    }
                } catch (Exception ex) {
                    Logger.Write("DungeonMark.isPointsToOtherArea(): " + ex.Message);
                }
                return false;
            }
        }
    }
}
