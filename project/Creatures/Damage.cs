/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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

namespace MysteriesRL.Creatures
{
    public class Damage
    {
        public const int DMG_GENERIC = 0;
        public const int DMG_CUT = 1;
        public const int DMG_BLUNT = 2;
        public const int DMG_NONLETHAL = 3;
        public const int DMG_BLOODLOSS = 4;

        public readonly int Type;
        public readonly int Value;

        private Creature fInflictor;

        public Damage(int value)
        {
            Value = value;
            Type = 0;
        }

        public Damage(int value, int damageType)
        {
            Value = value;
            Type = damageType;
        }

        public Damage(int value, int damageType, Creature inflictor)
        {
            Value = value;
            Type = damageType;
            fInflictor = inflictor;
        }

        public Creature Inflictor
        {
            get {
                return fInflictor;
            }
            set {
                fInflictor = value;
            }
        }
    }
}
