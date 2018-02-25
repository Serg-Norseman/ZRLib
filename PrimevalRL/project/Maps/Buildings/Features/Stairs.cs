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
using PrimevalRL.Game;
using ZRLib.Core;
using ZRLib.Core.Action;
using ZRLib.Map;

namespace PrimevalRL.Maps.Buildings.Features
{
    public sealed class Stairs : BuildingFeature
    {
        private bool fIsDesc = false;
        private readonly ExtPoint fDestination;

        public Stairs(GameSpace space, object owner, int x, int y, int tx, int ty)
            : base(space, owner, x, y)
        {
            fDestination = new ExtPoint(tx, ty);
        }

        public Stairs(GameSpace space, object owner, ExtPoint src, ExtPoint dest)
            : base(space, owner, src.X, src.Y)
        {
            fDestination = dest;
        }

        public ExtPoint Destination
        {
            get { return fDestination; }
        }

        public override TileID TileID
        {
            get {
                return fIsDesc ? TileID.tid_StairsD : TileID.tid_StairsA;
            }
        }

        public override void Render()
        {
            IMap map = Map;
            map.GetTile(PosX, PosY).Foreground = (ushort)TileID;
        }

        public bool Descending
        {
            set { fIsDesc = value; }
        }

        public override IList<IAction> GetActionsList()
        {
            var list = new List<IAction>();
            if (fIsDesc) {
                list.Add(new ActionDescend(this, "Go down"));
            } else {
                list.Add(new ActionAscend(this, "Go up"));
            }
            return list;
        }

        private class ActionDescend : BaseEntityAction<Stairs>
        {
            private readonly Stairs fStairs;
            
            public ActionDescend(Stairs owner, string name) : base(owner, name)
            {
                fStairs = owner;
            }

            public override void Execute(object invoker)
            {
                ((Human)invoker).Descend(fStairs.Destination);
            }
        }

        private class ActionAscend : BaseEntityAction<Stairs>
        {
            private readonly Stairs fStairs;
            
            public ActionAscend(Stairs owner, string name) : base(owner, name)
            {
                fStairs = owner;
            }

            public override void Execute(Object invoker)
            {
                ((Human)invoker).Ascend(fStairs.Destination);
            }
        }
    }
}
