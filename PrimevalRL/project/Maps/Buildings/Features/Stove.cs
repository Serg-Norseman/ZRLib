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

using System.Collections.Generic;
using PrimevalRL.Game;
using ZRLib.Core;
using ZRLib.Core.Action;

namespace PrimevalRL.Maps.Buildings.Features
{
    public sealed class Stove : BuildingFeature
    {
        public Stove(GameSpace space, object owner, int x, int y)
            : base(space, owner, x, y)
        {
            fTileID = TileID.tid_Stove;
            Durability = 20;
        }

        public override IList<IAction> GetActionsList()
        {
            var list = new List<IAction>();
            list.Add(new ActionCook(this, "Cook in stove"));
            return list;
        }

        private class ActionCook : BaseEntityAction<Stove>
        {
            public ActionCook(Stove owner, string name) : base(owner, name)
            {
            }

            public override void Execute(object invoker)
            {
            }
        }
    }
}
