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

using ZRLib.Core;
using ZRLib.Map;

namespace PrimevalRL.Maps.Buildings.Features
{
    public class BuildingFeature : MapFeature
    {
        public BuildingFeature(GameSpace space, object owner)
            : base(space, owner)
        {
        }

        public BuildingFeature(GameSpace space, object owner, int x, int y)
            : base(space, owner, x, y)
        {
        }

        public IMap Map
        {
            get {
                if (Owner is IMap) {
                    return (IMap)Owner;
                } else {
                    Building bld = (Building)Owner;
                    return bld.Map;
                }
            }
        }
    }
}
