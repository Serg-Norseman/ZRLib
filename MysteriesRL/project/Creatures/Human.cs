/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih.
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

using System.Collections.Generic;
using System.Text;
using BSLib;
using MysteriesRL.Game;
using MysteriesRL.Game.Events;
using MysteriesRL.Items;
using MysteriesRL.Maps.Buildings;
using ZRLib.Core;
using ZRLib.Core.Body;
using ZRLib.Engine;
using ZRLib.Map;

namespace MysteriesRL.Creatures
{
    public class Human : Creature
    {
        private int fAge = 30;
        private Building fApartment;
        private AbstractBody fBody;
        private readonly IList<Human> fChildrens;
        private readonly IList<CrimeRecord> fCrimeRecords;
        private string fName;
        private readonly NPCStats fStats;
        private readonly ItemsList fInventory;
        private Human fParent;
        private readonly Personality fPersonality;
        private CreatureSex fSex = CreatureSex.csMale;
        private readonly IList<Human> fSiblings;
        private Human fSpouse;


        public bool Adult
        {
            get {
                return fAge >= 18;
            }
        }

        public int Age
        {
            get {
                return fAge;
            }
            set {
                fAge = value;
            }
        }

        public IList<CrimeRecord> CrimeRecords
        {
            get {
                return fCrimeRecords;
            }
        }

        public Human Parent
        {
            get {
                return fParent;
            }
            set {
                fParent = value;
            }
        }

        public CreatureSex Sex
        {
            get {
                return fSex;
            }
            set {
                fSex = value;
            }
        }

        public Human Spouse
        {
            get {
                return fSpouse;
            }
            set {
                if (fSpouse == null) {
                    fSpouse = value;
                    value.Spouse = this;
                }
            }
        }

        public override int DamageValue
        {
            get {
                return fStats.Str + GetEquipBonus("damage");
            }
        }

        public virtual int DefenceValue
        {
            get {
                return GetEquipBonus("defence");
            }
        }

        public override int HPMax
        {
            get {
                return fStats.HPMax;
            }
        }

        public override int FovRadius
        {
            get {
                int maxFov = (int)(7 + 1.2 * fStats.Per);
                int minFov = (int)(maxFov * 0.7);
    
                int fov = (int)(minFov + (maxFov - minFov) * Space.LightFactor);
                return fov;
            }
        }

        public override int HearRadius
        {
            get {
                // slightly better than LOS, and date of time does not affect our senses
                return (int)(7 + 1.5 * fStats.Per);
            }
        }

        public override int DamageType
        {
            get {
                int dmgTypeId = Damage.DMG_GENERIC;
    
                for (int i = 0; i < fInventory.Count; i++) {
                    Item item = fInventory[i];
    
                    if (item.Used) {
                        int dmgType = item.DamageType;
    
                        if (dmgType != Damage.DMG_GENERIC) {
                            dmgTypeId = dmgType;
                        }
                    }
                }
    
                return dmgTypeId;
            }
        }

        public int Money
        {
            get {
                int result = 0;
    
                /*int num = fItems.getCount();
                for (int i = 0; i < num; i++) {
                    Item item = fItems.getItem(i);
                    if (item.CLSID == GlobalVars.iid_Coin) {
                        result += (int) item.Count;
                    }
                }*/
    
                return result;
            }
        }

