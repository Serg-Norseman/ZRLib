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

import jzrlib.core.Point;
import jzrlib.utils.TypeUtils;

/**
 * It is basic tile implementation.
 * 
 * In the simplest case, any tile should have an identifier which shows - what 
 * is located in that the tile: only grass, tree, or a wall. In the case where 
 * image of tiles do not allow us to see what is on the ground level under 
 * a tree or under a wall - quite enough one identifier. However, even in this 
 * case, it is better to divide identifier into two portions (ground level and 
 * a higher level) for accurate processing and capabilities for the future 
 * expansion.
 * 
 * To abstract from the specific implementation, these two levels are named: 
 * <b>background</b> and <b>foreground</b>.
 * 
 * @author Serg V. Zhdanovskih
 */
public class BaseTile
{
    public short Background;
    public short Foreground;
    public short BackgroundExt;
    public short ForegroundExt;
    public int States;

    // PathSearch runtime
    public byte pf_status; // see TilePathStatus
    public Point pf_prev;

    public BaseTile()
    {
        this.Background = 0;
        this.Foreground = 0;
        this.BackgroundExt = 0;
        this.ForegroundExt = 0;
        this.States = 0;

        this.pf_status = 0;
        this.pf_prev = null;
    }

    public void assign(BaseTile source)
    {
        this.Background = source.Background;
        this.Foreground = source.Foreground;
        this.BackgroundExt = source.BackgroundExt;
        this.ForegroundExt = source.ForegroundExt;
        this.States = source.States;
    }

    // <editor-fold defaultstate="collapsed" desc="States methods">

    public final void addStates(int states)
    {
        this.States |= states;
    }
    
    public final boolean isEmptyStates()
    {
        return TileStates.isEmpty(this.States);
    }
    
    public final void includeState(int state)
    {
        this.States = TileStates.include(this.States, state);
    }

    public final void excludeState(int state)
    {
        this.States = TileStates.exclude(this.States, state);
    }

    public final void includeStates(int... states)
    {
        this.States = TileStates.include(this.States, states);
    }

    public final boolean hasState(int state)
    {
        return TileStates.hasState(this.States, state);
    }

    // </editor-fold>
    
    public static short getVarID(byte base, byte var)
    {
        return (short) ((var & 0xFF) << 8 | (base & 0xFF));
    }

    public final int getBackBase()
    {
        return TypeUtils.getShortLo(this.Background);
    }

    public final int getBackVar()
    {
        return TypeUtils.getShortHi(this.Background);
    }

    public final void setBack(int base)
    {
        this.Background = (short) base;
    }

    public final void setBack(int base, int var)
    {
        this.Background = (short) TypeUtils.fitShort(base, var);
    }

    public final int getForeBase()
    {
        return TypeUtils.getShortLo(this.Foreground);
    }

    public final int getForeVar()
    {
        return TypeUtils.getShortHi(this.Foreground);
    }

    public final void setFore(int base)
    {
        this.Foreground = (short) base;
    }

    public final void setFore(int base, int var)
    {
        this.Foreground = (short) TypeUtils.fitShort(base, var);
    }
}
