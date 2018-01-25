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

using BSLib;

namespace ZRLib.Map.Dungeons
{
    public sealed class CryptRoom : CustomStaticArea
    {
        private static readonly string[] CryptArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new CryptRoom(owner, parentMark);
        }

        public CryptRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(19, 19);
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return CryptArea[aY][aX];
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            ExtRect other = anArea.DimensionRect;
            return other.IsIntersect(ExtRect.Create(Left + 6, Top, Left + 12, Top + 2)) || other.IsIntersect(ExtRect.Create(Left + 3, Top + 3, Left + 15, Top + 5)) || other.IsIntersect(ExtRect.Create(Left + 0, Top + 6, Left + 18, Top + 12)) || other.IsIntersect(ExtRect.Create(Left + 3, Top + 13, Left + 15, Top + 15)) || other.IsIntersect(ExtRect.Create(Left + 6, Top + 16, Left + 12, Top + 18));
        }

        static CryptRoom()
        {
            CryptArea = new string[] {
                "      XXXNXXX      ",
                "      X.....X      ",
                "      X.....X      ",
                "   XXXX.....XXXX   ",
                "   X..XX...XX..X   ",
                "   X...XX.XX...X   ",
                "XXXXX...X.X...XXXXX",
                "X...XX.......XX...X",
                "X....XX.....XX....X",
                "W.................E",
                "X....XX.....XX....X",
                "X...XX.......XX...X",
                "XXXXX...X.X...XXXXX",
                "   X...XX.XX...X   ",
                "   X..XX...XX..X   ",
                "   XXXX.....XXXX   ",
                "      X.....X      ",
                "      X.....X      ",
                "      XXXSXXX      "
            };
        }
    }
}
