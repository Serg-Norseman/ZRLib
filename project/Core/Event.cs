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

namespace ZRLib.Core
{
    public class Event
    {
        private bool fDispatched;
        private int fEventId;
        private EventManager fManager;
        private long fTimestamp;

        public Event()
        {
            fDispatched = false;
            fEventId = 0;
            fManager = null;
            fTimestamp = 0;
        }

        public virtual void Dispatch()
        {
            fDispatched = true;
        }

        public bool Dispatched
        {
            get {
                return fDispatched;
            }
        }

        public long Timestamp
        {
            get {
                return fTimestamp;
            }
            set {
                fTimestamp = value;
            }
        }


        public long GetAge(long timestamp)
        {
            return (timestamp - fTimestamp);
        }

        public int EventId
        {
            get {
                return fEventId;
            }
            set {
                fEventId = value;
            }
        }


        public EventManager Manager
        {
            set {
                fManager = value;
            }
        }

        public virtual void Post()
        {
            if (fManager == null) {
                fManager = ScheduledEventManager.EventManager;
            }

            fManager.NotifyEvent(this);
        }

        public virtual void Rollback()
        {
            // dummy
        }
    }
}
