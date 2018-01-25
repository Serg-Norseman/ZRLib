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
using ZRLib.Core;

namespace ZRLib.Map
{
    public static class PathSearch
    {
        // PathSearch tile statuses
        public const byte tps_Unvisited = 0;
        public const byte tps_Path = 1;
        public const byte tps_Bound = 2;
        public const byte tps_Passed = 3;
        public const byte tps_Start = 4;
        public const byte tps_Finish = 5;
        public const byte tps_Barrier = 6;

        public const float BARRIER_COST = 1000.0f;

        public sealed class PSResult
        {
            public readonly int Dist;
            public readonly ExtPoint Step;
            public readonly IList<ExtPoint> Path;

            internal PSResult(int dist, ExtPoint step, IList<ExtPoint> path)
            {
                Dist = dist;
                Step = step;
                Path = path;
            }
        }

        public static PSResult Search(IMap map, ExtPoint src, ExtPoint dst, CreatureEntity creature)
        {
            return Search(map, src, dst, creature, false);
        }

        public static PSResult Search(IMap map, ExtPoint src, ExtPoint dst, CreatureEntity creature, bool needpath)
        {
            PSResult result = null;

            try {
                PathSearch.Clear(map);

                AStar ast = new AStar(map, creature, src.X, src.Y, dst.X, dst.Y);

                if (ast.Search()) {
                    IList<ExtPoint> path = null;
                    if (needpath) {
                        path = new List<ExtPoint>();
                        path.Add(new ExtPoint(dst.X, dst.Y));
                    }

                    int len = 1;
                    ExtPoint prv = map.GetTile(dst.X, dst.Y).Pf_prev;
                    while (prv.X != src.X || prv.Y != src.Y) {
                        len++;
                        if (path != null) {
                            path.Insert(0, prv);
                        }

                        BaseTile m_tile = map.GetTile(prv.X, prv.Y);
                        m_tile.Pf_status = PathSearch.tps_Path;
                        prv = m_tile.Pf_prev;

                        if (len > map.Width << 1) {
                            return null;
                        }
                    }

                    result = new PSResult(len, prv, path);
                }
            } catch (Exception ex) {
                Logger.Write("PathSearch.search(): " + ex.Message);
                result = null;
            }

            return result;
        }

        public static void Clear(IMap map)
        {
            int height = map.Height;
            int width = map.Width;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    BaseTile m_tile = map.GetTile(x, y);
                    m_tile.Pf_status = PathSearch.tps_Unvisited;
                    m_tile.Pf_prev = ExtPoint.Empty;
                }
            }
        }
    }
}
