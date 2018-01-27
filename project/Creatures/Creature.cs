/*
 *  "PrimevalRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih.
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

using PrimevalRL.Game;
using PrimevalRL.Maps;
using ZRLib.Core;
using ZRLib.Map;

namespace PrimevalRL.Creatures
{
    public abstract class Creature : CreatureEntity, ITerminalEntity
    {
        public abstract int AppearanceColor { get; }
        public abstract char Appearance { get; }
        protected int fHPCur;

        private bool fIsPlayer;

        protected Creature(GameSpace space, object owner)
            : base(space, owner)
        {
        }

        public new MRLGame Space
        {
            get { return (MRLGame)fSpace; }
        }

        public IMap Map
        {
            get {
                return (IMap)Owner;
            }
            set {
                IMap prevMap = Map;
                if (prevMap != null) {
                    ((Layer)prevMap).Creatures.Remove(this);
                }
    
                Owner = value;
    
                if (value != null) {
                    ((Layer)value).Creatures.Add(this);
                }
            }
        }

        public bool Player
        {
            get {
                return fIsPlayer;
            }
        }

        public bool IsPlayer
        {
            set {
                fIsPlayer = value;
            }
        }

        public virtual void DoTurn()
        {
            // dummy
        }

        public override void MoveTo(int newX, int newY)
        {
            base.MoveTo(newX, newY);
        }

        public int HP
        {
            get {
                return fHPCur;
            }
            set {
                fHPCur = value;
    
                if (fHPCur > HPMax) {
                    fHPCur = HPMax;
                }
            }
        }

        public bool Alive
        {
            get {
                return fHPCur > 0;
            }
        }

        public virtual int HPMax
        {
            get {
                return 0;
            }
        }

        public virtual void TakeDamage(Damage damage)
        {
            fHPCur = fHPCur - damage.Value;

            if (!Alive) {
                Die(damage.Inflictor);
            }
        }

        public virtual void Die(Creature killer)
        {
        }

        public virtual void Kill(Creature victim)
        {
        }

        public virtual int DamageValue
        {
            get {
                return 0;
            }
        }

        public virtual int DamageType
        {
            get {
                return Damage.DMG_GENERIC;
            }
        }

        protected virtual void Attack(Creature enemy)
        {
        }

        public virtual int GetEquipBonus(string bonusId)
        {
            return 0;
        }

        /*
         * This method inflicts damage on given entity,
         * based on current combat mechanics of this entity
         */
        public virtual void InflictDamage(Creature enemy)
        {
            if (enemy == null || enemy == this) {
                return;
            }

            Attack(enemy);

            int type = DamageType;
            int value = DamageValue;

            // non-lethal weapons ignore all damage including stat bonuses
            if (type == Damage.DMG_NONLETHAL) {
                value = 0;
            }

            Damage dmg = new Damage(value, type, this);
            enemy.TakeDamage(dmg);
        }

        public abstract int FovRadius { get; }

        public abstract int HearRadius { get; }
    }
}
