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
    public sealed class Alt5Room : CustomStaticArea
    {
        private static readonly string[] AREA1;
        private static readonly string[] AREA2;
        private static readonly string[] AREA3;

        private new string[] fArea;

        public static DungeonArea CreateArea(DungeonBuilder owner, DungeonMark parentMark)
        {
            return new Alt5Room(owner, parentMark);
        }

        public Alt5Room(DungeonBuilder owner, DungeonMark parentMark)
            : base(owner, parentMark)
        {
            SetDimension(21, 11);

            int idx = RandomHelper.GetRandom(3);
            if (idx == 0) {
                fArea = AREA1;
            } else if (idx == 1) {
                fArea = AREA2;
            } else {
                fArea = AREA3;
            }
        }

        protected override char GetCustomCell(int aX, int aY)
        {
            return fArea[aY][aX];
        }

        static Alt5Room()
        {
            AREA1 = new string[] {
                "XXXXNXXXXXNXXXXXNXXXX",
                "XX.................XX",
                "X...................X",
                "X...................X",
                "X...................X",
                "W...................E",
                "X...................X",
                "X...................X",
                "X...................X",
                "XX.................XX",
                "XXXXSXXXXXSXXXXXSXXXX"
            };

            AREA2 = new string[] {
                "XXXXNXXXXXNXXXXXNXXXX",
                "XX.................XX",
                "X...XXXXX...XXXXX...X",
                "X...X...........X...X",
                "X...X..XXX.XXX..X...X",
                "W...................E",
                "X...X..XXX.XXX..X...X",
                "X...X...........X...X",
                "X...XXXXX...XXXXX...X",
                "XX.................XX",
                "XXXXSXXXXXSXXXXXSXXXX"
            };

            AREA3 = new string[] {
                "XXXXNXXXXXNXXXXXNXXXX",
                "XX.................XX",
                "X...................X",
                "X..XXXXXX...XXXXXX..X",
                "X..X.............X..X",
                "W..X..XXXXXXXXX..X..E",
                "X..X.............X..X",
                "X..XXXXXX...XXXXXX..X",
                "X...................X",
                "XX.................XX",
                "XXXXSXXXXXSXXXXXSXXXX"
            };
        }
    }
}
