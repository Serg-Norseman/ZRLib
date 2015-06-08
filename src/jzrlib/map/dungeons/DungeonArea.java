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

import java.util.ArrayList;
import java.util.List;
import jzrlib.core.BaseObject;
import jzrlib.core.Rect;
import jzrlib.map.TileType;
import jzrlib.utils.Logger;

/**
 * 
 * @author Ruslan N. Garipov (aka Brigadir), 2003
 * @author Serg V. Zhdanovskih (aka Alchemist), 2003-2008, 2014-2015
 */
public abstract class DungeonArea extends BaseObject
{
    public final List<DungeonMark> MarksList;
    public final DungeonBuilder Owner;
    public final DungeonMark ParentMark;

    public DungeonArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        this.Owner = owner;
        this.ParentMark = parentMark;
        this.MarksList = new ArrayList<>();
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            //this.MarksList.dispose();
        }
        super.dispose(disposing);
    }

    protected final boolean isAllowedMark(DungeonMark mark)
    {
        boolean result;

        try {
            result = !mark.Location.equals(this.ParentMark.Location);

            if (result) {
                int markIterator = 0;
                while (markIterator < this.MarksList.size() & result) {
                    result = !mark.Location.equals((this.MarksList.get(markIterator)).Location);
                    markIterator++;
                }
            }
        } catch (Exception ex) {
            Logger.write("DungeonArea.isAllowedMark(): " + ex.getMessage());
            result = false;
        }

        return result;
    }

    protected final void flushMarksList()
    {
        try {
            for (DungeonMark mark : this.MarksList) {
                int state = mark.getState();
                TileType markType = TileType.ttUndefinedMark;

                switch (state) {
                    case DungeonMark.ms_Undefined:
                        markType = TileType.ttUndefinedMark;
                        break;
                    case DungeonMark.ms_AreaGenerator:
                        markType = TileType.ttAreaGeneratorMark;
                        break;
                    case DungeonMark.ms_RetriesExhaust:
                        markType = TileType.ttRetriesExhaustMark;
                        break;
                    case DungeonMark.ms_PointToOtherArea:
                        markType = TileType.ttPointToOtherAreaMark;
                        break;
                }

                this.Owner.setTile(mark.Location.X, mark.Location.Y, markType);
            }
        } catch (Exception ex) {
            Logger.write("DungeonArea.flushMarksList(): " + ex.getMessage());
            throw ex;
        }
    }

    protected final void tryInsertMark(int aX, int aY, int direction)
    {
        DungeonMark mark = new DungeonMark(this, aX, aY, direction);
        if (this.isAllowedMark(mark)) {
            this.MarksList.add(mark);
        } else {
            mark.dispose();
        }
    }

    public final boolean tryApplyThisArea()
    {
        try {
            boolean result = this.buildArea();
            boolean kill = false;

            this.ParentMark.RetriesLeft = this.ParentMark.RetriesLeft - 1;

            if (result) {
                kill = (this.ParentMark != null && this.ParentMark.isPointsToOtherArea());

                if (kill) {
                    result = false;
                    this.ParentMark.setState(DungeonMark.ms_PointToOtherArea);
                }
            }

            if (!kill) {
                if (result) {
                    result = this.Owner.isFitAreaDimension(this);
                }

                if (!result) {
                    if (this.ParentMark.RetriesLeft > 0) {
                        result = this.tryApplyThisArea();
                    } else {
                        this.ParentMark.setState(DungeonMark.ms_RetriesExhaust);
                    }
                }

                if (result) {
                    this.ParentMark.setState(DungeonMark.ms_AreaGenerator);
                }
            }

            return result;
        } catch (Exception ex) {
            Logger.write("DungeonArea.tryApplyThisArea(): " + ex.getMessage());
            throw ex;
        }
    }

    protected abstract boolean buildArea();

    protected abstract boolean isWallPoint(int ptX, int ptY);

    public abstract boolean isOwnedPoint(int ptX, int ptY);

    public abstract boolean isIntersectWithArea(DungeonArea area);

    public abstract void flushToMap();

    public abstract void generateMarksList();

    public abstract boolean isAllowedPointAsMark(int ptX, int ptY);

    public abstract int getDevourArea();

    public abstract Rect getDimensionRect();
}
