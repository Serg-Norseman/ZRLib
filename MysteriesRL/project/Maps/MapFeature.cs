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

using System.Collections.Generic;
using MysteriesRL.Game;
using ZRLib.Core;
using ZRLib.Core.Action;

namespace MysteriesRL.Maps
{
    public class MapFeature : LocatedEntity, IActor
    {
        protected TileID fTileID;

        public int Durability;

        public override GameSpace Space
        {
            get { return (MRLGame)fSpace; }
        }

        public virtual TileID TileID
        {
            get { return fTileID; }
        }


        public MapFeature(GameSpace space, object owner)
            : base(space, owner)
        {
        }

        public MapFeature(GameSpace space, object owner, TileID tileId)
            : base(space, owner)
        {
            fTileID = tileId;
        }

        public MapFeature(GameSpace space, object owner, int x, int y)
            : base(space, owner)
        {
            SetPos(x, y);
        }

        public virtual void Render()
        {
            // dummy
        }

        public virtual IList<IAction> GetActionsList()
        {
            return new List<IAction>();
        }
    }
}
