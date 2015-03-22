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
package mrl.creatures;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class Damage
{
    public static final int DMG_GENERIC = 0;
    public static final int DMG_CUT = 1;
    public static final int DMG_BLUNT = 2;
    public static final int DMG_NONLETHAL = 3;
    public static final int DMG_BLOODLOSS = 4;
    
    public final int Type;
    public final int Value;

    private Creature fInflictor;

    public Damage(int value)
    {
        this.Value = value;
        this.Type = 0;
    }

    public Damage(int value, int damageType)
    {
        this.Value = value;
        this.Type = damageType;
    }

    public Damage(int value, int damageType, Creature inflictor)
    {
        this.Value = value;
        this.Type = damageType;
        this.fInflictor = inflictor;
    }

    public final Creature getInflictor()
    {
        return this.fInflictor;
    }

    public final void setInflictor(Creature inflictor)
    {
        this.fInflictor = inflictor;
    }
}
