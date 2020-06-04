/*
 *  "MysteriesRL", roguelike game.
 *  Copyright (C) 2015 by Serg V. Zhdanovskih.
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using MysteriesRL.Data;
using MysteriesRL.Game;
using MysteriesRL.Views;
using ZRLib.Core;
using ZRLib.Engine;


[assembly: AssemblyTitle(MRLData.MRL_NAME)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct(MRLData.MRL_NAME)]
[assembly: AssemblyCopyright(MRLData.MRL_COPYRIGHT)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("0.5.0.0")]


namespace MysteriesRL
{
    public sealed class MRLWin : Terminal
    {
        private Locale fLocale;
        private MainView fMainView;
        private System.Timers.Timer fTimer;

        public MRLWin()
            : base(160, 80, GetAppPath() + "resources\\cp866_8x8.bmp", 32, 8, 8)
        {
            System.Caption = MRLData.MRL_NAME;

            Logger.LogInit(GetAppPath() + "MysteriesRL.log");

            fLocale = new Locale();

            fMainView = new MainView(null, this);

            fTimer = new System.Timers.Timer(250);
            fTimer.Elapsed += TickTimer;
            fTimer.Start();
        }

        private void TickTimer(object sender, ElapsedEventArgs e)
        {
            fMainView.Tick();
        }

        protected override void UpdateView()
        {
            fMainView.UpdateView();
        }

        public override void ProcessGameStep()
        {
            // dummy
        }

        public override void ProcessKeyDown(KeyEventArgs e)
        {
            fMainView.KeyPressed(e);
        }

        public override void ProcessKeyPress(KeyPressEventArgs e)
        {
            fMainView.KeyTyped(e);
        }

        public override void ProcessMouseMove(MouseMoveEventArgs e)
        {
            fMainView.MouseMoved(e);
        }

        public override void ProcessMouseDown(MouseEventArgs e)
        {
            fMainView.MouseClicked(e);
        }

        public static string GetAppPath()
        {
            Module[] mods = Assembly.GetExecutingAssembly().GetModules();
            string fn = mods[0].FullyQualifiedName;
            return Path.GetDirectoryName(fn) + Path.DirectorySeparatorChar;
        }

        public void Run()
        {
            try {
                System.Run();
            } catch (Exception ex) {
                Logger.Write("MRLWin.Run(): " + ex.Message);
            }
        }

        public static void Main(string[] args)
        {
            bool isFirstInstance;
            using (Mutex mtx = new Mutex(true, MRLData.MRL_NAME, out isFirstInstance)) {
                if (isFirstInstance) {
                    var win = new MRLWin();
                    win.Run();
                    win.Dispose();
                }
            }
        }
    }
}
