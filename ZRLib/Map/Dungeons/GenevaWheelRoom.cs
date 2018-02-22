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
    public sealed class GenevaWheelRoom : CustomStaticArea
    {
        private static readonly string[] GwArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new GenevaWheelRoom(owner, parentMark);
        }

        public GenevaWheelRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(21, 21);
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            ExtRect other = anArea.DimensionRect;
            return other.IsIntersect(ExtRect.Create(Left + 7, Top, Left + 13, Top + 6)) || other.IsIntersect(ExtRect.Create(Left + 0, Top + 7, Left + 20, Top + 13)) || other.IsIntersect(ExtRect.Create(Left + 7, Top + 14, Left + 13, Top + 20));
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return GwArea[aY][aX];
        }

        public override int DevourArea
        {
            get {
                return fDevourArea + 32;
            }
        }

        static GenevaWheelRoom()
        {
            GwArea = new string[] {
                "       XXXNXXX       ",
                "       X.....X       ",
                "       X.....X       ",
                "       XX...XX       ",
                "        X...X        ",
                "        X...X        ",
                "        X...X        ",
                "XXXX    XX.XX    XXXX",
                "X..XXXXX X.X XXXXX..X",
                "X......XXX.XXX......X",
                "W...................E",
                "X......XXX.XXX......X",
                "X..XXXXX X.X XXXXX..X",
                "XXXX    XX.XX    XXXX",
                "        X...X        ",
                "        X...X        ",
                "        X...X        ",
                "       XX...XX       ",
                "       X.....X       ",
                "       X.....X       ",
                "       XXXSXXX       "
            };
        }
    }
}
