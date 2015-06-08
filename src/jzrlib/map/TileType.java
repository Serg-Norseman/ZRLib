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
package jzrlib.map;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public enum TileType
{
    ttUndefined(0),

    ttGround(1),
    ttGrass(2),
    ttFloor(3),

    ttWater(4),
    ttDeepWater(5),
    ttDeeperWater(6),

    ttRock(7),
    ttCaveFloor(8),
    ttCaveWall(9),

    ttMountain(10),
    ttTree(12),

    ttWall(14),
    ttBWallN(15),
    ttBWallS(16),
    ttBWallW(17),
    ttBWallE(18),
    ttBWallNW(19),
    ttBWallNE(20),
    ttBWallSW(21),
    ttBWallSE(22),

    ttDoor(23),
    ttDoorXX_Closed(24),
    ttDoorYY_Closed(25),
    ttDoorXX_Opened(26),
    ttDoorYY_Opened(27),

    ttDungeonWall(28),

    ttUndefinedMark(29),
    ttAreaGeneratorMark(30),
    ttRetriesExhaustMark(31),
    ttPointToOtherAreaMark(32),

    ttLinearCorridorWall(33),
    ttRectRoomWall(34),
    ttCylindricityRoomWall(35);

    private final int fValue;

    private TileType(int value)
    {
        this.fValue = value;
    }

    public int getValue()
    {
        return this.fValue;
    }
}
