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

using BSLib;

namespace ZRLib.Core
{
    public class LocatedEntityList : EntityList
    {
        public LocatedEntityList(object owner, bool ownsObjects)
            : base(owner, ownsObjects)
        {
        }

        public new virtual LocatedEntity GetItem(int index)
        {
            return (LocatedEntity)base.GetItem(index);
        }

        public virtual LocatedEntity SearchItemByPos(int aX, int aY)
        {
            int num = Count;
            for (int i = 0; i < num; i++) {
                LocatedEntity entry = GetItem(i);
                if (entry.PosX == aX && entry.PosY == aY) {
                    return entry;
                }
            }
            return null;
        }

        public ExtList<LocatedEntity> SearchListByPos(int aX, int aY)
        {
            ExtList<LocatedEntity> result = new ExtList<LocatedEntity>(false);

            int num = Count;
            for (int i = 0; i < num; i++) {
                LocatedEntity entry = GetItem(i);
                if (entry.PosX == aX && entry.PosY == aY) {
                    result.Add(entry);
                }
            }

            return result;
        }

        public ExtList<LocatedEntity> SearchListByArea(ExtRect rect)
        {
            ExtList<LocatedEntity> result = new ExtList<LocatedEntity>(false);

            int num = Count;
            for (int i = 0; i < num; i++) {
                LocatedEntity entry = GetItem(i);
                if (entry.InRect(rect)) {
                    result.Add(entry);
                }
            }

            return result;
        }
    }
}
