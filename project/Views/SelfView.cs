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

using System;
using System.Drawing;
using System.Windows.Forms;
using MysteriesRL.Creatures;
using MysteriesRL.Game;
using MysteriesRL.Maps;
using ZRLib.Terminal;

namespace MysteriesRL.Views
{
    public sealed class SelfView : SubView
    {
        public SelfView(BaseView ownerView, Terminal terminal)
            : base(ownerView, terminal)
        {
        }

        public override MRLGame GameSpace
        {
            get {
                return ((MainView)fOwnerView).GameSpace;
            }
        }

        internal override void UpdateView()
        {
            fTerminal.Clear();
            fTerminal.TextBackground = Color.Black;
            fTerminal.TextForeground = Color.White;
            fTerminal.DrawBox(0, 0, 159, 79, false);

            MRLGame gameSpace = GameSpace;
            Human player = gameSpace.PlayerController.Player;
            NPCStats stats = player.Stats;
            HumanBody body = (HumanBody)player.Body;
            City city = gameSpace.City;
            Layer layer = (Layer)gameSpace.PlainMap;

            fTerminal.TextForeground = Color.LightGray;
            fTerminal.DrawBox(2, 2, 50, 50, false, "Player");
            fTerminal.Write(4, 4, "Player name:  " + player.Name, Color.White);
            fTerminal.Write(4, 6, "Sex:          " + player.Sex.ToString(), Color.White);
            fTerminal.Write(4, 8, "Age:          " + Convert.ToString(player.Age), Color.White);
            //fTerminal.write(4, 10, "Personality:  " + player.getPersonality().getDesc(), Color.white);

            fTerminal.Write(4, 14, "Strength:     " + Convert.ToString(stats.Str), Color.White);
            fTerminal.Write(4, 16, "Perception:   " + Convert.ToString(stats.Per), Color.White);
            fTerminal.Write(4, 18, "Endurance:    " + Convert.ToString(stats.End), Color.White);
            fTerminal.Write(4, 20, "Charisma:     " + Convert.ToString(stats.Chr), Color.White);
            fTerminal.Write(4, 22, "Intelligence: " + Convert.ToString(stats.Int), Color.White);
            fTerminal.Write(4, 24, "Agility:      " + Convert.ToString(stats.Agi), Color.White);
            fTerminal.Write(4, 26, "Luck:         " + Convert.ToString(stats.Luk), Color.White);

            fTerminal.Write(4, 30, "Level:        " + Convert.ToString(stats.Level), Color.White);
            fTerminal.Write(4, 32, "XP:           " + Convert.ToString(stats.CurXP) + " / " + Convert.ToString(stats.NextLevelXP), Color.White);
            fTerminal.Write(4, 34, "Carry Weight: " + Convert.ToString(0) + " / " + Convert.ToString(stats.CarryWeight), Color.White);
            fTerminal.Write(4, 36, "HP:           " + Convert.ToString(player.HP) + " / " + Convert.ToString(player.HPMax), Color.White);

            fTerminal.Write(4, 40, "Stamina:      " + body.GetAttribute("stamina"), Color.White);
            fTerminal.Write(4, 42, "Hunger:       " + body.GetAttribute("hunger"), Color.White);

            fTerminal.DrawBox(103, 2, 157, 14, false, "City");
            fTerminal.Write(105, 4, "Name:        " + city.Name, Color.White);
            fTerminal.Write(105, 6, "Streets:     " + Convert.ToString(city.Streets.Count), Color.White);
            fTerminal.Write(105, 8, "Districts:   " + Convert.ToString(city.Districts.Count), Color.White);
            fTerminal.Write(105, 10, "Buildings:   " + Convert.ToString(city.Buildings.Count), Color.White);
            fTerminal.Write(105, 12, "Inhabitants: " + Convert.ToString(layer.Creatures.Count), Color.White);

            fTerminal.DrawBox(52, 2, 101, 50, false, "Crimes");

            fTerminal.DrawBox(2, 52, 157, 78, false, "Description");
            fTerminal.Write(4, 54, player.Desc, Color.White);
        }

        public override void Tick()
        {
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            switch (e.KeyCode) {
                case Keys.Escape:
                    MainView.View = ViewType.vtGame;
                    break;
            }
        }

        public override void KeyTyped(KeyPressEventArgs e)
        {
        }

        public override void MouseClicked(MouseEventArgs e)
        {
        }

        public override void MouseMoved(MouseEventArgs e)
        {
        }

        public override void Show()
        {
        }
    }
}
