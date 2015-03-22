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
package mrl.creatures.body;

import mrl.core.EquipmentType;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public enum BodypartType
{
    bpt_Head("head", 3, EquipmentType.ET_HEAD),
    bpt_Torso("torso", 1, EquipmentType.ET_TORSO),
    bpt_LeftArm("left arm", 1, EquipmentType.ET_LHAND),
    bpt_RightArm("right arm", 1, EquipmentType.ET_RHAND),
    bpt_LeftLeg("left leg", 1, null),
    bpt_RightLeg("right leg", 1, null),
    bpt_LeftEye("left eye", 0.2f, null),
    bpt_RightEye("right eye", 0.2f, null);

    public final String Name;
    public final float DmgMultiply;
    public final EquipmentType Equipment;

    private BodypartType(String name, float dmgMultiply, EquipmentType equipment)
    {
        this.Name = name;
        this.DmgMultiply = dmgMultiply;
        this.Equipment = equipment;
    }
}
