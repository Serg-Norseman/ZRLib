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

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.external.bsp.BSPNode;
import jzrlib.external.bsp.BSPTree;
import jzrlib.external.bsp.LocationType;
import jzrlib.external.bsp.SideType;
import jzrlib.map.FOV;
import jzrlib.map.IMap;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.RefObject;
import mrl.core.GlobalData;
import mrl.core.IProgressController;
import mrl.core.Locale;
import mrl.core.RS;
import mrl.maps.Axis;
import mrl.maps.TileID;
import mrl.maps.Utils;
import mrl.maps.buildings.Building;
import mrl.maps.city.City;
import mrl.maps.city.CityRegion;
import mrl.maps.city.District;
import mrl.maps.city.DistrictType;
import mrl.maps.city.Prosperity;
import mrl.maps.city.Street;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class CityGenerator
{
    private final IMap fMap;
    private final City fCity;
    private final IProgressController fProgressController;
    
    public int DistrictMinSize = 50;
    public int DistrictMaxSize = 75;

    // <editor-fold defaultstate="collapsed" desc="Generation parameters">

    private Rect cityArea;
    private Point cityCenter;
    private int cityRadius;
    private int rad13;
    private int rad23;
    private int rad14;
    private int rad12;
    
    // </editor-fold>
    
    public CityGenerator(IMap map, City city, IProgressController progressController)
    {
        this.fMap = map;
        this.fCity = city;
        this.fProgressController = progressController;
    }

    public final void setDistrictsRange(int minSize, int maxSize)
    {
        this.DistrictMinSize = minSize;
        this.DistrictMaxSize = maxSize;
    }

    public final void buildCity()
    {
        fProgressController.setStage(Locale.getStr(RS.rs_CityGeneration), 0);
        
        this.cityArea = fCity.getArea();
        this.cityCenter = cityArea.getCenter();
        this.cityRadius = ((AuxUtils.getRandom(2) == 0) ? cityArea.getHeight() : cityArea.getWidth()) / 2;
        this.rad13 = (int) (this.cityRadius * 0.33f);
        this.rad23 = (int) (this.cityRadius * 0.66f);
        this.rad14 = (int) (this.cityRadius * 0.22f);
        this.rad12 = (int) (this.cityRadius * 0.55f);

        // generate the tree of nodes
        BSPTree tree = new BSPTree(cityArea, this.DistrictMinSize, this.DistrictMaxSize, true, this::splitCityProc, this::splitCityHandler);

        // create list of districts
        List<BSPNode> leaves = tree.getLeaves();
        fProgressController.setStage(Locale.getStr(RS.rs_DistrictsGeneration), leaves.size());
        for (BSPNode node : leaves) {
            Rect distrArea = new Rect(node.x1, node.y1, node.x2, node.y2);

            boolean res = true;
            
            //Point distrCenter = distrArea.getCenter();
            //int dist = AuxUtils.Distance(cityCenter, distrCenter);
            //if (dist < rad || (dist >= rad && AuxUtils.getRandom(100) > 25)) {
            if (res) {
                fCity.addDistrict(distrArea);
            }
            //}
            
            fProgressController.complete(0);
        }

        this.makeDistricts();

        // render streets
        List<Street> streets = fCity.getStreets();
        fProgressController.setStage(Locale.getStr(RS.rs_StreetsGeneration), streets.size());
        for (Street street : streets) {
            Rect area = street.getArea();
            fMap.fillArea(area.Left, area.Top, area.Right, area.Bottom, TileID.tid_Road.Value, false);
            FOV.openVisited(fMap, area);
            
            fProgressController.complete(0);
        }
        
        if (GlobalData.DEBUG_CITYGEN) {
            FOV.openVisited(fMap, cityArea);
        }
    }
    
    private void splitCityProc(int x1, int y1, int x2, int y2)
    {
        fCity.addStreet(new Rect(x1, y1, x2, y2));
    }

    private void splitCityHandler(BSPNode node, RefObject<Integer> refSplitWidth)
    {
        if (node.depth <= 2) {
            refSplitWidth.argValue = (AuxUtils.getRandom(2) == 1) ? 4 : 5;
        } else {
            refSplitWidth.argValue = 2 + AuxUtils.getRandom(2);
        }
    }

    private void makeDistricts()
    {
        int parksCount = AuxUtils.getBoundedRnd(2, 5);
        int squaresCount = AuxUtils.getBoundedRnd(1, 4);
        List<District> parkCandidates = new ArrayList<>();
        List<District> squareCandidates = new ArrayList<>();
        List<District> gyCandidates = new ArrayList<>();

        int rad45 = (int) (this.cityRadius * 0.8f);
        
        for (District distr : fCity.getDistricts()) {
            Point dstCtr = distr.getArea().getCenter();
            int dist = AuxUtils.distance(dstCtr, cityCenter);

            if (dist > this.rad13 && dist < this.rad23) { // parks preparing
                parkCandidates.add(distr);
            } else if (dist < this.rad23) { // squares preparing
                squareCandidates.add(distr);
            }

            if (dist <= this.rad14) {
                distr.setProsperity(Prosperity.pLux);
            } else if (dist < this.rad12) {
                distr.setProsperity(Prosperity.pGood);
            } else {
                distr.setProsperity(Prosperity.pPoor);
            }
            
            if (dist > rad45) {
                boolean hasWater = Utils.hasTiles(this.fMap, distr.getArea(), TileID.tid_Water.Value, false);
                if (!hasWater) {
                    gyCandidates.add(distr);
                }
            }
        }

        for (int i = 1; i <= parksCount; i++) {
            District park = AuxUtils.getRandomItem(parkCandidates);
            park.setType(DistrictType.dtPark);
        }

        for (int i = 1; i <= squaresCount; i++) {
            District square = AuxUtils.getRandomItem(squareCandidates);
            square.setType(DistrictType.dtSquare);
        }

        District graveyard = AuxUtils.getRandomItem(gyCandidates);
        graveyard.setType(DistrictType.dtGraveyard);

        List<District> districts = fCity.getDistricts();
        fProgressController.setStage(Locale.getStr(RS.rs_DistrictsBuildingsGeneration), districts.size());
        for (District distr : districts) {
            switch (distr.getType()) {
                case dtDefault:
                    this.makeBuildings(distr);
                    break;

                case dtPark:
                    this.makePark(distr);
                    break;

                case dtSquare:
                    this.makeSquare(distr);
                    break;
                    
                case dtGraveyard:
                    this.makeGraveyard(distr);
                    break;
            }
            
            fProgressController.complete(0);
        }
    }

    private void makeBuildings(District distr)
    {
        Prosperity prosp = distr.getProsperity();
        Rect area = distr.getArea().clone();
        boolean hasWater = Utils.hasTiles(this.fMap, area, TileID.tid_Water.Value, false);
        
        switch (prosp) {
            case pPoor:
                Utils.genRareTiles(this.fMap, area, 0.05f, TileID.tid_Tree.Value, true);
                break;

            case pGood:
                break;

            case pLux:
                if (hasWater) {
                    prosp = Prosperity.pGood;
                    distr.setProsperity(prosp);
                }
                break;
        }

        if (GlobalData.DEBUG_DISTRICTS) {
            TileID tile = TileID.tid_DebugDistrictP;
            switch (prosp) {
                case pPoor:
                    tile = TileID.tid_DebugDistrictP;
                    break;
                case pGood:
                    tile = TileID.tid_DebugDistrictG;
                    break;
                case pLux:
                    tile = TileID.tid_DebugDistrictL;
                    break;
            }
            fMap.fillBorder(area.Left, area.Top, area.Right, area.Bottom, tile.Value, false);
            area.inflate(-1, -1);
        }
        
        // generate the tree of nodes
        BSPTree tree = new BSPTree(area, prosp.minHouseSize, prosp.maxHouseSize, true, null, this::splitDistrHandler);

        // create list of buildings
        List<BSPNode> leaves = tree.getLeaves();
        for (BSPNode node : leaves) {
            if (prosp != Prosperity.pPoor && node.nodeType == LocationType.ltInner) {
                System.out.println("inner district's node " + String.valueOf(distr.Num));
            } else {            
                Rect bldArea = new Rect(node.x1, node.y1, node.x2, node.y2);
                Point bldCenter = bldArea.getCenter();
                int dist = AuxUtils.distance(bldCenter, this.cityCenter);
                
                boolean res = (dist <= this.cityRadius);
                if (!res) {
                    //int chance = AuxUtils.getRandom(15) + (int) ((1 - ((float) this.cityRadius / dist)) * 100);
                    res = (AuxUtils.getRandom(100) < 50);
                } else {
                    if (dist > this.rad23) {
                        res = (AuxUtils.getRandom(100) < 95);
                    }
                }
                
                if (res) {
                    res = !Utils.hasTiles(this.fMap, bldArea, TileID.tid_Water.Value, false);
                }
                
                if (res) {
                    this.genBuilding(distr, bldArea, prosp, node.nodeType);
                }
            }
        }
    }

    private void splitDistrHandler(BSPNode node, RefObject<Integer> refSplitWidth)
    {
        refSplitWidth.argValue = 1;
    }

    private void setBuildingStreet(District distr, Building bld, Rect area)
    {
        Street result = null;
        
        Rect distrArea = distr.getArea();
        ArrayList<BuildingSide> sides = new ArrayList<>();
        sides.add(new BuildingSide(SideType.stTop, Math.abs(distrArea.Top - area.Top)));
        sides.add(new BuildingSide(SideType.stLeft, Math.abs(distrArea.Left - area.Left)));
        sides.add(new BuildingSide(SideType.stRight, Math.abs(distrArea.Right - area.Right)));
        sides.add(new BuildingSide(SideType.stBottom, Math.abs(distrArea.Bottom - area.Bottom)));

        sides.sort(new Comparator<BuildingSide>()
        {
            @Override
            public int compare(BuildingSide o1, BuildingSide o2)
            {
                if (o1.Dist < o2.Dist) {
                    return -1;
                }
                if (o1.Dist > o2.Dist) {
                    return 1;
                }
                return 0;
            }
        });
        
        BuildingSide bSide = sides.get(0);
        Point strPt;
        switch (bSide.Side) {
            case stTop:
                strPt = new Point(AuxUtils.getBoundedRnd(area.Left, area.Right), distrArea.Top - 1);
                break;
            case stRight: 
                strPt = new Point(distrArea.Right + 1, AuxUtils.getBoundedRnd(area.Top, area.Bottom));
                break;
            case stBottom:
                strPt = new Point(AuxUtils.getBoundedRnd(area.Left, area.Right), distrArea.Bottom + 1);
                break;
            case stLeft:
                strPt = new Point(distrArea.Left - 1, AuxUtils.getBoundedRnd(area.Top, area.Bottom));
                break;
            default:
                strPt = null;
        }
        
        CityRegion region = this.fCity.findRegion(strPt.X, strPt.Y);
        if (region != null && region instanceof Street) {
            result = (Street) region;
            bld.setStreet(result, bSide.Side);
        } else {
            bSide = AuxUtils.getRandomItem(sides);
            bld.setStreet(null, bSide.Side);
        }
    }
    
    private void genBuilding(District distr, Rect area, Prosperity prosp, LocationType location)
    {
        int arH = 0, arW = 0, bH = 0, bW = 0;
        try {
            Rect innerArea = area.clone();
            
            Rect privathandArea = null;
            if (prosp != Prosperity.pPoor) {
                privathandArea = area.clone();
                this.fMap.fillBorder(area.Left, area.Top, area.Right, area.Bottom, TileID.tid_HouseBorder.Value, true);
            }

            innerArea.inflate(-1, -1);

            arH = innerArea.getHeight();
            arW = innerArea.getWidth();

            bH = AuxUtils.getBoundedRnd(arH - prosp.buildSpaces, arH);
            bW = AuxUtils.getBoundedRnd(arW - prosp.buildSpaces, arW);

            int dx = arW - bW;
            int dy = arH - bH;

            dx = (dx == 0) ? 0 : AuxUtils.getRandom(dx);
            dy = (dy == 0) ? 0 : AuxUtils.getRandom(dy);

            int bX = innerArea.Left + dx;
            int bY = innerArea.Top + dy;

            Rect bldArea = new Rect(bX, bY, bX + bW - 1, bY + bH - 1);
            Building bld = this.fCity.addBuilding(privathandArea, bldArea, prosp);
            bld.Location = location;
            this.setBuildingStreet(distr, bld, area);
            
            ApartmentGenerator apartmentFactory = new ApartmentGenerator(bld);
            apartmentFactory.build();
        } catch (Exception ex) {
            System.out.println(String.valueOf(arH + arW + bH + bW));
        }
    }

    private void makePark(District distr)
    {
        Rect area = distr.getArea();
        this.fMap.fillArea(area.Left, area.Top, area.Right, area.Bottom, TileID.tid_Grass.Value, false);
        Utils.genRareTiles(this.fMap, area, 0.4f, TileID.tid_Tree.Value, true);
    }

    private void makeSquare(District distr)
    {
        Rect area = distr.getArea();
        this.fMap.fillArea(area.Left, area.Top, area.Right, area.Bottom, TileID.tid_Square.Value, false);
    }

    private void makeGraveyard(District distr)
    {
        Rect area = distr.getArea().clone();
        this.fMap.fillArea(area.Left, area.Top, area.Right, area.Bottom, TileID.tid_Grass.Value, false);

        // generate the tree of nodes
        BSPTree tree = new BSPTree(area, 3, 7, true, null, this::splitDistrHandler);

        // create list of graves
        List<BSPNode> leaves = tree.getLeaves();
        for (BSPNode node : leaves) {
            Rect gArea = new Rect(node.x1, node.y1, node.x2, node.y2);
            
            this.fMap.fillArea(gArea.Left, gArea.Top, gArea.Right, gArea.Bottom, TileID.tid_Ground.Value, false);

            int arh = gArea.getHeight();
            int arw = gArea.getWidth();
            
            Axis axis = (arh < arw) ? Axis.axVert : Axis.axHorz;
            int count = (axis == Axis.axHorz) ? arh / 3 : arw / 3;
            for (int i = 0; i < count; i++) {
                int gx, gy;
                switch (axis) {
                    case axHorz:
                        gx = gArea.Left + 1;
                        gy = gArea.Top + (i * 3) + 1;
                        this.fMap.getTile(gx, gy).Foreground = (short) TileID.tid_Grave.Value;
                        this.fMap.getTile(gx + 1, gy).Foreground = (short) TileID.tid_Grave.Value;
                        break;

                    case axVert:
                        gx = gArea.Left + (i * 3) + 1;
                        gy = gArea.Top + 1;
                        this.fMap.getTile(gx, gy).Foreground = (short) TileID.tid_Grave.Value;
                        this.fMap.getTile(gx, gy + 1).Foreground = (short) TileID.tid_Grave.Value;
                        break;
                }
            }
        }
    }

    // <editor-fold defaultstate="collapsed" desc="Aux classes">
    
    private static class BuildingSide
    {
        public SideType Side;
        public int Dist;
        
        public BuildingSide(SideType side, int dist)
        {
            this.Side = side;
            this.Dist = dist;
        }
    }
    
    // </editor-fold>
}
