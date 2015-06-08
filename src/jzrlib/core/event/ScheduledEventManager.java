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

/**
 * Scheduled event manager.
 * 
 * @author Serg V. Zhdanovskih
 */
public class ScheduledEventManager
{
    private static final ArrayList<Event> fScheduledEvents = new ArrayList<>();

    private static final EventManager fEventManager = new EventManager()
    {
        @Override
        public void notifyEvent(Event event)
        {
            /*NE_GUI_System ui = Game.get_game_mode().get_ui().get_nge_ui();
            if (ui != null) {
                ui.e_on_event(event);
            }*/

            /*
             *  Note, that event manager does not notify
             *  GUI System as regular listener.
             *  It makes explicit call to ensure that
             *  message is registered by GUI overlay first
             *  and dispatched if necessary
             */
            super.notifyEvent(event);
        }
    };

    public static EventManager getEventManager()
    {
        return fEventManager;
    }

    public static void subscribe(IEventListener listener)
    {
        fEventManager.subscribe(listener);
    }

    public static void addEvent(Event event)
    {
        fScheduledEvents.add(event);
    }

    public static synchronized void update()
    {
        for (Event event : fScheduledEvents) {
            System.out.println("posting scheduled event of type " + event.getClass().getName());
            event.post();
        }
        fScheduledEvents.clear();
    }
}
