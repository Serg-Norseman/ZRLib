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

import jzrlib.utils.AuxUtils;
import jzrlib.utils.Logger;
import jzrlib.core.Directions;
import jzrlib.map.TileType;

/**
 * 
 * @author Ruslan N. Garipov (aka Brigadir), 2003
 * @author Serg V. Zhdanovskih (aka Alchemist), 2003-2008, 2014
 */
public final class QuakeIIArena extends StaticArea
{
    private static final String[] qaArea;

    public static DungeonArea createArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        return new QuakeIIArena(owner, parentMark);
    }

    public QuakeIIArena(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        super.setDimension(21, 21);
    }

    @Override
    protected boolean buildArea()
    {
        boolean result = false;

        for (int y = 0; y < 21; y++) {
            for (int x = 0; x < 21; x++) {
                char c = QuakeIIArena.qaArea[y].charAt(x);
                if (c != '.') {
                    if (c == 'X') {
                        super.fArea[x][y] = TileType.ttDungeonWall;
                    }
                } else {
                    super.fArea[x][y] = TileType.ttUndefined;
                }
            }
        }

        switch (super.ParentMark.Direction) {
            case Directions.dtNorth:
                super.setPosition(super.ParentMark.Location.X - AuxUtils.getBoundedRnd(1, this.Width - 2), super.ParentMark.Location.Y - 20);
                result = AuxUtils.isValueBetween(super.ParentMark.Location.X, this.Left, this.Left + this.Width - 1, false);
                break;
            case Directions.dtSouth:
                super.setPosition(super.ParentMark.Location.X - AuxUtils.getBoundedRnd(1, this.Width - 2), super.ParentMark.Location.Y);
                result = AuxUtils.isValueBetween(super.ParentMark.Location.X, this.Left, this.Left + this.Width - 1, false);
                break;
            case Directions.dtWest:
                super.setPosition(super.ParentMark.Location.X - 20, super.ParentMark.Location.Y - AuxUtils.getBoundedRnd(1, this.Height - 2));
                result = AuxUtils.isValueBetween(super.ParentMark.Location.Y, this.Top, this.Top + this.Height - 1, false);
                break;
            case Directions.dtEast:
                super.setPosition(super.ParentMark.Location.X, super.ParentMark.Location.Y - AuxUtils.getBoundedRnd(1, this.Height - 2));
                result = AuxUtils.isValueBetween(super.ParentMark.Location.Y, this.Top, this.Top + this.Height - 1, false);
                break;
        }
        
        return result;
    }

    @Override
    public void generateMarksList()
    {
        try {
            int marksCount = AuxUtils.getBoundedRnd(1, (int) super.Owner.RectRoomMarksLimit);
            int failedTries = 0;
            if (marksCount > 0) {
                do {
                    int markDirection = AuxUtils.getBoundedRnd(Directions.dtNorth, Directions.dtEast);
                    int tDirectionType = markDirection;
                    int markX = 0;
                    int markY = 0;

                    switch (tDirectionType) {
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
                } while (marksCount > 0);
            }
        } catch (Exception ex) {
            Logger.write("QuakeIIArena.generateMarksList(): " + ex.getMessage());
        }
    }

    @Override
    public boolean isAllowedPointAsMark(int ptX, int ptY)
    {
        return (AuxUtils.isValueBetween(ptX, this.Left, this.Left + this.Width - 1, false) && this.Top == ptY) || 
                (AuxUtils.isValueBetween(ptX, this.Left, this.Left + this.Width - 1, false) && this.Top + this.Height - 1 == ptY) || 
                (this.Left == ptX && AuxUtils.isValueBetween(ptY, this.Top, this.Top + this.Height - 1, false)) || 
                (this.Left + this.Width - 1 == ptX && AuxUtils.isValueBetween(ptY, this.Top, this.Top + this.Height - 1, false));
    }

    @Override
    public int getDevourArea()
    {
        return this.Width * this.Height;
    }

    @Override
    public boolean isOwnedPoint(int ptX, int ptY)
    {
        char ch = QuakeIIArena.qaArea[ptY - this.Top].charAt(ptX - this.Left - 1);
        return ("X+.").indexOf(ch) >= 0;
    }

    static {
        qaArea = new String[]{
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
            "XXXXXXXXXXXXXXXXXXXXX"};
    }
}
