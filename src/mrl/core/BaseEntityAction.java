/*
 *  "MysteriesRL", Java Roguelike game.
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
package mrl.core;

import jzrlib.core.GameEntity;
import jzrlib.core.action.IAction;
import mrl.game.Game;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class BaseEntityAction<T extends GameEntity> implements IAction<T>
{
    protected T fOwner;
    protected String fName;

    public BaseEntityAction()
    {
        this.fOwner = null;
        this.fName = "undefined";
    }

    @Override
    public String getName()
    {
        return this.fName;
    }

    @Override
    public void setName(String name)
    {
        this.fName = name;
    }

    @Override
    public T getOwner()
    {
        return fOwner;
    }

    @Override
    public void setOwner(T owner)
    {
        this.fOwner = owner;
    }

    @Override
    public void execute(Object invoker)
    {
        // dummy
    }

    public final Game getSpace()
    {
        return (Game) this.fOwner.getSpace();
    }
}
