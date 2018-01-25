/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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

using System.Collections.Generic;
using BSLib;
using MysteriesRL.Maps;
using MysteriesRL.Maps.Buildings;
using ZRLib.External.BSP;

namespace MysteriesRL.Generators
{
    public class ApartmentGenerator
    {
        private readonly Building fBuilding;
        private readonly IList<Room> fOuterRooms;
        private ExtPoint fMainDoor;

        private const int RoomsMinSize = 5;
        private const int RoomsMaxSize = 10;

        public ApartmentGenerator(Building building)
        {
            fBuilding = building;
            fOuterRooms = new List<Room>();
        }

        public void Build()
        {
            //SideType facadeSide = getFacade();

            // generate room blocks
            switch (fBuilding.Prosperity) {
                case Prosperity.pPoor:
                case Prosperity.pGood:
                    BuildRoomsBlock(fBuilding.Area);
                    break;

                case Prosperity.pLux:
                    int blockMinSize = RoomsMinSize * 3;
                    int blockMaxSize = RoomsMinSize * 4;

                    BSPTree tree = new BSPTree(fBuilding.Area, blockMinSize, blockMaxSize, true, null, SplitHandler);

                    IList<BSPNode> leaves = tree.Leaves;
                    foreach (BSPNode node in leaves) {
                        ExtRect blockArea = ExtRect.Create(node.X1, node.Y1, node.X2, node.Y2);
                        fBuilding.AddBlock(blockArea);
                        BuildRoomsBlock(blockArea);
                    }

                    break;
            }

            Decorate();
            fBuilding.Flush();
        }

        private void BuildRoomsBlock(ExtRect blockArea)
        {
            BSPTree tree = new BSPTree(blockArea, RoomsMinSize, RoomsMaxSize, false, null, null);
            tree.CheckLeaves();

            IList<Room> localOuterRooms = new List<Room>();

            IList<BSPNode> leaves = tree.Leaves;
            foreach (BSPNode node in leaves) {
                ExtRect area = ExtRect.Create(node.X1, node.Y1, node.X2, node.Y2);

                Room room = fBuilding.AddRoom(area);

                if (node.NodeType == LocationType.ltOuter) {
                    room.LocationType = RoomLocationType.rltBlockOuter;
                    fOuterRooms.Add(room);
                    localOuterRooms.Add(room);

                    foreach (NodeSide side in node.Sides) {
                        if (side.Location == LocationType.ltOuter) {
                            room.OuterSides.Add(side);
                        }
                    }
                } else {
                    room.LocationType = RoomLocationType.rltInner;
                }
            }

            // generate doors
            BSPNode root = tree.Root;
            ProcessBlockDoors(root, root);

            // check rooms types
            ExtRect bldArea = fBuilding.Area;
            foreach (Room room in localOuterRooms) {
                foreach (NodeSide side in room.OuterSides) {
                    ExtPoint pt = MMapUtils.GetCheckPoint(side.Type, room.Area, 1);
                    if (!bldArea.Contains(pt)) {
                        room.LocationType = RoomLocationType.rltHouseOuter;
                        break;
                    }
                }
            }

            // process interblock doors
            if (fBuilding.Prosperity == Prosperity.pLux) {
                IList<Room> blocksOuterRooms = new List<Room>();
                foreach (Room room in localOuterRooms) {
                    if (room.LocationType == RoomLocationType.rltBlockOuter) {
                        blocksOuterRooms.Add(room);
                    }
                }

                // generate door
                Room rm = RandomHelper.GetRandomItem(blocksOuterRooms);
                if (rm != null) {
                    NodeSide side = RandomHelper.GetRandomItem(rm.OuterSides);
                    MakeRoomDoor(rm, side.Type, false);
                }
            }
        }

