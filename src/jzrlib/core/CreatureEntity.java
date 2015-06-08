/*
 *  "JZRLib", Java Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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
package jzrlib.core;

import jzrlib.core.brain.BrainEntity;
import jzrlib.map.IMap;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class CreatureEntity extends LocatedEntity
{
    protected BrainEntity fBrain;

    public CreatureEntity(GameSpace space, Object owner)
    {
        super(space, owner);
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            if (this.fBrain != null) {
                this.fBrain.dispose();
            }
        }
        super.dispose(disposing);
    }

    public final BrainEntity getBrain()
    {
        return this.fBrain;
    }

    public final void setBrain(BrainEntity value)
    {
        if (this.fBrain != null) {
            this.fBrain.dispose();
        }
        this.fBrain = value;
    }

    public boolean canMove(IMap map, int aX, int aY)
    {
        return true;
    }

    public boolean isSeen(int aX, int aY, boolean checkLOS)
    {
        return true;
    }

    public void moveTo(int newX, int newY)
    {
        this.setPos(newX, newY);
    }
}
