/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015, 2017 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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

using BSLib;
using MysteriesRL.Creatures;
using MysteriesRL.Game;
using MysteriesRL.Items;
using MysteriesRL.Maps;
using ZRLib.Core;

namespace MysteriesRL.Generators
{
    public class NPCGenerator
    {
        public static int GenerateAge(bool isAdult)
        {
            if (isAdult) {
                return 24 + RandomHelper.GetRandom(40);
            } else {
                return 8 + RandomHelper.GetRandom(16);
            }
        }

        public virtual string GenerateFullName(bool isMale)
        {
            NameGenerator namegen = new NameGenerator();
            return namegen.GenerateFullName(isMale);
        }

        public Human GenerateHuman(Layer layer, int x, int y)
        {
            Human human = new Human(GameSpace.Instance, layer);
            human.SetPos(x, y);
            layer.AddCreature(human);
            return human;
        }

        public virtual Human GenerateNPC(Layer layer, int x, int y)
        {
            Human npc = GenerateHuman(layer, x, y);

            CreatureSex sex;
            if (RandomHelper.GetRandom(100) > 50) {
                sex = CreatureSex.csMale;
            } else {
                sex = CreatureSex.csFemale;
            }

            npc.Sex = sex;
            npc.Name = GenerateFullName(sex == CreatureSex.csMale);
            npc.Age = 8 + RandomHelper.GetRandom(87);

            GenerateNPCStats(npc);

            GenerateItems(npc);

            return npc;
        }

        public virtual void GenerateNPCStats(Human entity)
        {
            // generate criminal background
            int criminalRate = 30; //%
            if (RandomHelper.GetRandom(100) < criminalRate) {
                int crimeTypes = RandomHelper.GetRandom(5);
                for (int i = 0; i < crimeTypes; i++) {
                    entity.AddCrimeRecord(new CrimeRecord(RandomHelper.GetRandomEnum<CrimeType>(typeof(CrimeType))));
                }
            }

            HumanBody body = (HumanBody)entity.Body;
            if (body != null && RandomHelper.GetRandom(100) <= 25) {
                body.Infected = true;
            }
        }

        private void GenerateItems(Human entity)
        {
            GameSpace space = entity.Space;
            ItemsList inventory = entity.Inventory;

            inventory.Add(new Item(space, entity, ItemType.IT_SHIRT));
            inventory.Add(new Item(space, entity, ItemType.IT_BOOTS));

            if (entity.Sex == CreatureSex.csFemale) {
                inventory.Add(new Item(space, entity, ItemType.IT_SKIRT));
            } else {
                inventory.Add(new Item(space, entity, ItemType.IT_PANTS));
            }
        }
    }
}
