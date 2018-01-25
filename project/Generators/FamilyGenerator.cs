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
using MysteriesRL.Maps;
using MysteriesRL.Maps.Buildings;
using ZRLib.Core;

namespace MysteriesRL.Generators
{
    public sealed class FamilyGenerator : NPCGenerator
    {
        private readonly NameGenerator fNameGen;
        private readonly string fSurname;
        private readonly Religion fReligion;

        public FamilyGenerator()
        {
            fNameGen = new NameGenerator();
            fSurname = fNameGen.GenerateSurname();
            fReligion = RandomHelper.GetRandomEnum<Religion>(typeof(Religion));
        }

        public override string GenerateFullName(bool isMale)
        {
            string name = fNameGen.GenerateName(isMale);
            return name + " " + fSurname;
        }

        public Human GenerateNPC(Layer layer, Building apartment, CreatureSex sex, bool isAdult)
        {
            ExtPoint pt = apartment.RandomPos;
            Human npc = GenerateHuman(layer, pt.X, pt.Y);
            npc.Personality.Religion = fReligion;

            if (sex == CreatureSex.csNone) {
                if (RandomHelper.GetRandom(100) > 50) {
                    sex = CreatureSex.csMale;
                } else {
                    sex = CreatureSex.csFemale;
                }
            }

            npc.Sex = sex;
            npc.Name = GenerateFullName(sex == CreatureSex.csMale);
            npc.Age = GenerateAge(isAdult);

            GenerateNPCStats(npc);

            return npc;
        }

        public Human GenerateChild(Layer layer, Building apartment, Human parent)
        {
            Human child = GenerateNPC(layer, apartment, CreatureSex.csNone, false);
            parent.AddChild(child);
            return child;
        }
    }
}
