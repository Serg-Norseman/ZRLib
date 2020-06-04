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
using MysteriesRL.Game;
using ZRLib.Engine;

namespace MysteriesRL.Views
{
    public sealed class HelpView : SubView
    {
        private List<string> fText;

        public HelpView(BaseView ownerView, Terminal terminal)
            : base(ownerView, terminal)
        {
            fText = GameUtils.ReadResText("en_help.txt");
        }

        internal override void UpdateView()
        {
            fTerminal.Clear();
            fTerminal.TextBackground = Colors.Black;
            fTerminal.TextForeground = Colors.White;
            fTerminal.DrawBox(0, 0, 159, 79, false);

            fTerminal.TextForeground = Colors.LightGray;
            for (int i = 0; i < fText.Count; i++) {
                string line = fText[i];
                fTerminal.Write(4, 2 + i * 2, line);
            }
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            switch (e.Key) {
                case Keys.GK_ESCAPE:
                    MainView.View = ViewType.vtGame;
                    break;
            }
        }
    }
}
