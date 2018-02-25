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

using BSLib;
using PrimevalRL.Game;
using ZRLib.Engine;

namespace PrimevalRL.Views
{
    public enum ViewType
    {
        vtStartup,
        vtGame,
        vtMinimap,
        vtHelp,
        vtPlayerChoice,
        vtSelf
    }

    public abstract class BaseView
    {
        protected readonly BaseView fOwnerView;
        protected readonly Terminal fTerminal;

        public abstract MRLGame GameSpace { get; }

        protected BaseView(BaseView ownerView, Terminal terminal)
        {
            fOwnerView = ownerView;
            fTerminal = terminal;
        }

        public void DrawText(int x, int y, string str)
        {
            fTerminal.Write(x, y, str);
        }

        public void DrawText(int x, int y, string str, int foreground)
        {
            fTerminal.Write(x, y, str, foreground);
        }

        public void DrawStat(int x1, int x2, int y, string name, string value)
        {
            name = name + "[";
            fTerminal.Write(x1, y, name);
            fTerminal.Write(x2, y, "]");

            int xx = x1 + name.Length;
            int xval = ((x2 - xx) - value.Length) / 2;
            fTerminal.Write(xx + xval, y, value);
        }

        public void DrawStat(int x1, int x2, int y, string name, float value, float max, float redLevel)
        {
            name = name + "[";
            fTerminal.Write(x1, y, name);

            float factor = (value / max);
            int percent = (int)(factor * 100.0f);
            string val = ConvertHelper.AdjustNumber(percent, 3, ' ') + "%]";
            fTerminal.Write(x2 - val.Length + 1, y, val);

            int curfg = (factor < redLevel) ? Colors.Red : fTerminal.TextForeground;

            int xx1 = x1 + name.Length + 1;
            int xx2 = x2 - val.Length - 1;
            int len = (int)((xx2 - xx1) * factor);
            if (len > 0) {
                for (int i = xx1; i <= xx1 + len; i++) {
                    fTerminal.Write(i, y, (char)177, curfg);
                }
            }
        }

        internal abstract void UpdateView();

        public abstract void KeyPressed(KeyEventArgs e);
        public abstract void KeyTyped(KeyPressEventArgs e);

        public abstract void MouseClicked(MouseEventArgs e);
        public abstract void MouseMoved(MouseMoveEventArgs e);

        public abstract void Tick();
        public abstract void Show();
    }
}
