/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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
using MysteriesRL.Core;
using MysteriesRL.Maps;
using MysteriesRL.Maps.Buildings;
using ZRLib.External.BSP;
using ZRLib.Map;

namespace MysteriesRL.Generators
{
    public class CityGenerator
    {
        private readonly IMap fMap;
        private readonly City fCity;
        private readonly IProgressController fProgressController;

        public int DistrictMinSize = 50;
        public int DistrictMaxSize = 75;

        private ExtRect CityArea;
        private ExtPoint CityCenter;
        private int CityRadius;
        private int Rad13;
        private int Rad23;
        private int Rad14;
        private int Rad12;

        public CityGenerator(IMap map, City city, IProgressController progressController)
        {
            fMap = map;
            fCity = city;
            fProgressController = progressController;
        }

        public void SetDistrictsRange(int minSize, int maxSize)
        {
            DistrictMinSize = minSize;
            DistrictMaxSize = maxSize;
        }

        public void BuildCity()
        {
            fProgressController.SetStage(Locale.GetStr(RS.Rs_CityGeneration), 0);

            CityArea = fCity.Area;
            CityCenter = CityArea.GetCenter();
            CityRadius = ((RandomHelper.GetRandom(2) == 0) ? CityArea.GetHeight() : CityArea.GetWidth()) / 2;
            Rad13 = (int)(CityRadius * 0.33f);
            Rad23 = (int)(CityRadius * 0.66f);
            Rad14 = (int)(CityRadius * 0.22f);
            Rad12 = (int)(CityRadius * 0.55f);

            // generate the tree of nodes
            BSPTree tree = new BSPTree(CityArea, DistrictMinSize, DistrictMaxSize, true, SplitCityProc, SplitCityHandler);

            // create list of districts
            IList<BSPNode> leaves = tree.Leaves;
            fProgressController.SetStage(Locale.GetStr(RS.Rs_DistrictsGeneration), leaves.Count);
            foreach (BSPNode node in leaves) {
                ExtRect distrArea = ExtRect.Create(node.X1, node.Y1, node.X2, node.Y2);

                bool res = true;

                //Point distrCenter = distrArea.getCenter();
                //int dist = AuxUtils.Distance(cityCenter, distrCenter);
                //if (dist < rad || (dist >= rad && AuxUtils.getRandom(100) > 25)) {
                if (res) {
                    fCity.AddDistrict(distrArea);
                }
                //}

                fProgressController.Complete(0);
            }

            MakeDistricts();

            // render streets
            IList<Street> streets = fCity.Streets;
            fProgressController.SetStage(Locale.GetStr(RS.Rs_StreetsGeneration), streets.Count);
            foreach (Street street in streets) {
                ExtRect area = street.Area;
                fMap.FillArea(area.Left, area.Top, area.Right, area.Bottom, (int)TileID.tid_Road, false);
                FOV.OpenVisited(fMap, area);

                fProgressController.Complete(0);
            }

            if (MRLData.DEBUG_CITYGEN) {
                FOV.OpenVisited(fMap, CityArea);
            }
        }

        private void SplitCityProc(int x1, int y1, int x2, int y2)
        {
            fCity.AddStreet(ExtRect.Create(x1, y1, x2, y2));
        }

        private void SplitCityHandler(BSPNode node, ref int refSplitWidth)
        {
            if (node.Depth <= 2) {
                refSplitWidth = (RandomHelper.GetRandom(2) == 1) ? 4 : 5;
            } else {
                refSplitWidth = 2 + RandomHelper.GetRandom(2);
            }
        }

