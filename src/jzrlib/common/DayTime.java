/*
 *  "JZRLib", Java Roguelike games development Library.
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
package jzrlib.common;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public enum DayTime
{
    dt_Undefined(0, 0.0f, 0.0f, 127, 1.0f), // 0.5

    dt_NightFH(1, 22.0f, 23.59f, 0.25f, 0.5f), // 0.125
    dt_Midnight(2, 0.0f, 0.0f, 0.125f, 0.5f), // 0.125
    dt_NightSH(3, 0.00f, 2.0f, 0.25f, 0.5f), // 0.125
    dt_Dawn(4, 2.0f, 6.0f, 0.5f, 0.75f), // 0.25
    dt_DayFH(5, 6.0f, 11.00f, 0.80f, 1.0f), // 0.5
    dt_Noon(6, 11.00f, 13.00f, 1.0f, 1.0f), // 0.75
    dt_DaySH(7, 13.00f, 18.0f, 0.80f, 1.0f), // 0.5
    dt_Dusk(8, 18.0f, 22.0f, 0.5f, 0.75f); // 0.25

    public static final int dt_First = 1;
    public static final int dt_Last = 8;
    
    public final int Value;
    public final float bRange;
    public final float eRange;
    public final float Brightness;
    public final float RadMod;

    private DayTime(int value, float bRange, float eRange, float brightness, float radMod)
    {
        this.Value = value;
        this.bRange = bRange;
        this.eRange = eRange;
        this.Brightness = brightness;
        this.RadMod = radMod;
    }

    public int getValue()
    {
        return this.ordinal();
    }

    public static DayTime forValue(int value)
    {
        return values()[value];
    }
}
