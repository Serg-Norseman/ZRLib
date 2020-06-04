/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih.
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
using ZRLib.Core;
using ZRLib.Map;
using ZRLib.External.BSP;

namespace MysteriesRL.Maps
{
    public enum Axis
    {
        None,
        Horz,
        Vert
    }

    public static class MMapUtils
    {
        public static Axis GetTilesLineAxis(IMap map, int x, int y, int tileId)
        {
            if (map.CheckTile(x - 1, y, tileId, true) && map.CheckTile(x + 1, y, tileId, true)) {
                return Axis.Horz;
            } else if (map.CheckTile(x, y - 1, tileId, true) && map.CheckTile(x, y + 1, tileId, true)) {
                return Axis.Vert;
            }

            return Axis.None;
        }

        public static Axis GetSideAxis(SideType side)
        {
            switch (side) {
                case SideType.Top:
                case SideType.Bottom:
                    return Axis.Horz;

                case SideType.Right:
                case SideType.Left:
                    return Axis.Vert;

                default:
                    return Axis.None;
            }
        }

        public static ExtPoint GetCheckPoint(SideType side, ExtRect area, int delta)
        {
            ExtPoint result;
            switch (side) {
                case SideType.Top:
                    result = new ExtPoint(RandomHelper.GetBoundedRnd(area.Left + 1, area.Right - 1), area.Top - delta);
                    break;
                case SideType.Right:
                    result = new ExtPoint(area.Right + delta, RandomHelper.GetBoundedRnd(area.Top + 1, area.Bottom - 1));
                    break;
                case SideType.Bottom:
                    result = new ExtPoint(RandomHelper.GetBoundedRnd(area.Left + 1, area.Right - 1), area.Bottom + delta);
                    break;
                case SideType.Left:
                    result = new ExtPoint(area.Left - delta, RandomHelper.GetBoundedRnd(area.Top + 1, area.Bottom - 1));
                    break;
                default:
                    result = ExtPoint.Empty;
                    break;
            }

            return result;
        }

        public static void Gen_BigRiver(AbstractMap map, int normalizeLoops)
        {
            int mapWidth = map.Width;
            int mapHeight = map.Height;

            int x = 0;
            int y = 0;
            int x2 = RandomHelper.GetRandom(mapWidth / 2 - 2) + mapWidth / 2;
            int y2 = RandomHelper.GetRandom(mapHeight / 2 - 2) + mapHeight / 2;

            int num = RandomHelper.GetRandom(4);
            switch (num) {
                case 0:
                    x = RandomHelper.GetRandom(mapWidth - 2) + 1;
                    y = 1;
                    break;
                case 1:
                    x = 1;
                    y = RandomHelper.GetRandom(mapHeight - 2) + 1;
                    break;
                case 2:
                    x = mapWidth - 1;
                    y = RandomHelper.GetRandom(mapHeight - 2) + 1;
                    break;
                case 3:
                    x = RandomHelper.GetRandom(mapWidth - 2) + 1;
                    y = mapHeight - 1;
                    break;
            }

            ushort tidWater = (ushort)TileID.tid_Water;
            map.Gen_Path(x, y, x2, y2, map.AreaRect, tidWater, true, 20, true, null);

            for (int i = 1; i <= normalizeLoops; i++) {
                for (int yy = 1; yy <= mapHeight - 2; yy++) {
                    for (int xx = 1; xx <= mapWidth - 2; xx++) {
                        BaseTile tile = map.GetTile(xx, yy);

                        if (tile.Background == tidWater) {
                            int adjacents = map.CheckAdjacently(xx, yy, tidWater, false);

                            if (adjacents < 2) {
                                tile.Background = (ushort)TileID.tid_Grass;
                            } else if (adjacents >= 4) {
                                tile.Background = (ushort)TileID.tid_Water;
                            }
                        }
                    }
                }
            }
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
                MMapUtils.Gen_BigRiver(map, 0);
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
