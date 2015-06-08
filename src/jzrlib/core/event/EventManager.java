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
package jzrlib.core.event;

import java.util.ArrayList;
import java.util.List;
import jzrlib.utils.Logger;

/**
 * EventManager keeps a track of event stack and makes a transitive rollback if
 * event is timed out.
 * 
 * @author Serg V. Zhdanovskih
 */
public class EventManager
{
    public static final int EVENT_TIMEOUT = 60000; // in ms

    private final List<IEventListener> fListeners;

    public EventManager()
    {
        this.fListeners = new ArrayList<>();
    }

    public void subscribe(IEventListener listener)
    {
        if (this.fListeners.contains(listener)) {
            Logger.write("EventManager.subscribe(): listener is already subscribed");
            return;
        }

        /*
         Additional check is performed to test if multiple parasite instances of a same class has subscribed to the event listener
         The result of this may be catastrophic and very difficult to debug
         */
        for (IEventListener regListener : this.fListeners) {
            if (regListener.getClass().equals(listener.getClass())) {
                //throw new RuntimeException("Trying to subscribe non-unique instance of a class " + regListener.getClass().getCanonicalName());
            }
        }

        this.fListeners.add(listener);
    }

    public void notifyEvent(Event event)
    {
        if (event == null) {
            return;
        }

        if (event.isDispatched()) {
            return; // do not allow to handle events, catched by gui overlay
        }

        // WARN: problematic place
        // use defensive copy of list and than iterate it
        // or be ready for obscure ConcurentModification exception
        for (IEventListener listener : this.fListeners.toArray(new IEventListener[0])) {
            listener.onEvent(event);
        }
    }

    public static void rollbackEvent(Event event)
    {
        // dummy
    }

    public boolean hasListener(IEventListener listener)
    {
        return this.fListeners.contains(listener);
    }

    public void reset()
    {
        this.fListeners.clear();
    }
}
