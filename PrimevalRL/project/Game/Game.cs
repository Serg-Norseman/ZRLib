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
using System.Collections.Generic;
using BSLib;
using PrimevalRL.Creatures;
using PrimevalRL.Data;
using PrimevalRL.Game.Events;
using PrimevalRL.Maps;
using ZRLib.Core;
using ZRLib.Engine;

namespace PrimevalRL.Game
{
    public sealed class MRLGame : GameSpace, IEventListener
    {
        private Realm fCurrentRealm;
        private readonly Realm fBaseRealm;

        private readonly IList<TextMessage> fMessages;
        private readonly PlayerController fPlayerController;
        private readonly IList<Realm> fRealms;

        private int fTick;

        public DataLoader DataLoader;

        public Realm CurrentRealm
        {
            get { return fCurrentRealm; }
            set { fCurrentRealm = value; }
        }

        public PlayerController PlayerController
        {
            get { return fPlayerController; }
        }

        public IList<TextMessage> Messages
        {
            get { return fMessages; }
        }


        public MRLGame(object owner)
            : base(owner)
        {
            fBaseRealm = new BaseRealm(true);
            fCurrentRealm = fBaseRealm;

            fMessages = new List<TextMessage>();
            fPlayerController = new PlayerController();
            fRealms = new List<Realm>();

            DataLoader = new DataLoader();
            DataLoader.LoadCreatures(PRLWin.GetAppPath() + "resources\\data\\creatures.yml");
            DataLoader.LoadItems(PRLWin.GetAppPath() + "resources\\data\\items.yml");
            DataLoader.LoadGeoAges(PRLWin.GetAppPath() + "resources\\data\\timeline_geo.csv");

            ScheduledEventManager.Subscribe(this);
        }

        public void InitBaseRealm(IProgressController progressController)
        {
            fCurrentRealm.InitNew(this, 1890, progressController);
        }

        public Realm InitNewRealm(IProgressController progressController)
        {
            int year = RandomHelper.GetBoundedRnd(-2600000, 0);

            Realm realm = new Realm(false);
            realm.InitNew(this, year, progressController);
            fRealms.Add(realm);

            return realm;
        }

        public string GetLocationName(int px, int py)
        {
            Creature player = fPlayerController.Player;
            if (px < 0 && py < 0) {
                px = player.PosX;
                py = player.PosY;
            }

            return fCurrentRealm.GetLocationName(px, py);
        }

        public void AddMessage(string text)
        {
            AddMessage(text, Colors.LightGray);
        }

        public void AddMessage(string text, int color)
        {
            TextMessage msg = new TextMessage(text, color);
            fMessages.Add(msg);
        }

        public bool CheckTarget(int mx, int my)
        {
            if (fCurrentRealm.PlainMap.IsBarrier(mx, my)) {
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
                Creature creature = (Creature)fCurrentRealm.PlainMap.FindCreature(mx, my);
                if (creature != null) {
                    AddMessage("You see " + creature.Desc);
                }
            }
        }

        public float LightFactor
        {
            get {
                DayTime dt = fCurrentRealm.Time.DayTime;
                var timeRec = GameTime.DayTimes[(int)dt];
                return timeRec.RadMod;
            }
        }

        public float LightBrightness
        {
            get {
                DayTime dt = fCurrentRealm.Time.DayTime;
                var timeRec = GameTime.DayTimes[(int)dt];
                return timeRec.Brightness;
            }
        }

        // every 100 ms
        public void DoTurn()
        {
            fTick++;
            int ex = fTick % 20;
            if (ex == 0) {
                fCurrentRealm.UpdateWater(fPlayerController.Viewport);
            }

            ex = fTick % 2;
            if (ex == 0) {
                fCurrentRealm.UpdatePortals();
            }

            fCurrentRealm.Time.Tick(60);

            fPlayerController.DoPathStep();

            fPlayerController.Player.DoTurn();
        }

        public void OnEvent(Event @event)
        {
            if (@event is VandalismEvent) {
                AddMessage("vandalism!!!", Colors.Red);
                fPlayerController.Player.Stats.AddXP(50);
            }
        }

        public IList<GameEntity> GetEntities(ExtPoint pt, int radius)
        {
            return null;
        }
    }
}
