/*
 *  "PrimevalRL", roguelike game.
 *  Copyright (C) 2018 by Serg V. Zhdanovskih.
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

using System;
using BSLib;
using PrimevalRL.Maps;
using ZRLib.Core;
using ZRLib.Map;

namespace PrimevalRL.Generators
{
    public class RealmGenerator
    {
        public RealmGenerator()
        {
        }

        public static void Gen_BigRiver(AbstractMap map)
        {
            int y = 0;
            int x = 0;
            int cur_hgt = map.Height;
            int cur_wid = map.Width;

            int y2 = RandomHelper.GetRandom(cur_hgt / 2 - 2) + cur_hgt / 2;
            int x2 = RandomHelper.GetRandom(cur_wid / 2 - 2) + cur_wid / 2;

            int num = RandomHelper.GetRandom(4);
            switch (num) {
                case 0:
                    x = RandomHelper.GetRandom(cur_wid - 2) + 1;
                    y = 1;
                    break;
                case 1:
                    x = 1;
                    y = RandomHelper.GetRandom(cur_hgt - 2) + 1;
                    break;
                case 2:
                    x = cur_wid - 1;
                    y = RandomHelper.GetRandom(cur_hgt - 2) + 1;
                    break;
                case 3:
                    x = RandomHelper.GetRandom(cur_wid - 2) + 1;
                    y = cur_hgt - 1;
                    break;
            }

            map.Gen_Path(x, y, x2, y2, map.AreaRect, (int)TileID.tid_Water, true, true, null);
        }

        public static void Gen_ForeObjects(AbstractMap map, ushort tid, float freq)
        {
            int cnt = (int)Math.Round((double)(map.Width * map.Height) * freq);
            for (int t = 1; t <= cnt; t++) {
                int X = RandomHelper.GetBoundedRnd(1, map.Width - 2);
                int Y = RandomHelper.GetBoundedRnd(1, map.Height - 2);
                BaseTile tile = map.GetTile(X, Y);
                if (tile != null && tile.Foreground == 0) {
                    tile.Foreground = tid;
                }
            }
        }

        public static void Gen_MountainRanges(AbstractMap map)
        {
            RandomHelper.Randomize();
            Gen_ForeObjects(map, map.TranslateTile(TileType.ttMountain), 0.45f);

            int apply; // for debug in future
            do {
                for (int y = 0; y < map.Height; y++) {
                    for (int x = 0; x < map.Width; x++) {
                        int count = map.CheckForeAdjacently(x, y, TileType.ttMountain);

                        if (map.CheckFore(x, y, TileType.ttMountain)) {
                            if (count < 4) {
                                map.GetTile(x, y).Foreground = map.TranslateTile(TileType.ttUndefined);
                            }
                        } else {
                            if (count > 4) {
                                map.GetTile(x, y).Foreground = map.TranslateTile(TileType.ttMountain);
                            }
                        }
                    }
                }
                apply = 1;
            } while (apply < 1);
        }

        public static void Gen_Forest(AbstractMap map, ExtRect area, int tileID, bool fieldClear)
        {
            if (fieldClear) {
                map.FillBackground(map.TranslateTile(TileType.ttGrass));
                map.FillForeground(map.TranslateTile(TileType.ttUndefined));
            }

            int num = RandomHelper.GetBoundedRnd(15, 25);
            for (int i = 1; i <= num; i++) {
                int x = RandomHelper.GetBoundedRnd(area.Left, area.Right);
                int y = RandomHelper.GetBoundedRnd(area.Top, area.Bottom);

                int j = 5;
                do {
                    int dir = RandomHelper.GetBoundedRnd(Directions.DtNorth, Directions.DtSouthEast);
                    x += Directions.Data[dir].DX;
                    y += Directions.Data[dir].DY;

                    BaseTile tile = map.GetTile(x, y);
                    if (tile != null && tile.Foreground == 0) {
                        tile.Foreground = (ushort)tileID;
                    }
                    j--;
                } while (j != 0);
            }
        }

        public static void Gen_Valley(AbstractMap map, bool clear, bool river, bool lakes)
        {
            if (clear) {
                map.FillBackground(map.TranslateTile(TileType.ttGrass));
                map.FillForeground(map.TranslateTile(TileType.ttUndefined));
            }

            Gen_MountainRanges(map);
            Gen_Forest(map, map.AreaRect, (int)TileID.tid_Tree, false);

            if (river) {
                Gen_BigRiver(map);
            }

            if (lakes) {
                int cnt = RandomHelper.GetBoundedRnd(4, (int)Math.Round((float)map.Height / (float)map.Width * 15.0f));
                for (int i = 1; i <= cnt; i++) {
                    int X = RandomHelper.GetBoundedRnd(map.AreaRect.Left, map.AreaRect.Right);
                    int Y = RandomHelper.GetBoundedRnd(map.AreaRect.Top, map.AreaRect.Bottom);
                    ushort bg = map.GetTile(X, Y).Background;
                    if (!MapUtils.IsWaters(map, bg)) {
                        int rad = RandomHelper.GetBoundedRnd(4, 10);
                        map.Gen_Lake(ExtRect.Create(X - rad, Y - rad, X + rad, Y + rad), LakeTilesChanger);
                    }
                }
            }
        }

        private static void LakeTilesChanger(IMap map, int x, int y, object extData, ref bool  refContinue)
        {
            BaseTile tile = map.GetTile(x, y);
            if (tile != null && tile.BackBase == (ushort)TileID.tid_Grass && tile.Foreground == 0) {
                tile.Background = (ushort)TileID.tid_Water;
            }
        }
    }
}
