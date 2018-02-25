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

namespace PrimevalRL.Game
{
    public enum CrimeType
    {
        ct_Vandalism = 0,
        // broked a window
        ct_Trespassing = 1,
        // entered a restricted area
        ct_Thievery = 2,
        // stole something
        ct_Assault = 3,
        // hit someone
        ct_Murder = 4
        // killed someone
    }

    public sealed class CrimeRecord
    {
        public readonly CrimeType Type;
        public int Count;

        public CrimeRecord(CrimeType crimeType)
        {
            Type = crimeType;
            Count = 1;
        }

        public void IncCount()
        {
            Count++;
        }
    }
}
