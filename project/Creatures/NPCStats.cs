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

using System;

namespace MysteriesRL.Creatures
{
    public class NPCStats
    {
        private const float LB_FACTOR = 0.45f;

        private int fCurXP;
        // Experience Points

        public int Level;

        public int Str;
        // Strength
        public int Per;
        // Perception
        public int End;
        // Endurance
        public int Chr;
        // Charisma
        public int Int;
        // Intelligence
        public int Agi;
        // Agility
        public int Luk;
        // Luck

        public NPCStats()
        {
            Random rand = new Random();

            Level = 1;
            fCurXP = 0;

            Str = rand.Next(7) + 3;
            Per = rand.Next(7) + 3;
            End = rand.Next(7) + 3;
            Chr = rand.Next(7) + 3;
            Int = rand.Next(7) + 3;
            Agi = rand.Next(7) + 3;
            Luk = rand.Next(7) + 3;
        }

        public void InitDefaults()
        {
            Level = 1;
            fCurXP = 0;

            // average human, (c) Fallout
            Str = 5;
            Per = 5;
            End = 5;
            Chr = 5;
            Int = 5;
            Agi = 5;
            Luk = 5;
        }

        public int NextLevelXP
        {
            get { return GetLevelXP(Level + 1); }
        }

        public void AddXP(int value)
        {
            fCurXP += value;

            int curLimit = NextLevelXP;
            if (fCurXP >= curLimit) {
                Level += 1;
                fCurXP -= curLimit;
            }
        }

        public int CurXP
        {
            get {
                return fCurXP;
            }
        }

        public float CarryWeight
        {
            get {
                // (c) Fallout 1, 2
                return (25 + (Str * 25)) * LB_FACTOR;
            }
        }

        public int HPMax
        {
            get {
                //return 20 + 4 * End;
    
                // (c) Fallout 3
                return (90 + End * 20 + Level * 10);
            }
        }

        public int ArmorClass
        {
            get {
                // (c) Fallout 1,2,3
                return Agi;
            }
        }

        public int ActionPoints
        {
            get {
                // (c) Fallout 3
                return (65 + 2 * Agi);
            }
        }

        public int Sequence
        {
            get {
                // (c) Fallout 1,2
                return (2 * Per);
            }
        }

        public int CriticalChance
        {
            get {
                // (c) Fallout 1,2,3
                return (Luk);
            }
        }

        public int HealingRate
        {
            get {
                return (int)((1.0f / 3.0f) * End);
            }
        }

        public static int GetLevelXP(int level)
        {
            // (c) Fallout 3
            return ((level - 1) * ((level - 2) * 75 + 200));
        }
    }
}
