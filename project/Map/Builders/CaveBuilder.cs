/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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

namespace ZRLib.Map.Builders
{
    /// <summary>
    /// Generating caves.
    /// Newsgroups: rec.games.roguelike.development
    /// @author su <steve_ued@yahoo.com>, 28.08.2004
    /// @author Serg V. Zhdanovskih
    /// </summary>
    public sealed class CaveBuilder : BaseBuilder
    {
        private sealed class CavePoint
        {
            public int X;
            public int Y;
            public int Room;

            public CavePoint(int x, int y, int room)
            {
                X = x;
                Y = y;
                Room = room;
            }
        }

        private int fHeight, fWidth;
        private int[,] fConnectsMap;
        private int fRoomsCount;
        private List<CavePoint> fPoints;

        public CaveBuilder(IMap map)
            : base(map)
        {
        }

        public CaveBuilder(IMap map, ExtRect area)
            : base(map, area)
        {
        }

        public void Build()
        {
            ExtRect rt = fArea;
            fHeight = rt.GetHeight();
            fWidth = rt.GetWidth();

            for (int y = rt.Top; y <= rt.Bottom; y++) {
                for (int x = rt.Left; x <= rt.Right; x++) {
                    BaseTile tile = fMap.GetTile(x, y);
                    tile.Background = fMap.TranslateTile(TileType.ttCaveFloor);
                    tile.Foreground = fMap.TranslateTile(TileType.ttCaveWall);
                }
            }

            RandomHelper.Randomize();

            for (int y = rt.Top + 1; y <= rt.Bottom - 1; y++) {
                for (int x = rt.Left + 1; x <= rt.Right - 1; x++) {
                    int tmp = RandomHelper.GetRandom(10);
                    if (tmp % 2 == 1) {
                        fMap.GetTile(x, y).Foreground = 0;
                    }
                }
            }

            // 1
            int removed_counter = 0;
            for (int y = rt.Top + 2; y <= rt.Bottom - 2; y++) {
                for (int x = rt.Left + 2; x <= rt.Right - 2; x++) {
                    BaseTile tile = fMap.GetTile(x, y);

                    if (tile.Foreground == fMap.TranslateTile(TileType.ttCaveWall)) {
                        int adjacent_floor_count = 0;
                        //adjacent_floor_count = checkAdjacentlyTile(map, x, y, TileType.ttUndefined);
                        if (fMap.GetTile(x, y - 1).Foreground == 0) {
                            adjacent_floor_count++;
                        }
                        if (fMap.GetTile(x, y + 1).Foreground == 0) {
                            adjacent_floor_count++;
                        }
                        if (fMap.GetTile(x - 1, y).Foreground == 0) {
                            adjacent_floor_count++;
                        }
                        if (fMap.GetTile(x + 1, y).Foreground == 0) {
                            adjacent_floor_count++;
                        }

                        if (adjacent_floor_count >= 3) {
                            if (removed_counter + 1 == 10) {
                                removed_counter = 0;
                            } else {
                                tile.Foreground = 0;
                            }
                        }
                    }
                }
            }

            // 2
            for (int y = rt.Top + 1; y <= rt.Bottom - 1; y++) {
                for (int x = rt.Left + 1; x <= rt.Right - 1; x++) {
                    BaseTile tile = fMap.GetTile(x, y);

                    if (tile.Foreground == 0) {
                        int adjacents = fMap.CheckForeAdjacently(x, y, TileType.ttUndefined);

                        if (adjacents < 2) {
                            tile.Foreground = fMap.TranslateTile(TileType.ttCaveWall);
                        }
                    }
                }
            }

            fConnectsMap = new int[fHeight, fWidth];
            fRoomsCount = 0;
            fPoints = new List<CavePoint>();

            do {
                ProcessConnects();
            } while (fRoomsCount > 1);

            // only for debug
            /*int id = fMap.translateTile(TileType.ttGrass);
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (connects_map[y][x] > 0) {
                        BaseTile tile = fMap.getTile(x, y);
                        tile.backGround = (short) (id + connects_map[y][x]);
                    }
                }
            }*/
        }

        private void ProcessConnects()
        {
            ExtRect rt = fArea;
            for (int y = 0; y < fHeight; y++) {
                for (int x = 0; x < fWidth; x++) {
                    int mx = rt.Left + x;
                    int my = rt.Top + y;
                    fConnectsMap[y, x] = (fMap.GetTile(mx, my).Foreground == 0) ? 0 : -1;
                }
            }

            fRoomsCount = 0;
            for (int y = 0; y < fHeight; y++) {
                for (int x = 0; x < fWidth; x++) {
                    if (fConnectsMap[y, x] == 0) {
                        fRoomsCount++;
                        SearchFloodfill(x, y);
                    }
                }
            }

            if (fRoomsCount <= 1) {
                return;
            }

            fPoints.Clear();
            for (int y = 1; y < fHeight - 2; y++) {
                for (int x = 1; x < fWidth - 2; x++) {
                    int room = fConnectsMap[y, x];
                    if (room > 0 && CheckAdjacently(x, y)) {
                        fPoints.Add(new CavePoint(x, y, room));
                    }
                }
            }

            if (fPoints.Count < 2) {
                throw new Exception("gen_Cave(): no connect points");
            }

            CavePoint tmp = fPoints[0];

            int max = fWidth * fHeight;
            // find near point of other room
            int pt_dist = max;
            CavePoint ptx1 = null;
            for (int k = 1; k < fPoints.Count; k++) {
                CavePoint pt2 = fPoints[k];

                if (tmp.Room != pt2.Room) {
                    int d = MathHelper.Distance(tmp.X, tmp.Y, pt2.X, pt2.Y);
                    if (d < pt_dist) {
                        pt_dist = d;
                        ptx1 = pt2;
                    }
                }
            }

            if (ptx1 != null) {
                CavePoint ptx2 = FindNearPoint(ptx1.X, ptx1.Y, tmp.Room, max);

                if (ptx2 != null) {
                    fMap.Gen_Path(rt.Left + ptx1.X, rt.Top + ptx1.Y, rt.Left + ptx2.X, rt.Top + ptx2.Y, fArea, 0, false, false, null);
                }
            }
        }

        private bool IsValidCM(int x, int y)
        {
            return (x >= 0 && x < fWidth && y >= 0 && y < fHeight);
        }

        private void SearchFloodfill(int cx, int cy)
        {
            if (!IsValidCM(cx, cy) || fConnectsMap[cy, cx] != 0) {
                return;
            }

            fConnectsMap[cy, cx] = fRoomsCount;

            for (int yy = cy - 1; yy <= cy + 1; yy++) {
                for (int xx = cx - 1; xx <= cx + 1; xx++) {
                    SearchFloodfill(xx, yy);
                }
            }
        }

        private bool CheckAdjacently(int cx, int cy)
        {
            for (int yy = cy - 1; yy <= cy + 1; yy++) {
                for (int xx = cx - 1; xx <= cx + 1; xx++) {
                    if ((xx != cx) || (yy != cy)) {
                        if (fConnectsMap[yy, xx] == -1) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private CavePoint FindNearPoint(int xx, int yy, int needRoom, int max)
        {
            CavePoint result = null;
            int resDist = max;

            foreach (CavePoint cp in fPoints) {
                if (cp.Room == needRoom) {
                    int d = MathHelper.Distance(xx, yy, cp.X, cp.Y);
                    if (d < resDist) {
                        result = cp;
                        resDist = d;
                    }
                }
            }

            return result;
        }
    }
}
