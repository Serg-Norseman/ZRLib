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
using MysteriesRL.Creatures;
using MysteriesRL.Game;
using ZRLib.Core;
using ZRLib.Core.Action;

namespace MysteriesRL.Items
{
    public enum EquipmentType
    {
        ET_HEAD = 0,
        ET_NECK = 1,
        ET_TORSO = 2,
        ET_SHIRT = 3,
        ET_LHAND = 4,
        ET_RHAND = 5,
        ET_GLOVES = 6,
        ET_PANTS = 7,
        ET_BOOTS = 8,
        ET_CLOAK = 9,
        ET_POCKET = 10,
        ET_NONE
    }

    public enum ItemType
    {
        IT_COIN,
        IT_SHIRT,
        IT_PANTS,
        IT_SKIRT,
        IT_BOOTS,
        IT_KNIFE
    }


    public class Item : LocatedEntity, IActor
    {
        private readonly ItemType fType;

        private int fCount;
        private bool fIsUsed;


        public ItemType Type
        {
            get { return fType; }
        }

        public bool Stacked
        {
            get {
                var rec = MRLData.ItemTypes[(int)fType];
                return rec.IsStacked;
            }
        }

        public EquipmentType EquipmentType
        {
            get {
                var rec = MRLData.ItemTypes[(int)fType];
                return rec.Equipment;
            }
        }

        public virtual int Count
        {
            get { return fCount; }
            set { fCount = value; }
        }

        public bool Used
        {
            get { return fIsUsed; }
        }

        public bool IsUsed
        {
            set { fIsUsed = value; }
        }

        public virtual float Weight
        {
            get {
                var rec = MRLData.ItemTypes[(int)fType];
                return (fCount * rec.Weight);
            }
        }

        public virtual int DamageType
        {
            get {
                int dmgTypeId = Damage.DMG_GENERIC;
                return dmgTypeId;
            }
        }


        public Item(GameSpace space, object owner, ItemType type)
            : base(space, owner)
        {
            fType = type;
        }

        public IList<IAction> GetActionsList()
        {
            var list = new List<IAction>();
            list.Add(new ActionPickUp(this, "Pick up"));
            return list;
        }

        private class ActionPickUp : BaseEntityAction<Item>
        {
            public ActionPickUp(Item owner, string name) : base(owner, name)
            {
            }

            public override void Execute(object invoker)
            {
            }
        }
    }
}
