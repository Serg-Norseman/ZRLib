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

using System.Collections.Generic;
using BSLib;
using MysteriesRL.Maps.Buildings;
using ZRLib.Core;
using ZRLib.Map;

namespace MysteriesRL.Maps
{
    public sealed class City : AreaEntity
    {
        private readonly List<Building> fBuildings;
        private readonly List<District> fDistricts;
        private readonly IMap fMap;
        private string fName;
        private readonly List<Street> fStreets;


        public List<Building> Buildings
        {
            get { return fBuildings; }
        }

        public List<District> Districts
        {
            get { return fDistricts; }
        }

        public override string Name
        {
            get { return fName; }
        }

        public List<Street> Streets
        {
            get { return fStreets; }
        }


        public City(GameSpace space, IMap map, ExtRect area)
            : base(space, null)
        {
            fMap = map;
            Area = area;

            fDistricts = new List<District>();
            fStreets = new List<Street>();
            fBuildings = new List<Building>();

            fName = "Default-City";
        }

        public District AddDistrict(ExtRect area)
        {
            District res = new District(fSpace, this, fMap, area);
            fDistricts.Add(res);
            return res;
        }

        public Street AddStreet(ExtRect area)
        {
            Street res = new Street(fSpace, this, fMap, area);
            fStreets.Add(res);
            return res;
        }

        public Building AddBuilding(ExtRect privathandArea, ExtRect area, Prosperity prosperity)
        {
            Building res = new Building(fSpace, this, fMap, privathandArea, area, prosperity);
            fBuildings.Add(res);
            return res;
        }

        public CityRegion FindRegion(int x, int y)
        {
            foreach (District distr in fDistricts) {
                if (distr.Area.Contains(x, y)) {
                    return distr;
                }
            }

            foreach (Street street in fStreets) {
                if (street.Area.Contains(x, y)) {
                    return street;
                }
            }

            return null;
        }

        public Building FindBuilding(int x, int y)
        {
            foreach (Building bld in fBuildings) {
                if (bld.Area.Contains(x, y)) {
                    return bld;
                }
            }

            return null;
        }
    }
}
