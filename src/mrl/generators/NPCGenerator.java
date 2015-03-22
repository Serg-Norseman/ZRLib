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
package mrl.generators;

import jzrlib.common.CreatureSex;
import jzrlib.core.GameSpace;
import jzrlib.utils.AuxUtils;
import mrl.creatures.Human;
import mrl.creatures.body.HumanBody;
import mrl.game.social.CrimeRecord;
import mrl.game.social.CrimeType;
import mrl.items.Item;
import mrl.items.ItemType;
import mrl.items.ItemsList;
import mrl.maps.Layer;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class NPCGenerator
{
    public static int generateAge(boolean isAdult)
    {
        if (isAdult) {
            return 24 + AuxUtils.getRandom(40);
        } else {
            return 8 + AuxUtils.getRandom(16);
        }
    }

    public String generateFullName(boolean isMale)
    {
        NameGenerator namegen = new NameGenerator();
        return namegen.generateFullName(isMale);
    }

    public final Human generateHuman(Layer layer, int x, int y)
    {
        Human human = new Human(GameSpace.getInstance(), layer);
        human.setPos(x, y);
        layer.addCreature(human);
        return human;
    }

    public Human generateNPC(Layer layer, int x, int y)
    {
        Human npc = this.generateHuman(layer, x, y);

        CreatureSex sex;
        if (AuxUtils.getRandom(100) > 50) {
            sex = CreatureSex.csMale;
        } else {
            sex = CreatureSex.csFemale;
        }

        npc.setSex(sex);
        npc.setName(this.generateFullName(sex == CreatureSex.csMale));
        npc.setAge(8 + AuxUtils.getRandom(87));

        this.generateNPCStats(npc);
        
        this.generateItems(npc);

        return npc;
    }

    public void generateNPCStats(Human entity)
    {
        // generate criminal background
        int criminalRate = 30; //%
        if (AuxUtils.getRandom(100) < criminalRate) {
            int crimeTypes = AuxUtils.getRandom(5);
            for (int i = 0; i < crimeTypes; i++) {
                entity.addCrimeRecord(new CrimeRecord(AuxUtils.getRandomEnum(CrimeType.class)));
            }
        }

        HumanBody body = (HumanBody) entity.getBody();
        if (body != null && AuxUtils.getRandom(100) <= 25) {
            body.setInfected(true);
        }
    }
    
    private void generateItems(Human entity)
    {
        GameSpace space = entity.getSpace();
        ItemsList inventory = entity.getInventory();
        
        inventory.add(new Item(space, entity, ItemType.IT_SHIRT));
        inventory.add(new Item(space, entity, ItemType.IT_BOOTS));
        
        if (entity.getSex() == CreatureSex.csFemale) {
            inventory.add(new Item(space, entity, ItemType.IT_SKIRT));
        } else {
            inventory.add(new Item(space, entity, ItemType.IT_PANTS));
        }
    }
}
