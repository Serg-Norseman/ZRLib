/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
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

using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
using ZRLib.Core;
using MysteriesRL.Core;
using MysteriesRL.Views;
using ZRLib.Terminal;

namespace MysteriesRL
{
    public sealed class MRLWin : Form
    {
        private readonly Terminal fTerminal;

        private Locale fLocale;
        private MainView fMainView;
        private System.Timers.Timer fTimer;

        public MRLWin()
            : base()
        {
            fTerminal = new Terminal(160, 80);
            fTerminal.KeyPress += KeyTyped;
            fTerminal.KeyUp += KeyPressed;
            fTerminal.MouseDown += MouseClicked;
            fTerminal.MouseMove += MouseMoved;
            fTerminal.Dock = DockStyle.Fill;

            ClientSize = fTerminal.Size;
            Controls.Add(fTerminal);
            MaximizeBox = false;
            MinimizeBox = true;
            Text = MRLData.MRL_NAME;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;

            Logger.LogInit(GetAppPath() + "MysteriesRL.log");

            fLocale = new Locale();

            fMainView = new MainView(null, fTerminal);

            fTimer = new System.Timers.Timer(250);
            fTimer.Elapsed += TickTimer;
            fTimer.Start();
        }

        private void TickTimer(object sender, ElapsedEventArgs e)
        {
            fMainView.Tick();
            fTerminal.Invalidate(false);
        }

        public void KeyPressed(object sender, KeyEventArgs e)
        {
            fMainView.KeyPressed(e);
        }

        public void KeyTyped(object sender, KeyPressEventArgs e)
        {
            fMainView.KeyTyped(e);
        }

        public void MouseMoved(object sender, MouseEventArgs e)
        {
            fMainView.MouseMoved(e);
        }

        public void MouseClicked(object sender, MouseEventArgs e)
        {
            fMainView.MouseClicked(e);
        }

        public static string GetAppPath()
        {
            Module[] mods = Assembly.GetExecutingAssembly().GetModules();
            string fn = mods[0].FullyQualifiedName;
            return Path.GetDirectoryName(fn) + Path.DirectorySeparatorChar;
        }
    }
}
