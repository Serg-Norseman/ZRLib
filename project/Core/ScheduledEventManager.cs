/*
 *  "ZRLib", Roguelike games development Library.
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

using System;
using System.Collections.Generic;

namespace ZRLib.Core
{
    /// <summary>
    /// Scheduled event manager.
    /// </summary>
    public static class ScheduledEventManager
    {
        private static readonly List<Event> fScheduledEvents = new List<Event>();

        private static readonly EventManager fEventManager = new DefaultEventManager();

        private class DefaultEventManager : EventManager
        {
            public override void NotifyEvent(Event @event)
            {
                /*
                 *  Note, that event manager does not notify
                 *  GUI System as regular listener.
                 *  It makes explicit call to ensure that
                 *  message is registered by GUI overlay first
                 *  and dispatched if necessary
                 */
                base.NotifyEvent(@event);
            }
        }

        public static EventManager EventManager
        {
            get {
                return fEventManager;
            }
        }

        public static void Subscribe(IEventListener listener)
        {
            fEventManager.Subscribe(listener);
        }

        public static void AddEvent(Event evt)
        {
            fScheduledEvents.Add(evt);
        }

        public static void Update()
        {
            lock (typeof(ScheduledEventManager)) {
                foreach (Event @event in fScheduledEvents) {
                    Console.WriteLine("posting scheduled event of type " + @event.GetType().Name);
                    @event.Post();
                }
                fScheduledEvents.Clear();
            }
        }
    }
}
