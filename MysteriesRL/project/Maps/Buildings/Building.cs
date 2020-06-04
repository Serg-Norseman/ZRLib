/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih.
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
using BSLib;
using MysteriesRL.Creatures;
using MysteriesRL.Generators;
using MysteriesRL.Maps.Buildings.Features;
using ZRLib.Core;
using ZRLib.External.BSP;
using ZRLib.Map;

namespace MysteriesRL.Maps.Buildings
{
    public enum HouseStatus
    {
        None,
        Shack,
        House,
        DetachedHouse,
        Mansion
    }

    public sealed class Building : CityRegion
    {
        private readonly ExtRect fPrivathandArea;
        private readonly List<ExtRect> fBlocks;
        private readonly List<Room> fRooms;
        private readonly List<Door> fDoors;
        private readonly List<Window> fWindows;

        // interior
        private Prosperity fProsperity;
        private HouseStatus fStatus;

        // exterior, address
        private Street fStreet;
        private SideType fFacadeSide;
        private ExtRect fInputAlley;

        public int Num;
        public LocationType Location;
        public bool Populated = false;


        public string Address
        {
            get {
                string res = "";

                if (fStreet != null) {
                    res = Convert.ToString(fStreet.Num) + " street, " + Convert.ToString(Num);
                } else {
                    res = "unknown address";
                }

                return res;
            }
        }

        public IList<ExtRect> Blocks
        {
            get { return fBlocks; }
        }

        public IList<Door> Doors
        {
            get { return fDoors; }
        }

        public SideType FacadeSide
        {
            get { return fFacadeSide; }
        }

        public ExtRect PrivathandArea
        {
            get { return fPrivathandArea; }
        }

        public Prosperity Prosperity
        {
            get { return fProsperity; }
            set { fProsperity = value; }
        }

        public ExtPoint RandomPos
        {
            get {
                Room room = GetRandomRoom();
                if (room == null) {
                    return ExtPoint.Empty;
                } else {
                    ExtRect area = room.InnerArea;
                    return GetFreeTile(area);
                }
            }
        }

        public IList<Room> Rooms
        {
            get { return fRooms; }
        }

        public HouseStatus Status
        {
            get { return fStatus; }
            set { fStatus = value; }
        }


        public Building(GameSpace space, object owner, IMap map, ExtRect privathandArea, ExtRect area, Prosperity prosperity)
            : base(space, owner, map, area)
        {
            fPrivathandArea = privathandArea;
            fBlocks = new List<ExtRect>();
            fRooms = new List<Room>();
            fDoors = new List<Door>();
            fWindows = new List<Window>();
            fProsperity = prosperity;
            fStatus = HouseStatus.None;
        }

        public void SetStreet(Street street, SideType facadeSide)
        {
            fStreet = street;
            fFacadeSide = facadeSide;

            Num = (street == null) ? -1 : street.NextHouseNumber;
        }

        public void AddBlock(ExtRect area)
        {
            fBlocks.Add(area);
        }

        public Room AddRoom(ExtRect area)
        {
            Room res = new Room(fSpace, this);
            res.Area = area;
            fRooms.Add(res);
            return res;
        }

        public Door AddDoor(ExtPoint pt, Axis sideAxis, bool isMain)
        {
            Door res = new Door(fSpace, this, sideAxis);
            res.SetPos(pt.X, pt.Y);
            res.IsMain = isMain;
            fDoors.Add(res);
            fMap.Features.Add(res);
            return res;
        }

        public Window AddWindow(ExtPoint pt, Axis axis)
        {
            Window res = new Window(fSpace, this, axis);
            res.SetPos(pt.X, pt.Y);
            fWindows.Add(res);
            fMap.Features.Add(res);
            return res;
        }

        public void Flush()
        {
            ExtRect area = Area;

            if (fProsperity != Prosperity.Poor) {
                ExtRect prhArea = fPrivathandArea;
                fMap.FillArea(prhArea.Left, prhArea.Top, prhArea.Right, prhArea.Bottom, (int)TileID.tid_Grass, false);

                prhArea.Inflate(-1, -1);
                MapUtils.GenRareTiles(fMap, prhArea, 0.1f, (int)TileID.tid_Tree, (short)TileID.tid_Water, true);

                ExtRect alley = fInputAlley;
                fMap.FillArea(alley.Left, alley.Top, alley.Right, alley.Bottom, (int)TileID.tid_Road, false);
                fMap.FillArea(alley.Left, alley.Top, alley.Right, alley.Bottom, 0, true);
            } else {
                foreach (Door door in fDoors) {
                    if (door.IsMain) {
                        int x = door.PosX;
                        int y = door.PosY;
                        switch (door.SideAxis) {
                            case Axis.Horz:
                                fMap.GetTile(x, y - 1).Foreground = 0;
                                fMap.GetTile(x, y + 1).Foreground = 0;
                                break;
                            case Axis.Vert:
                                fMap.GetTile(x - 1, y).Foreground = 0;
                                fMap.GetTile(x + 1, y).Foreground = 0;
                                break;
                        }
                        break;
                    }
                }
            }

            fMap.FillArea(area.Left, area.Top, area.Right, area.Bottom, (int)TileID.tid_HouseFloor, false);
            fMap.FillArea(area.Left, area.Top, area.Right, area.Bottom, 0, true);

            BuildingRenderer.Render(fMap, this);

            foreach (Door door in fDoors) {
                door.Render();

                // for other generators
                SetNFM(door.PosX, door.PosY, door.SideAxis);
            }

            foreach (Window win in fWindows) {
                win.Render();
            }
        }

