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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BSLib;
using MysteriesRL.Creatures;
using MysteriesRL.Game;
using MysteriesRL.Maps;
using ZRLib.Core;
using ZRLib.Core.Action;
using ZRLib.Map;
using ZRLib.Terminal;

namespace MysteriesRL.Views
{
    public sealed class GameView : SubView
    {
        private ExtRect fMapRect;
        private ExtPoint fMouseClick;
        private int fTick;

        public GameView(BaseView ownerView, Terminal terminal)
            : base(ownerView, terminal)
        {
        }

        public override MRLGame GameSpace
        {
            get {
                return ((MainView)fOwnerView).GameSpace;
            }
        }

        public ExtPoint GetLocalePoint(ExtPoint terminalPoint)
        {
            int mpx = (terminalPoint.X - MRLData.GV_BOUNDS.Left) + fMapRect.Left;
            int mpy = (terminalPoint.Y - MRLData.GV_BOUNDS.Top) + fMapRect.Top;
            return new ExtPoint(mpx, mpy);
        }

        internal override void UpdateView()
        {
            fTerminal.Clear();
            fTerminal.TextBackground = Color.Black;
            fTerminal.TextForeground = Color.White;
            fTerminal.DrawBox(0, 0, 102, 70, false);
            fTerminal.DrawBox(0, 71, 159, 79, false);

            MRLGame gameSpace = GameSpace;
            City city = gameSpace.City;

            DrawGameField(gameSpace);
            DrawMessages(gameSpace);

            fTerminal.TextForeground = Color.White;

            DrawText(105, 1, "Time: " + gameSpace.Time.ToString(true, true), Color.White);
            DrawText(105, 3, "City name: " + city.Name, Color.White);

            Human player = gameSpace.PlayerController.Player;
            NPCStats stats = player.Stats;
            HumanBody body = (HumanBody)player.Body;

            DrawStat(105, 159, 5, "Player name ", player.Name);
            DrawStat(105, 159, 7, "XP          ", stats.CurXP, stats.NextLevelXP, 0);
            DrawStat(105, 159, 9, "HP          ", player.HP, player.HPMax, 0.25f);
            DrawStat(105, 159, 11, "Stamina     ", body.GetAttribute("stamina"), 100, 0.25f);
            DrawStat(105, 159, 13, "Hunger      ", body.GetAttribute("hunger"), 100, 0.25f);

            //drawStat(105, 159, 13, "Carry Weight: " + String.valueOf(stats.getCarryWeight()), Color.white);

            DrawAvailableActions(gameSpace.PlayerController);

            //fTerminal.write(105, 73, "Dark: " + String.valueOf(darkness), Color.white);
        }

        private void DrawAvailableActions(PlayerController playerCtl)
        {
            IList<IAction> actions = playerCtl.AvailableActions;
            if (actions != null) {
                int top = 50;
                int idx = 0;
                foreach (var action in actions) {
                    char key = (char)('A' + idx++);
                    DrawText(105, top, "[" + key + "] " + action.Name, Color.White);
                    top += 2;
                }
            }
        }

        private void DrawMessages(MRLGame gameSpace)
        {
            var messages = gameSpace.Messages;
            if (messages.Count < 1) {
                return;
            }

            int top = 72;
            int hb = messages.Count - 1;
            int lb = Math.Max(0, hb - 6);

            for (int i = lb; i <= hb; i++) {
                TextMessage msg = messages[i];
                int yy = top + (i - lb);
                fTerminal.Write(1, yy, msg.Text, msg.Color);
            }
        }

        private void DrawGameField(MRLGame gameSpace)
        {
            float darkness = 1 - gameSpace.LightBrightness;
            PlayerController playerController = gameSpace.PlayerController;
            Human player = playerController.Player;
            IMap map = player.Map;

            int px = player.PosX;
            int py = player.PosY;
            fMapRect = playerController.Viewport;

            for (int ay = fMapRect.Top; ay <= fMapRect.Bottom; ay++) {
                for (int ax = fMapRect.Left; ax <= fMapRect.Right; ax++) {
                    int sx = MRLData.GV_BOUNDS.Left + (ax - fMapRect.Left);
                    int sy = MRLData.GV_BOUNDS.Top + (ay - fMapRect.Top);

                    DrawTile(map, px, py, ax, ay, sx, sy, darkness);
                }
            }
        }

