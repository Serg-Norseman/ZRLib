/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015, 2020 by Serg V. Zhdanovskih.
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

namespace ZRLib.Core.Brain
{
    public abstract class BrainEntity : BaseObject
    {
        protected CreatureEntity fSelf;
        protected List<GoalEntity> fGoals;

        public BytesSet EmittersX;

        public CreatureEntity Self
        {
            get {
                return fSelf;
            }
        }

        public int GoalsCount
        {
            get {
                return fGoals.Count;
            }
        }

        protected virtual EmitterList Emitters
        {
            get {
                // dummy
                return null;
            }
        }


        protected BrainEntity(CreatureEntity owner)
        {
            fSelf = owner;
            fGoals = new List<GoalEntity>();
            EmittersX = new BytesSet();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                ClearGoals();

                fGoals.Clear();
                fGoals = null;
            }
            base.Dispose(disposing);
        }

        public virtual ExtPoint GetEvadePos(CreatureEntity enemy)
        {
            // dummy
            return ExtPoint.Empty;
        }

        public virtual void Attack(CreatureEntity enemy, bool onlyRemote)
        {
            // dummy
        }

        public virtual void StepTo(int aX, int aY)
        {
            // dummy
        }

        public GoalEntity GetGoal(int index)
        {
            return fGoals[index];
        }

        protected virtual void EvaluateGoal(GoalEntity goal)
        {
            // dummy
        }

        protected virtual void PrepareEmitter(Emitter emitter)
        {
            // dummy
        }

        protected virtual void PrepareGoals()
        {
            // dummy
        }

        public void ClearGoals()
        {
            foreach (GoalEntity goal in fGoals) {
                goal.Dispose();
            }
            fGoals.Clear();
        }

        public GoalEntity DefineGoal(int goalKind)
        {
            GoalEntity goal = FindGoalByKind(goalKind);

            if (goal == null) {
                goal = CreateGoal(goalKind);
            }

            return goal;
        }

        public GoalEntity FindGoalByKind(int kind)
        {
            foreach (GoalEntity goal in fGoals) {
                if (goal.Kind == kind) {
                    return goal;
                }
            }
            return null;
        }

        public virtual bool IsAwareOfEmitter(Emitter emitter)
        {
            // dummy
            return false;
        }

        protected abstract GoalEntity CreateGoalEx(int goalKind);

        public GoalEntity CreateGoal(int goalKind)
        {
            GoalEntity result = CreateGoalEx(goalKind);
            if (result != null) {
                result.Kind = goalKind;
                fGoals.Add(result);
            }
            return result;
        }

        public void ReleaseGoal(GoalEntity goal)
        {
            fGoals.Remove(goal);
            goal.Dispose();
        }

        private int FindGoalByEmitter(int emitterID)
        {
            int num = fGoals.Count;
            for (int i = 0; i < num; i++) {
                GoalEntity goal = fGoals[i];
                if (goal.EmitterID == emitterID) {
                    return i;
                }
            }
            return -1;
        }

        public void Think()
        {
            try {
                try {
                    PrepareGoals();
                } catch (Exception ex) {
                    Logger.Write("BrainEntity.think0(" + GetType().Name + "): " + ex.Message);
                }

                try {
                    EmitterList curEmitters = Emitters;
                    if (curEmitters != null) {
                        int num = curEmitters.Count;
                        for (int i = 0; i < num; i++) {
                            Emitter emit = curEmitters[i];
                            bool check = EmittersX.Contains(emit.EmitterKind) && (double)MathHelper.Distance(fSelf.Location, emit.Position) <= (double)emit.Radius;
                            if (check) {
                                int j = FindGoalByEmitter(emit.UID);
                                if (j < 0 && IsAwareOfEmitter(emit)) {
                                    PrepareEmitter(emit);
                                }
                            }
                        }
                    }
                } catch (Exception ex) {
                    Logger.Write("BrainEntity.think1(" + GetType().Name + "): " + ex.Message);
                }

                GoalEntity highestGoal = null;

                try {
                    float highestValue = -1f;
                    int i = 0;
                    while (i < fGoals.Count) {
                        GoalEntity goal = fGoals[i];
                        if (goal.Duration != GoalEntity.PersistentGoal) {
                            goal.Duration--;
                            if (goal.Duration <= 0) {
                                ReleaseGoal(goal);
                                continue;
                            }
                        }

                        EvaluateGoal(goal);

                        if (goal.Value > highestValue) {
                            highestValue = goal.Value;
                            highestGoal = goal;
                        }

                        i++;
                    }
                } catch (Exception ex) {
                    Logger.Write("BrainEntity.think().evaluateGoals(" + GetType().Name + "): " + ex.Message);
                }

                try {
                    if (highestGoal != null) {
                        highestGoal.Execute();

                        if (highestGoal.IsComplete) {
                            ReleaseGoal(highestGoal);
                        }
                    }
                } catch (Exception ex) {
                    Logger.Write("BrainEntity.think().executeHighestGoal(" + GetType().Name + "): " + ex.Message);
                }
            } catch (Exception ex) {
                Logger.Write("BrainEntity.think(" + GetType().Name + "): " + ex.Message);
            }
        }
    }
}
