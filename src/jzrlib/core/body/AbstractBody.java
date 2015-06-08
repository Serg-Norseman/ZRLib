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
package jzrlib.core.body;

import java.util.ArrayList;
import jzrlib.core.BaseEntity;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class AbstractBody extends BaseEntity
{
    protected ArrayList<Bodypart> fParts;

    public AbstractBody(Object owner)
    {
        super(owner);
        this.fParts = new ArrayList<>();
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.fParts = null;
        }
        super.dispose(disposing);
    }

    public final int getPartsCount()
    {
        return this.fParts.size();
    }

    public final Bodypart getPart(int index)
    {
        Bodypart result = null;
        if (index >= 0 && index < this.fParts.size()) {
            result = this.fParts.get(index);
        }
        return result;
    }

    public final void clear()
    {
        this.fParts.clear();
    }

    public final Bodypart addPart(int type)
    {
        return this.addPart(new Bodypart(type));
    }

    public final Bodypart addPart(int type, int state)
    {
        return this.addPart(new Bodypart(type, state));
    }

    public final Bodypart addPart(Bodypart bodypart)
    {
        this.fParts.add(bodypart);
        return bodypart;
    }

    public final Bodypart getOccupiedPart(Object item)
    {
        if (item == null) return null;

        int num = this.fParts.size();
        for (int i = 0; i < num; i++) {
            Bodypart entry = this.fParts.get(i);
            if (entry.Item == item) {
                return entry;
            }
        }

        return null;
    }

    public final Bodypart getUnoccupiedPart(int part)
    {
        int num = this.fParts.size();
        for (int i = 0; i < num; i++) {
            Bodypart entry = this.fParts.get(i);
            if (entry.Type == part && entry.Item == null) {
                return entry;
            }
        }

        return null;
    }
    
    public abstract void update();
}
