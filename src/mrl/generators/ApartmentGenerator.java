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
package mrl.generators;

import java.util.ArrayList;
import java.util.List;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.external.bsp.BSPNode;
import jzrlib.external.bsp.BSPTree;
import jzrlib.external.bsp.LocationType;
import jzrlib.external.bsp.NodeSide;
import jzrlib.external.bsp.SideType;
import jzrlib.external.bsp.SplitterType;
import jzrlib.utils.AuxUtils;
import jzrlib.utils.RefObject;
import mrl.maps.Axis;
import mrl.maps.Utils;
import mrl.maps.buildings.Building;
import mrl.maps.buildings.HouseStatus;
import mrl.maps.buildings.Room;
import mrl.maps.buildings.RoomLocationType;
import mrl.maps.buildings.RoomType;
import mrl.maps.city.Prosperity;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class ApartmentGenerator
{
    private final Building fBuilding;
    private final List<Room> fOuterRooms;
    private Point fMainDoor = null;
    
    private static final int RoomsMinSize = 5;
    private static final int RoomsMaxSize = 10;

    public ApartmentGenerator(Building building)
    {
        this.fBuilding = building;
        this.fOuterRooms = new ArrayList<>();
    }
    
    public final void build()
    {
        //SideType facadeSide = getFacade();
        
        // generate room blocks
        switch (this.fBuilding.getProsperity()) {
            case pPoor:
            case pGood:
                buildRoomsBlock(this.fBuilding.getArea());
                break;

            case pLux:
                int blockMinSize = RoomsMinSize * 3;
                int blockMaxSize = RoomsMinSize * 4;
                
                BSPTree tree = new BSPTree(this.fBuilding.getArea(), blockMinSize, blockMaxSize, true, null, this::splitHandler);

                List<BSPNode> leaves = tree.getLeaves();
                for (BSPNode node : leaves) {
                    Rect blockArea = new Rect(node.x1, node.y1, node.x2, node.y2);
                    this.fBuilding.addBlock(blockArea);
                    this.buildRoomsBlock(blockArea);
                }

                break;
        }
        
        this.decorate();
        this.fBuilding.flush();
    }

    private void buildRoomsBlock(Rect blockArea)
    {
        BSPTree tree = new BSPTree(blockArea, RoomsMinSize, RoomsMaxSize, false, null, null);
        tree.checkLeaves();

        List<Room> localOuterRooms = new ArrayList<>();
        
        List<BSPNode> leaves = tree.getLeaves();
        for (BSPNode node : leaves) {
            Rect area = new Rect(node.x1, node.y1, node.x2, node.y2);

            Room room = this.fBuilding.addRoom(area);

            if (node.nodeType == LocationType.ltOuter) {
                room.LocationType = RoomLocationType.rltBlockOuter;
                this.fOuterRooms.add(room);
                localOuterRooms.add(room);
                
                for (NodeSide side : node.sides) {
                    if (side.location == LocationType.ltOuter) {
                        room.OuterSides.add(side);
                    }
                }
            } else {
                room.LocationType = RoomLocationType.rltInner;
            }
        }
        
        // generate doors
        BSPNode root = tree.getRoot();
        this.processBlockDoors(root, root);        

        // check rooms types
        Rect bldArea = this.fBuilding.getArea();       
        for (Room room : localOuterRooms) {
            for (NodeSide side : room.OuterSides) {
                Point pt = Utils.getCheckPoint(side.type, room.getArea(), 1);
                if (!bldArea.contains(pt)) {
                    room.LocationType = RoomLocationType.rltHouseOuter;
                    break;
                }
            }
        }
        
        // process interblock doors
        if (this.fBuilding.getProsperity() == Prosperity.pLux) {
            List<Room> blocksOuterRooms = new ArrayList<>();
            for (Room room : localOuterRooms) {
                if (room.LocationType == RoomLocationType.rltBlockOuter) {
                    blocksOuterRooms.add(room);
                }
            }

            // generate door
            Room room = AuxUtils.getRandomItem(blocksOuterRooms);
            if (room != null) {
                NodeSide side = AuxUtils.getRandomItem(room.OuterSides);
                this.makeRoomDoor(room, side.type, false);
            }
        }
    }

    private void decorate()
    {
        this.processMainDoor();
        this.processWindows();
        
        Prosperity prosp = this.fBuilding.getProsperity();
        int rooms = this.fBuilding.getRooms().size();
        
        switch (prosp) {
            case pPoor:
                if (rooms <= 1) {
                    this.fBuilding.setStatus(HouseStatus.hsShack);
                } else {
                    this.fBuilding.setStatus(HouseStatus.hsHouse);
                }
                break;

            case pGood:
                this.fBuilding.setStatus(HouseStatus.hsDetachedHouse);
                break;

            case pLux:
                this.fBuilding.setStatus(HouseStatus.hsMansion);
                break;
        }
    }

    private void processMainDoor()
    {
        SideType facadeSide = this.fBuilding.getFacadeSide();
        List<Room> halls = new ArrayList<>();
        Rect bldArea = this.fBuilding.getArea();
        
        for (Room room : this.fOuterRooms) {
            if (room.LocationType == RoomLocationType.rltHouseOuter) {
                Rect roomArea = room.getArea();

                boolean res = false;
                switch (facadeSide) {
                    case stTop:
                        res = (roomArea.Top == bldArea.Top);
                        break;
                    case stRight: 
                        res = (roomArea.Right == bldArea.Right);
                        break;
                    case stBottom:
                        res = (roomArea.Bottom == bldArea.Bottom);
                        break;
                    case stLeft:
                        res = (roomArea.Left == bldArea.Left);
                        break;
                }

                if (res) {
                    halls.add(room);
                }
            }
        }
        
        Point apDoor = null;
        
        Room room = AuxUtils.getRandomItem(halls);
        if (room != null) {
            room.getTypes().include(RoomType.rtHall);
            apDoor = this.makeRoomDoor(room, facadeSide, true);
            this.fMainDoor = apDoor.clone();
        }
        
        if (apDoor != null && this.fBuilding.getProsperity() != Prosperity.pPoor) {
            Rect prArea = this.fBuilding.getPrivathandArea();
            Rect inputAlley = null;
            
            switch (facadeSide) {
                case stTop:
                    inputAlley = new Rect(apDoor.X, prArea.Top, apDoor.X, apDoor.Y - 1);
                    break;
                case stBottom:
                    inputAlley = new Rect(apDoor.X, apDoor.Y + 1, apDoor.X, prArea.Bottom);
                    break;
                case stLeft:
                    inputAlley = new Rect(prArea.Left, apDoor.Y, apDoor.X - 1, apDoor.Y);
                    break;
                case stRight:
                    inputAlley = new Rect(apDoor.X + 1, apDoor.Y, prArea.Right, apDoor.Y);
                    break;
            }
            
            this.fBuilding.setInputAlley(inputAlley);
        }
    }

    private Point makeRoomDoor(Room room, SideType side, boolean isMain)
    {
        Rect roomArea = room.getArea();

        int dx, dy;
        Point res = null;
        switch (side) {
            case stTop:
                dy = roomArea.Top;
                dx = AuxUtils.getBoundedRnd(roomArea.Left + 1, roomArea.Right - 1);
                break;
            case stRight:
                dx = roomArea.Right;
                dy = AuxUtils.getBoundedRnd(roomArea.Top + 1, roomArea.Bottom - 1);
                break;
            case stBottom:
                dy = roomArea.Bottom;
                dx = AuxUtils.getBoundedRnd(roomArea.Left + 1, roomArea.Right - 1);
                break;
            case stLeft:
                dx = roomArea.Left;
                dy = AuxUtils.getBoundedRnd(roomArea.Top + 1, roomArea.Bottom - 1);
                break;
            default:
                return res;
        }
        
        res = new Point(dx, dy);
        fBuilding.addDoor(res, Utils.getSideAxis(side), isMain);
        return res;
    }
    
    private void processBlockDoors(BSPNode root, BSPNode node)
    {
        if (node != null) {
            if (node.left != null && node.right != null) {
                List<Point> points = new ArrayList<>();

                Axis sideAxis = Axis.axNone;
                SplitterType splitType = node.splitterType;
                if (splitType == SplitterType.stHorz) {
                    sideAxis = Axis.axHorz;
                    int ay = node.right.y1;
                    for (int ax = node.left.x1 + 2; ax <= node.left.x2 - 2; ax++) {
                        if (isValidDoor(root, ax, ay, splitType)) {
                            points.add(new Point(ax, ay));
                        }
                    }
                } else if (splitType == SplitterType.stVert) {
                    sideAxis = Axis.axVert;
                    int ax = node.right.x1;
                    for (int ay = node.right.y1 + 2; ay <= node.right.y2 - 2; ay++) {
                        if (isValidDoor(root, ax, ay, splitType)) {
                            points.add(new Point(ax, ay));
                        }
                    }
                }

                Point pt = AuxUtils.getRandomItem(points);
                if (pt != null) {
                    fBuilding.addDoor(pt, sideAxis, false);
                    points.clear();
                }

                this.processBlockDoors(root, node.left);
                this.processBlockDoors(root, node.right);
            }
        }
    }

    private boolean isValidDoor(BSPNode node, int ax, int ay, SplitterType splitAxis)
    {
        if (node != null) {
            if (node.left != null && node.right != null) {
                SplitterType splitType = node.splitterType;

                if (splitType != splitAxis) {
                    if (splitType == SplitterType.stHorz && ay == node.right.y1 && (ax == node.right.x1 || ax == node.right.x2)) {
                        return false;
                    } else if (splitType == SplitterType.stVert && ax == node.right.x1 && (ay == node.right.y1 || ay == node.right.y2)) {
                        return false;
                    }
                }

                return isValidDoor(node.left, ax, ay, splitAxis) && isValidDoor(node.right, ax, ay, splitAxis);
            }
            return true;
        }
        return true;
    }
    
    private void splitHandler(BSPNode node, RefObject<Integer> refSplitWidth)
    {
        refSplitWidth.argValue = 1;
    }
    
    private void processWindows()
    {
        Rect bldArea = this.fBuilding.getArea();
        
        for (Room room : this.fOuterRooms) {
            if (room.LocationType == RoomLocationType.rltHouseOuter) {
                for (NodeSide side : room.OuterSides) {
                    Point pt = Utils.getCheckPoint(side.type, room.getArea(), 1);
                    if (!bldArea.contains(pt)) {
                        Point winPt = getWinPoint(side.type, room.getArea());
                        
                        if (!winPt.equals(this.fMainDoor)) {
                            this.fBuilding.addWindow(winPt, Utils.getSideAxis(side.type));
                        }
                    }
                }
            }
        }
    }
    
    private static Point getWinPoint(SideType side, Rect area)
    {
        Point result;
        switch (side) {
            case stTop:
                result = new Point(AuxUtils.getBoundedRnd(area.Left + 2, area.Right - 2), area.Top);
                break;
            case stRight: 
                result = new Point(area.Right, AuxUtils.getBoundedRnd(area.Top + 2, area.Bottom - 2));
                break;
            case stBottom:
                result = new Point(AuxUtils.getBoundedRnd(area.Left + 2, area.Right - 2), area.Bottom);
                break;
            case stLeft:
                result = new Point(area.Left, AuxUtils.getBoundedRnd(area.Top + 2, area.Bottom - 2));
                break;
            default:
                result = null;
        }

        return result;
    }
}
