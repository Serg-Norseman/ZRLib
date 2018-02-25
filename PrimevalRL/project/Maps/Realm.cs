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
using PrimevalRL.Game;
using PrimevalRL.Generators;
using PrimevalRL.Maps;
using ZRLib.Core;
using ZRLib.Map;

namespace PrimevalRL.Maps
{
    /// <summary>
    /// 
    /// </summary>
    public class Realm
    {
        protected readonly Layer fPlainMap;
        protected readonly Layer fUndergroundMap;
        protected readonly GameTime fTime;

        public Layer PlainMap
        {
            get { return fPlainMap; }
        }

        public Layer UndergroundMap
        {
            get { return fUndergroundMap; }
        }

        public IMap fMinimap;

        /// <summary>
        /// This year value is based on the sign 32-bit int, i.e. for the game with geochronology 
        /// can count the dates to Paleoproterozoic (2.1 billion years).
        /// </summary>
        public GameTime Time
        {
            get { return fTime; }
        }

        public Realm(bool underground)
        {
            fPlainMap = new Layer(false, "plain");
            if (underground) {
                fUndergroundMap = new Layer(true, "underground");
            }
            fTime = new GameTime();
            fMinimap = null;
        }

        public virtual void InitNew(MRLGame gameSpace, int year, IProgressController progressController)
        {
            fTime.Set(year, 7, 1, 12, 0, 0);
            RealmGenerator.Gen_Valley(fPlainMap, true, true, true);
            fMinimap = new Minimap(fPlainMap, null);
        }

        public virtual string GetLocationName(int px, int py)
        {
            return "unknown";
        }

        public ExtPoint SearchFreeLocation(ExtRect area, int tries = 50)
        {
            ExtPoint pt = fPlainMap.SearchFreeLocation(area);
            return pt;
        }

        public void UpdateWater(ExtRect viewport)
        {
            fPlainMap.UpdateWater(viewport);
        }

        public void UpdatePortals()
        {
            IMap map = fPlainMap;
            for (int i = 0; i < map.Features.Count; i++) {
                var portal = map.Features.GetItem(i) as Portal;
                if (portal != null) {
                    portal.DoTurn();
                }
            }
        }
    }
}
