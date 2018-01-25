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

using System.Collections.Generic;
using BSLib;
using MysteriesRL.Creatures;
using MysteriesRL.Game.Events;
using ZRLib.Core;

namespace MysteriesRL.Game
{
    public sealed class SocialController : IEventListener
    {
        private static readonly IList<ExtPoint> fCrimeplaces = new List<ExtPoint>();
        private static SocialController fInstance = new SocialController();

        public static SocialController Instance
        {
            get {
                lock (typeof(SocialController)) {
                    if (fInstance == null) {
                        fInstance = new SocialController();
                        ScheduledEventManager.EventManager.Subscribe(fInstance);
                    }
                }
                return fInstance;
            }
        }

        public void OnEvent(Event evt)
        {
            if (evt is NPCReportCrimeEvent) {
                if (fCrimeplaces.Contains(((NPCReportCrimeEvent)evt).Location)) {
                    return;
                }
                AddCrimeplace(((NPCReportCrimeEvent)evt).Location);
            }

            if (evt is SuspiciousSoundEvent) {
                MRLGame space = (MRLGame)GameSpace.Instance;

                IList<GameEntity> list = space.GetEntities(((SuspiciousSoundEvent)evt).Location, ((SuspiciousSoundEvent)evt).Radius);
                foreach (GameEntity ent in list) {
                    if (ent is IEventListener) {
                        ((IEventListener)ent).OnEvent(evt);
                    }
                }
            }

            if (evt is VandalismEvent) {
                VandalismEvent vandalism = (VandalismEvent)evt;
                if (vandalism.Criminal is Human) {
                    ((Human)vandalism.Criminal).AddCrimeRecord(new CrimeRecord(CrimeType.ct_Vandalism));
                }
            }
        }

        public static IList<ExtPoint> Crimeplaces
        {
            get { return fCrimeplaces; }
        }

        public static void AddCrimeplace(ExtPoint crimeplace)
        {
            fCrimeplaces.Add(crimeplace);
        }

        public static bool HasCrimeplace(ExtPoint origin)
        {
            return fCrimeplaces.Contains(origin);
        }
    }
}
