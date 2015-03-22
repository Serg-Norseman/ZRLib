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
package mrl.maps;

import java.awt.Color;
import java.util.HashMap;
import jzrlib.jterm.TermColors;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public enum TileID
{
    tid_Ground(1, '.', Color.darkGray, Color.black, false, new TileFlags(), 1.25f),
    tid_Grass(2, '.', Color.green, TermColors.darkGreen, true, new TileFlags(), 1.5f),
    tid_Road(3, (char) 176, Color.lightGray, Color.black, true, new TileFlags(), 1.0f),
    tid_Tree(4, /*'T'*/ (char) 6, Color.green, TermColors.darkGreen, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Square(5, (char) 177, Color.darkGray, Color.black, true, new TileFlags(), 1.0f),
    tid_Water(6, (char) 247, Color.cyan, Color.blue, true, new TileFlags(), 1000.0f),
    tid_Water2(6 | (1 << 8), (char) 247, TermColors.darkBlue, Color.blue, true, new TileFlags(), 1000.0f),
    tid_Water3(6 | (2 << 8), (char) 247, TermColors.darkCyan, Color.blue, true, new TileFlags(), 1000.0f),
    tid_Grave(7, (char) 177, Color.darkGray, Color.black, false, new TileFlags(TileFlags.tfForeground), 30.0f),

    tid_HouseWall(20, '+', Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_HouseDoor(21, '/', Color.magenta, Color.black, false, new TileFlags(/*TileFlags.tfBarrier, */TileFlags.tfBlockLOS, TileFlags.tfForeground), 20.0f),
    tid_HouseBorder(22, '#', Color.white, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f),
    tid_HouseWindow(23, '%', Color.cyan, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f),
    tid_HouseFloor(24, '.', Color.lightGray, Color.black, false, new TileFlags(), 1.0f),

    tid_HouseWindowH(31, (char) 196, Color.cyan, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f),
    tid_HouseWindowV(32, (char) 179, Color.cyan, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f),
    tid_HouseWindowB(33, '%', Color.cyan, Color.black, false, new TileFlags(TileFlags.tfForeground), 1.0f),
    
    tid_HouseDoorHO(34, '/', Color.lightGray, Color.black, false, new TileFlags(TileFlags.tfForeground), 20.0f),
    tid_HouseDoorHC(35, (char) 196, Color.lightGray, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_HouseDoorVO(36, '\\', Color.lightGray, Color.black, false, new TileFlags(TileFlags.tfForeground), 20.0f),
    tid_HouseDoorVC(37, (char) 179, Color.lightGray, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),

    tid_StairsA(38, '>', Color.lightGray, Color.black, false, new TileFlags(TileFlags.tfForeground), 20.0f),
    tid_StairsD(39, '<', Color.lightGray, Color.black, false, new TileFlags(TileFlags.tfForeground), 20.0f),

    tid_RoomWall(50, 'r', Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_SLT(50 | (0 << 8), (char) 218, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_SRT(50 | (1 << 8), (char) 191, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_SLB(50 | (2 << 8), (char) 192, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_SRB(50 | (3 << 8), (char) 217, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_SH(50 | (4 << 8), (char) 196, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_SV(50 | (5 << 8), (char) 179, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_SBI(50 | (6 << 8), (char) 193, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_STI(50 | (7 << 8), (char) 194, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_SLI(50 | (8 << 8), (char) 195, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_SRI(50 | (9 << 8), (char) 180, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_SC(50 | (10 << 8), (char) 197, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    
    tid_BlockWall(70, 'b', Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DLT(70 | (0 << 8), (char) 201, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DRT(70 | (1 << 8), (char) 187, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DLB(70 | (2 << 8), (char) 200, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DRB(70 | (3 << 8), (char) 188, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DH(70 | (4 << 8), (char) 205, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DV(70 | (5 << 8), (char) 186, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DBI(70 | (6 << 8), (char) 202, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DTI(70 | (7 << 8), (char) 203, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DLI(70 | (8 << 8), (char) 204, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DRI(70 | (9 << 8), (char) 185, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DC(70 | (10 << 8), (char) 206, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),

    tid_Wall_DSBI(70 | (11 << 8), (char) 207, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DSTI(70 | (12 << 8), (char) 209, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DSLI(70 | (13 << 8), (char) 199, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    tid_Wall_DSRI(70 | (14 << 8), (char) 182, Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfBlockLOS, TileFlags.tfForeground), 1000.0f),
    
    tid_PathStart(101, '!', Color.red, Color.black, false, new TileFlags(TileFlags.tfForeground), 1.0f),
    tid_PathFinish(102, '!', Color.white, Color.black, false, new TileFlags(TileFlags.tfForeground), 1.0f),
    tid_Path(103, '*', Color.cyan, Color.black, false, new TileFlags(TileFlags.tfForeground), 1.0f),

    tid_Stove(150, 'S', Color.red, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f),
    tid_Bed(151, 'B', Color.lightGray, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f),
    
    tid_DungeonFloor(171, '.', Color.darkGray, Color.black, false, new TileFlags(), 1.0f),
    tid_DungeonWall(172, 'X', Color.darkGray, Color.black, false, new TileFlags(TileFlags.tfBarrier, TileFlags.tfForeground), 1000.0f),
    
    tid_DebugDistrictP(200, 'D', Color.red, Color.black, true, new TileFlags(), 1.0f),
    tid_DebugDistrictG(201, 'D', Color.yellow, Color.black, true, new TileFlags(), 1.0f),
    tid_DebugDistrictL(202, 'D', Color.green, Color.black, true, new TileFlags(), 1.0f);

    public final int Value;
    public final char Sym;
    public final Color SymColor;
    public final Color BackColor;
    public final boolean MiniMap;
    public final TileFlags Flags;
    public final float Cost;
    
    private TileID(int value, char sym, Color symColor, Color backColor, boolean miniMap, TileFlags flags, float cost)
    {
        this.Value = value;
        this.Sym = sym;
        this.SymColor = symColor;
        this.BackColor = backColor;
        this.MiniMap = miniMap;
        this.Flags = flags;
        this.Cost = cost;
        getMappings().put(value, this);
    }

    private static HashMap<Integer, TileID> mappings;

    private static HashMap<Integer, TileID> getMappings()
    {
        synchronized (TileID.class) {
            if (mappings == null) {
                mappings = new HashMap<>();
            }
        }
        return mappings;
    }

    public static TileID forValue(int value)
    {
        return getMappings().get(value);
    }
}
