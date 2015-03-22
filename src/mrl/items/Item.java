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
package mrl.items;

import java.util.List;
import jzrlib.core.GameSpace;
import jzrlib.core.LocatedEntity;
import jzrlib.core.action.ActionList;
import jzrlib.core.action.IAction;
import jzrlib.core.action.IActor;
import mrl.core.BaseEntityAction;
import mrl.core.EquipmentType;
import mrl.creatures.Damage;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class Item extends LocatedEntity implements IActor
{
    private final ItemType fType;

    private int fCount;
    private boolean fIsUsed;

    public Item(GameSpace space, Object owner, ItemType type)
    {
        super(space, owner);
        this.fType = type;
    }
    
    // <editor-fold defaultstate="collapsed" desc="Template support">
    
    public final ItemType getType()
    {
        return this.fType;
    }
    
    public final boolean isStacked()
    {
        return this.fType.IsStacked;
    }
    
    public final EquipmentType getEquipmentType()
    {
        return this.fType.Equipment;
    }
    
    // </editor-fold>
    
    public int getCount()
    {
        return this.fCount;
    }
    
    public void setCount(int value)
    {
        this.fCount = value;
    }
    
    public final boolean isUsed()
    {
        return this.fIsUsed;
    }
    
    public final void setIsUsed(boolean value)
    {
        this.fIsUsed = value;
    }
    
    public float getWeight()
    {
        return (this.fCount * this.fType.Weight);
    }

    public int getDamageType()
    {
        int dmgTypeId = Damage.DMG_GENERIC;
        return dmgTypeId;
    }
    
    @Override
    public List<IAction> getActionsList()
    {
        class ActionPickUp extends BaseEntityAction
        {
            @Override
            public void execute(Object invoker)
            {
            }
        }

        ActionList<Item> list = new ActionList();
        list.setOwner(this);
        list.addAction(new ActionPickUp(), "Pick up");

        return list.getList();
    }
}
