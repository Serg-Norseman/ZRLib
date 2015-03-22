/*
 *  "MysteriesRL", Java Roguelike game.
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
package mrl.generators;

import jzrlib.core.BytesSet;
import jzrlib.core.Rect;
import jzrlib.map.BaseTile;
import jzrlib.map.IMap;
import jzrlib.map.TileTransformer;
import jzrlib.utils.Logger;
import mrl.maps.TileID;
import mrl.maps.Utils;
import mrl.maps.buildings.Building;
import mrl.maps.buildings.Room;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class BuildingRenderer
{
    private static final TileTransformer.MagicRec[] RmMagics;
    private static final TileTransformer.MagicRec[] BlMagics;

    static {
        RmMagics = new TileTransformer.MagicRec[11];
        RmMagics[0] = new TileTransformer.MagicRec(new BytesSet(20), new BytesSet());
        RmMagics[1] = new TileTransformer.MagicRec(new BytesSet(80), new BytesSet());
        RmMagics[2] = new TileTransformer.MagicRec(new BytesSet(5), new BytesSet());
        RmMagics[3] = new TileTransformer.MagicRec(new BytesSet(65), new BytesSet());
        RmMagics[4] = new TileTransformer.MagicRec(new BytesSet(68, 78, 228, 70, 76, 196, 100, 204, 102, 64, 4), new BytesSet()); // -
        RmMagics[5] = new TileTransformer.MagicRec(new BytesSet(17, 57, 147, 145, 19, 49, 25, 153, 51, 1, 16), new BytesSet()); // |
        RmMagics[6] = new TileTransformer.MagicRec(new BytesSet(69, 77, 101), new BytesSet());
        RmMagics[7] = new TileTransformer.MagicRec(new BytesSet(84, 212, 86), new BytesSet());
        RmMagics[8] = new TileTransformer.MagicRec(new BytesSet(21, 149, 53), new BytesSet());
        RmMagics[9] = new TileTransformer.MagicRec(new BytesSet(81, 83, 89), new BytesSet());
        RmMagics[10] = new TileTransformer.MagicRec(new BytesSet(85), new BytesSet());

        BlMagics = new TileTransformer.MagicRec[15];
        BlMagics[0] = new TileTransformer.MagicRec(new BytesSet(20), new BytesSet(0));
        BlMagics[1] = new TileTransformer.MagicRec(new BytesSet(80), new BytesSet(0));
        BlMagics[2] = new TileTransformer.MagicRec(new BytesSet(5), new BytesSet(0));
        BlMagics[3] = new TileTransformer.MagicRec(new BytesSet(65), new BytesSet(0));
        BlMagics[4] = new TileTransformer.MagicRec(new BytesSet(68, 78, 228, 70, 76, 196, 100, 204, 102, 64, 4, 198, 108), new BytesSet(0)); // -
        BlMagics[5] = new TileTransformer.MagicRec(new BytesSet(17, 57, 147, 145, 19, 49, 25, 153, 51, 1, 16, 27, 177), new BytesSet(0)); // |
        BlMagics[6] = new TileTransformer.MagicRec(new BytesSet(69, 77, 101), new BytesSet(0));
        BlMagics[7] = new TileTransformer.MagicRec(new BytesSet(84, 212, 86), new BytesSet(0));
        BlMagics[8] = new TileTransformer.MagicRec(new BytesSet(21, 149, 53), new BytesSet(0));
        BlMagics[9] = new TileTransformer.MagicRec(new BytesSet(81, 83, 89), new BytesSet(0));
        BlMagics[10] = new TileTransformer.MagicRec(new BytesSet(85), new BytesSet(0));
        BlMagics[11] = new TileTransformer.MagicRec(new BytesSet(68), new BytesSet(1));
        BlMagics[12] = new TileTransformer.MagicRec(new BytesSet(68), new BytesSet(16));
        BlMagics[13] = new TileTransformer.MagicRec(new BytesSet(17), new BytesSet(4));
        BlMagics[14] = new TileTransformer.MagicRec(new BytesSet(17), new BytesSet(64));
    }

    public static final void render(IMap map, Building building)
    {
        Rect bldArea = building.getArea();
        
        Utils.fillBorder(map, bldArea, 0, TileID.tid_BlockWall.Value);

        for (Rect block : building.getBlocks()) {
            Utils.fillBorder(map, block, 0, TileID.tid_BlockWall.Value);
        }

        for (Room room : building.getRooms()) {
            Rect area = room.getArea();
            Utils.fillBorder(map, area, 0, TileID.tid_RoomWall.Value);
        }
        
        BuildingRenderer.normalize(map, bldArea);
    }

    private static void normalize(IMap map, Rect rt)
    {
        try {
            int tid0r = TileID.tid_RoomWall.Value;
            
            for (int y = rt.Top; y <= rt.Bottom; y++) {
                for (int x = rt.Left; x <= rt.Right; x++) {
                    BaseTile tile = map.getTile(x, y);
                    
                    int fg = tile.Foreground;

                    if (fg == tid0r) {
                        int magic = TileTransformer.getAdjacentMagic(map, x, y, 0, tid0r, TileTransformer.TSL_FORE);
                        if (magic != 0) {
                            for (int offset = 0; offset < RmMagics.length; offset++) {
                                if (RmMagics[offset].main.contains(magic)) {
                                    tile.setFore(tid0r, offset);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            int tid0b = TileID.tid_BlockWall.Value;

            for (int y = rt.Top; y <= rt.Bottom; y++) {
                for (int x = rt.Left; x <= rt.Right; x++) {
                    BaseTile tile = map.getTile(x, y);
                    
                    int fg = tile.Foreground;

                    if (fg == tid0b) {
                        int magic = TileTransformer.getAdjacentMagic(map, x, y, 0, tid0b, TileTransformer.TSL_FORE);
                        int magic_ext = TileTransformer.getAdjacentMagic(map, x, y, 0, tid0r, TileTransformer.TSL_FORE);
                        
                        if (magic != 0) {
                            for (int offset = BlMagics.length - 1; offset >= 0; offset--) {
                                boolean res = BlMagics[offset].main.contains(magic);
                                
                                if (offset > 10) {
                                    res = res && BlMagics[offset].ext.contains(magic_ext);
                                }
                                
                                if (res) {
                                    tile.setFore(tid0b, offset);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        } catch (Exception ex) {
            Logger.write("BuildingRenderer.normalize(): " + ex.getMessage());
        }
    }
}
