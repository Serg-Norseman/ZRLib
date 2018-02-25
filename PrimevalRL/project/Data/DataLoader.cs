/*
 *  "PrimevalRL", roguelike game.
 *  Copyright (C) 2018 by Serg V. Zhdanovskih.
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using BSLib;
using PrimevalRL.Game;
using ZRLib.Core;

namespace PrimevalRL.Data
{
    public enum DamageType
    {
        Natural,
        Blunt,
        Slash,
        Point
    }

    public enum BodyPart
    {
        None,
        Hands,
        Head,
        Body
    }

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
        public string Sign;
        public int Color;
    }

    public enum TimeUnits
    {
        // BC
        Ma,
        Ka,
        // AD
        AD
    }

    public sealed class Period
    {
        public double Beg;
        public double End;
        public TimeUnits Units;
    }

    public sealed class CreatureRec
    {
        public string Name;

        public int Agility;
        public int Constitution;
        public int Sight;
        public int Strength;
        public int Skin;

        public Sprite Sprite;

        public HabitatType Habitat;
        public CreatureType Type;

        public string Period;
        public Period PeriodStruct;

        public string[] Area { get; set; }
        public string[] Loot { get; set; }
    }

    public class CreaturesList
    {
        public CreatureRec[] Creatures { get; set; }

        public CreaturesList()
        {
            Creatures = new CreatureRec[0];
        }
    }

    public sealed class Weapon
    {
        public DamageType DamageType;
        public int Damage;
    }

    /*public sealed class CraftSource
    {
        public string Name;
        public int Amount;
    }*/

    public sealed class Crafting
    {
        public string[] Sources;
        public string[] Tools;
    }

    public sealed class ItemRec
    {
        public string Name;
        public string ManyName;

        public string Desc;

        public float Weight;
        public BodyPart Wearable;
        public Weapon Weapon;
        public Sprite Sprite;

        public string[] Props { get; set; }
        public Crafting Crafting { get; set; }
    }

    public class ItemsList
    {
        public ItemRec[] Items { get; set; }

        public ItemsList()
        {
            Items = new ItemRec[0];
        }
    }

    public sealed class GeoTime
    {
        public float TimeBeg;
        public float TimeEnd;
        public string Name;
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataLoader
    {
        public CreaturesList fCreatures;
        public ItemsList fItems;
        public List<GeoTime> fTimes;

        public DataLoader()
        {
            fCreatures = new CreaturesList();
            fItems = new ItemsList();
            fTimes = new List<GeoTime>();
        }

        public void LoadCreatures(string fileName)
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
                Logger.Write("DataLoader.LoadCreatures(): " + ex.Message);
            }
        }

        public void LoadItems(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            try {
                // loading database
                using (var reader = new StreamReader(fileName, Encoding.UTF8)) {
                    string content = reader.ReadToEnd();
                    var rawData = YamlHelper.Deserialize(content, typeof(ItemsList));
                    fItems = rawData[0] as ItemsList;
                }
            } catch (Exception ex) {
                Logger.Write("DataLoader.LoadItems(): " + ex.Message);
            }
        }

        public string GetTimeName(int year)
        {
            string result = "";
            float y = year / 1000000.0f;
            foreach (var time in fTimes) {
                float beg = -time.TimeBeg;
                float end = -time.TimeEnd;

                if (y > beg && (y < end || end == 0.0f)) {
                    result = time.Name;
                    break;
                }
            }
            return result;
        }

        public void LoadGeoAges(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            try {
                // loading database
                using (var reader = new StreamReader(fileName, Encoding.UTF8)) {
                    var csv = CSVReader.CreateFromTextReader(reader, ";");

                    GeoTime prevTime = null;
                    List<object> row;
                    int num = 0;
                    while ((row = csv.ReadRow()) != null) {
                        num += 1;
                        if (num == 1)
                            continue;

                        var newTime = new GeoTime();
                        newTime.TimeBeg = (float)ConvertHelper.ParseFloat(row[0].ToString(), 0.0f, true);
                        newTime.Name = row[3].ToString() + " " + row[4].ToString() + " " + row[5].ToString() + " " + row[6].ToString();
                        fTimes.Add(newTime);

                        if (prevTime != null) {
                            prevTime.TimeEnd = newTime.TimeBeg;
                        }
                        prevTime = newTime;
                    }
                }
            } catch (Exception ex) {
                Logger.Write("DataLoader.LoadGeoAges(): " + ex.Message);
            }
        }
    }
}
