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

using ZRLib.Engine;

namespace PrimevalRL.Views
{
    public sealed class HelpView : SubView
    {
        public HelpView(BaseView ownerView, Terminal terminal)
            : base(ownerView, terminal)
        {
        }

        internal override void UpdateView()
        {
            fTerminal.Clear();
            fTerminal.TextBackground = Colors.Black;
            fTerminal.TextForeground = Colors.White;
            fTerminal.DrawBox(0, 0, 159, 79, false);

            fTerminal.TextForeground = Colors.LightGray;
            fTerminal.Write(2, 2, "Keys: ");
        }

        public override void Tick()
        {
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            switch (e.Key) {
                case Keys.GK_ESCAPE:
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

        public override void MouseMoved(MouseMoveEventArgs e)
        {
        }

        public override void Show()
        {
        }
    }
}
