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
using System.Drawing;
using BSLib;
using MysteriesRL.Core;
using MysteriesRL.Creatures;
using MysteriesRL.Game.Events;
using MysteriesRL.Generators;
using MysteriesRL.Maps;
using MysteriesRL.Maps.Buildings;
using MysteriesRL.Maps.Buildings.Features;
using ZRLib.Core;
using ZRLib.Map;

namespace MysteriesRL.Game
{
    public sealed class MRLGame : GameSpace, IEventListener
    {
        private readonly Layer fPlainMap;
        private readonly Layer fCellarsMap;
        private readonly Layer fDungeonsMap;

        private readonly IList<TextMessage> fMessages;
        private readonly PlayerController fPlayerController;
        private readonly GameTime fTime;

        private City fCity;

        public IMap fMinimap;

        public MRLGame(object owner)
            : base(owner)
        {
            fPlainMap = new Layer(false, "plain");
            fCellarsMap = new Layer(true, "cellars");
            fDungeonsMap = new Layer(true, "dungeons");

            fMinimap = null;
            fMessages = new List<TextMessage>();
            fPlayerController = new PlayerController();
            fTime = new GameTime();

            ScheduledEventManager.Subscribe(this);
        }

        public IMap PlainMap
        {
            get { return fPlainMap; }
        }

        public IMap CellarsMap
        {
            get { return fCellarsMap; }
        }

        public IMap DungeonsMap
        {
            get { return fDungeonsMap; }
        }

        public City City
        {
            get { return fCity; }
        }

        public PlayerController PlayerController
        {
            get { return fPlayerController; }
        }

        public void InitNew(IProgressController progressController)
        {
            fTime.Set(1890, 0x7, 0x1, 12, 0x0, 0x0);

            fPlainMap.InitPlainLayer(progressController);

            int mw = fPlainMap.Width;
            int mh = fPlainMap.Height;

            int ctH = RandomHelper.GetBoundedRnd(mh / 2, (int)(mh * 0.66f));
            int ctW = RandomHelper.GetBoundedRnd(mw / 2, (int)(mw * 0.66f));

            int ctX = RandomHelper.GetRandom(mw - ctW);
            int ctY = RandomHelper.GetRandom(mh - ctH);

            ExtRect cityArea = ExtRect.Create(ctX, ctY, ctX + ctW, ctY + ctH);

            fDungeonsMap.InitDungeonLayer(cityArea, progressController);

            fCity = new City(this, fPlainMap, cityArea);
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
                if (bld.Status == HouseStatus.hsMansion) {
                    GenCellar(bld);
                }

                progressController.Complete(0);
            }

            // flush features
            EntityList feats = fPlainMap.Features;
            for (int i = 0; i < feats.Count; i++) {
                GameEntity entity = feats.GetItem(i);

                if (entity is MapFeature) {
                    MapFeature mf = (MapFeature)entity;

                    fPlainMap.GetTile(mf.PosX, mf.PosY).Foreground = (ushort)mf.TileID;
                }
            }
        }

        private void GenCellar(Building bld)
        {
            ExtRect privArea = bld.PrivathandArea;
            if (privArea.IsEmpty()) {
                privArea = bld.Area;
            }

            fCellarsMap.InitDungeon(privArea, ExtPoint.Empty, true);

            GenStairway(fPlainMap, fCellarsMap, privArea);
            //GenStairway(fCellarsMap, fDungeonsMap, privArea);
        }

        private void GenStairway(IMap map1, IMap map2, ExtRect area)
        {
            ExtPoint src = map1.SearchFreeLocation(area);
            ExtPoint dst = map2.SearchFreeLocation(area);

            if (!src.IsEmpty && !dst.IsEmpty) {
                Stairs stairs = new Stairs(this, map1, src, dst);
                stairs.Descending = true;
                map1.Features.Add(stairs);
                stairs.Render();

                stairs = new Stairs(this, map2, dst, src);
                stairs.Descending = false;
                map2.Features.Add(stairs);
                stairs.Render();
            } else {
                Logger.Write("Game.genStairway(): null");
            }
        }

        public GameTime Time
        {
            get { return fTime; }
        }

        public string GetLocationName(int px, int py)
        {
            PlayerController playerController = PlayerController;
            Creature player = playerController.Player;

            if (px < 0 && py < 0) {
                px = player.PosX;
                py = player.PosY;
            }

            CityRegion region = fCity.FindRegion(px, py);
            if (region != null) {
                if (region is Street) {
                    return Convert.ToString(((Street)region).Num) + " street";
                } else if (region is District) {
                    string res;
                    res = Convert.ToString(((District)region).Num) + " district";

                    Building bld = fCity.FindBuilding(px, py);
                    if (bld != null) {
                        res += ", " + bld.Address;

                        HouseStatus status = bld.Status;
                        var hsRec = MRLData.HouseStatuses[(int)status];
                        res += " (" + hsRec.Name + ")";

                        Room room = bld.FindRoom(px, py);
                        if (room != null) {
                            res += "[" + room.Desc + "]";
                        }
                    }

                    return res;
                }
            }

            return "unknown";
        }

        public IList<TextMessage> Messages
        {
            get {
                return fMessages;
            }
        }

        public void AddMessage(string text)
        {
            AddMessage(text, Color.LightGray);
        }

        public void AddMessage(string text, Color color)
        {
            TextMessage msg = new TextMessage(text, color);
            fMessages.Add(msg);
        }

        public bool CheckTarget(int mx, int my)
        {
            if (fPlainMap.IsBarrier(mx, my)) {
                return false;
            }
            return true;
        }

        public void Look(int mx, int my)
        {
            bool here = (mx == -1 && my == -1);

            string prefix;
            if (here) {
                prefix = "Current location: ";
            } else {
                prefix = "Location: ";
            }

            string location = prefix + GetLocationName(mx, my);
            AddMessage(location);

            if (!here) {
                Creature creature = (Creature)fPlainMap.FindCreature(mx, my);
                if (creature != null) {
                    AddMessage("You see " + creature.Desc);
                }
            }
        }

        public float LightFactor
        {
            get {
                DayTime dt = fTime.DayTime;
                var timeRec = GameTime.DayTimes[(int)dt];
                return timeRec.RadMod;
            }
        }

        public float LightBrightness
        {
            get {
                DayTime dt = fTime.DayTime;
                var timeRec = GameTime.DayTimes[(int)dt];
                return timeRec.Brightness;
            }
        }

        // every 100 ms
        public void DoTurn()
        {
            fTime.Tick(60);

            PlayerController.DoPathStep();

            PlayerController.Player.DoTurn();
        }

        public void UpdateWater()
        {
            fPlainMap.UpdateWater(PlayerController.Viewport);
        }

        public void OnEvent(Event @event)
        {
            if (@event is VandalismEvent) {
                AddMessage("vandalism!!!", Color.Red);
                fPlayerController.Player.Stats.AddXP(50);
            }
        }

        public IList<GameEntity> GetEntities(ExtPoint pt, int radius)
        {
            return null;
        }
    }
}
