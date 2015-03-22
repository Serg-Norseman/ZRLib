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

import java.awt.Color;
import java.util.HashMap;
import java.util.Map;
import jzrlib.utils.AuxUtils;
import mrl.creatures.Creature;
import mrl.creatures.Damage;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class HumanBody extends CustomBody
{
    private final Map<String, Float> fAttributes;
    private int fBloodLevel = 100;
    private Creature fBleedInflictor = null;
    private boolean fIsBleeding = false; // кровотечение
    private boolean fIsStunned = false; // оглушенный
    private boolean fIsFainted = false; // ослабевший
    private boolean fIsInfected = false; // зараженный
    private int fStunDuration = 0;

    public HumanBody(Object owner)
    {
        super(owner);
        
        this.fAttributes = new HashMap<>(4);
        fAttributes.put("stamina", 100f); // запас жизненных сил
        fAttributes.put("hunger", 100f); // испытывать голод
        fAttributes.put("bloodlust", 90.0f); // 
        fAttributes.put("libido", 0f); // 

        /*for (BodypartType part : BodypartType.values()) {
            this.addPart(part.ordinal());
        }*/
        
        this.addPart(BodypartType.bpt_Head.ordinal());
        this.addPart(BodypartType.bpt_Torso.ordinal());
        this.addPart(BodypartType.bpt_LeftArm.ordinal());
        this.addPart(BodypartType.bpt_RightArm.ordinal());
        this.addPart(BodypartType.bpt_LeftLeg.ordinal());
        this.addPart(BodypartType.bpt_RightLeg.ordinal());
        this.addPart(BodypartType.bpt_LeftEye.ordinal());
        this.addPart(BodypartType.bpt_RightEye.ordinal());
    }

    public final boolean isBleeding()
    {
        return this.fIsBleeding;
    }

    public final boolean isStunned()
    {
        return this.fIsStunned;
    }

    public final boolean isFainted()
    {
        return this.fIsFainted;
    }

    public final void setFainted(boolean fainted)
    {
        this.fIsFainted = fainted;
    }

    public final boolean isInfected()
    {
        return this.fIsInfected;
    }

    public final void setInfected(boolean infected)
    {
        this.fIsInfected = infected;
    }

    public final float getAttribute(String attr)
    {
        return this.fAttributes.get(attr);
    }

    public final void setAttribute(String attr, float value)
    {
        if (value < 0) {
            value = 0;
        }

        if (value > 100) {
            value = 100;
        }

        this.fAttributes.put(attr, value);
    }

    public final void adjustAttribute(String key, float value)
    {
        Float attrVal = this.fAttributes.get(key);
        this.setAttribute(key, attrVal + value);
    }

    public final float getHunger()
    {
        return this.getAttribute("hunger");
    }

    public final void restoreHunger(float value)
    {
        this.adjustAttribute("hunger", value);
    }

    public final float getStamina()
    {
        return this.getAttribute("stamina");
    }

    public final float getBloodlust()
    {
        return this.getAttribute("bloodlust");
    }

    public void takeDamage(Damage damage)
    {
        Creature inflictor = damage.getInflictor();
        
        switch (damage.Type) {
            case Damage.DMG_CUT:
                if (!fIsBleeding) {
                    this.getSpace().addMessage(this.getOwner().getName() + " is bleeding", Color.red);
                }
                fIsBleeding = true;
                fBleedInflictor = inflictor;
                break;

            case Damage.DMG_GENERIC:
                break;

            case Damage.DMG_BLUNT:
            case Damage.DMG_NONLETHAL:
                if (inflictor != null) {
                    int stunChance = inflictor.getEquipBonus("stun_chance");

                    if (AuxUtils.getRandom(100) < stunChance) {
                        this.fIsStunned = true;
                        this.fStunDuration = inflictor.getEquipBonus("stun_duration");

                        this.getSpace().addMessage(this.getOwner().getName() + " is stunned", Color.orange);
                    }
                }
                break;
        }
    }

    @Override
    public void update()
    {
        if (this.fIsBleeding) {
            this.fBloodLevel -= 5;

            if (this.fBloodLevel <= 0 && this.getOwner().isAlive()) {
                Damage damage = new Damage(10, Damage.DMG_BLOODLOSS);
                damage.setInflictor(this.fBleedInflictor);

                this.getOwner().takeDamage(damage);
            }

            if (this.getOwner().isAlive()) {
                // make blood more like drops rather than trail
                /*BaseTile tile = ...;
                 tile.setBlood(...);*/
            } else {
                /*a pool of blood*/
            }

            if (this.fBloodLevel <= -50) {
                this.fIsBleeding = false;
            }
        }

        if (this.fIsStunned) {
            // lying unconcious will restore a bit of stamina
            this.adjustAttribute("stamina", 2.5f);

            this.fStunDuration -= 1;

            if (this.fStunDuration <= 0) {
                this.fIsStunned = false;
            }
        }

        if (this.fIsFainted) {
            this.setFainted(false);
        }

        this.adjustAttribute("hunger", -0.05f);
        this.adjustAttribute("stamina", +0.1f);
        this.adjustAttribute("bloodlust", 0.05f);
        this.adjustAttribute("libido", 0.5f);

        if (this.getStamina() <= 20) {         
            // stamina < 20% - you start skipping turns
            if (AuxUtils.getRandom(100) <= 20) {
                // 20% chance to skip turn
                this.setFainted(true);
                if (this.getOwner().isPlayer()) {
                    this.getSpace().addMessage("You feel tired", Color.orange);
                }
            }
        }

        if (this.getStamina() == 0) {
            this.fIsStunned = true;
            this.fStunDuration = 10;

            if (this.getOwner().isPlayer()) {
                this.getSpace().addMessage("You are unconscious", Color.orange);
            }
        }
    }

    public void takeBodypartDamage(Damage damage)
    {
        /*if (!damaged) {
            damaged = true;
            //addMessage(part.getName() + "'s" + name + " is damaged", Color.magenta);
        }*/
    }
}
