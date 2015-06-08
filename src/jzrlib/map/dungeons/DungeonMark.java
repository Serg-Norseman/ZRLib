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

import jzrlib.core.BaseObject;
import jzrlib.core.Point;
import jzrlib.utils.Logger;

/**
 * 
 * @author Ruslan N. Garipov (aka Brigadir), 2003
 * @author Serg V. Zhdanovskih (aka Alchemist), 2003-2008, 2014
 */
public final class DungeonMark extends BaseObject
{
    // Marker State
    public static final byte ms_Undefined = 0;
    public static final byte ms_AreaGenerator = 1;
    public static final byte ms_RetriesExhaust = 2;
    public static final byte ms_PointToOtherArea = 3;

    private byte fState = DungeonMark.ms_Undefined;

    public boolean AtCorridorEnd;
    public int Direction;
    public IDungeonAreaCreateProc ForcedAreaCreateProc;
    public Point Location;
    public final DungeonArea ParentArea;
    public int RetriesLeft;

    /**
     *
     * @return see {@link DungeonMark} Marker states
     */
    public final byte getState()
    {
        byte result;
        if (this.fState == DungeonMark.ms_RetriesExhaust) {
            if (this.RetriesLeft > 0) {
                result = DungeonMark.ms_Undefined;
            } else {
                result = DungeonMark.ms_RetriesExhaust;
            }
        } else {
            result = this.fState;
        }
        return result;
    }

    /**
     *
     * @param value see {@link DungeonMark} Marker states
     */
    public final void setState(byte value)
    {
        if (this.fState != value) {
            if (value == DungeonMark.ms_RetriesExhaust) {
                if (this.RetriesLeft > 0) {
                    this.fState = DungeonMark.ms_Undefined;
                } else {
                    this.fState = DungeonMark.ms_RetriesExhaust;
                }
            } else {
                this.fState = value;
            }
        }
    }

    public DungeonMark(DungeonArea parentArea, int aX, int aY, int direction)
    {
        this.ParentArea = parentArea;
        this.Location = new Point(aX, aY);
        this.Direction = direction;
        this.ForcedAreaCreateProc = null;
        this.AtCorridorEnd = false;
        this.fState = DungeonMark.ms_Undefined;

        if (this.ParentArea != null) {
            this.RetriesLeft = this.ParentArea.Owner.MarkRetriesLimit;
        } else {
            this.RetriesLeft = DungeonBuilder.DefaultMarkRetriesLimit;
        }
    }

    public final boolean isPointsToOtherArea()
    {
        try {
            if (this.ParentArea != null && this.ParentArea.Owner != null) {
                return this.ParentArea.Owner.isPointsToOtherArea(this.Location.X, this.Location.Y, this.ParentArea);
            }
        } catch (Exception ex) {
            Logger.write("DungeonMark.isPointsToOtherArea(): " + ex.getMessage());
        }
        return false;
    }
}
