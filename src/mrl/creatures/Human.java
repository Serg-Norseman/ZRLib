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

import java.awt.Color;
import java.util.ArrayList;
import java.util.List;
import jzrlib.common.CreatureSex;
import jzrlib.core.GameSpace;
import jzrlib.core.IEquippedCreature;
import jzrlib.core.body.AbstractBody;
import mrl.core.Locale;
import mrl.core.RS;
import mrl.creatures.body.HumanBody;
import mrl.game.events.CriminalEvent;
import mrl.game.events.SuspiciousSoundEvent;
import mrl.game.social.CrimeRecord;
import mrl.items.Item;
import mrl.items.ItemsList;
import mrl.maps.buildings.Building;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class Human extends Creature implements IEquippedCreature
{
    private int fAge = 30;
    private Building fApartment;
    private String fName;
    private CreatureSex fSex = CreatureSex.csMale;

    private AbstractBody fBody;
    private final NPCStats fStats;
    private final ItemsList fInventory;
    private final Personality fPersonality;

    // <editor-fold defaultstate="collapsed" desc="sample">
    
    // </editor-fold>
    
    // <editor-fold defaultstate="collapsed" desc="Social stuff">
    
    private Human fParent;
    private Human fSpouse;
    private final List<Human> fChildrens;
    private final List<Human> fSiblings;
    public List<CrimeRecord> fCrimeRecords;
    
    // </editor-fold>

    public Human(GameSpace space, Object owner)
    {
        super(space, owner);

        this.fBody = new HumanBody(this);
        this.fStats = new NPCStats();
        this.fInventory = new ItemsList(this, true);
        this.fPersonality = new Personality();

        this.fParent = null;
        this.fSpouse = null;
        this.fChildrens = new ArrayList<>();
        this.fSiblings = new ArrayList<>();
        this.fCrimeRecords = new ArrayList<>();
        
        this.setHP(this.getHPMax());
    }

    @Override
    public char getAppearance()
    {
        if (this.isPlayer()) {
            return '@';
        }
        
        if (this.isAlive()) {
            switch (this.fSex) {
                case csFemale:
                    if (this.isAdult()) {
                        return 'W';
                    } else {
                        return 'w';
                    }

                case csMale:
                    if (this.isAdult()) {
                        return 'M';
                    } else {
                        return 'm';
                    }

                default:
                    return 'H';
            }
        } else {
            return '%';
        }
    }

    @Override
    public Color getAppearanceColor()
    {
        if (this.isPlayer()) {
            return Color.yellow;
        }
        
        return Color.white;
    }

    @Override
    public String getName()
    {
        return this.fName;
    }

    @Override
    public void setName(String value)
    {
        this.fName = value;
    }

    public final Building getApartment()
    {
        return this.fApartment;
    }

    public final void setApartment(Building apt)
    {
        this.fApartment = apt;
    }

    public final NPCStats getStats()
    {
        return fStats;
    }
    
    public final Personality getPersonality()
    {
        return this.fPersonality;
    }

    public final AbstractBody getBody()
    {
        return this.fBody;
    }

    public final void setBody(AbstractBody value)
    {
        if (this.fBody != null) {
            this.fBody.dispose();
        }
        this.fBody = value;
    }

    @Override
    public String getDesc()
    {
        StringBuilder result = new StringBuilder();
        result.append(this.getName() + ". ");
        
        boolean isMale = (this.getSex() == CreatureSex.csMale);
        
        String age;
        if (isMale) {
            age = Locale.format(RS.rs_HeIsNYearsOld, this.getAge());
        } else {
            age = Locale.format(RS.rs_SheIsNYearsOld, this.getAge());
        }
        result.append(age);
        
        /*if (this.getSpouse() != null) {
            result.append("His/her spouse " + this.getSpouse().getName() + ". ");
        }

        if (this.getParent() != null) {
            result.append("His/her parent " + this.getParent().getName() + ". ");
        }*/

        int rsid;
        if (isMale) {
            rsid = RS.rs_HeHasXHairAndYEyes;
        } else {
            rsid = RS.rs_SheHasXHairAndYEyes;
        }
        String pers = Locale.format(rsid, this.fPersonality.hairColor.name().toLowerCase(), this.fPersonality.eyeColor.name().toLowerCase());
        result.append(pers);
        
        return result.toString();
    }
    
    public final ItemsList getInventory()
    {
        return this.fInventory;
    }
    
    @Override
    public void doTurn()
    {
        HumanBody body = (HumanBody) this.fBody;
        body.update();

        if (!body.isStunned()){
            super.doTurn();
        }
    }

    @Override
    public void moveTo(int newX, int newY)
    {
        HumanBody body = (HumanBody) this.fBody;
        if (body.isFainted()) {
            return;
        }

        super.moveTo(newX, newY);

        body.adjustAttribute("stamina", -0.5f);
    }
    
    // <editor-fold defaultstate="collapsed" desc="Social stuff">
    
    public final int getAge()
    {
        return this.fAge;
    }

    public final void setAge(int age)
    {
        this.fAge = age;
    }

    public final CreatureSex getSex()
    {
        return this.fSex;
    }

    public final void setSex(CreatureSex sex)
    {
        this.fSex = sex;
    }

    public final boolean isAdult()
    {
        return this.fAge >= 18;
    }

    public final Human getParent()
    {
        return this.fParent;
    }

    public final void setParent(Human parent)
    {
        this.fParent = parent;
    }

    public final Human getSpouse()
    {
        return this.fSpouse;
    }

    public final void setSpouse(Human spouse)
    {
        if (this.fSpouse == null) {
            this.fSpouse = spouse;
            spouse.setSpouse(this);
        }
    }

    public final void addChild(Human child)
    {
        for (Human otherChild : this.fChildrens) {
            otherChild.addSibling(child);
        }

        this.fChildrens.add(child);
        child.setParent(this);
    }

    public final void addSibling(Human child)
    {
        if (!this.fSiblings.contains(child)) {
            this.fSiblings.add(child);
            child.addSibling(this);
        }
    }

    public final void addCrimeRecord(CrimeRecord crimeRecord)
    {
        for (CrimeRecord crime : fCrimeRecords) {
            if (crime.Type == crimeRecord.Type) {
                crime.incCount();
                return;
            }
        }
        fCrimeRecords.add(crimeRecord);
    }
    
    // </editor-fold>
    
    // <editor-fold defaultstate="collapsed" desc="Combat implementation">

    @Override
    public int getHPMax()
    {
        return this.fStats.getHPMax();
    }

    @Override
    public int getFovRadius()
    {
        int maxFov = (int) (7 + 1.2 * fStats.Per);
        int minFov = (int) (maxFov * 0.7);

        int fov = (int) (minFov + (maxFov - minFov) * this.getSpace().getLightFactor());

        return fov;
    }

    @Override
    public int getHearRadius()
    {
        // slightly better than LOS, and date of time does not affect our senses
        return (int) (7 + 1.5 * fStats.Per);
    }

    @Override
    public int getDamageType()
    {
        int dmgTypeId = Damage.DMG_GENERIC;

        for (int i = 0; i < this.fInventory.getCount(); i++) {
            Item item = this.fInventory.getItem(i);

            if (item.isUsed()) {
                int dmgType = item.getDamageType();

                if (dmgType != Damage.DMG_GENERIC) {
                    dmgTypeId = dmgType;
                }
            }
        }

        return dmgTypeId;
    }

    @Override
    public int getEquipBonus(String bonusId)
    {
        int bonus = 0;

        for (int i = 0; i < this.fInventory.getCount(); i++) {
            Item item = this.fInventory.getItem(i);

            if (item.isUsed()) {
                
            }
        }

        return bonus;
    }

    @Override
    public int getDamageValue()
    {
        return this.fStats.Str + this.getEquipBonus("damage");
    }

    public int getDefenceValue()
    {
        return this.getEquipBonus("defence");
    }

    @Override
    public void takeDamage(Damage damage)
    {
        super.takeDamage(damage);
        this.getSpace().addMessage(this.getName() + " took " + damage.Value + " damage", new Color(255, 0, 255));

        HumanBody body = (HumanBody) this.getBody();
        if (body != null) {
            body.takeDamage(damage);
        }

        if (!this.isAlive()) {
            Creature inflictor = damage.getInflictor();
            inflictor.kill(this);
        }
    }

    @Override
    public void attack(Creature enemy)
    {
        this.getSpace().addMessage(this.getName() + " is attacking " + enemy.getName(), new Color(255, 128, 128));

        CriminalEvent event = new CriminalEvent(this.getLocation(), this);
        event.post();

        if (this.isPlayer()) {
            SuspiciousSoundEvent soundEvent = new SuspiciousSoundEvent(this.getLocation(), 15);
            soundEvent.post();
        }
    }

    @Override
    public void die(Creature killer)
    {
        super.die(killer);

        if (this.isPlayer()) {
            this.getSpace().addMessage("You were killed by a " + killer.getName() + ".");
        } else {
            this.getSpace().addMessage(this.getName() + " has died.", Color.red);
        }
    }

    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="IEquippedCreature implementation">

    @Override
    public final int getMoney()
    {
        int result = 0;

        /*int num = this.fItems.getCount();
        for (int i = 0; i < num; i++) {
            Item item = this.fItems.getItem(i);
            if (item.CLSID == GlobalVars.iid_Coin) {
                result += (int) item.Count;
            }
        }*/

        return result;
    }

    // </editor-fold>
}
