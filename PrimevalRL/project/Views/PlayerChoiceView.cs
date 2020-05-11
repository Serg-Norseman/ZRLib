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

using System;
using System.Collections.Generic;
using System.Text;
using BSLib;
using PrimevalRL.Creatures;
using PrimevalRL.Game;
using PrimevalRL.Maps;
using PrimevalRL.Maps.Buildings;
using PrimevalRL.Views.Controls;
using ZRLib.Core;
using ZRLib.Engine;

namespace PrimevalRL.Views
{
    public sealed class PlayerChoiceView : SubView
    {
        private readonly IList<Human> fCandidates;
        private readonly ChoicesArea fChoicesArea;

        public PlayerChoiceView(BaseView ownerView, Terminal terminal)
            : base(ownerView, terminal)
        {
            fCandidates = new List<Human>();
            fChoicesArea = new ChoicesArea(terminal, 10, GfxHelper.Darker(Colors.Green, 0.5f));
        }

        internal override void UpdateView()
        {
            fTerminal.Clear();
            fTerminal.TextBackground = Colors.Black;
            fTerminal.TextForeground = Colors.White;
            fTerminal.DrawBox(0, 0, 159, 79, false);

            fTerminal.TextForeground = Colors.LightGray;
            fTerminal.WriteCenter(1, 158, 2, Locale.GetStr(RS.Rs_PlayerChoices));

            fChoicesArea.Draw();

            fTerminal.WriteCenter(1, 158, 65, Locale.GetStr(RS.Rs_PressNumberOfChoice));
        }

        public override void Show()
        {
            LocatedEntityList population = ((Layer)GameSpace.CurrentRealm.PlainMap).Creatures;

            for (int i = 0; i < population.Count; i++) {
                Human human = (Human)population.GetItem(i);
                int age = human.Age;

                Building bld = human.Apartment;

                if (age >= 20 && age <= 35 && bld != null && bld.Status == HouseStatus.hsMansion) {
                    fCandidates.Add(human);
                    if (fCandidates.Count == 10) {
                        break;
                    }
                }
            }

            int maxw = 0;
            foreach (Human human in fCandidates) {
                if (maxw < human.Name.Length) {
                    maxw = human.Name.Length;
                }
            }

            int idx = 0;
            foreach (Human human in fCandidates) {
                string name = human.Name;
                string desc = GetHumanDesc(human);

                fChoicesArea.AddChoice(Convert.ToString(idx)[0], name.PadRight(maxw, ' ') + "  " + desc);
                idx++;
            }
        }

        private string GetHumanDesc(Human human)
        {
            StringBuilder result = new StringBuilder();

            string str = Convert.ToString(human.Age);
            result.Append("age ");
            result.Append(str);

            NPCStats stats = human.Stats;
            str = string.Format(", str {0:D}, per {1:D}, end {2:D}, chr {3:D}, int {4:D}, agi {5:D}, luck {6:D}", stats.Str, stats.Per, stats.End, stats.Chr, stats.Int, stats.Agi, stats.Luk);
            result.Append(str);

            return result.ToString();
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            switch (e.Key) {
                case Keys.GK_ESCAPE:
                    MainView.View = ViewType.vtStartup;
                    break;
            }
        }

        public override void KeyTyped(KeyPressEventArgs e)
        {
            char keyChar = e.Key;
            if (keyChar >= '0' && keyChar <= '9') {
                int num = Convert.ToInt32("0" + keyChar);
                if (num >= 0 && num < fCandidates.Count) {
                    GameSpace.PlayerController.Attach(fCandidates[num]);
                    MainView.View = ViewType.vtGame;
                }
            }
        }
    }
}
