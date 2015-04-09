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
package mrl.game;

import java.util.ArrayList;
import java.util.List;
import jzrlib.core.Directions;
import jzrlib.core.EntityList;
import jzrlib.core.ExtList;
import jzrlib.core.GameEntity;
import jzrlib.core.LocatedEntity;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.core.action.IActor;
import jzrlib.map.AbstractMap;
import jzrlib.map.FOV;
import jzrlib.map.IMap;
import jzrlib.map.PathSearch;
import jzrlib.utils.AuxUtils;
import mrl.core.GlobalData;
import mrl.creatures.Creature;
import mrl.creatures.Human;
import mrl.maps.Layer;
import mrl.maps.buildings.Building;
import mrl.maps.buildings.Room;
import mrl.maps.buildings.features.Door;
import mrl.maps.city.City;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class PlayerController
{
    private Human fPlayer;
    private List<Point> fPath = new ArrayList<>();
    private final Rect fViewport;
    
    public boolean CircularFOV = false;
    
    public PlayerController()
    {
        this.fPlayer = null;
        this.fViewport = new Rect();
    }
    
    public final Human getPlayer()
    {
        return this.fPlayer;
    }
    
    public final void attach(Human human)
    {
        this.fPlayer = human;
        
        human.setIsPlayer(true);
        
        Building privHouse = human.getApartment();
        IMap map = this.fPlayer.getMap();
        FOV.openVisited(map, privHouse.getArea());

        Room room = AuxUtils.getRandomItem(privHouse.getRooms());
        Rect rt = (room != null) ? room.getInnerArea() : privHouse.getArea();
        Point pt = AuxUtils.getRandomPoint(rt);
        this.moveTo(pt.X, pt.Y);
    }

    public final void setPath(List<Point> path)
    {
        synchronized (PlayerController.class) {
            this.fPath = path;
        }
    }
    
    public final void clearPath()
    {
        synchronized (PlayerController.class) {
            this.fPath = null;
        }
    }
    
    public final Point getPathStep()
    {
        synchronized (PlayerController.class) {
            if (this.fPath != null && this.fPath.size() > 0) {
                Point pt = this.fPath.get(0);
                this.fPath.remove(0);
                
                if (this.fPath.isEmpty()) {
                    PathSearch.clear(this.fPlayer.getMap());
                }
                
                return pt;
            } else {
                return null;
            }
        }
    }
    
    public final boolean doPathStep()
    {
        Point pt = this.getPathStep();
        if (pt != null) {
            if (!this.fPlayer.getMap().isBarrier(pt.X, pt.Y)) {
                this.moveTo(pt.X, pt.Y);
                return true;
            }
        }
        return false;
    }

    public void walk(Point newPoint)
    {
        PathSearch.PSResult res = PathSearch.search(this.fPlayer.getMap(), this.fPlayer.getLocation(), newPoint, null, true);
        if (res != null) {
            this.setPath(res.Path);
        }
    }
    
    public final Rect getViewport()
    {
        int px = this.fPlayer.getPosX();
        int py = this.fPlayer.getPosY();
        fViewport.setBounds(px - GlobalData.GV_XRad, py - GlobalData.GV_YRad, px + GlobalData.GV_XRad, py + GlobalData.GV_YRad);
        return this.fViewport;
    }
    
    public void switchLock()
    {
        int px = this.fPlayer.getPosX();
        int py = this.fPlayer.getPosY();
        Point pt = this.fPlayer.getLocation();
        Rect rt = new Rect(px - 1, py - 1, px + 1, py + 1);
        
        List<Door> doors = new ArrayList<>();
        
        City city = this.fPlayer.getSpace().getCity();
        for (Building bld : city.getBuildings()) {
            Rect exrt = bld.getArea().clone();
            exrt.inflate(+1, +1);
            if (exrt.contains(pt)) {
                for (Door door : bld.getDoors()) {
                    if (rt.contains(door.getLocation())) {
                        doors.add(door);
                    }
                }
                break;
            }
        }
        
        if (doors.size() < 1) {
            // not doors near
        } else if (doors.size() > 1) {
            // too many doors
        } else {
            Door door = doors.get(0);
            door.Opened = !door.Opened;
            // FIXME: message
            
            this.fPlayer.getMap().getTile(door.getPosX(), door.getPosY()).Foreground = (short) door.getTileID().Value;
        }
    }
    
    public final void attack()
    {
        int px = this.fPlayer.getPosX();
        int py = this.fPlayer.getPosY();
        
        Rect rt = new Rect(px - 1, py - 1, px + 1, py + 1);
        ExtList<LocatedEntity> list = ((Layer) this.fPlayer.getMap()).getCreatures().searchListByArea(rt);
        if (list.getCount() > 0) {
            int idx = AuxUtils.getRandom(list.getCount());
            Creature enemy = (Creature) list.get(idx);
            
            this.fPlayer.inflictDamage(enemy);
        }
    }

    public void moveTo(int newX, int newY)
    {
        int posX = this.fPlayer.getPosX();
        int posY = this.fPlayer.getPosY();
        int dir = (this.CircularFOV) ? 0 : Directions.getDirByCoords(posX, posY, newX, newY);

        if (!this.fPlayer.getMap().isBarrier(newX, newY)) {
            this.fPlayer.setPos(newX, newY);
            posX = newX;
            posY = newY;
        }
        
        FOV.FOV_Start((AbstractMap) this.fPlayer.getMap(), posX, posY, this.fPlayer.getFovRadius(), dir, this.getViewport());
    }
    
    public IActor getNearFeature()
    {
        int px = this.fPlayer.getPosX();
        int py = this.fPlayer.getPosY();
        
        Rect rt = new Rect(px - 1, py - 1, px + 1, py + 1);
        ExtList<LocatedEntity> list = ListByArea(((Layer) this.fPlayer.getMap()).getFeatures(), rt);
        
        /*int i = list.indexOf(this.fPlayer);
        if (i >= 0) {
            list.extract(fPlayer)
        }*/
        
        if (list.getCount() > 0) {
            int idx = AuxUtils.getRandom(list.getCount());
            LocatedEntity entity = list.get(idx);
            if (entity instanceof IActor) {
                return (IActor) entity;
            }
            return null;
        } else {
            return null;
        }
    }

    public static final ExtList<LocatedEntity> ListByArea(EntityList list, Rect rect)
    {
        ExtList<LocatedEntity> result = new ExtList<>(false);

        int num = list.getCount();
        for (int i = 0; i < num; i++) {
            GameEntity entry = list.getItem(i);
            
            if (entry instanceof LocatedEntity) {
                LocatedEntity locEntry = (LocatedEntity) entry;

                if (locEntry.inRect(rect)) {
                    result.add(locEntry);
                }
            }
        }

        return result;
    }
    
    public boolean inRange(GameEntity entity)
    {
        if (entity instanceof LocatedEntity) {
            LocatedEntity located = (LocatedEntity) entity;
            int dist = AuxUtils.distance(this.fPlayer.getLocation(), located.getLocation());
            return (dist <= 1);
        } else {
            return false;
        }
    }
}
