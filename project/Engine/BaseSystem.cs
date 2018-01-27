/*
 *  "ZRLib", Roguelike games development Library.
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
using System.Threading;
using BSLib;
using ZRLib.Core;

namespace ZRLib.Engine
{
    public abstract class BaseSystem : BaseObject
    {
        private const bool DEBUG_FRAMES = false;

        protected int fFrameDuration;
        protected bool fFullScreen;
        protected IMainWindow fMainWindow;
        protected BaseScreen fScreen;
        protected bool fSysCursorVisible;
        protected bool fTerminate;

        private int fFLCount;
        private long fFLSum;

        protected BaseSystem(IMainWindow mainWindow, int width, int height, bool fullScreen)
        {
            fMainWindow = mainWindow;
            fFullScreen = fullScreen;
            fTerminate = false;
            fScreen = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (fScreen != null) {
                    fScreen.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public BaseScreen Screen
        {
            get { return fScreen; }
        }

        public virtual bool SysCursorVisible
        {
            get {
                return fSysCursorVisible;
            }
            set {
                if (fSysCursorVisible != value) {
                    fSysCursorVisible = value;
                }
            }
        }

        public virtual bool InitFullScreen()
        {
            return false;
        }

        public virtual void DoneFullScreen()
        {
        }

        public void Help(string fileName)
        {
            Exec("explorer.exe \"" + fileName + "\"");
        }

        public void Resize(int width, int height)
        {
            if (fScreen != null) {
                fScreen.SetSize(width, height);
            }
        }

        public void Quit()
        {
            fTerminate = true;
        }

        protected abstract void ProcessEvents();

        public abstract string Caption { set; }

        public void Run()
        {
            try {
                if (fFullScreen) {
                    fFullScreen = InitFullScreen();
                }

                try {
                    IMainWindow mainWindow = fMainWindow;

                    long drawTimer = TickCount;
                    while (!fTerminate) {
                        ProcessEvents();

                        mainWindow.ProcessGameStep();

                        long now = TickCount;
                        long late = now - drawTimer;
                        if (late >= fFrameDuration * 5) {
                            drawTimer = now;
                            late = 0;
                        }

                        if (late < fFrameDuration) {
                            mainWindow.Update(now);

                            late = TickCount - drawTimer;
                            if (late < fFrameDuration) {
                                long intval = fFrameDuration - late;

                                if (DEBUG_FRAMES) {
                                    if (fFLCount == 200) {
                                        fFLCount = 0;
                                        fFLSum = 0;
                                    }

                                    fFLCount++;
                                    fFLSum += intval;

                                    Caption = Convert.ToString(fFLSum / fFLCount);
                                }

                                Sleep((int)intval);
                            }
                        }

                        drawTimer += fFrameDuration;
                    }
                } finally {
                    if (fFullScreen) {
                        DoneFullScreen();
                    }
                }
            } catch (Exception ex) {
                Logger.Write("BaseSystem.run(): " + ex.Message);
            }
        }

        public static void Exec(string command)
        {
            try {
                //Process p = Runtime.Runtime.exec(command);
                //p.waitFor();
                //System.out.println(p.exitValue());
            } catch (Exception ex) {
                Logger.Write("BaseSystem.exec(): " + ex.Message);
            }
        }

        public static long TickCount
        {
            get { return Environment.TickCount; }
        }

        public static void Sleep(int milliseconds)
        {
            try {
                Thread.Sleep(milliseconds);
            } catch (Exception) {
            }
        }
    }
}
