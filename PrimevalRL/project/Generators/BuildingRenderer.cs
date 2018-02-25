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

using System;
using BSLib;
using PrimevalRL.Maps;
using PrimevalRL.Maps.Buildings;
using ZRLib.Core;
using ZRLib.Map;

namespace PrimevalRL.Generators
{
    public static class BuildingRenderer
    {
        private static readonly MapUtils.MagicRec[] RmMagics;
        private static readonly MapUtils.MagicRec[] BlMagics;

        static BuildingRenderer()
        {
            RmMagics = new MapUtils.MagicRec[11];
            RmMagics[0] = new MapUtils.MagicRec(new BytesSet(20), new BytesSet());
            RmMagics[1] = new MapUtils.MagicRec(new BytesSet(80), new BytesSet());
            RmMagics[2] = new MapUtils.MagicRec(new BytesSet(5), new BytesSet());
            RmMagics[3] = new MapUtils.MagicRec(new BytesSet(65), new BytesSet());
            RmMagics[4] = new MapUtils.MagicRec(new BytesSet(68, 78, 228, 70, 76, 196, 100, 204, 102, 64, 4), new BytesSet()); // -
            RmMagics[5] = new MapUtils.MagicRec(new BytesSet(17, 57, 147, 145, 19, 49, 25, 153, 51, 1, 16), new BytesSet()); // |
            RmMagics[6] = new MapUtils.MagicRec(new BytesSet(69, 77, 101), new BytesSet());
            RmMagics[7] = new MapUtils.MagicRec(new BytesSet(84, 212, 86), new BytesSet());
            RmMagics[8] = new MapUtils.MagicRec(new BytesSet(21, 149, 53), new BytesSet());
            RmMagics[9] = new MapUtils.MagicRec(new BytesSet(81, 83, 89), new BytesSet());
            RmMagics[10] = new MapUtils.MagicRec(new BytesSet(85), new BytesSet());

            BlMagics = new MapUtils.MagicRec[15];
            BlMagics[0] = new MapUtils.MagicRec(new BytesSet(20), new BytesSet(0));
            BlMagics[1] = new MapUtils.MagicRec(new BytesSet(80), new BytesSet(0));
            BlMagics[2] = new MapUtils.MagicRec(new BytesSet(5), new BytesSet(0));
            BlMagics[3] = new MapUtils.MagicRec(new BytesSet(65), new BytesSet(0));
            BlMagics[4] = new MapUtils.MagicRec(new BytesSet(68, 78, 228, 70, 76, 196, 100, 204, 102, 64, 4, 198, 108), new BytesSet(0)); // -
            BlMagics[5] = new MapUtils.MagicRec(new BytesSet(17, 57, 147, 145, 19, 49, 25, 153, 51, 1, 16, 27, 177), new BytesSet(0)); // |
            BlMagics[6] = new MapUtils.MagicRec(new BytesSet(69, 77, 101), new BytesSet(0));
            BlMagics[7] = new MapUtils.MagicRec(new BytesSet(84, 212, 86), new BytesSet(0));
            BlMagics[8] = new MapUtils.MagicRec(new BytesSet(21, 149, 53), new BytesSet(0));
            BlMagics[9] = new MapUtils.MagicRec(new BytesSet(81, 83, 89), new BytesSet(0));
            BlMagics[10] = new MapUtils.MagicRec(new BytesSet(85), new BytesSet(0));
            BlMagics[11] = new MapUtils.MagicRec(new BytesSet(68), new BytesSet(1));
            BlMagics[12] = new MapUtils.MagicRec(new BytesSet(68), new BytesSet(16));
            BlMagics[13] = new MapUtils.MagicRec(new BytesSet(17), new BytesSet(4));
            BlMagics[14] = new MapUtils.MagicRec(new BytesSet(17), new BytesSet(64));
        }

        public static void Render(IMap map, Building building)
        {
            ExtRect bldArea = building.Area;

            MapUtils.FillBorder(map, bldArea, 0, (int)TileID.tid_BlockWall);

            foreach (ExtRect block in building.Blocks) {
                MapUtils.FillBorder(map, block, 0, (int)TileID.tid_BlockWall);
            }

            foreach (Room room in building.Rooms) {
                ExtRect area = room.Area;
                MapUtils.FillBorder(map, area, 0, (int)TileID.tid_RoomWall);
            }

            Normalize(map, bldArea);
        }

        private static void Normalize(IMap map, ExtRect rt)
        {
            try {
                int tid0r = (int)TileID.tid_RoomWall;

                for (int y = rt.Top; y <= rt.Bottom; y++) {
                    for (int x = rt.Left; x <= rt.Right; x++) {
                        BaseTile tile = map.GetTile(x, y);

                        int fg = tile.Foreground;

                        if (fg == tid0r) {
                            int magic = MapUtils.GetAdjacentMagic(map, x, y, 0, tid0r, MapUtils.TSL_FORE);
                            if (magic != 0) {
                                for (int offset = 0; offset < RmMagics.Length; offset++) {
                                    if (RmMagics[offset].Main.Contains(magic)) {
                                        tile.SetFore(tid0r, offset);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                int tid0b = (int)TileID.tid_BlockWall;

                for (int y = rt.Top; y <= rt.Bottom; y++) {
                    for (int x = rt.Left; x <= rt.Right; x++) {
                        BaseTile tile = map.GetTile(x, y);

                        int fg = tile.Foreground;

                        if (fg == tid0b) {
                            int magic = MapUtils.GetAdjacentMagic(map, x, y, 0, tid0b, MapUtils.TSL_FORE);
                            int magic_ext = MapUtils.GetAdjacentMagic(map, x, y, 0, tid0r, MapUtils.TSL_FORE);

                            if (magic != 0) {
                                for (int offset = BlMagics.Length - 1; offset >= 0; offset--) {
                                    bool res = BlMagics[offset].Main.Contains(magic);

                                    if (offset > 10) {
                                        res = res && BlMagics[offset].Ext.Contains(magic_ext);
                                    }

                                    if (res) {
                                        tile.SetFore(tid0b, offset);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.Write("BuildingRenderer.normalize(): " + ex.Message);
            }
        }
    }
}
