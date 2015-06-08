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
package jzrlib.core.brain;

import jzrlib.core.BaseObject;
import jzrlib.core.CreatureEntity;
import jzrlib.utils.AuxUtils;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class GoalEntity extends BaseObject
{
    public static final int PersistentGoal = AuxUtils.MaxInt;

    protected final BrainEntity fBrain;

    public int Duration;
    public int EmitterID;
    public boolean IsComplete;
    public int Kind;
    public int SourceID;
    public float Value;

    public GoalEntity(BrainEntity owner)
    {
        this.fBrain = owner;

        this.Duration = GoalEntity.PersistentGoal;
        this.EmitterID = 0;
        this.IsComplete = false;
        this.Kind = -1;
        this.SourceID = -1;
        this.Value = 0f;
    }

    public final BrainEntity getBrain()
    {
        return this.fBrain;
    }

    public final CreatureEntity getSelf()
    {
        return this.fBrain.fSelf;
    }

    public abstract void execute();
}
