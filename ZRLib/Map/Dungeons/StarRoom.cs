/*
 *  "NorseWorld: Ragnarok", a roguelike game for PCs.
 *  Copyright (C) 2002-2008, 2014 by Serg V. Zhdanovskih.
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
    public sealed class StarRoom : CustomStaticArea
    {
        private static readonly string[] SrArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new StarRoom(owner, parentMark);
        }

        public StarRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(23, 23);
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return SrArea[aY][aX];
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            ExtRect other = anArea.DimensionRect;
            return other.IsIntersect(ExtRect.Create(Left + 6, Top, Left + 16, Top + 5)) || other.IsIntersect(ExtRect.Create(Left + 0, Top + 6, Left + 22, Top + 16)) || other.IsIntersect(ExtRect.Create(Left + 6, Top + 17, Left + 16, Top + 22));
        }

        static StarRoom()
        {
            SrArea = new string[] {
                "      XXXNX            ",
                "      W...XX           ",
                "      XXX..XX          ",
                "        XX..X          ",
                "         XX.X          ",
                "          X.X          ",
                "         XX.XX      XNX",
                "       XXX...XXX    X.X",
                "       X.......X   XX.X",
                "      XX.......XX XX..E",
                "  XXXXX.........XXX..XX",
                " XX.................XX ",
                "XX..XXX.........XXXXX  ",
                "W..XX XX.......XX      ",
                "X.XX   X.......X       ",
                "X.X    XXX...XXX       ",
                "XSX      XX.XX         ",
                "          X.X          ",
                "          X.XX         ",
                "          X..XX        ",
                "          XX..XXX      ",
                "           XX...E      ",
                "            XSXXX      "
            };
        }
    }
}
