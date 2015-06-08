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

import java.util.HashMap;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class GameSpace extends BaseObject
{
    private static GameSpace fInstance;

    private final HashMap<Integer, GameEntity> fEntityTable;

    public GameSpace(Object owner)
    {
        fInstance = this;
        this.fEntityTable = new HashMap<>();
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.fEntityTable.clear();

            fInstance = null;
        }
        super.dispose(disposing);
    }

    public static final GameSpace getInstance()
    {
        return fInstance;
    }

    public void clear()
    {
        this.fEntityTable.clear();
    }

    // <editor-fold defaultstate="collapsed" desc="Entities search functions">

    public final void addEntity(GameEntity entity)
    {
        this.fEntityTable.put(entity.UID, entity);
    }

    public final void deleteEntity(GameEntity entity)
    {
        this.fEntityTable.remove(entity.UID);
    }

    public final GameEntity findEntity(int UID)
    {
        return this.fEntityTable.get(UID);
    }

    // </editor-fold>
}