        private void Decorate()
        {
            ProcessMainDoor();
            ProcessWindows();

            Prosperity prosp = fBuilding.Prosperity;
            int rooms = fBuilding.Rooms.Count;

            switch (prosp) {
                case Prosperity.pPoor:
                    if (rooms <= 1) {
                        fBuilding.Status = HouseStatus.hsShack;
                    } else {
                        fBuilding.Status = HouseStatus.hsHouse;
                    }
                    break;

                case Prosperity.pGood:
                    fBuilding.Status = HouseStatus.hsDetachedHouse;
                    break;

                case Prosperity.pLux:
                    fBuilding.Status = HouseStatus.hsMansion;
                    break;
            }
        }

        private void ProcessMainDoor()
        {
            SideType facadeSide = fBuilding.FacadeSide;
            IList<Room> halls = new List<Room>();
            ExtRect bldArea = fBuilding.Area;

            foreach (Room room in fOuterRooms) {
                if (room.LocationType == RoomLocationType.rltHouseOuter) {
                    ExtRect roomArea = room.Area;

                    bool res = false;
                    switch (facadeSide) {
                        case SideType.stTop:
                            res = (roomArea.Top == bldArea.Top);
                            break;
                        case SideType.stRight:
                            res = (roomArea.Right == bldArea.Right);
                            break;
                        case SideType.stBottom:
                            res = (roomArea.Bottom == bldArea.Bottom);
                            break;
                        case SideType.stLeft:
                            res = (roomArea.Left == bldArea.Left);
                            break;
                    }

                    if (res) {
                        halls.Add(room);
                    }
                }
            }

            ExtPoint apDoor = ExtPoint.Empty;

            Room rm = RandomHelper.GetRandomItem(halls);
            if (rm != null) {
                rm.Types.Include(RoomType.rtHall);
                apDoor = MakeRoomDoor(rm, facadeSide, true);
                fMainDoor = apDoor;
            }

            if (!apDoor.IsEmpty && fBuilding.Prosperity != Prosperity.pPoor) {
                ExtRect prArea = fBuilding.PrivathandArea;
                ExtRect inputAlley;

                switch (facadeSide) {
                    case SideType.stTop:
                        inputAlley = ExtRect.Create(apDoor.X, prArea.Top, apDoor.X, apDoor.Y - 1);
                        break;
                    case SideType.stBottom:
                        inputAlley = ExtRect.Create(apDoor.X, apDoor.Y + 1, apDoor.X, prArea.Bottom);
                        break;
                    case SideType.stLeft:
                        inputAlley = ExtRect.Create(prArea.Left, apDoor.Y, apDoor.X - 1, apDoor.Y);
                        break;
                    case SideType.stRight:
                        inputAlley = ExtRect.Create(apDoor.X + 1, apDoor.Y, prArea.Right, apDoor.Y);
                        break;
                    default:
                        inputAlley = ExtRect.Empty;
                        break;
                }

                fBuilding.InputAlley = inputAlley;
            }
        }

        private ExtPoint MakeRoomDoor(Room room, SideType side, bool isMain)
        {
            ExtRect roomArea = room.Area;

            int dx, dy;
            ExtPoint res;
            switch (side) {
                case SideType.stTop:
                    dy = roomArea.Top;
                    dx = RandomHelper.GetBoundedRnd(roomArea.Left + 1, roomArea.Right - 1);
                    break;
                case SideType.stRight:
                    dx = roomArea.Right;
                    dy = RandomHelper.GetBoundedRnd(roomArea.Top + 1, roomArea.Bottom - 1);
                    break;
                case SideType.stBottom:
                    dy = roomArea.Bottom;
                    dx = RandomHelper.GetBoundedRnd(roomArea.Left + 1, roomArea.Right - 1);
                    break;
                case SideType.stLeft:
                    dx = roomArea.Left;
                    dy = RandomHelper.GetBoundedRnd(roomArea.Top + 1, roomArea.Bottom - 1);
                    break;
                default:
                    return ExtPoint.Empty;
            }

            res = new ExtPoint(dx, dy);
            fBuilding.AddDoor(res, MMapUtils.GetSideAxis(side), isMain);
            return res;
        }

