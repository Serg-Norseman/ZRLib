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
using BSLib;
using PrimevalRL.Creatures;
using PrimevalRL.Maps;
using PrimevalRL.Maps.Buildings;
using ZRLib.Core;
using ZRLib.Core.Action;
using ZRLib.Map;

namespace PrimevalRL.Game
{
    public sealed class PlayerController
    {
        private Human fPlayer;
        private IList<ExtPoint> fPath = new List<ExtPoint>();
        private ExtRect fViewport;

        public bool CircularFOV = false;

        public IList<IAction> AvailableActions = null;

        public PlayerController()
        {
            fPlayer = null;
            fViewport = new ExtRect();
        }

        public Human Player
        {
            get {
                return fPlayer;
            }
        }

        public void Attach(Human human)
        {
            fPlayer = human;

            human.IsPlayer = true;

            Building privHouse = human.Apartment;
            IMap map = fPlayer.Map;
            FOV.OpenVisited(map, privHouse.Area);

            Room room = RandomHelper.GetRandomItem(privHouse.Rooms);
            ExtRect rt = (room != null) ? room.InnerArea : privHouse.Area;
            ExtPoint pt = RandomHelper.GetRandomPoint(rt);
            MoveTo(pt.X, pt.Y);
        }

        public IList<ExtPoint> Path
        {
            set {
                lock (typeof(PlayerController)) {
                    fPath = value;
                }
            }
        }

        public void ClearPath()
        {
            lock (typeof(PlayerController)) {
                fPath = null;
            }
        }

        public ExtPoint PathStep
        {
            get {
                lock (typeof(PlayerController)) {
                    if (fPath != null && fPath.Count > 0) {
                        ExtPoint pt = fPath[0];
                        fPath.RemoveAt(0);
    
                        if (fPath.Count == 0) {
                            PathSearch.Clear(fPlayer.Map);
                        }
    
                        return pt;
                    } else {
                        return ExtPoint.Empty;
                    }
                }
            }
        }

        public bool DoPathStep()
        {
            ExtPoint pt = PathStep;
            if (!pt.IsEmpty) {
                if (!fPlayer.Map.IsBarrier(pt.X, pt.Y)) {
                    MoveTo(pt.X, pt.Y);
                    return true;
                }
            }
            return false;
        }

        public void Walk(ExtPoint newPoint)
        {
            PathSearch.PSResult res = PathSearch.Search(fPlayer.Map, fPlayer.Location, newPoint, null, true);
            if (res != null) {
                Path = res.Path;
            }
        }

        public ExtRect Viewport
        {
            get {
                int px = fPlayer.PosX;
                int py = fPlayer.PosY;
                //var rt = ExtRect.Create(px - MRLData.GV_XRad, py - MRLData.GV_YRad, px + MRLData.GV_XRad, py + MRLData.GV_YRad);
                fViewport = ExtRect.Create(px - MRLData.GV_XRad, py - MRLData.GV_YRad, px + MRLData.GV_XRad, py + MRLData.GV_YRad);
                return fViewport;//rt;
            }
        }

        public void Attack()
        {
            int px = fPlayer.PosX;
            int py = fPlayer.PosY;

            ExtRect rt = ExtRect.Create(px - 1, py - 1, px + 1, py + 1);
            ExtList<LocatedEntity> list = ((Layer)fPlayer.Map).Creatures.SearchListByArea(rt);
            if (list.Count > 0) {
                int idx = RandomHelper.GetRandom(list.Count);
                Creature enemy = (Creature)list[idx];

                fPlayer.InflictDamage(enemy);
            }
        }

        public void MoveTo(int newX, int newY)
        {
            int posX = fPlayer.PosX;
            int posY = fPlayer.PosY;
            int dir = (CircularFOV) ? 0 : Directions.GetDirByCoords(posX, posY, newX, newY);

            if (!fPlayer.Map.IsBarrier(newX, newY)) {
                fPlayer.SetPos(newX, newY);
                posX = newX;
                posY = newY;
            }

            FOV.FOV_Start((AbstractMap)fPlayer.Map, posX, posY, fPlayer.FovRadius, dir, Viewport);

            PostMove();
        }

        private void PostMove()
        {
            AvailableActions = NearFeature;
        }

        public IList<IAction> NearFeature
        {
            get {
                IList<IAction> result = new List<IAction>();
    
                int px = fPlayer.PosX;
                int py = fPlayer.PosY;
                ExtRect rt = ExtRect.Create(px - 1, py - 1, px + 1, py + 1);
    
                ExtList<LocatedEntity> list = ListByArea(((Layer)fPlayer.Map).Features, rt);
                for (int i = 0; i < list.Count; i++) {
                    LocatedEntity entity = list[i];
                    if (entity is IActor) {
                        IList<IAction> featActions = ((IActor)entity).GetActionsList();
                        result.AddRange(featActions);
                    }
                }
    
                /*int i = list.indexOf(fPlayer);
                if (i >= 0) {
                    list.extract(fPlayer)
                }*/
    
                return result;
            }
        }

        public static ExtList<LocatedEntity> ListByArea(EntityList list, ExtRect rect)
        {
            ExtList<LocatedEntity> result = new ExtList<LocatedEntity>(false);

            int num = list.Count;
            for (int i = 0; i < num; i++) {
                GameEntity entry = list.GetItem(i);

                if (entry is LocatedEntity) {
                    LocatedEntity locEntry = (LocatedEntity)entry;

                    if (locEntry.InRect(rect)) {
                        result.Add(locEntry);
                    }
                }
            }

            return result;
        }

        public bool InRange(GameEntity entity)
        {
            if (entity is LocatedEntity) {
                LocatedEntity located = (LocatedEntity)entity;
                int dist = MathHelper.Distance(fPlayer.Location, located.Location);
                return (dist <= 1);
            } else {
                return false;
            }
        }
    }
}
