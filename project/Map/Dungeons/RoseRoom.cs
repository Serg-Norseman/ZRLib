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
    public sealed class RoseRoom : CustomStaticArea
    {
        private static readonly string[] RrArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new RoseRoom(owner, parentMark);
        }

        public RoseRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(19, 19);
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return RrArea[aY][aX];
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            ExtRect other = anArea.DimensionRect;
            return other.IsIntersect(ExtRect.Create(Left + 5, Top, Left + 13, Top + 4)) || other.IsIntersect(ExtRect.Create(Left + 0, Top + 5, Left + 18, Top + 13)) || other.IsIntersect(ExtRect.Create(Left + 5, Top + 14, Left + 13, Top + 18));
        }

        static RoseRoom()
        {
            RrArea = new string[] {
                "      XXXNXXX      ",
                "     XX.....XX     ",
                "     X.......X     ",
                "     X.......X     ",
                "     XX.....XX     ",
                " XXXX X.....X XXXX ",
                "XX..XXXX...XXXX..XX",
                "X.....XXX.XXX.....X",
                "X......X...X......X",
                "W.................E",
                "X......X...X......X",
                "X.....XXX.XXX.....X",
                "XX..XXXX...XXXX..XX",
                " XXXX X.....X XXXX ",
                "     XX.....XX     ",
                "     X.......X     ",
                "     X.......X     ",
                "     XX.....XX     ",
                "      XXXSXXX      "
            };
        }
    }
}
