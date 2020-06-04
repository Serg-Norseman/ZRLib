/*
 *  "MysteriesRL", roguelike game.
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
using System.IO;
using System.Text;
using MysteriesRL.Game;
using ZRLib.Core;

namespace MysteriesRL.Data
{
    public class CreaturesList
    {
        public CreatureRec[] Creatures { get; set; }

        public CreaturesList()
        {
            Creatures = new CreatureRec[0];
        }
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


    public class ItemsList
    {
        public ItemRec[] Items { get; set; }

        public ItemsList()
        {
            Items = new ItemRec[0];
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class DataLoader
    {
        public CreaturesList fCreatures;
        public ItemsList fItems;

        public DataLoader()
        {
            fCreatures = new CreaturesList();
            fItems = new ItemsList();
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
    }
}
