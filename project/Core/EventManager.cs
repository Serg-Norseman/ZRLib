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

using System.Linq;
using System.Collections.Generic;

namespace ZRLib.Core
{
    public interface IEventListener
    {
        void OnEvent(Event @event);
    }

    /// <summary>
    /// EventManager keeps a track of event stack and makes a transitive rollback if
    /// event is timed out.
    /// </summary>
    public class EventManager
    {
        public const int EVENT_TIMEOUT = 60000; // in ms

        private readonly IList<IEventListener> fListeners;

        public EventManager()
        {
            fListeners = new List<IEventListener>();
        }

        public virtual void Subscribe(IEventListener listener)
        {
            if (fListeners.Contains(listener)) {
                Logger.Write("EventManager.subscribe(): listener is already subscribed");
                return;
            }

            /*
             Additional check is performed to test if multiple parasite instances of a same class has subscribed to the event listener
             The result of this may be catastrophic and very difficult to debug
             */
            foreach (IEventListener regListener in fListeners) {
                if (regListener.GetType() == listener.GetType()) {
                    //throw new RuntimeException("Trying to subscribe non-unique instance of a class " + regListener.getClass().getCanonicalName());
                }
            }

            fListeners.Add(listener);
        }

        public virtual void NotifyEvent(Event @event)
        {
            if (@event == null) {
                return;
            }

            if (@event.Dispatched) {
                return; // do not allow to handle events, catched by gui overlay
            }

            // WARN: problematic place
            // use defensive copy of list and than iterate it
            // or be ready for obscure ConcurentModification exception
            foreach (IEventListener listener in fListeners.ToArray()) {
                listener.OnEvent(@event);
            }
        }

        public static void RollbackEvent(Event @event)
        {
            // dummy
        }

        public virtual bool HasListener(IEventListener listener)
        {
            return fListeners.Contains(listener);
        }

        public virtual void Reset()
        {
            fListeners.Clear();
        }
    }
}
