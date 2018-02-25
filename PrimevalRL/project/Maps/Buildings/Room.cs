/*
 *  "PrimevalRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih.
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
using System.Text;
using BSLib;
using ZRLib.Core;
using ZRLib.External.BSP;

namespace PrimevalRL.Maps.Buildings
{
    using RoomTypes = EnumSet<RoomType>;

    public enum RoomType
    {
        rtNone,
        rtHall,
        rtKitchen,
        rtBedroom,
        rtLibrary,
        // кабинет
        rtPrivateOffice,
        // детская
        rtNursery,
        // гостевая
        rtGuestRoom,
        // столовая
        rtDiningRoom,
        rtBathroom,
        rtStoreroom
    }

    public enum RoomLocationType
    {
        rltInner,
        rltBlockOuter,
        rltHouseOuter
    }

    public sealed class Room : AreaEntity
    {
        private ExtRect fInnerArea;
        private RoomTypes fTypes;

        public readonly IList<NodeSide> OuterSides = new List<NodeSide>();
        public RoomLocationType LocationType = RoomLocationType.rltInner;

        public Room(GameSpace space, object owner)
            : base(space, owner)
        {
            fInnerArea = ExtRect.Empty;
            fTypes = new RoomTypes();
        }

        public override ExtRect Area
        {
            get {
                return base.Area;
            }
            set {
                base.Area = value;
    
                fInnerArea = value;
                fInnerArea.Inflate(-1, -1);
            }
        }

        public ExtRect InnerArea
        {
            get { return fInnerArea; }
        }

        public RoomTypes Types
        {
            get { return fTypes; }
        }

        public override string Desc
        {
            get {
                StringBuilder result = new StringBuilder();
    
                var types = Extensions.GetValues<RoomType>();
                foreach (RoomType rt in types) {
                    if (fTypes.Contains(rt)) {
                        if (result.Length != 0) {
                            result.Append(", ");
                        }
                        result.Append(rt.ToString());
                    }
                }
    
                return result.ToString();
            }
        }
    }
}