        private void MakeDistricts()
        {
            int parksCount = RandomHelper.GetBoundedRnd(2, 5);
            int squaresCount = RandomHelper.GetBoundedRnd(1, 4);
            IList<District> parkCandidates = new List<District>();
            IList<District> squareCandidates = new List<District>();
            IList<District> gyCandidates = new List<District>();

            int rad45 = (int)(CityRadius * 0.8f);

            foreach (District distr in fCity.Districts) {
                ExtPoint dstCtr = distr.Area.GetCenter();
                int dist = MathHelper.Distance(dstCtr, CityCenter);

                if (dist > Rad13 && dist < Rad23) { // parks preparing
                    parkCandidates.Add(distr);
                } else if (dist < Rad23) { // squares preparing
                    squareCandidates.Add(distr);
                }

                if (dist <= Rad14) {
                    distr.Prosperity = Prosperity.pLux;
                } else if (dist < Rad12) {
                    distr.Prosperity = Prosperity.pGood;
                } else {
                    distr.Prosperity = Prosperity.pPoor;
                }

                if (dist > rad45) {
                    bool hasWater = MapUtils.HasTiles(fMap, distr.Area, (int)TileID.tid_Water, false);
                    if (!hasWater) {
                        gyCandidates.Add(distr);
                    }
                }
            }

            for (int i = 1; i <= parksCount; i++) {
                District park = RandomHelper.GetRandomItem(parkCandidates);
                park.Type = DistrictType.dtPark;
            }

            for (int i = 1; i <= squaresCount; i++) {
                District square = RandomHelper.GetRandomItem(squareCandidates);
                square.Type = DistrictType.dtSquare;
            }

            District graveyard = RandomHelper.GetRandomItem(gyCandidates);
            graveyard.Type = DistrictType.dtGraveyard;

            IList<District> districts = fCity.Districts;
            fProgressController.SetStage(Locale.GetStr(RS.Rs_DistrictsBuildingsGeneration), districts.Count);
            foreach (District distr in districts) {
                switch (distr.Type) {
                    case DistrictType.dtDefault:
                        MakeBuildings(distr);
                        break;

                    case DistrictType.dtPark:
                        MakePark(distr);
                        break;

                    case DistrictType.dtSquare:
                        MakeSquare(distr);
                        break;

                    case DistrictType.dtGraveyard:
                        MakeGraveyard(distr);
                        break;
                }

                fProgressController.Complete(0);
            }
        }

        private void MakeBuildings(District distr)
        {
            Prosperity prosp = distr.Prosperity;
            ExtRect area = distr.Area;
            bool hasWater = MapUtils.HasTiles(fMap, area, (int)TileID.tid_Water, false);

            switch (prosp) {
                case Prosperity.pPoor:
                    {
                        ExtRect innerArea = area;
                        //innerArea.Inflate(+1, +1);
                        MapUtils.GenRareTiles(fMap, innerArea, 0.05f, (int)TileID.tid_Tree, (short)TileID.tid_Water, true);
                    }
                    break;

                case Prosperity.pGood:
                    break;

                case Prosperity.pLux:
                    if (hasWater) {
                        prosp = Prosperity.pGood;
                        distr.Prosperity = prosp;
                    }
                    break;
            }

            if (MRLData.DEBUG_DISTRICTS) {
                TileID tile = TileID.tid_DebugDistrictP;
                switch (prosp) {
                    case Prosperity.pPoor:
                        tile = TileID.tid_DebugDistrictP;
                        break;
                    case Prosperity.pGood:
                        tile = TileID.tid_DebugDistrictG;
                        break;
                    case Prosperity.pLux:
                        tile = TileID.tid_DebugDistrictL;
                        break;
                }
                fMap.FillBorder(area.Left, area.Top, area.Right, area.Bottom, (ushort)tile, false);
                area.Inflate(-1, -1);
            }

            // generate the tree of nodes
            var prospRec = MRLData.Prosperities[(int)prosp];
            BSPTree tree = new BSPTree(area, prospRec.MinHouseSize, prospRec.MaxHouseSize, true, null, SplitDistrHandler);

            // create list of buildings
            IList<BSPNode> leaves = tree.Leaves;
            foreach (BSPNode node in leaves) {
                if (prosp != Prosperity.pPoor && node.NodeType == LocationType.ltInner) {
                    Console.WriteLine("inner district's node " + Convert.ToString(distr.Num));
                } else {
                    ExtRect bldArea = ExtRect.Create(node.X1, node.Y1, node.X2, node.Y2);
                    ExtPoint bldCenter = bldArea.GetCenter();
                    int dist = MathHelper.Distance(bldCenter, CityCenter);

                    bool res = (dist <= CityRadius);
                    if (!res) {
                        //int chance = AuxUtils.getRandom(15) + (int) ((1 - ((float) cityRadius / dist)) * 100);
                        res = (RandomHelper.GetRandom(100) < 50);
                    } else {
                        if (dist > Rad23) {
                            res = (RandomHelper.GetRandom(100) < 95);
                        }
                    }

                    if (res) {
                        res = !MapUtils.HasTiles(fMap, bldArea, (int)TileID.tid_Water, false);
                    }

                    if (res) {
                        GenBuilding(distr, bldArea, prosp, node.NodeType);
                    }
                }
            }
        }

