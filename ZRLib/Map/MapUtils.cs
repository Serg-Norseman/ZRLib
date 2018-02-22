/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih.
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
using System.Collections.Generic;
using BSLib;
using ZRLib.Core;

namespace ZRLib.Map
{
    public static class MapUtils
    {
        private static readonly TileType[] DtWalls;
        private static readonly TileType[] DtWaters;
        private static readonly TileType[,] DtDoors;
        private static readonly TileType[] DtBuilding;

        static MapUtils()
        {
            DtWalls = new TileType[] {
                TileType.ttBWallN,
                TileType.ttBWallS,
                TileType.ttBWallW,
                TileType.ttBWallE,
                TileType.ttBWallNW,
                TileType.ttBWallNE,
                TileType.ttBWallSW,
                TileType.ttBWallSE,
                TileType.ttWall,
                TileType.ttDoor,
                TileType.ttDoorXX_Closed,
                TileType.ttDoorYY_Closed,
                TileType.ttDoorXX_Opened,
                TileType.ttDoorYY_Opened
            };

            DtWaters = new TileType[] {
                TileType.ttWater,
                TileType.ttDeepWater,
                TileType.ttDeeperWater
            };

            DtDoors = new TileType[2, 2];
            DtDoors[0, 0] = TileType.ttDoorXX_Opened;
            DtDoors[0, 1] = TileType.ttDoorXX_Closed;
            DtDoors[1, 0] = TileType.ttDoorYY_Opened;
            DtDoors[1, 1] = TileType.ttDoorYY_Closed;

            DtBuilding = new TileType[] {
                TileType.ttWall,
                TileType.ttFloor,
                TileType.ttDoor,
                TileType.ttDoorXX_Closed,
                TileType.ttDoorYY_Closed,
                TileType.ttBWallN,
                TileType.ttBWallS,
                TileType.ttBWallW,
                TileType.ttBWallE,
                TileType.ttBWallNW,
                TileType.ttBWallNE,
                TileType.ttBWallSW,
                TileType.ttBWallSE,
                TileType.ttDoorXX_Opened,
                TileType.ttDoorYY_Opened
            };
        }

        public static TileType GetWallKind(int x, int y, ExtRect r)
        {
            TileType result = TileType.ttFloor;

            if (x == r.Left && y == r.Top) {
                result = TileType.ttBWallNW;
            } else if (x == r.Right && y == r.Top) {
                result = TileType.ttBWallNE;
            } else if (x == r.Left && y == r.Bottom) {
                result = TileType.ttBWallSW;
            } else if (x == r.Right && y == r.Bottom) {
                result = TileType.ttBWallSE;
            } else if (x > r.Left && x < r.Right && y == r.Top) {
                result = TileType.ttBWallN;
            } else if (x > r.Left && x < r.Right && y == r.Bottom) {
                result = TileType.ttBWallS;
            } else if (y > r.Top && y < r.Bottom && x == r.Left) {
                result = TileType.ttBWallW;
            } else if (y > r.Top && y < r.Bottom && x == r.Right) {
                result = TileType.ttBWallE;
            }

            return result;
        }

