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
import jzrlib.core.Point;
import jzrlib.utils.AuxUtils;
import mrl.creatures.Human;
import mrl.creatures.Personality;
import mrl.maps.Layer;
import mrl.maps.buildings.Building;

/**
 * 
 * @author Serg V. Zhdanovskih
 */
public final class FamilyGenerator extends NPCGenerator
{
    private final NameGenerator fNameGen;
    private final String fSurname;
    private final Personality.Religion fReligion;

    public FamilyGenerator()
    {
        this.fNameGen = new NameGenerator();
        this.fSurname = fNameGen.generateSurname();
        this.fReligion = AuxUtils.getRandomEnum(Personality.Religion.class);
    }

    @Override
    public String generateFullName(boolean isMale)
    {
        String name = fNameGen.generateName(isMale);
        return name + " " + fSurname;
    }
    
    public Human generateNPC(Layer layer, Building apartment, CreatureSex sex, boolean isAdult)
    {
        Point pt = apartment.getRandomPos();
        Human npc = this.generateHuman(layer, pt.X, pt.Y);
        npc.getPersonality().religion = this.fReligion;

        if (sex == CreatureSex.csNone) {
            if (AuxUtils.getRandom(100) > 50) {
                sex = CreatureSex.csMale;
            } else {
                sex = CreatureSex.csFemale;
            }
        }

        npc.setSex(sex);
        npc.setName(this.generateFullName(sex == CreatureSex.csMale));
        npc.setAge(NPCGenerator.generateAge(isAdult));

        this.generateNPCStats(npc);

        return npc;
    }

    public Human generateChild(Layer layer, Building apartment, Human parent)
    {
        Human child = this.generateNPC(layer, apartment, CreatureSex.csNone, false);
        parent.addChild(child);
        return child;
    }
}
