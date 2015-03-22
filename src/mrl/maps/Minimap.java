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
package mrl.maps;

import java.util.HashMap;
import java.util.Map;
import jzrlib.core.Rect;
import jzrlib.map.BaseTile;
import jzrlib.map.IMap;
import mrl.maps.buildings.Building;
import mrl.maps.city.City;
import mrl.maps.city.District;
import mrl.maps.city.DistrictType;
import mrl.maps.city.Street;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class Minimap extends BaseMap
{
    public static final int SCALE = 10;
    
    public Minimap(IMap source, City city)
    {
        super(source.getHeight() / SCALE, source.getWidth() / SCALE);

        this.makePlane(source, city);
        this.makeCity(source, city);
    }

    public final void makePlane(IMap source, City city)
    {
        Map<Integer, Integer> bgMap = new HashMap<>();
        Map<Integer, Integer> fgMap = new HashMap<>();

        for (int y = 0; y < this.getHeight(); y++) {
            for (int x = 0; x < this.getWidth(); x++) {
                bgMap.clear();
                fgMap.clear();
                
                for (int yy = 0; yy < SCALE; yy++) {
                    for (int xx = 0; xx < SCALE; xx++) {
                        int sx = x * 10 + xx;
                        int sy = y * 10 + yy;
                        BaseTile tile = source.getTile(sx, sy);

                        int srcTile = tile.Background;
                        Integer count = bgMap.get(srcTile);
                        if (count != null) {
                            count++;
                        } else {
                            count = 1;
                        }
                        bgMap.put(srcTile, count);

                        srcTile = tile.Foreground;
                        count = fgMap.get(srcTile);
                        if (count != null) {
                            count++;
                        } else {
                            count = 1;
                        }
                        fgMap.put(srcTile, count);
                    }
                }
                
                int maxBID = -1;
                int maxB = 0;
                for (Map.Entry<Integer, Integer> entry : bgMap.entrySet()) {
                    TileID tid = TileID.forValue(entry.getKey());
                    
                    if (entry.getKey() == TileID.tid_Road.Value) {
                        maxBID = entry.getKey();
                        break;
                    } else {
                        if (tid.MiniMap && entry.getValue() > maxB) {
                            maxB = entry.getValue();
                            maxBID = entry.getKey();
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
                    this.getTile(x, y).Background = (short) maxBID;
                }

                if (maxFID >= 0) {
                    this.getTile(x, y).Foreground = (short) maxFID;
                }
            }
        }
    }

    public final void makeCity(IMap source, City city)
    {
        for (Street street : city.getStreets()) {
            Rect srcRect = street.getArea().clone();
            Rect dstRect = new Rect(srcRect.Left / 10, srcRect.Top / 10, srcRect.Right / 10, srcRect.Bottom / 10);
 
            this.fillArea(dstRect.Left, dstRect.Top, dstRect.Right, dstRect.Bottom, TileID.tid_Road.Value, false);
        }
        
        for (Building bld : city.getBuildings()) {
            Rect srcRect = bld.getArea().clone();
            Rect dstRect = new Rect(srcRect.Left / 10, srcRect.Top / 10, srcRect.Right / 10, srcRect.Bottom / 10);
 
            for (int yy = dstRect.Top; yy <= dstRect.Bottom; yy++) {
                for (int xx = dstRect.Left; xx <= dstRect.Right; xx++) {
                    BaseTile tile = this.getTile(xx, yy);
                    if (tile != null) {
                        /*if (fg) {
                            tile.Foreground = (short) tid;
                        } else {
                            tile.Background = (short) tid;
                        }*/
                        
                        if (tile.Background != TileID.tid_Road.Value) {
                            tile.Foreground = (short) TileID.tid_HouseWall.Value;
                        }
                    }
                    //this.setTile(xx, yy, tid, fg);
                }
            }

            //this.fillArea(dstRect.Left, dstRect.Top, dstRect.Right, dstRect.Bottom, TileID.tid_HouseFloor.Value, false);
            //this.fillBorder(dstRect.Left, dstRect.Top, dstRect.Right, dstRect.Bottom, TileID.tid_HouseWall.Value, true);
        }
        
        for (District distr : city.getDistricts()) {
            TileID tid;
            if (distr.getType() == DistrictType.dtPark) {
                tid = TileID.tid_Grass;
            } else if (distr.getType() == DistrictType.dtSquare) {
                tid = TileID.tid_Square;
            } else {
                continue;
            }
            
            Rect srcRect = distr.getArea().clone();
            Rect dstRect = new Rect(srcRect.Left / 10, srcRect.Top / 10, srcRect.Right / 10, srcRect.Bottom / 10);
 
            for (int yy = dstRect.Top; yy <= dstRect.Bottom; yy++) {
                for (int xx = dstRect.Left; xx <= dstRect.Right; xx++) {
                    BaseTile tile = this.getTile(xx, yy);
                    if (tile != null) {
                        if (tile.Background != TileID.tid_Road.Value) {
                            tile.Background = (short) tid.Value;
                        }
                    }
                }
            }
        }
    }
}
