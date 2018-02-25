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

using BSLib;
using ZRLib.Core;
using ZRLib.Map;

namespace PrimevalRL.Maps
{
    public sealed class Street : CityRegion
    {
        private static int LastStreetNum = 0;

        private int fLastHouseNum;
        public readonly int Num;

        public Street(GameSpace space, object owner, IMap map, ExtRect area)
            : base(space, owner, map, area)
        {
            Num = ++LastStreetNum;
            fLastHouseNum = 0;
        }

        public int NextHouseNumber
        {
            get { return ++fLastHouseNum; }
        }

        public static void ResetNum()
        {
            LastStreetNum = 0;
        }
    }
}
