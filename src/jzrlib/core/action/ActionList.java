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
package jzrlib.core.action;

import java.util.ArrayList;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class ActionList<T>
{
    private final ArrayList<IAction<T>> fActionsList;
    private T fOwner;

    public ActionList()
    {
        this.fOwner = null;
        this.fActionsList = new ArrayList<>();
    }

    public final void setOwner(T owner)
    {
        this.fOwner = owner;
    }

    public final void addAction(IAction<T> action, String name)
    {
        action.setOwner(this.fOwner);
        action.setName(name);
        this.fActionsList.add(action);
    }

    public final ArrayList getList()
    {
        return this.fActionsList;
    }

    public final void addAll(ArrayList superList)
    {
        this.fActionsList.addAll(superList);
    }
}
