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
using System.Collections.Generic;
using BSLib;
using MysteriesRL.Game;
using MysteriesRL.Generators;
using MysteriesRL.Maps;
using MysteriesRL.Maps.Buildings;
using MysteriesRL.Maps.Buildings.Features;
using ZRLib.Core;
using ZRLib.Map;

namespace MysteriesRL.Maps
{
    public class BaseRealm : Realm
    {
        private City fCity;

        public City City
        {
            get { return fCity; }
        }


        public BaseRealm(bool underground) : base(true)
        {
        }

        public override string GetLocationName(int px, int py)
        {
            string result = "unknown";

            CityRegion region = fCity.FindRegion(px, py);
            if (region != null) {
                result = fCity.Name;

                if (region is Street) {
                    result += ", " + Convert.ToString(((Street)region).Num) + " street";
                } else if (region is District) {
                    result += ", " + Convert.ToString(((District)region).Num) + " district";

                    Building bld = fCity.FindBuilding(px, py);
                    if (bld != null) {
                        result += ", " + bld.Address;

                        HouseStatus status = bld.Status;
                        var hsRec = MRLData.HouseStatuses[(int)status];
                        result += " (" + hsRec.Name + ")";

                        Room room = bld.FindRoom(px, py);
                        if (room != null) {
                            result += "[" + room.Desc + "]";
                        }
                    }
                }
            }

            return result;
        }

        public override void InitNew(MRLGame gameSpace, int year, IProgressController progressController)
        {
            fTime.Set(year, 7, 1, 20, 0, 0);

            fPlainMap.InitPlainLayer(progressController);

            int mw = fPlainMap.Width;
            int mh = fPlainMap.Height;

            int ctH = RandomHelper.GetBoundedRnd(mh / 2, (int)(mh * 0.66f));
            int ctW = RandomHelper.GetBoundedRnd(mw / 2, (int)(mw * 0.66f));

            int ctX = RandomHelper.GetRandom(mw - ctW);
            int ctY = RandomHelper.GetRandom(mh - ctH);

            ExtRect cityArea = ExtRect.Create(ctX, ctY, ctX + ctW, ctY + ctH);

            fCity = new City(gameSpace, fPlainMap, cityArea);
            CityGenerator civFactory = new CityGenerator(fPlainMap, fCity, progressController);
            civFactory.BuildCity();

            fMinimap = new Minimap(fPlainMap, fCity);

            IList<Building> blds = fCity.Buildings;
            progressController.SetStage(Locale.GetStr(RS.Rs_BuildingsPopulate), blds.Count);
            foreach (Building bld in blds) {
                if (!bld.Populated) {
                    bld.Fill();
                }

                progressController.Complete(0);
            }

            progressController.SetStage(Locale.GetStr(RS.Rs_CellarsGeneration), blds.Count);
            foreach (Building bld in blds) {
                if (bld.Status == HouseStatus.Mansion) {
                    GenCellar(gameSpace, bld);
                }

                progressController.Complete(0);
            }

            // flush features
            EntityList<GameEntity> feats = fPlainMap.Features;
            for (int i = 0; i < feats.Count; i++) {
                GameEntity entity = feats[i];

                if (entity is MapFeature) {
                    MapFeature mf = (MapFeature)entity;

                    fPlainMap.GetTile(mf.PosX, mf.PosY).Foreground = (ushort)mf.TileID;
                }
            }
        }

        private void GenCellar(MRLGame gameSpace, Building bld)
        {
            ExtRect privArea = bld.PrivathandArea;
            if (privArea.IsEmpty()) {
                privArea = bld.Area;
            }

            fUndergroundMap.InitDungeon(privArea, ExtPoint.Empty, true);

            GenStairway(gameSpace, fPlainMap, fUndergroundMap, privArea);
            //GenStairway(fCellarsMap, fDungeonsMap, privArea);
        }

        private void GenStairway(MRLGame gameSpace, IMap map1, IMap map2, ExtRect area)
        {
            ExtPoint src = map1.SearchFreeLocation(area);
            ExtPoint dst = map2.SearchFreeLocation(area);

            if (!src.IsEmpty && !dst.IsEmpty) {
                Stairs stairs = new Stairs(gameSpace, map1, src, dst);
                stairs.Descending = true;
                map1.Features.Add(stairs);
                stairs.Render();

                stairs = new Stairs(gameSpace, map2, dst, src);
                stairs.Descending = false;
                map2.Features.Add(stairs);
                stairs.Render();
            } else {
                Logger.Write("BaseRealm.GenStairway(): null");
            }
        }
    }
}
