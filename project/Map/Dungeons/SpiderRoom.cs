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
    public sealed class SpiderRoom : CustomStaticArea
    {
        private static readonly string[] SprArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new SpiderRoom(owner, parentMark);
        }

        public SpiderRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(31, 31);
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return SprArea[aY][aX];
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            ExtRect other = anArea.DimensionRect;
            return other.IsIntersect(ExtRect.Create(Left + 7, Top, Left + 23, Top + 6)) || other.IsIntersect(ExtRect.Create(Left + 0, Top + 7, Left + 30, Top + 22)) || other.IsIntersect(ExtRect.Create(Left + 7, Top + 23, Left + 23, Top + 30));
        }

        static SpiderRoom()
        {
            SprArea = new string[] {
                "       XXXX   XNX   XXXX       ",
                "       X..X   X.X   X..X       ",
                "       W..X   X.X   X..E       ",
                "       X..XX  X.X  XX..X       ",
                "       XX..X  X.X  X..XX       ",
                "        X..XXXX.XXXX..X        ",
                "        XX...XX.XX...XX        ",
                "XXNXX    XX.........XX    XXNXX",
                "X...XXX   XXX.....XXX   XXX...X",
                "X.....XX    XXX.XXX    XX.....X",
                "XXXX...XX     X.X     XX...XXXX",
                "   XXX..X     X.X     X..XXX   ",
                "     X..XX   XX.XX   XX..X     ",
                "     XX..X  XX...XX  X..XX     ",
                "XXXXXXX..XXXX.....XXXX..XXXXXXX",
                "W.............................E",
                "XXXXXXX..XXXX.....XXXX..XXXXXXX",
                "     XX..X  XX...XX  X..XX     ",
                "     X..XX   XX.XX   XX..X     ",
                "   XXX..X     X.X     X..XXX   ",
                "XXXX...XX     X.X     XX...XXXX",
                "X.....XX    XXX.XXX    XX.....X",
                "X...XXX   XXX.....XXX   XXX...X",
                "XXSXX    XX.........XX    XXSXX",
                "        XX...XX.XX...XX        ",
                "        X..XXXX.XXXX..X        ",
                "       XX..X  X.X  X..XX       ",
                "       X..XX  X.X  XX..X       ",
                "       W..X   X.X   X..E       ",
                "       X..X   X.X   X..X       ",
                "       XXXX   XSX   XXXX       "
            };
        }
    }
}
