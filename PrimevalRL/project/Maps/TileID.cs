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

namespace PrimevalRL.Maps
{
    public enum TileID
    {
        /*  */ tid_Ground = 1,
        /*  */ tid_Grass = 2,
        /*  */ tid_Road = 3,
        /*  */ tid_Tree = 4,
        /*  */ tid_Square = 5,
        /*  */ tid_Water = 6,
        /*  */ tid_Water2 = 6 | (1 << 8),
        /*  */ tid_Water3 = 6 | (2 << 8),
        /*  */ tid_Grave = 7,

        /*  */ tid_HouseWall = 20,
        /*  */ tid_HouseDoor = 21,
        /*  */ tid_HouseBorder = 22,
        /*  */ tid_HouseWindow = 23,
        /*  */ tid_HouseFloor = 24,

        /*  */ tid_HouseWindowH = 31,
        /*  */ tid_HouseWindowV = 32,
        /*  */ tid_HouseWindowB = 33,

        /*  */ tid_HouseDoorHO = 34,
        /*  */ tid_HouseDoorHC = 35,
        /*  */ tid_HouseDoorVO = 36,
        /*  */ tid_HouseDoorVC = 37,

        /*  */ tid_StairsA = 38,
        /*  */ tid_StairsD = 39,

        /*  */ tid_RoomWall = 50,
        /*  */ tid_Wall_SLT = 50 | (0 << 8),
        /*  */ tid_Wall_SRT = 50 | (1 << 8),
        /*  */ tid_Wall_SLB = 50 | (2 << 8),
        /*  */ tid_Wall_SRB = 50 | (3 << 8),
        /*  */ tid_Wall_SH  = 50 | (4 << 8),
        /*  */ tid_Wall_SV  = 50 | (5 << 8),
        /*  */ tid_Wall_SBI = 50 | (6 << 8),
        /*  */ tid_Wall_STI = 50 | (7 << 8),
        /*  */ tid_Wall_SLI = 50 | (8 << 8),
        /*  */ tid_Wall_SRI = 50 | (9 << 8),
        /*  */ tid_Wall_SC  = 50 | (10 << 8),

        /*  */ tid_BlockWall = 70,
        /*  */ tid_Wall_DLT  = 70 | ( 0 << 8),
        /*  */ tid_Wall_DRT  = 70 | ( 1 << 8),
        /*  */ tid_Wall_DLB  = 70 | ( 2 << 8),
        /*  */ tid_Wall_DRB  = 70 | ( 3 << 8),
        /*  */ tid_Wall_DH   = 70 | ( 4 << 8),
        /*  */ tid_Wall_DV   = 70 | ( 5 << 8),
        /*  */ tid_Wall_DBI  = 70 | ( 6 << 8),
        /*  */ tid_Wall_DTI  = 70 | ( 7 << 8),
        /*  */ tid_Wall_DLI  = 70 | ( 8 << 8),
        /*  */ tid_Wall_DRI  = 70 | ( 9 << 8),
        /*  */ tid_Wall_DC   = 70 | (10 << 8),
        /*  */ tid_Wall_DSBI = 70 | (11 << 8),
        /*  */ tid_Wall_DSTI = 70 | (12 << 8),
        /*  */ tid_Wall_DSLI = 70 | (13 << 8),
        /*  */ tid_Wall_DSRI = 70 | (14 << 8),

        /*  */ tid_PathStart = 101,
        /*  */ tid_PathFinish = 102,
        /*  */ tid_Path = 103,

        /*  */ tid_Stove = 150,
        /*  */ tid_Bed = 151,
        /*  */ tid_Portal0 = 152,
        /*  */ tid_Portal1 = 153,
        /*  */ tid_Portal2 = 154,
        /*  */ tid_Portal3 = 155,

        /*  */ tid_DungeonFloor = 171,
        /*  */ tid_DungeonWall = 172,

        /*  */ tid_DebugDistrictP = 200,
        /*  */ tid_DebugDistrictG = 201,
        /*  */ tid_DebugDistrictL = 202
    }
}
