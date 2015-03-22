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
package mrl.game.social;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public enum CrimeType
{
    ct_Vandalism(0), // broked a window
    ct_Trespassing(1), // entered a restricted area
    ct_Thievery(2), // stole something
    ct_Assault(3), // hit someone
    ct_Murder(4); // killed someone

    public final int Value;

    private CrimeType(int value)
    {
        this.Value = value;
    }
}
