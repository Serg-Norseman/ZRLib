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
package mrl.creatures;

import jzrlib.utils.AuxUtils;

/**
 * Personality traits.
 *
 * @author Serg V. Zhdanovskih
 */
public class Personality
{
    public enum EyeColor
    {
        Blue,
        Green,
        Gray,
        Brown;

        public static EyeColor forValue(int value)
        {
            return values()[value];
        }
    }

    public enum HairColor
    {
        Black,
        Brown,
        Blonde,
        Red;
        
        public static HairColor forValue(int value)
        {
            return values()[value];
        }
    }

    public enum Religion
    {
        Atheist,
        Christian,
        Protestant,
        Jewish;
    }

    public final EyeColor eyeColor;
    public final HairColor hairColor;
    public Religion religion;

    public Personality()
    {
        this.eyeColor = AuxUtils.getRandomEnum(EyeColor.class);
        this.hairColor = AuxUtils.getRandomEnum(HairColor.class);
        this.religion = AuxUtils.getRandomEnum(Religion.class);
    }
}
