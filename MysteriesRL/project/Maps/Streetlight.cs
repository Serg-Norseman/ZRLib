/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2020 by Serg V. Zhdanovskih.
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
using ZRLib.Core;
using ZRLib.Map;

namespace MysteriesRL.Maps
{
    /// <summary>
    /// 
    /// </summary>
    public class Streetlight : MapFeature, ILightSource
    {
        private int fPower;

        public IMap Map
        {
            get {
                return (IMap)Owner;
            }
        }

        public int Power
        {
            get {
                return fPower;
            }
        }

        public override TileID TileID
        {
            get {
                return TileID.tid_Streetlight;
            }
        }


        public Streetlight(GameSpace space, object owner)
            : base(space, owner)
        {
        }

        public Streetlight(GameSpace space, object owner, int x, int y, int power = 20)
            : base(space, owner, x, y)
        {
            fPower = power;
        }

        public override void Render()
        {
            Map.GetTile(PosX, PosY).Foreground = (ushort)TileID;
        }
    }
}