        private void SplitDistrHandler(BSPNode node, ref int refSplitWidth)
        {
            refSplitWidth = 1;
        }

        private void SetBuildingStreet(District distr, Building bld, ExtRect area)
        {
            Street result = null;

            ExtRect distrArea = distr.Area;
            List<BuildingSide> sides = new List<BuildingSide>();
            sides.Add(new BuildingSide(SideType.stTop, Math.Abs(distrArea.Top - area.Top)));
            sides.Add(new BuildingSide(SideType.stLeft, Math.Abs(distrArea.Left - area.Left)));
            sides.Add(new BuildingSide(SideType.stRight, Math.Abs(distrArea.Right - area.Right)));
            sides.Add(new BuildingSide(SideType.stBottom, Math.Abs(distrArea.Bottom - area.Bottom)));

            sides.Sort(CompareSides);

            BuildingSide bSide = sides[0];
            ExtPoint strPt;
            switch (bSide.Side) {
                case SideType.stTop:
                    strPt = new ExtPoint(RandomHelper.GetBoundedRnd(area.Left, area.Right), distrArea.Top - 1);
                    break;
                case SideType.stRight:
                    strPt = new ExtPoint(distrArea.Right + 1, RandomHelper.GetBoundedRnd(area.Top, area.Bottom));
                    break;
                case SideType.stBottom:
                    strPt = new ExtPoint(RandomHelper.GetBoundedRnd(area.Left, area.Right), distrArea.Bottom + 1);
                    break;
                case SideType.stLeft:
                    strPt = new ExtPoint(distrArea.Left - 1, RandomHelper.GetBoundedRnd(area.Top, area.Bottom));
                    break;
                default:
                    strPt = ExtPoint.Empty;
                    break;
            }

            CityRegion region = fCity.FindRegion(strPt.X, strPt.Y);
            if (region != null && region is Street) {
                result = (Street)region;
                bld.SetStreet(result, bSide.Side);
            } else {
                bSide = RandomHelper.GetRandomItem(sides);
                bld.SetStreet(null, bSide.Side);
            }
        }

        private int CompareSides(BuildingSide o1, BuildingSide o2)
        {
            if (o1.Dist < o2.Dist) {
                return -1;
            }
            if (o1.Dist > o2.Dist) {
                return 1;
            }
            return 0;
        }

