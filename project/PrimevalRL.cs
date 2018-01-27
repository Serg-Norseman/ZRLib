/*
 *  "PrimevalRL", roguelike game.
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
using System.Timers;
using PrimevalRL.Game;
using PrimevalRL.Views;
using ZRLib.Core;
using ZRLib.Engine;


[assembly: AssemblyTitle("PrimevalRL")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("PrimevalRL")]
[assembly: AssemblyCopyright("Copyright (C) 2015 by Serg V. Zhdanovskih")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("0.5.0.0")]


namespace PrimevalRL
{
    public sealed class PRLWin : Terminal
    {
        private Locale fLocale;
        private MainView fMainView;
        private System.Timers.Timer fTimer;

        public PRLWin()
            : base(160, 80, GetAppPath() + "resources\\cp866_8x8.bmp", 32, 8, 8)
        {
            System.Caption = MRLData.MRL_NAME;

            Logger.LogInit(GetAppPath() + "PrimevalRL.log");

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
                //AppCreate();
                try {
                    this.System.Run();
                } finally {
                    //AppDestroy();
                }
            } catch (Exception ex) {
                Logger.Write("PRLWin.Run(): " + ex.Message);
            }
        }

        public static void Main(string[] args)
        {
            //InitEnvironment(StaticData.Rs_GameName);
            //if (!InstanceExists)
            {
                var win = new PRLWin();
                win.Run();
                win.Dispose();
            }
        }
    }
}
