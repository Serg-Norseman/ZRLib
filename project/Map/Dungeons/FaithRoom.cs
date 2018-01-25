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
    public sealed class FaithRoom : CustomStaticArea
    {
        private static readonly string[] FrArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new FaithRoom(owner, parentMark);
        }

        public FaithRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(31, 31);
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return FrArea[aY][aX];
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            ExtRect other = anArea.DimensionRect;
            return other.IsIntersect(ExtRect.Create(Left + 11, Top, Left + 19, Top + 10)) || other.IsIntersect(ExtRect.Create(Left + 0, Top + 11, Left + 30, Top + 19)) || other.IsIntersect(ExtRect.Create(Left + 11, Top + 20, Left + 19, Top + 30));
        }

        static FaithRoom()
        {
            FrArea = new string[] {
                "             XXNXX             ",
                "             X...X             ",
                "            XX...XX            ",
                "           XX.....XX           ",
                "           X.......X           ",
                "           W.......E           ",
                "           X.......X           ",
                "           XX.....XX           ",
                "            XX...XX            ",
                "             X...X             ",
                "           XXX...XXX           ",
                "   XXNXX  XX.......XX  XXNXX   ",
                "  XX...XX X.........X XX...XX  ",
                "XXX.....XXX.........XXX.....XXX",
                "X.............................X",
                "W.............................E",
                "X.............................X",
                "XXX.....XXX.........XXX.....XXX",
                "  XX...XX X.........X XX...XX  ",
                "   XXSXX  XX.......XX  XXSXX   ",
                "           XXX...XXX           ",
                "             X...X             ",
                "            XX...XX            ",
                "           XX.....XX           ",
                "           X.......X           ",
                "           W.......E           ",
                "           X.......X           ",
                "           XX.....XX           ",
                "            XX...XX            ",
                "             X...X             ",
                "             XXSXX             "
            };
        }
    }
}
