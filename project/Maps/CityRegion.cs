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

using BSLib;
using ZRLib.Core;
using ZRLib.Map;

namespace MysteriesRL.Maps
{
    public abstract class CityRegion : AreaEntity
    {
        protected readonly IMap fMap;

        protected CityRegion(GameSpace space, object owner, IMap map, ExtRect area)
            : base(space, owner)
        {
            fMap = map;
            Area = area;
        }

        public City City
        {
            get { return (City)Owner; }
        }

        public IMap Map
        {
            get { return fMap; }
        }
    }
}
