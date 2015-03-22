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

import java.util.Random;

/**
 * 
 * @author Serg V. Zhdanovskih
 */
public class NPCStats
{
    private static final float LB_FACTOR = 0.45f;
    
    private int fCurXP; // Experience Points

    public int Level;
    
    public int Str; // Strength
    public int Per; // Perception
    public int End; // Endurance
    public int Chr; // Charisma
    public int Int; // Intelligence
    public int Agi; // Agility
    public int Luk; // Luck

    public NPCStats()
    {
        Random rand = new Random();

        this.Level = 1;
        this.fCurXP = 0;
        
        this.Str = rand.nextInt(7) + 3;
        this.Per = rand.nextInt(7) + 3;
        this.End = rand.nextInt(7) + 3;
        this.Chr = rand.nextInt(7) + 3;
        this.Int = rand.nextInt(7) + 3;
        this.Agi = rand.nextInt(7) + 3;
        this.Luk = rand.nextInt(7) + 3;
    }
    
    public final void initDefaults()
    {
        this.Level = 1;
        this.fCurXP = 0;
        
        // average human, (c) Fallout
        this.Str = 5;
        this.Per = 5;
        this.End = 5;
        this.Chr = 5;
        this.Int = 5;
        this.Agi = 5;
        this.Luk = 5;
    }

    public final int getNextLevelXP()
    {
        return getLevelXP(this.Level + 1);
    }

    public final void addXP(int value)
    {
        this.fCurXP += value;
        
        int curLimit = this.getNextLevelXP();
        if (this.fCurXP >= curLimit) {
            this.Level += 1;
            this.fCurXP -= curLimit;
        }
    }

    public final int getCurXP()
    {
        return this.fCurXP;
    }

    public final float getCarryWeight()
    {
        // (c) Fallout 1, 2
        return (25 + (this.Str * 25)) * LB_FACTOR;
    }

    public final int getHPMax()
    {
        //return 20 + 4 * this.End;
        
        // (c) Fallout 3
        return (90 + this.End * 20 + this.Level * 10);
    }
    
    public final int getArmorClass()
    {
        // (c) Fallout 1,2,3
        return this.Agi;
    }
    
    public final int getActionPoints()
    {
        // (c) Fallout 3
        return (65 + 2 * this.Agi);
    }
    
    public final int getSequence()
    {
        // (c) Fallout 1,2
        return (2 * this.Per);
    }
    
    public final int getCriticalChance()
    {
        // (c) Fallout 1,2,3
        return (this.Luk);
    }
    
    public final int getHealingRate()
    {
        return (int) ((1.0f / 3.0f) * this.End);
    }
    
    // <editor-fold defaultstate="collapsed" desc="SPECIAL functions">

    public static final int getLevelXP(int level)
    {
        // (c) Fallout 3
        return ((level - 1) * ((level - 2) * 75 + 200));
    }
    
    // </editor-fold>
}
