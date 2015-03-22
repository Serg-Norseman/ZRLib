/*
 *  "MysteriesRL", Java Roguelike game.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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
package mrl.maps.city;

import java.util.ArrayList;
import jzrlib.core.AreaEntity;
import jzrlib.core.GameSpace;
import jzrlib.core.Rect;
import jzrlib.map.IMap;
import mrl.maps.buildings.Building;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class City extends AreaEntity
{
    private final IMap fMap;
    private final ArrayList<District> fDistricts;
    private final ArrayList<Street> fStreets;
    private final ArrayList<Building> fBuildings;

    private String fName;

    public City(GameSpace space, IMap map, Rect area)
    {
        super(space, null);
        
        this.fMap = map;
        this.setArea(area);

        this.fDistricts = new ArrayList<>();
        this.fStreets = new ArrayList<>();
        this.fBuildings = new ArrayList<>();
        
        this.fName = "Default-City";
    }

    public final ArrayList<District> getDistricts()
    {
        return this.fDistricts;
    }

    public final ArrayList<Street> getStreets()
    {
        return this.fStreets;
    }

    public final ArrayList<Building> getBuildings()
    {
        return this.fBuildings;
    }

    @Override
    public final String getName()
    {
        return this.fName;
    }

    public final District addDistrict(Rect area)
    {
        District res = new District(this.fSpace, this, this.fMap, area);
        this.fDistricts.add(res);
        return res;
    }

    public final Street addStreet(Rect area)
    {
        Street res = new Street(this.fSpace, this, this.fMap, area);
        this.fStreets.add(res);
        return res;
    }

    public final Building addBuilding(Rect privathandArea, Rect area, Prosperity prosperity)
    {
        Building res = new Building(this.fSpace, this, this.fMap, privathandArea, area, prosperity);
        this.fBuildings.add(res);
        return res;
    }
    
    public final CityRegion findRegion(int x, int y)
    {
        for (District distr : this.fDistricts) {
            if (distr.getArea().contains(x, y)) {
                return distr;
            }
        }

        for (Street street : this.fStreets) {
            if (street.getArea().contains(x, y)) {
                return street;
            }
        }

        return null;
    }
    
    public final Building findBuilding(int x, int y)
    {
        for (Building bld : this.fBuildings) {
            if (bld.getArea().contains(x, y)) {
                return bld;
            }
        }

        return null;
    }
}
