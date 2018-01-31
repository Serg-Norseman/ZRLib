/*
 *  "GEDKeeper", the personal genealogical database editor.
 *  Copyright (C) 2017 by Sergey V. Zhdanovskih.
 *
 *  This file is part of "GEDKeeper".
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

using System;
using System.IO;
using PrimevalRL.Game;
using ZRLib.Core;

namespace PrimevalRL.Data
{
    public enum CreatureType
    {
        Carnivore,
        Herbivore
    }

    public enum HabitatType
    {
        Ground,
        Underground,
        Water,
        Underwater,
        Flying
    }

    public sealed class Sprite
    {
        public char Sign;
        public int Color;
    }

    public sealed class CreatureRec
    {
        public string Name;

        public int Agility;
        public int Constitution;
        public int Sight;
        public int Strength;

        public Sprite Sprite;

        public HabitatType Habitat;
        public CreatureType Type;

        public string Period;
    }

    internal class CreaturesList
    {
        public CreatureRec[] Creatures { get; set; }

        public CreaturesList()
        {
            Creatures = new CreatureRec[0];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataLoader
    {
        public static CreatureRec LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            try {
                // loading database
                using (var reader = new StreamReader(fileName)) {
                    string content = reader.ReadToEnd();
                    var rawData = YamlHelper.Deserialize(content, typeof(CreatureRec));
                    return rawData[0] as CreatureRec;
                }
            } catch (Exception ex) {
                Logger.Write("DataLoader.LoadFromFile(): " + ex.Message);
                return null;
            }
        }

        private CreaturesList fCreatures;

        public DataLoader()
        {
            fCreatures = new CreaturesList();
        }

        public void Load(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            try {
                // loading database
                using (var reader = new StreamReader(fileName)) {
                    string content = reader.ReadToEnd();
                    var rawData = YamlHelper.Deserialize(content, typeof(CreaturesList));
                    fCreatures = rawData[0] as CreaturesList;
                }
            } catch (Exception ex) {
                Logger.Write("DataLoader.Load(): " + ex.Message);
            }
        }
    }
}
