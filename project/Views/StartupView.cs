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
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MysteriesRL.Core;
using MysteriesRL.Views.Controls;
using ZRLib.Core;
using ZRLib.Terminal;

namespace MysteriesRL.Views
{
    public sealed class StartupView : SubView, IProgressController
    {
        private Color SV_COL = Color.FromArgb(0, 128, 128);

        private string[] fTitle;
        private readonly ChoicesArea fChoicesArea;
        private bool fMenuMode;
        private int fGenStages = 50;
        private int fGenCompleted = 5;
        private string fGenLabel = null;

        public StartupView(BaseView ownerView, Terminal terminal)
            : base(ownerView, terminal)
        {
            ParseTitle();

            fChoicesArea = new ChoicesArea(terminal, 40, Color.DarkGreen);
            fChoicesArea.AddChoice('N', Locale.GetStr(RS.Rs_NewGame));
            fChoicesArea.AddChoice('Q', Locale.GetStr(RS.Rs_Quit));

            fMenuMode = true;
        }

        private void ParseTitle()
        {
            fTitle = new string[36];

            try {
                Assembly assembly = typeof(StartupView).Assembly;
                int i = 0;
                using (Stream book_names = assembly.GetManifestResourceStream("resources.startup.txt")) {
                    using (StreamReader strd = new StreamReader(book_names, Encoding.GetEncoding(1251))) {
                        while (strd.Peek() != -1) {
                            string ns = strd.ReadLine().Trim();
                            fTitle[i] = ns;
                            i++;
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.Write("StartupView.parseTitle(): " + ex.Message);
            }
        }

        internal override void UpdateView()
        {
            fTerminal.TextBackground = Color.Black;
            fTerminal.TextForeground = SV_COL;
            fTerminal.Clear();
            fTerminal.DrawBox(0, 0, 159, 79, false);

            for (int i = 0; i < fTitle.Length; i++) {
                string line = fTitle[i];
                if (line != null) {
                    for (int x = 0; x < line.Length; x++) {
                        char chr = line[x];
                        chr = (chr != '.') ? (char)177 : ' ';
                        fTerminal.Write(1 + x, 1 + i, chr);
                    }
                }
            }

            fTerminal.TextForeground = Color.AliceBlue;
            int top = fTitle.Length;
            fTerminal.WriteCenter(1, 158, top + 1, MRLData.MRL_VER);
            fTerminal.WriteCenter(1, 158, top + 3, MRLData.MRL_COPYRIGHT);

            if (fMenuMode) {
                fChoicesArea.Draw();
            }

            DrawProgress();
        }

        public override void KeyPressed(KeyEventArgs e)
        {
        }

        public override void KeyTyped(KeyPressEventArgs e)
        {
            switch (e.KeyChar) {
                case 'q':
                case 'Q':
                    Environment.Exit(0);
                    break;

                case 'n':
                case 'N':
                    fMenuMode = false;
                    GameSpace.InitNew(this);
                    MainView.View = ViewType.vtPlayerChoice;
                    break;
            }
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

        public override void Tick()
        {
            UpdateView();
        }

        private void DrawProgress()
        {
            if (fGenLabel != null) {
                fTerminal.TextForeground = Color.White;
                fTerminal.Write(20, 70, fGenLabel);
                fTerminal.DrawProgress(20, 159 - 20, 71, fGenCompleted, fGenStages);
            }
        }

        public void Complete(int stage)
        {
            fGenCompleted++;

            UpdateView();
            MainView.RepaintImmediately();
        }

        public void SetStage(string label, int size)
        {
            fGenLabel = label;
            fGenStages = size;
            fGenCompleted = 0;

            UpdateView();
            MainView.RepaintImmediately();
        }
    }
}
