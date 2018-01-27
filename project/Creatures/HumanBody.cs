/*
 *  "PrimevalRL", roguelike game.
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
using BSLib;
using ZRLib.Core;
using ZRLib.Engine;

namespace PrimevalRL.Creatures
{
    public enum BodypartType
    {
        bpt_Head,
        bpt_Torso,
        bpt_LeftArm,
        bpt_RightArm,
        bpt_LeftLeg,
        bpt_RightLeg,
        bpt_LeftEye,
        bpt_RightEye
    }

    public class HumanBody : CustomBody
    {
        private readonly IDictionary<string, float> fAttributes;
        private int fBloodLevel = 100;
        private Creature fBleedInflictor = null;
        private bool fIsBleeding = false;
        // кровотечение
        private bool fIsStunned = false;
        // оглушенный
        private bool fIsFainted = false;
        // ослабевший
        private bool fIsInfected = false;
        // зараженный
        private int fStunDuration = 0;

        public HumanBody(object owner)
            : base(owner)
        {
            fAttributes = new Dictionary<string, float>(4);
            fAttributes["stamina"] = 100f; // запас жизненных сил
            fAttributes["hunger"] = 100f; // испытывать голод
            fAttributes["bloodlust"] = 90.0f;
            fAttributes["libido"] = 0f;

            /*for (BodypartType part : BodypartType.values()) {
                addPart(part);
            }*/

            AddPart((int)BodypartType.bpt_Head);
            AddPart((int)BodypartType.bpt_Torso);
            AddPart((int)BodypartType.bpt_LeftArm);
            AddPart((int)BodypartType.bpt_RightArm);
            AddPart((int)BodypartType.bpt_LeftLeg);
            AddPart((int)BodypartType.bpt_RightLeg);
            AddPart((int)BodypartType.bpt_LeftEye);
            AddPart((int)BodypartType.bpt_RightEye);
        }

        public bool Bleeding
        {
            get { return fIsBleeding; }
        }

        public bool Stunned
        {
            get { return fIsStunned; }
        }

        public bool Fainted
        {
            get {
                return fIsFainted;
            }
            set {
                fIsFainted = value;
            }
        }


        public bool Infected
        {
            get {
                return fIsInfected;
            }
            set {
                fIsInfected = value;
            }
        }


        public float GetAttribute(string attr)
        {
            return fAttributes.GetValueOrNull(attr);
        }

        public void SetAttribute(string attr, float value)
        {
            if (value < 0) {
                value = 0;
            }

            if (value > 100) {
                value = 100;
            }

            fAttributes[attr] = value;
        }

        public void AdjustAttribute(string key, float value)
        {
            float? attrVal = fAttributes.GetValueOrNull(key);
            SetAttribute(key, attrVal.Value + value);
        }

        public float Hunger
        {
            get {
                return GetAttribute("hunger");
            }
        }

        public void RestoreHunger(float value)
        {
            AdjustAttribute("hunger", value);
        }

        public float Stamina
        {
            get { return GetAttribute("stamina"); }
        }

        public float Bloodlust
        {
            get { return GetAttribute("bloodlust"); }
        }

        public virtual void TakeDamage(Damage damage)
        {
            Creature inflictor = damage.Inflictor;

            switch (damage.Type) {
                case Damage.DMG_CUT:
                    if (!fIsBleeding) {
                        Space.AddMessage(Owner.Name + " is bleeding", Colors.Red);
                    }
                    fIsBleeding = true;
                    fBleedInflictor = inflictor;
                    break;

                case Damage.DMG_GENERIC:
                    break;

                case Damage.DMG_BLUNT:
                case Damage.DMG_NONLETHAL:
                    if (inflictor != null) {
                        int stunChance = inflictor.GetEquipBonus("stun_chance");

                        if (RandomHelper.GetRandom(100) < stunChance) {
                            fIsStunned = true;
                            fStunDuration = inflictor.GetEquipBonus("stun_duration");

                            Space.AddMessage(Owner.Name + " is stunned", Colors.Orange);
                        }
                    }
                    break;
            }
        }

        public override void Update()
        {
            if (fIsBleeding) {
                fBloodLevel -= 5;

                if (fBloodLevel <= 0 && Owner.Alive) {
                    Damage damage = new Damage(10, Damage.DMG_BLOODLOSS);
                    damage.Inflictor = fBleedInflictor;

                    Owner.TakeDamage(damage);
                }

                if (Owner.Alive) {
                    // make blood more like drops rather than trail
                    /*BaseTile tile = ...;
                     tile.setBlood(...);*/
                } else {
                    /*a pool of blood*/
                }

                if (fBloodLevel <= -50) {
                    fIsBleeding = false;
                }
            }

            if (fIsStunned) {
                // lying unconcious will restore a bit of stamina
                AdjustAttribute("stamina", 2.5f);

                fStunDuration -= 1;

                if (fStunDuration <= 0) {
                    fIsStunned = false;
                }
            }

            if (fIsFainted) {
                Fainted = false;
            }

            AdjustAttribute("hunger", -0.05f);
            AdjustAttribute("stamina", +0.1f);
            AdjustAttribute("bloodlust", 0.05f);
            AdjustAttribute("libido", 0.5f);

            if (Stamina <= 20) {
                // stamina < 20% - you start skipping turns
                if (RandomHelper.GetRandom(100) <= 20) {
                    // 20% chance to skip turn
                    Fainted = true;
                    if (Owner.Player) {
                        Space.AddMessage("You feel tired", Colors.Orange);
                    }
                }
            }

            if (Stamina == 0) {
                fIsStunned = true;
                fStunDuration = 10;

                if (Owner.Player) {
                    Space.AddMessage("You are unconscious", Colors.Orange);
                }
            }
        }

        public virtual void TakeBodypartDamage(Damage damage)
        {
            /*if (!damaged) {
                damaged = true;
                //addMessage(part.getName() + "'s" + name + " is damaged", Colors.magenta);
            }*/
        }
    }
}
