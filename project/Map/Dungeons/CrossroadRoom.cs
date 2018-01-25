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
    public sealed class CrossroadRoom : CustomStaticArea
    {
        private static readonly string[] CrArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new CrossroadRoom(owner, parentMark);
        }

        public CrossroadRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(21, 25);
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return CrArea[aY][aX];
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            ExtRect other = anArea.DimensionRect;
            return other.IsIntersect(ExtRect.Create(Left, Top, Left + 20, Top + 8)) ||
            other.IsIntersect(ExtRect.Create(Left + 5, Top + 9, Left + 15, Top + 15)) ||
            other.IsIntersect(ExtRect.Create(Left, Top + 16, Left + 20, Top + 24));
        }

        static CrossroadRoom()
        {
            CrArea = new string[] {
                "XXX      XNX      XXX",
                "X.X     XX.XX     X.X",
                "X.XX   XX...XX   XX.X",
                "W..XX  X.....X  XX..E",
                "XX..XXXX.....XXXX..XX",
                " XX...XXX...XXX...XX ",
                "  XXX...........XXX  ",
                "    XXXXX...XXXXX    ",
                "        X...X        ",
                "     XXXXX.XXXXX     ",
                "     X.........X     ",
                "     X.........X     ",
                "     W.........E     ",
                "     X.........X     ",
                "     X.........X     ",
                "     XXXXX.XXXXX     ",
                "        X...X        ",
                "    XXXXX...XXXXX    ",
                "  XXX...........XXX  ",
                " XX...XXX...XXX...XX ",
                "XX..XXXX.....XXXX..XX",
                "W..XX  X.....X  XX..E",
                "X.XX   XX...XX   XX.X",
                "X.X     XX.XX     X.X",
                "XXX      XSX      XXX"
            };
        }
    }
}
