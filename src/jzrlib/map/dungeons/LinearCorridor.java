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

import java.util.List;
import jzrlib.core.Directions;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.Logger;

/**
 * 
 * @author Ruslan N. Garipov (aka Brigadir), 2003
 * @author Serg V. Zhdanovskih (aka Alchemist), 2003-2008, 2014
 */
public final class LinearCorridor extends RectangularRoom
{
    private boolean fForceMarkAtEnd;
    private DungeonMark fForcedMark;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new LinearCorridor(owner, parentMark);
    }

    public LinearCorridor(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);

        if (super.ParentMark != null) {
            super.ParentMark.AtCorridorEnd = true;
        }
        this.fForceMarkAtEnd = false;
    }

    @Override
    protected boolean buildArea()
    {
        boolean result = super.buildArea();

        if (result) {
            boolean flag = false;
            List<DungeonArea> areas = null;
            int direction = super.ParentMark.Direction;

            if (direction != Directions.dtNorth && direction != Directions.dtSouth) {
                if (direction == Directions.dtWest || direction == Directions.dtEast) {
                    int directionFactor;
                    int rockWashingStart;
                    int rockWashingLimit;
                    if (super.ParentMark.Direction == Directions.dtWest) {
                        directionFactor = -1;
                        rockWashingStart = this.Left + this.Width - 1 - ((int) super.Owner.CorridorLengthBottomLimit - 1);
                        rockWashingLimit = this.Left;
                    } else {
                        directionFactor = 1;
                        rockWashingStart = this.Left + ((int) super.Owner.CorridorLengthBottomLimit - 1);
                        rockWashingLimit = this.Left + this.Width - 1;
                    }

                    int corridorLengthIterator = rockWashingStart;
                    while (corridorLengthIterator < rockWashingLimit && !flag) {
                        int corridorWidthIterator = this.Top;
                        while (corridorWidthIterator <= this.Top + this.Height - 1 && !flag) {
                            areas = this.Owner.isAreaExistsAtPoint(corridorLengthIterator, corridorWidthIterator, this);
                            flag = (areas != null);
                            corridorWidthIterator++;
                        }
                        corridorLengthIterator += directionFactor;
                    }

                    if (flag) {
                        if (super.ParentMark.Direction == Directions.dtWest) {
                            this.setDimension(corridorLengthIterator - directionFactor, this.Top, corridorLengthIterator - directionFactor + this.Width - 1, this.Height);
                        } else {
                            this.setDimension(this.Left, this.Top, corridorLengthIterator - directionFactor - this.Left + 1, this.Height);
                        }

                        if (areas != null) {
                            this.fForceMarkAtEnd = false;
                            corridorLengthIterator = 0;
                            while (corridorLengthIterator < areas.size() && !this.fForceMarkAtEnd) {
                                DungeonArea curArea = areas.get(corridorLengthIterator);

                                int corridorWidthIterator = 0;
                                while (corridorWidthIterator < curArea.MarksList.size() && !this.fForceMarkAtEnd) {
                                    DungeonMark curMark = curArea.MarksList.get(corridorWidthIterator);
                                    this.fForceMarkAtEnd = (AuxUtils.isValueBetween(curMark.Location.Y, this.Top, this.Top + this.Height - 1, false));
                                    corridorWidthIterator++;
                                }

                                corridorLengthIterator++;
                            }

                            if (!this.fForceMarkAtEnd) {
                                this.fForceMarkAtEnd = true;
                                if (super.ParentMark.Direction == Directions.dtWest) {
                                    rockWashingStart = this.Left;
                                } else {
                                    rockWashingStart = this.Left + this.Width - 1;
                                }

                                this.fForcedMark = new DungeonMark(this, rockWashingStart, AuxUtils.getBoundedRnd(this.Top + 1, this.Top + this.Height - 2), super.ParentMark.Direction);
                                try {
                                    this.fForcedMark.AtCorridorEnd = true;
                                } catch (Exception ex) {
                                    Logger.write("LinearCorridor.buildArea.1(): " + ex.getMessage());
                                }
                            }
                        }
                    }
                }
            } else {
                int directionFactor;
                int rockWashingStart;
                int rockWashingLimit;

                if (super.ParentMark.Direction == Directions.dtNorth) {
                    directionFactor = -1;
                    rockWashingStart = this.Top + this.Height - 1 - (super.Owner.CorridorLengthBottomLimit - 1);
                    rockWashingLimit = this.Top;
                } else {
                    directionFactor = 1;
                    rockWashingStart = this.Top + (super.Owner.CorridorLengthBottomLimit - 1);
                    rockWashingLimit = this.Top + this.Height - 1;
                }

                int corridorLengthIterator = rockWashingStart;
                while (corridorLengthIterator < rockWashingLimit && !flag) {
                    int corridorWidthIterator = this.Left;
                    while (corridorWidthIterator <= this.Left + this.Width - 1 && !flag) {
                        areas = this.Owner.isAreaExistsAtPoint(corridorWidthIterator, corridorLengthIterator, this);
                        flag = (areas != null);
                        corridorWidthIterator++;
                    }
                    corridorLengthIterator += directionFactor;
                }

                if (flag) {
                    if (super.ParentMark.Direction == Directions.dtNorth) {
                        this.setDimension(this.Left, corridorLengthIterator - directionFactor, this.Width, corridorLengthIterator - directionFactor + this.Height - 1);
                    } else {
                        this.setDimension(this.Left, this.Top, this.Width, corridorLengthIterator - directionFactor - this.Top + 1);
                    }

                    if (areas != null) {
                        this.fForceMarkAtEnd = false;
                        corridorLengthIterator = 0;
                        while (corridorLengthIterator < areas.size() && !this.fForceMarkAtEnd) {
                            DungeonArea curArea = areas.get(corridorLengthIterator);

                            int corridorWidthIterator = 0;
                            while (corridorWidthIterator < curArea.MarksList.size() && !this.fForceMarkAtEnd) {
                                DungeonMark curMark = curArea.MarksList.get(corridorWidthIterator);
                                this.fForceMarkAtEnd = (AuxUtils.isValueBetween(curMark.Location.X, this.Left, this.Left + this.Width - 1, false));
                                corridorWidthIterator++;
                            }

                            corridorLengthIterator++;
                        }

                        if (!this.fForceMarkAtEnd) {
                            this.fForceMarkAtEnd = true;
                            if (super.ParentMark.Direction == Directions.dtNorth) {
                                rockWashingStart = this.Top;
                            } else {
                                rockWashingStart = this.Top + this.Height - 1;
                            }

                            this.fForcedMark = new DungeonMark(this, AuxUtils.getBoundedRnd(this.Left + 1, this.Left + this.Width - 2), rockWashingStart, super.ParentMark.Direction);
                            try {
                                this.fForcedMark.AtCorridorEnd = true;
                            } catch (Exception ex) {
                                Logger.write("LinearCorridor.buildArea.2(): " + ex.getMessage());
                            }
                        }
                    }
                }
            }
            result = true;
        }
        return result;
    }

    @Override
    protected void setDimension(int left, int top, int width, int height)
    {
        if (super.ParentMark != null) {
            int direction = super.ParentMark.Direction;

            if (direction == Directions.dtNorth || direction == Directions.dtSouth) {
                width = Math.min(width, super.Owner.CorridorWidthLimit);
            } else if (direction == Directions.dtWest || direction == Directions.dtEast) {
                height = Math.min(height, super.Owner.CorridorWidthLimit);
            }
        }

        super.setDimension(left, top, width, height);
    }

    @Override
    public void generateMarksList()
    {
        try {
            if (super.ParentMark != null) {
                boolean placeMarkAtOppositeSide = !this.fForceMarkAtEnd && AuxUtils.getBoundedRnd(0, 1) > 0;
                int direction = super.ParentMark.Direction;
                int markX = 0;
                int markY = 0;

                Directions allowedDirection = new Directions();
                switch (direction) {
                    case Directions.dtNorth:
                        if (placeMarkAtOppositeSide) {
                            markX = this.Left + AuxUtils.getBoundedRnd(1, this.Width - 2);
                            markY = this.Top;
                        }
                        allowedDirection.include(Directions.dtWest, Directions.dtEast);
                        break;
                    case Directions.dtSouth:
                        if (placeMarkAtOppositeSide) {
                            markX = this.Left + AuxUtils.getBoundedRnd(1, this.Width - 2);
                            markY = this.Top + this.Height - 1;
                        }
                        allowedDirection.include(Directions.dtWest, Directions.dtEast);
                        break;
                    case Directions.dtWest:
                        if (placeMarkAtOppositeSide) {
                            markX = this.Left;
                            markY = this.Top + AuxUtils.getBoundedRnd(1, this.Height - 2);
                        }
                        allowedDirection.include(Directions.dtNorth, Directions.dtSouth);
                        break;
                    case Directions.dtEast:
                        if (placeMarkAtOppositeSide) {
                            markX = this.Left + this.Width - 1;
                            markY = this.Top + AuxUtils.getBoundedRnd(1, this.Height - 2);
                        }
                        allowedDirection.include(Directions.dtNorth, Directions.dtSouth);
                        break;
                }

                if (placeMarkAtOppositeSide) {
                    DungeonMark mark = new DungeonMark(this, markX, markY, super.ParentMark.Direction);
                    this.MarksList.add(mark);
                    mark.AtCorridorEnd = true;
                } else {
                    if (this.fForcedMark != null) {
                        this.MarksList.add(this.fForcedMark);
                    }
                }
                int marksCount = AuxUtils.getBoundedRnd(1, (int) super.Owner.LinearCorridorMarksLimit);
                int failedTries = 0;
                if (marksCount > 0) {
                    while (true) {
                        int markDirection = (AuxUtils.getBoundedRnd(Directions.dtNorth, Directions.dtEast));
                        if (allowedDirection.contains(markDirection)) {
                            switch (markDirection) {
                                case Directions.dtNorth:
                                    markX = this.Left + AuxUtils.getBoundedRnd(1, this.Width - 2);
                                    markY = this.Top;
                                    break;
                                case Directions.dtSouth:
                                    markX = this.Left + AuxUtils.getBoundedRnd(1, this.Width - 2);
                                    markY = this.Top + this.Height - 1;
                                    break;
                                case Directions.dtWest:
                                    markX = this.Left;
                                    markY = this.Top + AuxUtils.getBoundedRnd(1, this.Height - 2);
                                    break;
                                case Directions.dtEast:
                                    markX = this.Left + this.Width - 1;
                                    markY = this.Top + AuxUtils.getBoundedRnd(1, this.Height - 2);
                                    break;
                            }

                            DungeonMark mark = new DungeonMark(this, markX, markY, markDirection);

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

                            if (marksCount <= 0) {
                                break;
                            }
                        }
                    }
                }
            }
        } catch (Exception ex) {
            Logger.write("LinearCorridor.generateMarksList(): " + ex.getMessage());
        }
    }
}
