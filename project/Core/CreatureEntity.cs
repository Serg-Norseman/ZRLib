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

using ZRLib.Core.Brain;
using ZRLib.Map;

namespace ZRLib.Core
{
    public class CreatureEntity : LocatedEntity
    {
        protected BrainEntity fBrain;

        public CreatureEntity(GameSpace space, object owner)
            : base(space, owner)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (fBrain != null) {
                    fBrain.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public BrainEntity Brain
        {
            get {
                return fBrain;
            }
            set {
                if (fBrain != null) {
                    fBrain.Dispose();
                }
                fBrain = value;
            }
        }


        public virtual bool CanMove(IMap map, int aX, int aY)
        {
            return true;
        }

        public virtual bool IsSeen(int aX, int aY, bool checkLOS)
        {
            return true;
        }

        public virtual void MoveTo(int newX, int newY)
        {
            SetPos(newX, newY);
        }
    }
}
