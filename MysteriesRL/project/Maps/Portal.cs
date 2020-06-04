/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2018 by Serg V. Zhdanovskih.
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
using MysteriesRL.Game;
using ZRLib.Core;
using ZRLib.Core.Action;
using ZRLib.Map;

namespace MysteriesRL.Maps
{
    public class Portal : MapFeature
    {
        private Realm fOldRealm;
        private Realm fNewRealm;
        private int fStage;

        public Realm OldRealm
        {
            get { return fOldRealm; }
        }

        public Realm NewRealm
        {
            get { return fNewRealm; }
        }

        public IMap Map
        {
            get { return (IMap)Owner; }
        }

        public override TileID TileID
        {
            get {
                switch (fStage) {
                    case 0:
                        fTileID = TileID.tid_Portal0;
                        break;
                    case 1:
                        fTileID = TileID.tid_Portal1;
                        break;
                    case 2:
                        fTileID = TileID.tid_Portal2;
                        break;
                    case 3:
                        fTileID = TileID.tid_Portal3;
                        break;
                }
                return fTileID;
            }
        }


        public Portal(GameSpace space, object owner)
            : base(space, owner)
        {
            fTileID = TileID.tid_Portal0;
            fStage = 0;
        }

        public Portal(GameSpace space, object owner, int x, int y, Realm oldRealm, Realm newRealm, int tx, int ty)
            : base(space, owner, x, y)
        {
            fTileID = TileID.tid_Portal0;
            fStage = 0;
            fOldRealm = oldRealm;
            fNewRealm = newRealm;
        }

        public override void Render()
        {
            Map.GetTile(PosX, PosY).Foreground = (ushort)TileID;
        }

        public void DoTurn()
        {
            fStage = (fStage == 3) ? 0 : fStage + 1;
            Render();
        }

        public override IList<IAction> GetActionsList()
        {
            var list = new List<IAction>();
            list.Add(new ActionEnterToPortal(this, "Enter to portal"));
            return list;
        }

        private class ActionEnterToPortal : BaseEntityAction<Portal>
        {
            public ActionEnterToPortal(Portal owner, string name)
                : base(owner, name)
            {
            }

            public override void Execute(object invoker)
            {
                MRLGame game = this.Space as MRLGame;
                game.CurrentRealm = (Owner as Portal).NewRealm;
                game.PlayerController.Player.Map = game.CurrentRealm.PlainMap;
            }
        }
    }
}
