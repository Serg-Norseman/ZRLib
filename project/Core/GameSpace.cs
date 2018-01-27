/*
 *  "ZRLib", Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih.
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

namespace ZRLib.Core
{
    public abstract class GameSpace : BaseObject
    {
        private static GameSpace fInstance;

        private readonly Dictionary<int?, GameEntity> fEntityTable;

        protected GameSpace(object owner)
        {
            fInstance = this;
            fEntityTable = new Dictionary<int?, GameEntity>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                fEntityTable.Clear();

                fInstance = null;
            }
            base.Dispose(disposing);
        }

        public static GameSpace Instance
        {
            get {
                return fInstance;
            }
        }

        public virtual void Clear()
        {
            fEntityTable.Clear();
        }

        public void AddEntity(GameEntity entity)
        {
            fEntityTable[entity.UID_Renamed] = entity;
        }

        public void DeleteEntity(GameEntity entity)
        {
            fEntityTable.Remove(entity.UID_Renamed);
        }

        public GameEntity FindEntity(int UID)
        {
            return fEntityTable.GetValueOrNull(UID);
        }


    }
}
