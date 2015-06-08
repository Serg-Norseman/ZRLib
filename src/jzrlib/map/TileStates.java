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
public final class TileStates
{
    public static final int TS_SEEN = 1;
    public static final int TS_VISITED = 2;
    public static final int TS_NFM = 4; // special "not free marker"
    
    public static final boolean isEmpty(int states)
    {
        return (states == 0);
    }
    
    public static final int include(int states, int state)
    {
        return (states | state);
    }

    public static final int include(int states, int... args)
    {
        for (int st : args) {
            states |= st;
        }
        return states;
    }

    public static final int exclude(int states, int state)
    {
        return states & (state ^ 255);
    }

    public static final boolean hasState(int states, int state)
    {
        return ((states & state) > 0);
    }
}
