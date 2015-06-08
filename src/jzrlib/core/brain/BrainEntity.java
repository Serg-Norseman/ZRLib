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

import java.util.ArrayList;
import jzrlib.core.BaseObject;
import jzrlib.core.BytesSet;
import jzrlib.core.CreatureEntity;
import jzrlib.core.Point;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.Logger;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class BrainEntity extends BaseObject
{
    protected CreatureEntity fSelf;
    protected ArrayList<GoalEntity> fGoals;

    public BytesSet Emitters;

    public BrainEntity(CreatureEntity owner)
    {
        this.fSelf = owner;
        this.fGoals = new ArrayList<>();
        this.Emitters = new BytesSet();
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.clearGoals();

            this.fGoals.clear();
            this.fGoals = null;
        }
        super.dispose(disposing);
    }

    public final CreatureEntity getSelf()
    {
        return this.fSelf;
    }

    public Point getEvadePos(CreatureEntity enemy)
    {
        // dummy
        return null;
    }

    public void attack(CreatureEntity enemy, boolean onlyRemote)
    {
        // dummy
    }

    public void stepTo(int aX, int aY)
    {
        // dummy
    }

    public final GoalEntity getGoal(int index)
    {
        return this.fGoals.get(index);
    }

    public final int getGoalsCount()
    {
        return this.fGoals.size();
    }

    protected void evaluateGoal(GoalEntity goal)
    {
        // dummy
    }

    protected EmitterList getEmitters()
    {
        // dummy
        return null;
    }

    protected void prepareEmitter(Emitter emitter)
    {
        // dummy
    }

    protected void prepareGoals()
    {
        // dummy
    }

    public final void clearGoals()
    {
        for (GoalEntity goal : this.fGoals) {
            goal.dispose();
        }
        this.fGoals.clear();
    }

    public final GoalEntity defineGoal(int goalKind)
    {
        GoalEntity goal = this.findGoalByKind(goalKind);

        if (goal == null) {
            goal = this.createGoal(goalKind);
        }

        return goal;
    }

    public final GoalEntity findGoalByKind(int kind)
    {
        for (GoalEntity goal : this.fGoals) {
            if (goal.Kind == kind) {
                return goal;
            }
        }
        return null;
    }

    public boolean isAwareOfEmitter(Emitter emitter)
    {
        // dummy
        return false;
    }

    protected abstract GoalEntity createGoalEx(int goalKind);

    public final GoalEntity createGoal(int goalKind)
    {
        GoalEntity result = this.createGoalEx(goalKind);
        if (result != null) {
            result.Kind = goalKind;
            this.fGoals.add(result);
        }
        return result;
    }

    public final void releaseGoal(GoalEntity goal)
    {
        this.fGoals.remove(goal);
        goal.dispose();
    }

    private int findGoalByEmitter(int emitterID)
    {
        int num = this.fGoals.size();
        for (int i = 0; i < num; i++) {
            GoalEntity goal = this.fGoals.get(i);
            if (goal.EmitterID == emitterID) {
                return i;
            }
        }
        return -1;
    }

    public final void think()
    {
        try {
            try {
                this.prepareGoals();
            } catch (Exception ex) {
                Logger.write("BrainEntity.think0(" + this.getClass().getName() + "): " + ex.getMessage());
            }

            try {
                EmitterList curEmitters = this.getEmitters();
                if (curEmitters != null) {
                    int num = curEmitters.getEmittersCount();
                    for (int i = 0; i < num; i++) {
                        Emitter emit = curEmitters.getEmitter(i);
                        boolean check = this.Emitters.contains(emit.EmitterKind) && (double) AuxUtils.distance(this.fSelf.getLocation(), emit.Position) <= (double) emit.Radius;
                        if (check) {
                            int j = this.findGoalByEmitter(emit.UID);
                            if (j < 0 && this.isAwareOfEmitter(emit)) {
                                this.prepareEmitter(emit);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.write("BrainEntity.think1(" + this.getClass().getName() + "): " + ex.getMessage());
            }

            GoalEntity highestGoal = null;

            try {
                float highestValue = -1f;
                for (GoalEntity goal : this.fGoals) {
                    if (goal.Duration != GoalEntity.PersistentGoal) {
                        goal.Duration--;
                        if (goal.Duration <= 0) {
                            this.releaseGoal(goal);
                            continue;
                        }
                    }

                    this.evaluateGoal(goal);

                    if (goal.Value > highestValue) {
                        highestValue = goal.Value;
                        highestGoal = goal;
                    }
                }
            } catch (Exception ex) {
                Logger.write("BrainEntity.think().evaluateGoals(" + this.getClass().getName() + "): " + ex.getMessage());
            }

            try {
                if (highestGoal != null) {
                    highestGoal.execute();

                    if (highestGoal.IsComplete) {
                        this.releaseGoal(highestGoal);
                    }
                }
            } catch (Exception ex) {
                Logger.write("BrainEntity.think().executeHighestGoal(" + this.getClass().getName() + "): " + ex.getMessage());
            }
        } catch (Exception ex) {
            Logger.write("BrainEntity.think(" + this.getClass().getName() + "): " + ex.getMessage());
        }
    }
}
