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
using MysteriesRL.Creatures;
using MysteriesRL.Game;
using MysteriesRL.Game.Events;
using ZRLib.Core;
using ZRLib.Core.Action;
using ZRLib.Map;

namespace MysteriesRL.Maps.Buildings.Features
{
    public sealed class Window : BuildingFeature
    {
        private readonly Axis fAxis;
        private bool fIsBroken;

        public bool IsBroken
        {
            get { return fIsBroken; }
            set { fIsBroken = value; }
        }

        public override TileID TileID
        {
            get {
                if (fIsBroken) {
                    return TileID.tid_HouseWindowB;
                }
    
                switch (fAxis) {
                    case Axis.Horz:
                        return TileID.tid_HouseWindowH;
                    case Axis.Vert:
                        return TileID.tid_HouseWindowV;
                    default:
                        return TileID.tid_HouseWindow;
                }
            }
        }


        public Window(GameSpace space, object owner, Axis axis)
            : base(space, owner)
        {
            fAxis = axis;
        }

        public override void Render()
        {
            IMap map = Map;
            map.GetTile(PosX, PosY).Foreground = (ushort)TileID;
        }

        public override IList<IAction> GetActionsList()
        {
            if (!fIsBroken) {
                var list = new List<IAction>();
                list.Add(new ActionBreak(this, "Break window"));
                return list;
            } else {
                return base.GetActionsList();
            }
        }

        private class ActionBreak : BaseEntityAction<Window>
        {
            private readonly Window fWindow;

            public ActionBreak(Window owner, string name) : base(owner, name)
            {
                fWindow = owner;
            }

            public override void Execute(Object invoker)
            {
                fWindow.IsBroken = true;

                VandalismEvent evt = new VandalismEvent(fWindow.Location, (Creature)invoker);
                evt.Post();

                fWindow.Render();
            }
        }
    }
}