        public static bool IsBuilding(AbstractMap map, ushort tile)
        {
            foreach (TileType buildingTile in DtBuilding) {
                if (map.TranslateTile(buildingTile) == tile) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsWalls(AbstractMap map, ushort tile)
        {
            foreach (TileType wallTile in DtWalls) {
                if (map.TranslateTile(wallTile) == tile) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsWaters(AbstractMap map, ushort tile)
        {
            foreach (TileType waterTile in DtWaters) {
                if (map.TranslateTile(waterTile) == tile) {
                    return true;
                }
            }
            return false;
        }

        public static void NormalizeDoors(AbstractMap map, ExtRect r, bool defClosed)
        {
            for (int x = r.Left; x <= r.Right; x++) {
                for (int y = r.Top; y <= r.Bottom; y++) {
                    BaseTile t = map.GetTile(x, y);
                    if (t != null && t.Foreground == map.TranslateTile(TileType.ttDoor)) {
                        if (IsWalls(map, map.GetTile(x, y + 1).Foreground) && IsWalls(map, map.GetTile(x, y - 1).Foreground)) {
                            t.Foreground = map.TranslateTile(DtDoors[0, defClosed ? 1 : 0]);
                        }
                        if (IsWalls(map, map.GetTile(x + 1, y).Foreground) && IsWalls(map, map.GetTile(x - 1, y).Foreground)) {
                            t.Foreground = map.TranslateTile(DtDoors[1, defClosed ? 1 : 0]);
                        }
                    }
                }
            }
        }

        public static bool HasTiles(IMap map, ExtRect area, int tileId, bool fg)
        {
            for (int y = area.Top; y <= area.Bottom; y++) {
                for (int x = area.Left; x <= area.Right; x++) {
                    BaseTile mtile = map.GetTile(x, y);

                    if (mtile != null) {
                        int tid = (fg) ? mtile.Foreground : mtile.Background;

                        if (tid == tileId) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void GenRareTiles(IMap map, ExtRect area, float densityFactor, ushort tileId, int skipId, bool fg)
        {
            int num = (int)(area.Square * densityFactor);
            for (int i = 1; i <= num; i++) {
                ExtPoint pt = RandomHelper.GetRandomPoint(area);
                BaseTile mtile = map.GetTile(pt.X, pt.Y);

                if (mtile != null && mtile.BackBase != skipId) {
                    if (fg) {
                        mtile.Foreground = tileId;
                    } else {
                        mtile.Background = tileId;
                    }
                }
            }
        }

        // FIXME: doublicate
        public static void FillBorder(IMap map, ExtRect mapRect, ushort check, ushort tid)
        {
            int x1 = mapRect.Left;
            int y1 = mapRect.Top;
            int x2 = mapRect.Right;
            int y2 = mapRect.Bottom;

            for (int xx = x1; xx <= x2; xx++) {
                SetTile(map, y1, xx, check, tid);
                SetTile(map, y2, xx, check, tid);
            }

            for (int yy = y1; yy <= y2; yy++) {
                SetTile(map, yy, x1, check, tid);
                SetTile(map, yy, x2, check, tid);
            }
        }

        private static void SetTile(IMap map, int y, int x, ushort check, ushort tid)
        {
            BaseTile tile = map.GetTile(x, y);
            if (tile != null && tile.Foreground == check) {
                tile.Foreground = tid;
            }
        }

        #region Tile Finder

        // Tile search flags
        public const int TSF_SIMPLE = 1;
        public const int TSF_BORDER = 2;
        public const int TSF_FREE = 4;

        public static ExtPoint FindTile(IMap map, ExtRect area, int flags)
        {
            if ((flags & TSF_SIMPLE) > 0) {
                int tries = 50;
                while (tries > 0) {
                    ExtPoint pt = RandomHelper.GetRandomPoint(area);
                    if (!pt.IsEmpty) {
                        if ((flags & TSF_FREE) > 0) {
                            if (!map.IsBarrier(pt.X, pt.Y)) {
                                return pt;
                            }
                        } else {
                            return pt;
                        }
                    }

                    tries--;
                }
            } else {
                IList<ExtPoint> pts = new List<ExtPoint>();

                int x1 = area.Left;
                int y1 = area.Top;
                int x2 = area.Right;
                int y2 = area.Bottom;

                if ((flags & TSF_BORDER) > 0) {
                    for (int xx = x1; xx <= x2; xx++) {
                        CheckTile(map, xx, y1, flags, pts);
                        CheckTile(map, xx, y2, flags, pts);
                    }

                    for (int yy = y1; yy <= y2; yy++) {
                        CheckTile(map, x1, yy, flags, pts);
                        CheckTile(map, x2, yy, flags, pts);
                    }
                } else {
                    for (int y = y1; y <= y2; y++) {
                        for (int x = x1; x <= x2; x++) {
                            CheckTile(map, x, y, flags, pts);
                        }
                    }
                }

                return RandomHelper.GetRandomItem(pts);
            }

            return ExtPoint.Empty;
        }

        private static void CheckTile(IMap map, int x, int y, int flags, IList<ExtPoint> pts)
        {
            try {
                if ((flags & TSF_FREE) > 0) {
                    if (!map.IsBarrier(x, y)) {
                        BaseTile tile = map.GetTile(x, y);
                        if (!tile.HasState(BaseTile.TS_NFM)) {
                            pts.Add(new ExtPoint(x, y));
                        }
                    }
                } else {
                    pts.Add(new ExtPoint(x, y));
                }
            } catch (Exception ex) {
                Logger.Write("MapUtils.CheckTile(): " + ex.Message);
            }
        }

        #endregion

        #region Tile Transformer

        public sealed class MagicRec
        {
            public BytesSet Main;
            public BytesSet Ext;

            public MagicRec(BytesSet main, BytesSet ext)
            {
                Main = main;
                Ext = ext;
            }
        }

        // Tile select levels
        public const int TSL_BACK = 0;
        public const int TSL_FORE = 1;
        public const int TSL_BACK_EXT = 2;
        public const int TSL_FORE_EXT = 3;
        public const int TSL_FOG = 4;

        // FIXME: NWR dup
        public static int GetAdjacentMagic(IMap map, int x, int y, int def, int check, int select)
        {
            int magic = 0;
            for (int i = 0; i < 8; i++) {
                int ax = x + AbstractMap.CoursesX[i];
                int ay = y + AbstractMap.CoursesY[i];

                int tid;
                BaseTile tile = map.GetTile(ax, ay);
                if (tile == null) {
                    tid = def;
                } else {
                    switch (select) {
                        case TSL_BACK:
                            tid = tile.BackBase;
                            break;
                        case TSL_FORE:
                            tid = tile.ForeBase;
                            break;
                        case TSL_BACK_EXT:
                            tid = AuxUtils.GetShortLo(tile.BackgroundExt);
                            break;
                        case TSL_FORE_EXT:
                            tid = AuxUtils.GetShortLo(tile.ForegroundExt);
                            break;
                    /*case TSL_FOG:
                            tid = AuxUtils.getShortLo(tile.FogID);
                            break;*/
                        default:
                            tid = 0;
                            break;
                    }
                }

                if (tid == check) {
                    magic = BitHelper.SetBit(magic, i);
                }
            }

            return magic;
        }

        #endregion
    }
}