        public override char Appearance
        {
            get {
                if (Player) {
                    return '@';
                }
    
                if (Alive) {
                    switch (fSex) {
                        case CreatureSex.csFemale:
                            if (Adult) {
                                return 'W';
                            } else {
                                return 'w';
                            }
    
                        case CreatureSex.csMale:
                            if (Adult) {
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
        }

        public override int AppearanceColor
        {
            get {
                if (Player) {
                    return Colors.Yellow;
                }
    
                return Colors.White;
            }
        }

        public override string Name
        {
            get {
                return fName;
            }
            set {
                fName = value;
            }
        }

        public Building Apartment
        {
            get {
                return fApartment;
            }
            set {
                fApartment = value;
            }
        }

        public NPCStats Stats
        {
            get {
                return fStats;
            }
        }

        public Personality Personality
        {
            get {
                return fPersonality;
            }
        }

        public AbstractBody Body
        {
            get {
                return fBody;
            }
            set {
                if (fBody != null) {
                    fBody.Dispose();
                }
                fBody = value;
            }
        }

        public override string Desc
        {
            get {
                StringBuilder result = new StringBuilder();
                result.Append(Name + ". ");
    
                bool isMale = (Sex == CreatureSex.csMale);
    
                string age;
                if (isMale) {
                    age = Locale.Format(RS.Rs_HeIsNYearsOld, Age);
                } else {
                    age = Locale.Format(RS.Rs_SheIsNYearsOld, Age);
                }
                result.Append(age);
    
                /*if (getSpouse() != null) {
                    result.append("His/her spouse " + getSpouse().getName() + ". ");
                }
        
                if (getParent() != null) {
                    result.append("His/her parent " + getParent().getName() + ". ");
                }*/
    
                int rsid;
                if (isMale) {
                    rsid = RS.Rs_HeHasXHairAndYEyes;
                } else {
                    rsid = RS.Rs_SheHasXHairAndYEyes;
                }
                string pers = Locale.Format(rsid, fPersonality.HairColor.ToString().ToLower(), fPersonality.EyeColor.ToString().ToLower());
                result.Append(pers);
    
                return result.ToString();
            }
        }

        public ItemsList Inventory
        {
            get {
                return fInventory;
            }
        }


        public Human(GameSpace space, object owner)
            : base(space, owner)
        {
            fBody = new HumanBody(this);
            fStats = new NPCStats();
            fInventory = new ItemsList(this);
            fPersonality = new Personality();

            fParent = null;
            fSpouse = null;
            fChildrens = new List<Human>();
            fSiblings = new List<Human>();
            fCrimeRecords = new List<CrimeRecord>();

            HP = HPMax;
        }

        public override void DoTurn()
        {
            HumanBody body = (HumanBody)fBody;
            body.Update();

            if (!body.Stunned) {
                base.DoTurn();
            }
        }

        public override void MoveTo(int newX, int newY)
        {
            HumanBody body = (HumanBody)fBody;
            if (body.Fainted) {
                return;
            }

            base.MoveTo(newX, newY);

            body.AdjustAttribute("stamina", -0.5f);
        }

        public virtual void Ascend(ExtPoint target)
        {
            IMap map = Map;
            if (map == Space.CurrentRealm.PlainMap) {
                //
            }
            if (map == Space.CurrentRealm.UndergroundMap) {
                Map = Space.CurrentRealm.PlainMap;
                MoveTo(target.X, target.Y);
            }
        }

        public virtual void Descend(ExtPoint target)
        {
            IMap map = Map;
            if (map == Space.CurrentRealm.PlainMap) {
                Map = Space.CurrentRealm.UndergroundMap;
                MoveTo(target.X, target.Y);
            }
        }

        public void AddChild(Human child)
        {
            foreach (Human otherChild in fChildrens) {
                otherChild.AddSibling(child);
            }

            fChildrens.Add(child);
            child.Parent = this;
        }

        public void AddSibling(Human child)
        {
            if (!fSiblings.Contains(child)) {
                fSiblings.Add(child);
                child.AddSibling(this);
            }
        }

        public void AddCrimeRecord(CrimeRecord crimeRecord)
        {
            foreach (CrimeRecord crime in fCrimeRecords) {
                if (crime.Type == crimeRecord.Type) {
                    crime.IncCount();
                    return;
                }
            }
            fCrimeRecords.Add(crimeRecord);
        }

        public override int GetEquipBonus(string bonusId)
        {
            int bonus = 0;

            for (int i = 0; i < fInventory.Count; i++) {
                Item item = fInventory[i];

                if (item.Used) {

                }
            }

            return bonus;
        }

        public override void TakeDamage(Damage damage)
        {
            base.TakeDamage(damage);
            Space.AddMessage(Name + " took " + damage.Value + " damage", Colors.Fuchsia);

            HumanBody body = (HumanBody)Body;
            if (body != null) {
                body.TakeDamage(damage);
            }

            if (!Alive) {
                Creature inflictor = damage.Inflictor;
                inflictor.Kill(this);
            }
        }

        protected override void Attack(Creature enemy)
        {
            Space.AddMessage(Name + " is attacking " + enemy.Name, 0xFF8080);

            CriminalEvent @event = new CriminalEvent(Location, this);
            @event.Post();

            if (Player) {
                SuspiciousSoundEvent soundEvent = new SuspiciousSoundEvent(Location, 15);
                soundEvent.Post();
            }
        }

        public override void Die(Creature killer)
        {
            base.Die(killer);

            if (Player) {
                Space.AddMessage("You were killed by a " + killer.Name + ".");
            } else {
                Space.AddMessage(Name + " has died.", Colors.Red);
            }
        }
    }
}
