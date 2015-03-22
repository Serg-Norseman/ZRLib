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
package mrl.items;

import java.awt.Color;
import java.util.HashMap;
import mrl.core.EquipmentType;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public enum ItemType
{
    IT_COIN(1, '$', Color.yellow, 0.01f, true, EquipmentType.ET_POCKET, 0.00f),
    IT_SHIRT(2, 'x', Color.white, 1f, false, EquipmentType.ET_SHIRT, 10.0f),
    IT_PANTS(3, 'x', Color.white, 2f, false, EquipmentType.ET_PANTS, 10.0f),
    IT_SKIRT(4, 'x', Color.white, 2f, false, EquipmentType.ET_PANTS, 10.0f),
    IT_BOOTS(5, 'x', Color.white, 2f, false, EquipmentType.ET_BOOTS, 0.00f),
    IT_KNIFE(6, '(', Color.red, 2f, false, EquipmentType.ET_RHAND, 0.00f);

    public final int Value;
    public final char Sym;
    public final Color SymColor;
    public final float Weight;
    public final boolean IsStacked;
    public final EquipmentType Equipment;
    public final float PocketCapacity;

    private ItemType(int value, char sym, Color symColor, float weight, boolean isStacked, EquipmentType equipment, float pocketCapacity)
    {
        this.Value = value;
        this.Sym = sym;
        this.SymColor = symColor;
        this.Weight = weight;
        this.IsStacked = isStacked;
        this.Equipment = equipment;
        this.PocketCapacity = pocketCapacity;
        getMappings().put(value, this);
    }

    private static HashMap<Integer, ItemType> mappings;

    private static HashMap<Integer, ItemType> getMappings()
    {
        synchronized (ItemType.class) {
            if (mappings == null) {
                mappings = new HashMap<>();
            }
        }
        return mappings;
    }

    public static ItemType forValue(int value)
    {
        return getMappings().get(value);
    }
}
