/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
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
import jzrlib.core.Directions;
import jzrlib.map.TileType;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.Logger;

/**
 * 
 * @author Serg V. Zhdanovskih (aka Alchemist), 2005, 2014-2015
 */
public abstract class CustomStaticArea extends StaticArea
{
    private static final class CustomMark
    {
        public int x;
        public int y;
        public int dir;

        public CustomMark(int x, int y, int dir)
        {
            this.x = x;
            this.y = y;
            this.dir = dir;
        }
    }

    private static final String DoorSym = "NSWE";
    private static final String CustomSym = "X.NSWE";

    private ArrayList<CustomMark> fAvailMarks;
    protected int fDevourArea;

    public CustomStaticArea(DungeonBuilder owner, DungeonMark parentMark)
    {
        super(owner, parentMark);
        this.fAvailMarks = new ArrayList<>();
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.fAvailMarks.clear();
            this.fAvailMarks = null;
        }
        super.dispose(disposing);
    }

    private void addDoor(ArrayList<CustomMark> doorsList, int aX, int aY, char aSym)
    {
        int dir;

        switch (aSym) {
            case 'E':
                dir = Directions.dtEast;
                break;
            case 'N':
                dir = Directions.dtNorth;
                break;
            case 'S':
                dir = Directions.dtSouth;
                break;
            case 'W':
                dir = Directions.dtWest;
                break;
            default:
                return;
        }

        doorsList.add(new CustomMark(aX, aY, dir));
    }

    @Override
    protected boolean buildArea()
    {
        boolean result = false;

        try {
            this.fAvailMarks.clear();
            this.fDevourArea = 0;

            ArrayList<CustomMark> doors = new ArrayList<>();

            for (int y = 0; y < super.Height; y++) {
                for (int x = 0; x < super.Width; x++) {
                    char sym = this.getCustomCell(x, y);
                    if (sym == '.') {
                        this.fDevourArea++;
                        super.fArea[x][y] = TileType.ttUndefined;
                    } else {
                        if (sym == 'E' || sym == 'N' || sym == 'S' || sym == 'W' || sym == 'X') {
                            this.fDevourArea++;
                            super.fArea[x][y] = TileType.ttDungeonWall;

                            if (CustomStaticArea.DoorSym.indexOf(sym) >= 0) {
                                this.addDoor(fAvailMarks, x, y, sym);
                                if (sym == Directions.Data[super.ParentMark.Direction].symOpposite) {
                                    this.addDoor(doors, x, y, sym);
                                }
                            }
                        }
                    }
                }
            }

            CustomMark door = AuxUtils.getRandomItem(doors);
            if (door != null) {
                super.setPosition(super.ParentMark.Location.X - door.x, super.ParentMark.Location.Y - door.y);
                result = true;
            }
        } catch (Exception ex) {
            Logger.write("CustomStaticArea.buildArea(): " + ex.getMessage());
            throw ex;
        }

        return result;
    }

    protected abstract char getCustomCell(int aX, int aY);

    @Override
    public void generateMarksList()
    {
        try {
            for (CustomMark mark : this.fAvailMarks) {
                super.tryInsertMark(this.Left + mark.x, this.Top + mark.y, mark.dir);
            }
        } catch (Exception ex) {
            Logger.write("CustomStaticArea.generateMarksList(): " + ex.getMessage());
            throw ex;
        }
    }

    @Override
    public int getDevourArea()
    {
        return this.fDevourArea;
    }

    @Override
    public boolean isOwnedPoint(int ptX, int ptY)
    {
        int ax = ptX - this.Left;
        int ay = ptY - this.Top;

        boolean result;
        if (ax < 0 || ax >= super.Width || ay < 0 || ay >= super.Height) {
            result = false;
        } else {
            char sym = this.getCustomCell(ax, ay);
            result = CustomStaticArea.CustomSym.indexOf(sym) >= 0;
        }
        return result;
    }
}
