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

using System.Collections.Generic;
using BSLib;
using ZRLib.Engine;

namespace PrimevalRL.Views.Controls
{
    public class ChoicesArea
    {
        protected class Choice
        {
            public char Key;
            public string Title;
            public string ItemText;

            public Choice(char key, string title)
            {
                Key = key;
                Title = title;
                ItemText = "[" + key + "] " + title;
            }
        }

        protected readonly Terminal fTerminal;
        protected readonly IList<Choice> fChoices;
        protected readonly int fTop;
        protected ExtRect fArea;
        protected int fColor;

        public ChoicesArea(Terminal terminal, int top, int color)
        {
            fTerminal = terminal;
            fChoices = new List<Choice>();
            fTop = top;
            fArea = new ExtRect();
            fColor = color;
        }

        public virtual void AddChoice(char key, string title)
        {
            fChoices.Add(new Choice(key, title));
            Adjust();
        }

        private void Adjust()
        {
            int mxw = 0;
            foreach (Choice choice in fChoices) {
                string item = choice.ItemText;
                if (mxw < item.Length) {
                    mxw = item.Length;
                }
            }

            int chW = mxw + 8;
            int chH = fChoices.Count + (fChoices.Count - 1) + 8;
            int chX = (fTerminal.TermWidth - chW) / 2;
            int chY = fTop + (fTerminal.TermHeight - fTop - chH) / 2;

            fArea.SetBounds(chX, chY, chX + chW - 1, chY + chH - 1);
        }

        public virtual void Draw()
        {
            fTerminal.TextForeground = fColor;
            fTerminal.DrawBox(fArea.Left, fArea.Top, fArea.Right, fArea.Bottom, false);

            int yy = fArea.Top + 4;
            int xx = fArea.Left + 4;

            foreach (Choice choice in fChoices) {
                string item = choice.ItemText;
                fTerminal.Write(xx, yy, item);
                yy += 2;
            }
        }
    }
}
