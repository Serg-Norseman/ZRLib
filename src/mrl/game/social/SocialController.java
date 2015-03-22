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
package mrl.game.social;

import java.util.ArrayList;
import java.util.List;
import jzrlib.core.GameEntity;
import jzrlib.core.GameSpace;
import jzrlib.core.Point;
import jzrlib.core.event.Event;
import jzrlib.core.event.IEventListener;
import jzrlib.core.event.ScheduledEventManager;
import mrl.creatures.Human;
import mrl.game.Game;
import mrl.game.events.NPCReportCrimeEvent;
import mrl.game.events.SuspiciousSoundEvent;
import mrl.game.events.VandalismEvent;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class SocialController implements IEventListener
{
    private static final List<Point> fCrimeplaces = new ArrayList<>();
    private static SocialController fInstance = new SocialController();

    public static final SocialController getInstance()
    {
        synchronized (SocialController.class) {
            if (fInstance == null) {
                fInstance = new SocialController();
                ScheduledEventManager.getEventManager().subscribe(fInstance);
            }
        }
        return fInstance;
    }
    
    @Override
    public void onEvent(Event event)
    {
        if (event instanceof NPCReportCrimeEvent) {
            if (fCrimeplaces.contains(((NPCReportCrimeEvent) event).getLocation())) {
                return;
            }
            addCrimeplace(((NPCReportCrimeEvent) event).getLocation());
        }

        if (event instanceof SuspiciousSoundEvent) {
            Game space = (Game) GameSpace.getInstance();
            
            List<GameEntity> list = space.getEntities(((SuspiciousSoundEvent) event).getLocation(), ((SuspiciousSoundEvent) event).Radius);
            for (GameEntity ent : list) {
                if (ent instanceof IEventListener) {
                    ((IEventListener) ent).onEvent(event);
                }
            }
        }
        
        if (event instanceof VandalismEvent) {
            VandalismEvent vandalism = (VandalismEvent) event;
            if (vandalism.Criminal instanceof Human) {
                ((Human) vandalism.Criminal).addCrimeRecord(new CrimeRecord(CrimeType.ct_Vandalism));
            }
        }
    }

    public static List<Point> getCrimeplaces()
    {
        return fCrimeplaces;
    }

    public static void addCrimeplace(Point crimeplace)
    {
        fCrimeplaces.add(crimeplace);
    }

    public static boolean hasCrimeplace(Point origin)
    {
        return fCrimeplaces.contains(origin);
    }
}
