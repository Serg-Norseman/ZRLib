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
    public sealed class LinearCorridor : RectangularRoom
    {
        private bool fForceMarkAtEnd;
        private DungeonMark fForcedMark;

        public new static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new LinearCorridor(owner, parentMark);
        }

        public LinearCorridor(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            if (ParentMark != null) {
                ParentMark.AtCorridorEnd = true;
            }
            fForceMarkAtEnd = false;
        }

        protected override bool BuildArea()
        {
            bool result = base.BuildArea();

            if (result) {
                bool flag = false;
                IList<DungeonArea> areas = null;
                int direction = ParentMark.Direction;

                if (direction != Directions.DtNorth && direction != Directions.DtSouth) {
                    if (direction == Directions.DtWest || direction == Directions.DtEast) {
                        int directionFactor;
                        int rockWashingStart;
                        int rockWashingLimit;
                        if (ParentMark.Direction == Directions.DtWest) {
                            directionFactor = -1;
                            rockWashingStart = Left + Width - 1 - ((int)Owner.CorridorLengthBottomLimit - 1);
                            rockWashingLimit = Left;
                        } else {
                            directionFactor = 1;
                            rockWashingStart = Left + ((int)Owner.CorridorLengthBottomLimit - 1);
                            rockWashingLimit = Left + Width - 1;
                        }

                        int corridorLengthIterator = rockWashingStart;
                        while (corridorLengthIterator < rockWashingLimit && !flag) {
                            int corridorWidthIterator = Top;
                            while (corridorWidthIterator <= Top + Height - 1 && !flag) {
                                areas = Owner.IsAreaExistsAtPoint(corridorLengthIterator, corridorWidthIterator, this);
                                flag = (areas != null);
                                corridorWidthIterator++;
                            }
                            corridorLengthIterator += directionFactor;
                        }

                        if (flag) {
                            if (ParentMark.Direction == Directions.DtWest) {
                                SetDimension(corridorLengthIterator - directionFactor, Top, corridorLengthIterator - directionFactor + Width - 1, Height);
                            } else {
                                SetDimension(Left, Top, corridorLengthIterator - directionFactor - Left + 1, Height);
                            }

                            if (areas != null) {
                                fForceMarkAtEnd = false;
                                corridorLengthIterator = 0;
                                while (corridorLengthIterator < areas.Count && !fForceMarkAtEnd) {
                                    DungeonArea curArea = areas[corridorLengthIterator];

                                    int corridorWidthIterator = 0;
                                    while (corridorWidthIterator < curArea.MarksList.Count && !fForceMarkAtEnd) {
                                        DungeonMark curMark = curArea.MarksList[corridorWidthIterator];
                                        fForceMarkAtEnd = (MathHelper.IsValueBetween(curMark.Location.Y, Top, Top + Height - 1, false));
                                        corridorWidthIterator++;
                                    }

                                    corridorLengthIterator++;
                                }

                                if (!fForceMarkAtEnd) {
                                    fForceMarkAtEnd = true;
                                    if (ParentMark.Direction == Directions.DtWest) {
                                        rockWashingStart = Left;
                                    } else {
                                        rockWashingStart = Left + Width - 1;
                                    }

                                    fForcedMark = new DungeonMark(this, rockWashingStart, RandomHelper.GetBoundedRnd(Top + 1, Top + Height - 2), ParentMark.Direction);
                                    try {
                                        fForcedMark.AtCorridorEnd = true;
                                    } catch (Exception ex) {
                                        Logger.Write("LinearCorridor.buildArea.1(): " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                } else {
                    int directionFactor;
                    int rockWashingStart;
                    int rockWashingLimit;

                    if (ParentMark.Direction == Directions.DtNorth) {
                        directionFactor = -1;
                        rockWashingStart = Top + Height - 1 - (Owner.CorridorLengthBottomLimit - 1);
                        rockWashingLimit = Top;
                    } else {
                        directionFactor = 1;
                        rockWashingStart = Top + (Owner.CorridorLengthBottomLimit - 1);
                        rockWashingLimit = Top + Height - 1;
                    }

                    int corridorLengthIterator = rockWashingStart;
                    while (corridorLengthIterator < rockWashingLimit && !flag) {
                        int corridorWidthIterator = Left;
                        while (corridorWidthIterator <= Left + Width - 1 && !flag) {
                            areas = Owner.IsAreaExistsAtPoint(corridorWidthIterator, corridorLengthIterator, this);
                            flag = (areas != null);
                            corridorWidthIterator++;
                        }
                        corridorLengthIterator += directionFactor;
                    }

                    if (flag) {
                        if (ParentMark.Direction == Directions.DtNorth) {
                            SetDimension(Left, corridorLengthIterator - directionFactor, Width, corridorLengthIterator - directionFactor + Height - 1);
                        } else {
                            SetDimension(Left, Top, Width, corridorLengthIterator - directionFactor - Top + 1);
                        }

                        if (areas != null) {
                            fForceMarkAtEnd = false;
                            corridorLengthIterator = 0;
                            while (corridorLengthIterator < areas.Count && !fForceMarkAtEnd) {
                                DungeonArea curArea = areas[corridorLengthIterator];

                                int corridorWidthIterator = 0;
                                while (corridorWidthIterator < curArea.MarksList.Count && !fForceMarkAtEnd) {
                                    DungeonMark curMark = curArea.MarksList[corridorWidthIterator];
                                    fForceMarkAtEnd = (MathHelper.IsValueBetween(curMark.Location.X, Left, Left + Width - 1, false));
                                    corridorWidthIterator++;
                                }

                                corridorLengthIterator++;
                            }

                            if (!fForceMarkAtEnd) {
                                fForceMarkAtEnd = true;
                                if (ParentMark.Direction == Directions.DtNorth) {
                                    rockWashingStart = Top;
                                } else {
                                    rockWashingStart = Top + Height - 1;
                                }

                                fForcedMark = new DungeonMark(this, RandomHelper.GetBoundedRnd(Left + 1, Left + Width - 2), rockWashingStart, ParentMark.Direction);
                                try {
                                    fForcedMark.AtCorridorEnd = true;
                                } catch (Exception ex) {
                                    Logger.Write("LinearCorridor.buildArea.2(): " + ex.Message);
                                }
                            }
                        }
                    }
                }
                result = true;
            }
            return result;
        }

        protected override void SetDimension(int left, int top, int width, int height)
        {
            if (ParentMark != null) {
                int direction = ParentMark.Direction;

                if (direction == Directions.DtNorth || direction == Directions.DtSouth) {
                    width = Math.Min(width, Owner.CorridorWidthLimit);
                } else if (direction == Directions.DtWest || direction == Directions.DtEast) {
                    height = Math.Min(height, Owner.CorridorWidthLimit);
                }
            }

            base.SetDimension(left, top, width, height);
        }

        public override void GenerateMarksList()
        {
            try {
                if (ParentMark != null) {
                    bool placeMarkAtOppositeSide = !fForceMarkAtEnd && RandomHelper.GetBoundedRnd(0, 1) > 0;
                    int direction = ParentMark.Direction;
                    int markX = 0;
                    int markY = 0;

                    Directions allowedDirection = new Directions();
                    switch (direction) {
                        case Directions.DtNorth:
                            if (placeMarkAtOppositeSide) {
                                markX = Left + RandomHelper.GetBoundedRnd(1, Width - 2);
                                markY = Top;
                            }
                            allowedDirection.Include(Directions.DtWest, Directions.DtEast);
                            break;
                        case Directions.DtSouth:
                            if (placeMarkAtOppositeSide) {
                                markX = Left + RandomHelper.GetBoundedRnd(1, Width - 2);
                                markY = Top + Height - 1;
                            }
                            allowedDirection.Include(Directions.DtWest, Directions.DtEast);
                            break;
                        case Directions.DtWest:
                            if (placeMarkAtOppositeSide) {
                                markX = Left;
                                markY = Top + RandomHelper.GetBoundedRnd(1, Height - 2);
                            }
                            allowedDirection.Include(Directions.DtNorth, Directions.DtSouth);
                            break;
                        case Directions.DtEast:
                            if (placeMarkAtOppositeSide) {
                                markX = Left + Width - 1;
                                markY = Top + RandomHelper.GetBoundedRnd(1, Height - 2);
                            }
                            allowedDirection.Include(Directions.DtNorth, Directions.DtSouth);
                            break;
                    }

                    if (placeMarkAtOppositeSide) {
                        DungeonMark mark = new DungeonMark(this, markX, markY, ParentMark.Direction);
                        MarksList.Add(mark);
                        mark.AtCorridorEnd = true;
                    } else {
                        if (fForcedMark != null) {
                            MarksList.Add(fForcedMark);
                        }
                    }
                    int marksCount = RandomHelper.GetBoundedRnd(1, (int)Owner.LinearCorridorMarksLimit);
                    int failedTries = 0;
                    if (marksCount > 0) {
                        while (true) {
                            int markDirection = (RandomHelper.GetBoundedRnd(Directions.DtNorth, Directions.DtEast));
                            if (allowedDirection.Contains(markDirection)) {
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

                                if (failedTries > (int)Owner.RightMarkSearchLimit) {
                                    marksCount--;
                                }

                                if (marksCount <= 0) {
                                    break;
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.Write("LinearCorridor.generateMarksList(): " + ex.Message);
            }
        }
    }
}
