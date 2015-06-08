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
package jzrlib.core.body;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class Bodypart
{
    public static final int STATE_NORMAL = 0;
    public static final int STATE_DAMAGED = 1;
    public static final int STATE_DESTROYED = 2;

    public int Type;
    public int State;
    public Object Item;
    
    public Bodypart(int type)
    {
        this.Type = type;
        this.State = STATE_NORMAL;
        this.Item = null;
    }
    
    public Bodypart(int type, int state)
    {
        this.Type = type;
        this.State = state;
        this.Item = null;
    }
}
