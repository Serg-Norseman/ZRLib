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

using BSLib;

namespace PrimevalRL.Creatures
{
    public enum EyeColor
    {
        Blue,
        Green,
        Gray,
        Brown
    }

    public enum HairColor
    {
        Black,
        Brown,
        Blonde,
        Red
    }

    public enum Religion
    {
        Atheist,
        Christian,
        Protestant,
        Jewish
    }

    /// <summary>
    /// Personality traits.
    /// </summary>
    public class Personality
    {
        public readonly EyeColor EyeColor;
        public readonly HairColor HairColor;
        public Religion Religion;

        public Personality()
        {
            EyeColor = RandomHelper.GetRandomEnum<EyeColor>(typeof(EyeColor));
            HairColor = RandomHelper.GetRandomEnum<HairColor>(typeof(HairColor));
            Religion = RandomHelper.GetRandomEnum<Religion>(typeof(Religion));
        }
    }
}
