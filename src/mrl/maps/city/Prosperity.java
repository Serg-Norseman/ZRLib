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

import java.util.HashMap;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public enum Prosperity
{
    pPoor(0, 5, 10, 10, 15, 1),
    pGood(1, 6, 12, 18, 20, 4),
    pLux(2, 10, 20, 40, 70, 10);

    public final int Value;
    public final int minRoomSize;
    public final int maxRoomSize;
    public final int minHouseSize;
    public final int maxHouseSize;
    public final int buildSpaces;

    private Prosperity(int value, int minRoomSize, int maxRoomSize, int minHouseSize, int maxHouseSize, int buildSpaces)
    {
        this.Value = value;
        this.minRoomSize = minRoomSize;
        this.maxRoomSize = maxRoomSize;
        this.minHouseSize = minHouseSize;
        this.maxHouseSize = maxHouseSize;
        this.buildSpaces = buildSpaces;
        getMappings().put(value, this);
    }

    private static HashMap<Integer, Prosperity> mappings;

    private static HashMap<Integer, Prosperity> getMappings()
    {
        synchronized (Prosperity.class) {
            if (mappings == null) {
                mappings = new HashMap<>();
            }
        }
        return mappings;
    }

    public static Prosperity forValue(int value)
    {
        return getMappings().get(value);
    }
}
