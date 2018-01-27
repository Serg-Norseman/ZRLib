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
    public sealed class TempleRoom : CustomStaticArea
    {
        private static readonly string[] TArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new TempleRoom(owner, parentMark);
        }

        public TempleRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(39, 39);
        }

        public override bool IsIntersectWithArea(DungeonArea anArea)
        {
            ExtRect other = anArea.DimensionRect;
            return other.IsIntersect(ExtRect.Create(Left + 16, Top, Left + 22, Top + 4)) || other.IsIntersect(ExtRect.Create(Left + 11, Top + 4, Left + 27, Top + 34)) || other.IsIntersect(ExtRect.Create(Left + 4, Top + 11, Left + 34, Top + 27)) || other.IsIntersect(ExtRect.Create(Left, Top + 16, Left + 4, Top + 22)) || other.IsIntersect(ExtRect.Create(Left + 16, Top + 34, Left + 22, Top + 38)) || other.IsIntersect(ExtRect.Create(Left + 34, Top + 16, Left + 38, Top + 22));
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return TArea[aY][aX];
        }

        public override int DevourArea
        {
            get {
                return fDevourArea + 64 + 36;
            }
        }

        static TempleRoom()
        {
            TArea = new string[] {
                "                XXXNXXX                ",
                "                X.....X                ",
                "                W.....E                ",
                "                X.....X                ",
                "                XX...XX                ",
                "                 X...X                 ",
                "                 X...X                 ",
                "                 X...X                 ",
                "                 XX.XX                 ",
                "                  X.X                  ",
                "                  X.X                  ",
                "                XXX.XXX                ",
                "            XXXXX.....XXXXX            ",
                "            X.............X            ",
                "            X.............X            ",
                "            X.............X            ",
                "XXNXX      XX.............XX      XXNXX",
                "X...XXXXX  X...............X  XXXXX...X",
                "X.......XXXX...............XXXX.......X",
                "W.....................................E",
                "X.......XXXX...............XXXX.......X",
                "X...XXXXX  X...............X  XXXXX...X",
                "XXSXX      XX.............XX      XXSXX",
                "            X.............X            ",
                "            X.............X            ",
                "            X.............X            ",
                "            XXXXX.....XXXXX            ",
                "                XXX.XXX                ",
                "                  X.X                  ",
                "                  X.X                  ",
                "                 XX.XX                 ",
                "                 X...X                 ",
                "                 X...X                 ",
                "                 X...X                 ",
                "                XX...XX                ",
                "                X.....X                ",
                "                W.....E                ",
                "                X.....X                ",
                "                XXXSXXX                "
            };
        }
    }

}