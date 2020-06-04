/*
 *  "MysteriesRL", roguelike game.
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

namespace MysteriesRL.Maps
{
    public enum DistrictType
    {
        Default,
        Park,
        Square,
        Graveyard
    }

    public enum Prosperity
    {
        Poor,
        Good,
        Lux
    }

    public sealed class District : CityRegion
    {
        private static int LastNum = 0;

        public readonly int Num;

        private DistrictType fType;
        private Prosperity fProsperity;

        public DistrictType Type
        {
            get { return fType; }
            set { fType = value; }
        }

        public Prosperity Prosperity
        {
            get { return fProsperity; }
            set { fProsperity = value; }
        }


        public District(GameSpace space, object owner, IMap map, ExtRect area)
            : base(space, owner, map, area)
        {
            fType = DistrictType.Default;
            fProsperity = Prosperity.Good;
            Num = ++LastNum;
        }

        public static void ResetNum()
        {
            LastNum = 0;
        }
    }
}
