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
using MysteriesRL.Core;
using ZRLib.Core;
using ZRLib.Core.Action;
using ZRLib.Map;

namespace MysteriesRL.Maps.Buildings.Features
{
    public sealed class Door : BuildingFeature
    {
        private readonly Axis fAxis;

        public bool IsMain = false;
        public bool Opened = true;

        public Door(GameSpace space, object owner, Axis axis)
            : base(space, owner)
        {
            fAxis = axis;
        }

        public Axis SideAxis
        {
            get { return fAxis; }
        }

        public override TileID TileID
        {
            get {
                switch (fAxis) {
                    case Axis.axHorz:
                        return Opened ? TileID.tid_HouseDoorHO : TileID.tid_HouseDoorHC;

                    case Axis.axVert:
                        return Opened ? TileID.tid_HouseDoorVO : TileID.tid_HouseDoorVC;

                    default:
                        return TileID.tid_HouseDoor;
                }
            }
        }

        public override void Render()
        {
            IMap map = Map;
            map.GetTile(PosX, PosY).Foreground = (ushort)TileID;
        }

        public override IList<IAction> GetActionsList()
        {
            var list = new List<IAction>();
            if (!Opened) {
                list.Add(new ActionOpen(this, "Open door"));
            } else {
                list.Add(new ActionClose(this, "Close door"));
            }
            list.Add(new ActionBreak(this, "Break door"));
            return list;
        }

        private class ActionOpen : BaseEntityAction<Door>
        {
            private readonly Door fDoor;
            
            public ActionOpen(Door owner, string name) : base(owner, name)
            {
                fDoor = owner;
            }

            public override void Execute(object invoker)
            {
                fDoor.Opened = true;
                fDoor.Render();
            }
        }

        private class ActionClose : BaseEntityAction<Door>
        {
            private readonly Door fDoor;
            
            public ActionClose(Door owner, string name) : base(owner, name)
            {
                fDoor = owner;
            }

            public override void Execute(object invoker)
            {
                fDoor.Opened = false;
                fDoor.Render();
            }
        }

        private class ActionBreak : BaseEntityAction<Door>
        {
            private readonly Door fDoor;
            
            public ActionBreak(Door owner, string name) : base(owner, name)
            {
                fDoor = owner;
            }

            public override void Execute(object invoker)
            {
                // dummy
            }
        }
    }
}
