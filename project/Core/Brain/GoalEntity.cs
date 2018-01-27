/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih.
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

using BSLib;

namespace ZRLib.Core.Brain
{
    public abstract class GoalEntity : BaseObject
    {
        public const int PersistentGoal = AuxUtils.MaxInt;

        protected readonly BrainEntity fBrain;

        public int Duration;
        public int EmitterID;
        public bool IsComplete;
        public int Kind;
        public int SourceID;
        public float Value;

        protected GoalEntity(BrainEntity owner)
        {
            fBrain = owner;

            Duration = PersistentGoal;
            EmitterID = 0;
            IsComplete = false;
            Kind = -1;
            SourceID = -1;
            Value = 0f;
        }

        public BrainEntity Brain
        {
            get { return fBrain; }
        }

        public CreatureEntity Self
        {
            get { return fBrain.Self; }
        }

        public abstract void Execute();
    }
}
