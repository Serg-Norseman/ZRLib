/*
 *  "JZRLib", Java Roguelike games development Library.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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
package jzrlib.core;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class LocatedEntityList extends EntityList
{
    public LocatedEntityList(Object owner, boolean ownsObjects)
    {
        super(owner, ownsObjects);
    }

    @Override
    public LocatedEntity getItem(int index)
    {
        return (LocatedEntity) super.getItem(index);
    }

    public LocatedEntity searchItemByPos(int aX, int aY)
    {
        int num = super.getCount();
        for (int i = 0; i < num; i++) {
            LocatedEntity entry = this.getItem(i);
            if (entry.getPosX() == aX && entry.getPosY() == aY) {
                return entry;
            }
        }
        return null;
    }

    public final ExtList<LocatedEntity> searchListByPos(int aX, int aY)
    {
        ExtList<LocatedEntity> result = new ExtList<>(false);

        int num = super.getCount();
        for (int i = 0; i < num; i++) {
            LocatedEntity entry = this.getItem(i);
            if (entry.getPosX() == aX && entry.getPosY() == aY) {
                result.add(entry);
            }
        }

        return result;
    }

    public final ExtList<LocatedEntity> searchListByArea(Rect rect)
    {
        ExtList<LocatedEntity> result = new ExtList<>(false);

        int num = super.getCount();
        for (int i = 0; i < num; i++) {
            LocatedEntity entry = this.getItem(i);
            if (entry.inRect(rect)) {
                result.add(entry);
            }
        }

        return result;
    }
}
