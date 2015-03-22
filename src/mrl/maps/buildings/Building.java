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
package mrl.maps.buildings;

import mrl.generators.BuildingRenderer;
import java.util.ArrayList;
import java.util.List;
import jzrlib.common.CreatureSex;
import jzrlib.core.GameSpace;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.external.bsp.LocationType;
import jzrlib.external.bsp.SideType;
import jzrlib.map.IMap;
import jzrlib.map.TileFinder;
import jzrlib.map.TileStates;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.Logger;
import mrl.creatures.Human;
import mrl.generators.FamilyGenerator;
import mrl.maps.Axis;
import mrl.maps.Layer;
import mrl.maps.TileID;
import mrl.maps.Utils;
import mrl.maps.buildings.features.Bed;
import mrl.maps.buildings.features.Door;
import mrl.maps.buildings.features.Stove;
import mrl.maps.buildings.features.Window;
import mrl.maps.city.CityRegion;
import mrl.maps.city.Prosperity;
import mrl.maps.city.Street;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Building extends CityRegion
{
    public int Num;

    private final Rect fPrivathandArea;
    private final ArrayList<Rect> fBlocks;
    private final ArrayList<Room> fRooms;
    private final ArrayList<Door> fDoors;
    private final ArrayList<Window> fWindows;

    // interior
    private Prosperity fProsperity;
    private HouseStatus fStatus;

    // exterior, address
    private Street fStreet;
    private SideType fFacadeSide;
    private Rect fInputAlley;

    public LocationType Location;
    public boolean Populated = false;

    public Building(GameSpace space, Object owner, IMap map, Rect privathandArea, Rect area, Prosperity prosperity)
    {
        super(space, owner, map, area);

        this.fPrivathandArea = privathandArea;
        this.fBlocks = new ArrayList<>();
        this.fRooms = new ArrayList<>();
        this.fDoors = new ArrayList<>();
        this.fWindows = new ArrayList<>();
        this.fProsperity = prosperity;
        this.fStatus = HouseStatus.hsNone;
    }

    public final Prosperity getProsperity()
    {
        return this.fProsperity;
    }

    public final void setProsperity(Prosperity value)
    {
        this.fProsperity = value;
    }

    public final Rect getPrivathandArea()
    {
        return this.fPrivathandArea;
    }

    public final void setStreet(Street street, SideType facadeSide)
    {
        this.fStreet = street;
        this.fFacadeSide = facadeSide;

        if (street != null) {
            this.Num = street.getNextHouseNumber();
        } else {
            this.Num = -1;
        }
    }

    public final SideType getFacadeSide()
    {
        return this.fFacadeSide;
    }
    
    public final void addBlock(Rect area)
    {
        this.fBlocks.add(area);
    }

    public final Room addRoom(Rect area)
    {
        Room res = new Room(this.fSpace, this);
        res.setArea(area);
        this.fRooms.add(res);
        return res;
    }

    public final Door addDoor(Point pt, Axis sideAxis, boolean isMain)
    {
        Door res = new Door(this.fSpace, this, sideAxis);
        res.setPos(pt.X, pt.Y);
        res.IsMain = isMain;
        this.fDoors.add(res);
        this.fMap.getFeatures().add(res);
        return res;
    }

    public final Window addWindow(Point pt, Axis axis)
    {
        Window res = new Window(this.fSpace, this, axis);
        res.setPos(pt.X, pt.Y);
        this.fWindows.add(res);
        this.fMap.getFeatures().add(res);
        return res;
    }

    public final List<Rect> getBlocks()
    {
        return this.fBlocks;
    }

    public final List<Room> getRooms()
    {
        return this.fRooms;
    }

    public final List<Door> getDoors()
    {
        return this.fDoors;
    }

    public final HouseStatus getStatus()
    {
        return this.fStatus;
    }

    public final void setStatus(HouseStatus value)
    {
        this.fStatus = value;
    }

    public void flush()
    {
        Rect area = this.getArea().clone();
        
        if (this.fProsperity != Prosperity.pPoor) {
            Rect prhArea = this.fPrivathandArea.clone();
            fMap.fillArea(prhArea.Left, prhArea.Top, prhArea.Right, prhArea.Bottom, TileID.tid_Grass.Value, false);

            prhArea.inflate(-1, -1);
            Utils.genRareTiles(this.fMap, prhArea, 0.1f, TileID.tid_Tree.Value, true);

            Rect alley = this.fInputAlley;
            fMap.fillArea(alley.Left, alley.Top, alley.Right, alley.Bottom, TileID.tid_Road.Value, false);
            fMap.fillArea(alley.Left, alley.Top, alley.Right, alley.Bottom, 0, true);
        } else {
            for (Door door : this.fDoors) {
                if (door.IsMain) {
                    int x = door.getPosX();
                    int y = door.getPosY();
                    switch (door.getSideAxis()) {
                        case axHorz:
                            fMap.getTile(x, y - 1).Foreground = 0;
                            fMap.getTile(x, y + 1).Foreground = 0;
                            break;
                        case axVert:
                            fMap.getTile(x - 1, y).Foreground = 0;
                            fMap.getTile(x + 1, y).Foreground = 0;
                            break;
                    }
                    break;
                }
            }
        }

        fMap.fillArea(area.Left, area.Top, area.Right, area.Bottom, TileID.tid_HouseFloor.Value, false);
        fMap.fillArea(area.Left, area.Top, area.Right, area.Bottom, 0, true);

        BuildingRenderer.render(fMap, this);

        for (Door door : this.fDoors) {
            door.render();

            // for other generators
            this.setNFM(door.getPosX(), door.getPosY(), door.getSideAxis());
        }

        for (Window win : this.fWindows) {
            win.render();
        }
    }

    private void setNFM(int x, int y, Axis sideAxis)
    {
        switch (sideAxis) {
            case axHorz:
                fMap.getTile(x, y - 1).addStates(TileStates.TS_NFM);
                fMap.getTile(x, y + 1).addStates(TileStates.TS_NFM);
                break;

            case axVert:
                fMap.getTile(x - 1, y).addStates(TileStates.TS_NFM);
                fMap.getTile(x + 1, y).addStates(TileStates.TS_NFM);
                break;
        }
    }
    
    public final String getAddress()
    {
        String res = "";

        if (this.fStreet != null) {
            res = String.valueOf(this.fStreet.Num) + " street, " + String.valueOf(this.Num);
        } else {
            res = "unknown address";
        }

        return res;
    }

    public void fill()
    {
        List<Human> residents = this.populate();

        this.fillApartmentRooms(residents);
    }
    
    private void fillApartmentRooms(List<Human> residents)
    {
        List<Room> rooms = this.fRooms;
        HouseStatus status = this.fStatus;
        
        // bedrooms for residents
        List<Human> bedsOwners = new ArrayList<>();
        for (Human res : residents) {
            if (!bedsOwners.contains(res)) {
                Human sp = res.getSpouse();
                if (sp == null || !bedsOwners.contains(sp)) {
                    bedsOwners.add(res);
                }
            }
        }

        // fill bedrooms
        List<Room> bedRooms = new ArrayList<>();
        for (Room room : rooms) {
            switch (status) {
                case hsShack:
                    bedRooms.add(room);
                    break;

                default:
                    if (!room.getTypes().contains(RoomType.rtHall) && room.LocationType == RoomLocationType.rltHouseOuter) {
                        bedRooms.add(room);
                    }
                    break;
            }
        }
        for (Human res : bedsOwners) {
            Room bedroom = AuxUtils.getRandomItem(bedRooms);
            this.fillRoom(bedroom, RoomType.rtBedroom, res);
        }

        // search free rooms
        List<Room> freeRooms = new ArrayList<>();
        for (Room room : rooms) {
            switch (status) {
                case hsShack:
                    freeRooms.add(room);
                    break;

                case hsHouse:
                    if (!room.getTypes().contains(RoomType.rtBedroom)) {
                        freeRooms.add(room);
                    }
                    break;

                default:
                    if (!room.getTypes().hasIntersect(RoomType.rtBedroom, RoomType.rtHall)) {
                        freeRooms.add(room);
                    }
                    break;
            }
        }

        // one kitchen
        Room kitchen = AuxUtils.getRandomItem(freeRooms);
        this.fillRoom(kitchen, RoomType.rtKitchen, null);
        rooms.remove(kitchen);
    }

    private void fillRoom(Room room, RoomType type, Object extObj)
    {
        if (room == null) {
            Logger.write("Building.fillRoom(): room is null, " + type.name());
            return;
        }
        
        try {
            room.getTypes().include(type);
            Rect area = room.getInnerArea();

            Point pt;
            switch (type) {
                case rtKitchen:
                    pt = TileFinder.findTile(fMap, area, TileFinder.TSF_BORDER | TileFinder.TSF_FREE);

                    Stove stove = new Stove(this.fSpace, this, pt.X, pt.Y);
                    this.fMap.getFeatures().add(stove);
                    break;

                case rtBedroom:
                    pt = TileFinder.findTile(fMap, area, TileFinder.TSF_BORDER | TileFinder.TSF_FREE);

                    Bed bed = new Bed(this.fSpace, this, pt.X, pt.Y);
                    this.fMap.getFeatures().add(bed);
                    break;

                case rtBathroom:
                    break;

                case rtStoreroom:
                    break;
            }
        } catch (Exception ex) {
            Logger.write("Building.fillRoom(): " + ex.getMessage());
        }
    }
    
    public final Room getRandomRoom()
    {
        return AuxUtils.getRandomItem(this.fRooms);
    }
    
    public final Point getFreeTile(Rect area)
    {
        return AuxUtils.getRandomPoint(area);
    }

    public final Point getRandomPos()
    {
        Room room = this.getRandomRoom();
        if (room == null) {
            return null;
        } else {
            Rect area = room.getInnerArea();
            return this.getFreeTile(area);
        }
    }

    private List<Human> populate()
    {
        List<Human> residents = new ArrayList<>();
        
        Layer layer = (Layer) this.fMap;

        FamilyGenerator familyGen = new FamilyGenerator();

        Human aptOwner = familyGen.generateNPC(layer, this, CreatureSex.csNone, true);
        aptOwner.setApartment(this);
        residents.add(aptOwner);

        // add family members
        // 60% owner have a spouse
        if (AuxUtils.getRandom(100) <= 60) {
            // set correct spouse sex
            CreatureSex spouseSex;
            if (aptOwner.getSex() == CreatureSex.csMale) {
                spouseSex = CreatureSex.csFemale;
            } else {
                spouseSex = CreatureSex.csMale;
            }

            Human spouse = familyGen.generateNPC(layer, this, spouseSex, true);
            spouse.setApartment(this);
            aptOwner.setSpouse(spouse);
            residents.add(spouse);
        }

        // 40% owner have a one child
        if (AuxUtils.getRandom(100) <= 40) {
            Human child = familyGen.generateChild(layer, this, aptOwner);
            child.setApartment(this);
            residents.add(child);
        }

        // 20% owner have a second child
        if (AuxUtils.getRandom(100) <= 20) {
            Human child = familyGen.generateChild(layer, this, aptOwner);
            child.setApartment(this);
            residents.add(child);
        }

        // 10% owner have a third child
        if (AuxUtils.getRandom(100) <= 10) {
            Human child = familyGen.generateChild(layer, this, aptOwner);
            child.setApartment(this);
            residents.add(child);
        }
        
        this.Populated = true;
        
        return residents;
    }
    
    public final Room findRoom(int x, int y)
    {
        for (Room room : this.fRooms) {
            Rect area = room.getInnerArea();
            if (area.contains(x, y)) {
                return room;
            }
        }

        return null;
    }
    
    public final void setInputAlley(Rect alley)
    {
        this.fInputAlley = alley;
    }
}