        private void ProcessBlockDoors(BSPNode root, BSPNode node)
        {
            if (node != null) {
                if (node.Left != null && node.Right != null) {
                    IList<ExtPoint> points = new List<ExtPoint>();

                    Axis sideAxis = Axis.axNone;
                    SplitterType splitType = node.SplitterType;
                    if (splitType == SplitterType.stHorz) {
                        sideAxis = Axis.axHorz;
                        int ay = node.Right.Y1;
                        for (int ax = node.Left.X1 + 2; ax <= node.Left.X2 - 2; ax++) {
                            if (IsValidDoor(root, ax, ay, splitType)) {
                                points.Add(new ExtPoint(ax, ay));
                            }
                        }
                    } else if (splitType == SplitterType.stVert) {
                        sideAxis = Axis.axVert;
                        int ax = node.Right.X1;
                        for (int ay = node.Right.Y1 + 2; ay <= node.Right.Y2 - 2; ay++) {
                            if (IsValidDoor(root, ax, ay, splitType)) {
                                points.Add(new ExtPoint(ax, ay));
                            }
                        }
                    }

                    ExtPoint pt = RandomHelper.GetRandomItem(points);
                    if (!pt.IsEmpty) {
                        fBuilding.AddDoor(pt, sideAxis, false);
                        points.Clear();
                    }

                    ProcessBlockDoors(root, node.Left);
                    ProcessBlockDoors(root, node.Right);
                }
            }
        }

        private bool IsValidDoor(BSPNode node, int ax, int ay, SplitterType splitAxis)
        {
            if (node != null) {
                if (node.Left != null && node.Right != null) {
                    SplitterType splitType = node.SplitterType;

                    if (splitType != splitAxis) {
                        if (splitType == SplitterType.stHorz && ay == node.Right.Y1 && (ax == node.Right.X1 || ax == node.Right.X2)) {
                            return false;
                        } else if (splitType == SplitterType.stVert && ax == node.Right.X1 && (ay == node.Right.Y1 || ay == node.Right.Y2)) {
                            return false;
                        }
                    }

                    return IsValidDoor(node.Left, ax, ay, splitAxis) && IsValidDoor(node.Right, ax, ay, splitAxis);
                }
                return true;
            }
            return true;
        }

        private void SplitHandler(BSPNode node, ref int refSplitWidth)
        {
            refSplitWidth = 1;
        }

        private void ProcessWindows()
        {
            ExtRect bldArea = fBuilding.Area;

            foreach (Room room in fOuterRooms) {
                if (room.LocationType == RoomLocationType.rltHouseOuter) {
                    foreach (NodeSide side in room.OuterSides) {
                        ExtPoint pt = MMapUtils.GetCheckPoint(side.Type, room.Area, 1);
                        if (!bldArea.Contains(pt)) {
                            ExtPoint winPt = GetWinPoint(side.Type, room.Area);

                            if (!winPt.Equals(fMainDoor)) {
                                fBuilding.AddWindow(winPt, MMapUtils.GetSideAxis(side.Type));
                            }
                        }
                    }
                }
            }
        }

        private static ExtPoint GetWinPoint(SideType side, ExtRect area)
        {
            ExtPoint result;
            switch (side) {
                case SideType.stTop:
                    result = new ExtPoint(RandomHelper.GetBoundedRnd(area.Left + 2, area.Right - 2), area.Top);
                    break;
                case SideType.stRight:
                    result = new ExtPoint(area.Right, RandomHelper.GetBoundedRnd(area.Top + 2, area.Bottom - 2));
                    break;
                case SideType.stBottom:
                    result = new ExtPoint(RandomHelper.GetBoundedRnd(area.Left + 2, area.Right - 2), area.Bottom);
                    break;
                case SideType.stLeft:
                    result = new ExtPoint(area.Left, RandomHelper.GetBoundedRnd(area.Top + 2, area.Bottom - 2));
                    break;
                default:
                    result = ExtPoint.Empty;
                    break;
            }

            return result;
        }
    }
}
