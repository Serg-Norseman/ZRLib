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
using PrimevalRL.Maps;
using ZRLib.Core;

namespace PrimevalRL.Maps
{
    /// <summary>
    /// 
    /// </summary>
    public class Realm
    {
        private readonly Layer fPlainMap;
        private readonly Layer fUndergroundMap;
        private readonly GameTime fTime;

        public Layer PlainMap
        {
            get { return fPlainMap; }
        }

        public Layer UndergroundMap
        {
            get { return fUndergroundMap; }
        }

        public GameTime Time
        {
            get { return fTime; }
        }

        public Realm()
        {
            fPlainMap = new Layer(false, "plain");
            fUndergroundMap = new Layer(true, "underground");
            fTime = new GameTime();
        }
    }
}