        private void GenBuilding(District distr, ExtRect area, Prosperity prosp, LocationType location)
        {
            int arH = 0, arW = 0, bH = 0, bW = 0;
            try {
                ExtRect innerArea = area;

                ExtRect privathandArea = ExtRect.Empty;
                if (prosp != Prosperity.pPoor) {
                    privathandArea = area;
                    fMap.FillBorder(area.Left, area.Top, area.Right, area.Bottom, (int)TileID.tid_HouseBorder, true);
                }

                innerArea.Inflate(-1, -1);

                arH = innerArea.GetHeight();
                arW = innerArea.GetWidth();

                var prospRec = MRLData.Prosperities[(int)prosp];
                bH = RandomHelper.GetBoundedRnd(arH - prospRec.BuildSpaces, arH);
                bW = RandomHelper.GetBoundedRnd(arW - prospRec.BuildSpaces, arW);

                int dx = arW - bW;
                int dy = arH - bH;

                dx = (dx == 0) ? 0 : RandomHelper.GetRandom(dx);
                dy = (dy == 0) ? 0 : RandomHelper.GetRandom(dy);

                int bX = innerArea.Left + dx;
                int bY = innerArea.Top + dy;

                ExtRect bldArea = ExtRect.Create(bX, bY, bX + bW - 1, bY + bH - 1);
                Building bld = fCity.AddBuilding(privathandArea, bldArea, prosp);
                bld.Location = location;
                SetBuildingStreet(distr, bld, area);

                ApartmentGenerator apartmentFactory = new ApartmentGenerator(bld);
                apartmentFactory.Build();
            } catch (Exception) {
                Console.WriteLine(Convert.ToString(arH + arW + bH + bW));
            }
        }

        private void MakePark(District distr)
        {
            ExtRect area = distr.Area;
            fMap.FillArea(area.Left, area.Top, area.Right, area.Bottom, (int)TileID.tid_Grass, false);
            //area.Inflate(+1, +1);
            MapUtils.GenRareTiles(fMap, area, 0.4f, (int)TileID.tid_Tree, (short)TileID.tid_Water, true);
        }

        private void MakeSquare(District distr)
        {
            ExtRect area = distr.Area;
            fMap.FillArea(area.Left, area.Top, area.Right, area.Bottom, (int)TileID.tid_Square, false);
        }

        private void MakeGraveyard(District distr)
        {
            ExtRect area = distr.Area;
            fMap.FillArea(area.Left, area.Top, area.Right, area.Bottom, (int)TileID.tid_Grass, false);

            // generate the tree of nodes
            BSPTree tree = new BSPTree(area, 3, 7, true, null, SplitDistrHandler);

            // create list of graves
            IList<BSPNode> leaves = tree.Leaves;
            foreach (BSPNode node in leaves) {
                ExtRect gArea = ExtRect.Create(node.X1, node.Y1, node.X2, node.Y2);

                fMap.FillArea(gArea.Left, gArea.Top, gArea.Right, gArea.Bottom, (int)TileID.tid_Ground, false);

                int arh = gArea.GetHeight();
                int arw = gArea.GetWidth();

                Axis axis = (arh < arw) ? Axis.axVert : Axis.axHorz;
                int count = (axis == Axis.axHorz) ? arh / 3 : arw / 3;
                for (int i = 0; i < count; i++) {
                    int gx, gy;
                    switch (axis) {
                        case Axis.axHorz:
                            gx = gArea.Left + 1;
                            gy = gArea.Top + (i * 3) + 1;
                            fMap.GetTile(gx, gy).Foreground = (ushort)TileID.tid_Grave;
                            fMap.GetTile(gx + 1, gy).Foreground = (ushort)TileID.tid_Grave;
                            break;

                        case Axis.axVert:
                            gx = gArea.Left + (i * 3) + 1;
                            gy = gArea.Top + 1;
                            fMap.GetTile(gx, gy).Foreground = (ushort)TileID.tid_Grave;
                            fMap.GetTile(gx, gy + 1).Foreground = (ushort)TileID.tid_Grave;
                            break;
                    }
                }
            }
        }

        private class BuildingSide
        {
            public SideType Side;
            public int Dist;

            public BuildingSide(SideType side, int dist)
            {
                Side = side;
                Dist = dist;
            }
        }
    }
}
