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

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class Event
{
    private boolean fDispatched;
    private int fEventId;
    private EventManager fManager;
    private long fTimestamp;

    public Event()
    {
        this.fDispatched = false;
        this.fEventId = 0;
        this.fManager = null;
        this.fTimestamp = 0;
    }

    public void dispatch()
    {
        this.fDispatched = true;
    }

    public final boolean isDispatched()
    {
        return this.fDispatched;
    }

    public final long getTimestamp()
    {
        return this.fTimestamp;
    }

    public final void setTimestamp(long timestamp)
    {
        this.fTimestamp = timestamp;
    }

    public final long getAge(long timestamp)
    {
        return (timestamp - this.fTimestamp);
    }

    public final int getEventId()
    {
        return this.fEventId;
    }

    public final void setEventId(int eventid)
    {
        this.fEventId = eventid;
    }

    public final void setManager(EventManager manager)
    {
        this.fManager = manager;
    }

    public void post()
    {
        if (this.fManager == null) {
            this.fManager = ScheduledEventManager.getEventManager();
        }

        this.fManager.notifyEvent(this);
    }

    public void rollback()
    {
        // dummy
    }
}
