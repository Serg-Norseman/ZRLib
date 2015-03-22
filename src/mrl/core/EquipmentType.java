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
package mrl.core;

import java.util.HashMap;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public enum EquipmentType
{
    ET_HEAD(0),
    ET_NECK(1),
    ET_TORSO(2),
    ET_SHIRT(3),
    ET_LHAND(4),
    ET_RHAND(5),
    ET_GLOVES(6),
    ET_PANTS(7),
    ET_BOOTS(8),
    ET_CLOAK(9),
    ET_POCKET(10);

    public final int Value;

    private EquipmentType(int value)
    {
        this.Value = value;
        getMappings().put(value, this);
    }

    private static HashMap<Integer, EquipmentType> mappings;

    private static HashMap<Integer, EquipmentType> getMappings()
    {
        synchronized (EquipmentType.class) {
            if (mappings == null) {
                mappings = new HashMap<>();
            }
        }
        return mappings;
    }

    public static EquipmentType forValue(int value)
    {
        return getMappings().get(value);
    }    
}