        private void DrawTile(IMap map, int px, int py, int ax, int ay, int sx, int sy, float darkness)
        {
            char tileChar = ' ';
            Color fgc = Color.White;
            Color bgc = Color.Black;

            if (!fMouseClick.IsEmpty && fMouseClick.Equals(ax, ay) && ax != px && ay != py) {
                tileChar = '*';
                fgc = Color.White;
            } else {
                BaseTile tile = map.GetTile(ax, ay);
                if (tile != null/* && tile.HasState(BaseTile.TS_VISITED)*/) {
                    bool seen = tile.HasState(BaseTile.TS_SEEN);

                    Creature creature = null;
                    if (seen) {
                        creature = (Creature)map.FindCreature(ax, ay);
                    }

                    if (creature != null) {
                        tileChar = creature.Appearance;
                        fgc = creature.AppearanceColor;
                    } else {
                        byte pf_status = tile.Pf_status;
                        int id;

                        switch (pf_status) {
                            case PathSearch.tps_Start:
                                id = (int)TileID.tid_PathStart;
                                break;
                            case PathSearch.tps_Finish:
                                id = (int)TileID.tid_PathFinish;
                                break;
                            case PathSearch.tps_Path:
                                id = (int)TileID.tid_Path;
                                break;
                            default:
                                id = (tile.Foreground != 0) ? tile.Foreground : tile.Background;
                                break;
                        }

                        if (id != 0) {
                            var tileRec = MRLData.GetTileRec(id);
                            tileChar = tileRec.Sym;
                            fgc = tileRec.SymColor;
                            bgc = tileRec.BackColor;
                        }

                        if (!seen) {
                            fgc = AuxUtils.Darker(fgc, darkness);
                        }
                    }
                }
            }

            fTerminal.Write(sx, sy, tileChar, fgc, bgc);
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            Keys code = e.KeyCode;
            //System.out.println(String.valueOf(code));

            PlayerController playerController = GameSpace.PlayerController;
            int newX = playerController.Player.PosX;
            int newY = playerController.Player.PosY;

            switch (code) {
                case Keys.Escape:
                    playerController.ClearPath();
                    break;

                case Keys.Tab:
                    MainView.View = ViewType.vtMinimap;
                    break;

                case Keys.Left:
                    newX--;
                    break;
                case Keys.Up:
                    newY--;
                    break;
                case Keys.Right:
                    newX++;
                    break;
                case Keys.Down:
                    newY++;
                    break;
            }

            playerController.MoveTo(newX, newY);
        }

        public override void KeyTyped(KeyPressEventArgs e)
        {
            PlayerController playerCtl = GameSpace.PlayerController;

            switch (e.KeyChar) {
                case 'a':
                    playerCtl.Attack();
                    break;

                case 'c':
                    MainView.View = ViewType.vtSelf;
                    break;

                case 'f':
                    playerCtl.CircularFOV = !playerCtl.CircularFOV;
                    break;

                case 'h':
                    MainView.View = ViewType.vtHelp;
                    break;

                case 'k':
                    playerCtl.SwitchLock();
                    break;

                case 'l':
                    GameSpace.Look(-1, -1);
                    break;

                case 't':
                    if (!fMouseClick.IsEmpty) {
                        playerCtl.MoveTo(fMouseClick.X, fMouseClick.Y);
                    }
                    break;

                case 'w':
                    DoWalk();
                    break;

                case 'q':
                    Application.Exit();
                    break;

                case ' ':
                    break;

                default:
                    IList<IAction> actions = playerCtl.AvailableActions;
                    if (actions != null) {
                        int idx = 0;
                        foreach (var action in actions) {
                            char key = (char)('A' + idx++);
                            if (e.KeyChar == key) {
                                action.Execute(playerCtl.Player);
                            }
                        }
                    }
                    break;
            }
        }

        public void DoWalk()
        {
            GameSpace.PlayerController.Walk(fMouseClick);
        }

        public override void MouseClicked(MouseEventArgs e)
        {
            ExtPoint termPt = fTerminal.GetTerminalPoint(e.X, e.Y);
            if (MRLData.GV_BOUNDS.Contains(termPt)) {
                ExtPoint lpt = GetLocalePoint(termPt);

                if (e.Button == MouseButtons.Left) {
                    if (GameSpace.CheckTarget(lpt.X, lpt.Y)) {
                        fMouseClick = lpt;
                    } else {
                        fMouseClick = ExtPoint.Empty;
                    }
                } else {
                    GameSpace.Look(lpt.X, lpt.Y);
                }
            }
        }

        public override void MouseMoved(MouseEventArgs e)
        {
        }

        public override void Tick()
        {
            fTick++;
            int ex = fTick % 20;
            if (ex == 0) {
                GameSpace.UpdateWater();
            }

            GameSpace.DoTurn();
            UpdateView();
        }

        public override void Show()
        {
            fTick = 0;
        }
    }
}