        private void SetNFM(int x, int y, Axis sideAxis)
        {
            switch (sideAxis) {
                case Axis.Horz:
                    fMap.GetTile(x, y - 1).AddStates(BaseTile.TS_NFM);
                    fMap.GetTile(x, y + 1).AddStates(BaseTile.TS_NFM);
                    break;

                case Axis.Vert:
                    fMap.GetTile(x - 1, y).AddStates(BaseTile.TS_NFM);
                    fMap.GetTile(x + 1, y).AddStates(BaseTile.TS_NFM);
                    break;
            }
        }

        public void Fill()
        {
            IList<Human> residents = Populate();

            FillApartmentRooms(residents);
        }

        private void FillApartmentRooms(IList<Human> residents)
        {
            IList<Room> rooms = fRooms;
            HouseStatus status = fStatus;

            // bedrooms for residents
            IList<Human> bedsOwners = new List<Human>();
            foreach (Human res in residents) {
                if (!bedsOwners.Contains(res)) {
                    Human sp = res.Spouse;
                    if (sp == null || !bedsOwners.Contains(sp)) {
                        bedsOwners.Add(res);
                    }
                }
            }

            // fill bedrooms
            IList<Room> bedRooms = new List<Room>();
            foreach (Room room in rooms) {
                switch (status) {
                    case HouseStatus.Shack:
                        bedRooms.Add(room);
                        break;

                    default:
                        if (!room.Types.Contains(RoomType.Hall) && room.LocationType == RoomLocationType.HouseOuter) {
                            bedRooms.Add(room);
                        }
                        break;
                }
            }
            foreach (Human res in bedsOwners) {
                Room bedroom = RandomHelper.GetRandomItem(bedRooms);
                FillRoom(bedroom, RoomType.Bedroom, res);
            }

            // search free rooms
            IList<Room> freeRooms = new List<Room>();
            foreach (Room room in rooms) {
                switch (status) {
                    case HouseStatus.Shack:
                        freeRooms.Add(room);
                        break;

                    case HouseStatus.House:
                        if (!room.Types.Contains(RoomType.Bedroom)) {
                            freeRooms.Add(room);
                        }
                        break;

                    default:
                        if (!room.Types.HasIntersect(RoomType.Bedroom, RoomType.Hall)) {
                            freeRooms.Add(room);
                        }
                        break;
                }
            }

            // one kitchen
            Room kitchen = RandomHelper.GetRandomItem(freeRooms);
            FillRoom(kitchen, RoomType.Kitchen, null);
            rooms.Remove(kitchen);
        }

        private void FillRoom(Room room, RoomType type, object extObj)
        {
            if (room == null) {
                Logger.Write("Building.fillRoom(): room is null, " + type.ToString());
                return;
            }

            try {
                room.Types.Include(type);
                ExtRect area = room.InnerArea;

                ExtPoint pt;
                switch (type) {
                    case RoomType.Kitchen:
                        pt = MapUtils.FindTile(fMap, area, MapUtils.TSF_BORDER | MapUtils.TSF_FREE);

                        Stove stove = new Stove(fSpace, this, pt.X, pt.Y);
                        fMap.Features.Add(stove);
                        break;

                    case RoomType.Bedroom:
                        pt = MapUtils.FindTile(fMap, area, MapUtils.TSF_BORDER | MapUtils.TSF_FREE);

                        Bed bed = new Bed(fSpace, this, pt.X, pt.Y);
                        fMap.Features.Add(bed);
                        break;

                    case RoomType.Bathroom:
                        break;

                    case RoomType.Storeroom:
                        break;
                }
            } catch (Exception ex) {
                Logger.Write("Building.fillRoom(): " + ex.Message);
            }
        }

        public Room GetRandomRoom()
        {
            return RandomHelper.GetRandomItem(fRooms);
        }

        public ExtPoint GetFreeTile(ExtRect area)
        {
            return RandomHelper.GetRandomPoint(area);
        }

        private IList<Human> Populate()
        {
            IList<Human> residents = new List<Human>();

            Layer layer = (Layer)fMap;

            FamilyGenerator familyGen = new FamilyGenerator();

            Human aptOwner = familyGen.GenerateNPC(layer, this, CreatureSex.csNone, true);
            aptOwner.Apartment = this;
            residents.Add(aptOwner);

            // add family members
            // 60% owner have a spouse
            if (RandomHelper.GetRandom(100) <= 60) {
                // set correct spouse sex
                CreatureSex spouseSex;
                if (aptOwner.Sex == CreatureSex.csMale) {
                    spouseSex = CreatureSex.csFemale;
                } else {
                    spouseSex = CreatureSex.csMale;
                }

                Human spouse = familyGen.GenerateNPC(layer, this, spouseSex, true);
                spouse.Apartment = this;
                aptOwner.Spouse = spouse;
                residents.Add(spouse);
            }

            // 40% owner have a one child
            if (RandomHelper.GetRandom(100) <= 40) {
                Human child = familyGen.GenerateChild(layer, this, aptOwner);
                child.Apartment = this;
                residents.Add(child);
            }

            // 20% owner have a second child
            if (RandomHelper.GetRandom(100) <= 20) {
                Human child = familyGen.GenerateChild(layer, this, aptOwner);
                child.Apartment = this;
                residents.Add(child);
            }

            // 10% owner have a third child
            if (RandomHelper.GetRandom(100) <= 10) {
                Human child = familyGen.GenerateChild(layer, this, aptOwner);
                child.Apartment = this;
                residents.Add(child);
            }

            Populated = true;

            return residents;
        }

        public Room FindRoom(int x, int y)
        {
            foreach (Room room in fRooms) {
                ExtRect area = room.InnerArea;
                if (area.Contains(x, y)) {
                    return room;
                }
            }

            return null;
        }

        public void SetInputAlley(ExtRect value)
        {
            fInputAlley = value;
        }
    }
}
