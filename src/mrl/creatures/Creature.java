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

import jzrlib.core.CreatureEntity;
import jzrlib.core.GameSpace;
import jzrlib.map.IMap;
import mrl.core.ITerminalEntity;
import mrl.game.Game;
import mrl.maps.Layer;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class Creature extends CreatureEntity implements ITerminalEntity
{
    protected int fHPCur;

    private boolean fIsPlayer;

    public Creature(GameSpace space, Object owner)
    {
        super(space, owner);
    }

    @Override
    public final Game getSpace()
    {
        return (Game) this.fSpace;
    }
    
    public final IMap getMap()
    {
        return (IMap) this.Owner;
    }

    public final void setMap(IMap value)
    {
        IMap prevMap = this.getMap();
        if (prevMap != null) { 
            ((Layer) prevMap).getCreatures().remove(this);
        }

        this.Owner = value;

        if (value != null) {
            ((Layer) value).getCreatures().add(this);
        }
    }
    
    public void ascend()
    {
        IMap map = this.getMap();
        if (map == this.getSpace().getPlainMap()) {
            //
        }
        if (map == this.getSpace().getCellarsMap()) {
            this.setMap(this.getSpace().getPlainMap());
        }
        if (map == this.getSpace().getDungeonsMap()) {
            this.setMap(this.getSpace().getCellarsMap());
        }
    }
    
    public void descend()
    {
        IMap map = this.getMap();
        if (map == this.getSpace().getPlainMap()) {
            this.setMap(this.getSpace().getCellarsMap());
        }
        if (map == this.getSpace().getCellarsMap()) {
            this.setMap(this.getSpace().getDungeonsMap());
        }
        if (map == this.getSpace().getDungeonsMap()) {
            //
        }
    }

    public final boolean isPlayer()
    {
        return this.fIsPlayer;
    }

    public final void setIsPlayer(boolean value)
    {
        this.fIsPlayer = value;
    }
    
    public void doTurn()
    {
        // dummy
    }

    @Override
    public void moveTo(int newX, int newY)
    {
        super.moveTo(newX, newY);
    }
    
    // <editor-fold defaultstate="collapsed" desc="Combat implementation">

    public final int getHP()
    {
        return this.fHPCur;
    }

    public final void setHP(int hp)
    {
        this.fHPCur = hp;

        if (this.fHPCur > this.getHPMax()) {
            this.fHPCur = this.getHPMax();
        }
    }

    public final boolean isAlive()
    {
        return this.fHPCur > 0;
    }

    public int getHPMax()
    {
        return 0;
    }

    public void takeDamage(Damage damage)
    {
        this.fHPCur = this.fHPCur - damage.Value;

        if (!this.isAlive()) {
            this.die(damage.getInflictor());
        }
    }

    public void die(Creature killer)
    {
    }

    public void kill(Creature victim)
    {
    }
    
    public int getDamageValue()
    {
        return 0;
    }

    public int getDamageType()
    {
        return Damage.DMG_GENERIC;
    }

    protected void attack(Creature enemy)
    {
    }

    public int getEquipBonus(String bonusId)
    {
        return 0;
    }
    
    /*
     * This method inflicts damage on given entity,
     * based on current combat mechanics of this entity
     */
    public void inflictDamage(Creature enemy)
    {
        if (enemy == null || enemy == this) {
            return;
        }

        this.attack(enemy);

        int type = this.getDamageType();
        int value = this.getDamageValue();

        // non-lethal weapons ignore all damage including stat bonuses
        if (type == Damage.DMG_NONLETHAL) {
            value = 0;
        }

        Damage dmg = new Damage(value, type, this);
        enemy.takeDamage(dmg);
    }

    public abstract int getFovRadius();
    
    public abstract int getHearRadius();
    
    // </editor-fold>
}
