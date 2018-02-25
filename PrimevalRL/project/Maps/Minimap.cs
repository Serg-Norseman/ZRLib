/*
 *  "PrimevalRL", roguelike game.
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

using System.Collections.Generic;
using BSLib;
using PrimevalRL.Game;
using PrimevalRL.Maps.Buildings;
using ZRLib.Core;
using ZRLib.Map;

namespace PrimevalRL.Maps
{
    public class Minimap : BaseMap
    {
        public const int SCALE = 10;

        public Minimap(IMap source, City city)
            : base(source.Height / SCALE, source.Width / SCALE)
        {
            MakePlane(source);

            if (city != null) {
                MakeCity(source, city);
            }
        }

        public void MakePlane(IMap source)
        {
            IDictionary<int, int> bgMap = new Dictionary<int, int>();
            IDictionary<int, int> fgMap = new Dictionary<int, int>();

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    bgMap.Clear();
                    fgMap.Clear();

                    for (int yy = 0; yy < SCALE; yy++) {
                        for (int xx = 0; xx < SCALE; xx++) {
                            int sx = x * 10 + xx;
                            int sy = y * 10 + yy;
                            BaseTile tile = source.GetTile(sx, sy);

                            int srcTile = tile.Background;
                            int? count = bgMap.GetValueOrNull(srcTile);
                            if (count != null) {
                                count++;
                            } else {
                                count = 1;
                            }
                            bgMap[srcTile] = count.Value;

                            srcTile = tile.Foreground;
                            count = fgMap.GetValueOrNull(srcTile);
                            if (count != null) {
                                count++;
                            } else {
                                count = 1;
                            }
                            fgMap[srcTile] = count.Value;
                        }
                    }

                    int maxBID = -1;
                    int maxB = 0;
                    foreach (KeyValuePair<int, int> entry in bgMap) {
                        TileID tid = (TileID)entry.Key;

                        if (tid == TileID.tid_Road) {
                            maxBID = entry.Key;
                            break;
                        } else {
                            var tileRec = MRLData.GetTileRec(entry.Key);
                            if (tileRec.MiniMap && entry.Value > maxB) {
                                maxB = entry.Value;
                                maxBID = entry.Key;
                            }
                        }
                    }

                    int maxFID = -1;
                    int maxF = 0;
                    /*for (Map.Entry<Integer, Integer> entry : fgMap.entrySet()) {
                        TileID tid = TileID.forValue(entry.getKey());
                        
                        if (entry.getKey() == TileID.tid_Road.Value) {
                            maxFID = entry.getKey();
                            break;
                        } else {
                            if (tid.MiniMap && entry.getValue() > maxF) {
                                maxF = entry.getValue();
                                maxFID = entry.getKey();
                            }
                        }
                    }*/

                    if (maxBID >= 0) {
                        GetTile(x, y).Background = (ushort)maxBID;
                    }

                    if (maxFID >= 0) {
                        GetTile(x, y).Foreground = (ushort)maxFID;
                    }
                }
            }
        }

        public void MakeCity(IMap source, City city)
        {
            foreach (Street street in city.Streets) {
                ExtRect srcRect = (ExtRect)street.Area;
                ExtRect dstRect = ExtRect.Create(srcRect.Left / 10, srcRect.Top / 10, srcRect.Right / 10, srcRect.Bottom / 10);

                FillArea(dstRect.Left, dstRect.Top, dstRect.Right, dstRect.Bottom, (int)TileID.tid_Road, false);
            }

            foreach (Building bld in city.Buildings) {
                ExtRect srcRect = (ExtRect)bld.Area;
                ExtRect dstRect = ExtRect.Create(srcRect.Left / 10, srcRect.Top / 10, srcRect.Right / 10, srcRect.Bottom / 10);

                for (int yy = dstRect.Top; yy <= dstRect.Bottom; yy++) {
                    for (int xx = dstRect.Left; xx <= dstRect.Right; xx++) {
                        BaseTile tile = GetTile(xx, yy);
                        if (tile != null) {
                            /*if (fg) {
                                tile.Foreground = (short) tid;
                            } else {
                                tile.Background = (short) tid;
                            }*/

                            if (tile.Background != (ushort)TileID.tid_Road) {
                                tile.Foreground = (ushort)TileID.tid_HouseWall;
                            }
                        }
                        //setTile(xx, yy, tid, fg);
                    }
                }

                //fillArea(dstRect.Left, dstRect.Top, dstRect.Right, dstRect.Bottom, TileID.tid_HouseFloor.Value, false);
                //fillBorder(dstRect.Left, dstRect.Top, dstRect.Right, dstRect.Bottom, TileID.tid_HouseWall.Value, true);
            }

            foreach (District distr in city.Districts) {
                TileID tid;
                switch (distr.Type)
                {
                    case DistrictType.dtPark:
                        tid = TileID.tid_Grass;
                        break;
                    case DistrictType.dtSquare:
                        tid = TileID.tid_Square;
                        break;
                    default:
                        continue;
                }

                ExtRect srcRect = distr.Area;
                ExtRect dstRect = ExtRect.Create(srcRect.Left / 10, srcRect.Top / 10, srcRect.Right / 10, srcRect.Bottom / 10);

                for (int yy = dstRect.Top; yy <= dstRect.Bottom; yy++) {
                    for (int xx = dstRect.Left; xx <= dstRect.Right; xx++) {
                        BaseTile tile = GetTile(xx, yy);
                        if (tile != null) {
                            if (tile.Background != (ushort)TileID.tid_Road) {
                                tile.Background = (ushort)tid;
                            }
                        }
                    }
                }
            }
        }
    }
}
