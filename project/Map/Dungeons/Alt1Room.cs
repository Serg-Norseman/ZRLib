/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
 *  Copyright (C) 2002-2008, 2014 by Serg V. Zhdanovskih (aka Alchemist).
 *
 *  This file is part of "NorseWorld: Ragnarok".
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

namespace ZRLib.Map.Dungeons
{
    public sealed class Alt1Room : CustomStaticArea
    {
        private static readonly string[] A1Area;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new Alt1Room(owner, parentMark);
        }

        public Alt1Room(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(9, 7);
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return A1Area[aY][aX];
        }

        static Alt1Room()
        {
            A1Area = new string[] {
                "XXXXNXXXX",
                "XX.....XX",
                "X...X...X",
                "W..XXX..E",
                "X...X...X",
                "XX.....XX",
                "XXXXSXXXX"
            };
        }
    }
}
