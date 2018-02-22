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

namespace ZRLib.Map.Dungeons
{
    public sealed class MonasticCellsRoom : CustomStaticArea
    {
        private static readonly string[] McArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new MonasticCellsRoom(owner, parentMark);
        }

        public MonasticCellsRoom(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(13, 15);
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return McArea[aY][aX];
        }

        static MonasticCellsRoom()
        {
            McArea = new string[] {
                "XXXXXXNXXXXXX",
                "X...........X",
                "X.XXXXXXXXX.X",
                "X.X...X...X.X",
                "X.X...X...X.X",
                "X.X...X...X.X",
                "X.XX.XXX.XX.X",
                "W...........E",
                "X.XX.XXX.XX.X",
                "X.X...X...X.X",
                "X.X...X...X.X",
                "X.X...X...X.X",
                "X.XXXXXXXXX.X",
                "X...........X",
                "XXXXXXSXXXXXX"
            };
        }
    }
}
