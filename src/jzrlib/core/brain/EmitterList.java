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
package jzrlib.core.brain;

import java.util.ArrayList;
import jzrlib.core.BaseObject;
import jzrlib.core.GameSpace;
import jzrlib.core.LocatedEntity;
import jzrlib.core.Point;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class EmitterList extends BaseObject
{
    private ArrayList<Emitter> fEmitters;

    public final Emitter getEmitter(int index)
    {
        Emitter result = null;
        if (index >= 0 && index < this.fEmitters.size()) {
            result = this.fEmitters.get(index);
        }
        return result;
    }

    public final int getEmittersCount()
    {
        return this.fEmitters.size();
    }

    public EmitterList()
    {
        this.fEmitters = new ArrayList<>();
    }

    @Override
    protected void dispose(boolean disposing)
    {
        if (disposing) {
            this.fEmitters.clear();
            this.fEmitters = null;
        }
        super.dispose(disposing);
    }

    public final int addEmitter(byte emitterKind, int sourceID, Point pos, float radius, int duration, boolean dynSource)
    {
        Emitter emitter = new Emitter(GameSpace.getInstance(), this);

        emitter.EmitterKind = emitterKind;
        emitter.SourceID = (int) sourceID;
        emitter.Position = pos;
        emitter.Radius = radius;
        emitter.ExpiryTime = duration;
        emitter.ExpiryTimeLeft = duration;
        emitter.DynamicSourcePos = dynSource;

        this.fEmitters.add(emitter);
        return (int) emitter.UID;
    }

    public final void clearEmitters()
    {
        this.fEmitters.clear();
    }

    public final void deleteEmitter(int index)
    {
        this.fEmitters.remove(index);
    }

    public final void updateEmitters(int elapsedTime)
    {
        for (int i = this.fEmitters.size() - 1; i >= 0; i--) {
            Emitter emitter = this.fEmitters.get(i);

            if ((double) emitter.ExpiryTime > (double) 0f) {
                emitter.ExpiryTimeLeft -= elapsedTime;
            }

            if ((double) emitter.ExpiryTime > (double) 0f && emitter.ExpiryTimeLeft <= 0) {
                this.deleteEmitter(i);
            } else {
                if (emitter.DynamicSourcePos) {
                    LocatedEntity source = (LocatedEntity) GameSpace.getInstance().findEntity(emitter.SourceID);

                    if (source == null) {
                        emitter.DynamicSourcePos = false;
                    } else {
                        emitter.Position = source.getLocation();
                    }
                }
            }
        }
    }
}
