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

using System;
using System.Collections.Generic;
using BSLib;
using MysteriesRL.Creatures;
using MysteriesRL.Game;
using MysteriesRL.Maps;
using ZRLib.Core;
using ZRLib.Core.Action;
using ZRLib.Engine;
using ZRLib.Map;

namespace MysteriesRL.Views
{
    public sealed class GameView : SubView
    {
        private ExtRect fMapRect;
        private ExtPoint fMouseClick;

        public GameView(BaseView ownerView, Terminal terminal)
            : base(ownerView, terminal)
        {
        }

        public ExtPoint GetLocalePoint(ExtPoint terminalPoint)
        {
            int mpx = (terminalPoint.X - MRLData.GV_BOUNDS.Left) + fMapRect.Left;
            int mpy = (terminalPoint.Y - MRLData.GV_BOUNDS.Top) + fMapRect.Top;
            return new ExtPoint(mpx, mpy);
        }

        internal override void UpdateView()
        {
            try {
                fTerminal.Clear();
                fTerminal.TextBackground = Colors.Black;
                fTerminal.TextForeground = Colors.White;
                fTerminal.DrawBox(0, 0, 102, 70, false);
                fTerminal.DrawBox(0, 71, 159, 79, false);

                MRLGame gameSpace = GameSpace;

                DrawGameField(gameSpace);
                DrawMessages(gameSpace);

                fTerminal.TextForeground = Colors.White;

                var time = gameSpace.CurrentRealm.Time;
                DrawText(105, 1, "Time: " + time.ToString(true, true), Colors.White);

                Human player = gameSpace.PlayerController.Player;
                NPCStats stats = player.Stats;
                HumanBody body = (HumanBody)player.Body;

                DrawStat(105, 159,  5, "Player name ", player.Name);
                DrawStat(105, 159,  7, "XP          ", stats.CurXP, stats.NextLevelXP, 0);
                DrawStat(105, 159,  9, "HP          ", player.HP, player.HPMax, 0.25f);
                DrawStat(105, 159, 11, "Stamina     ", body.GetAttribute("stamina"), 100, 0.25f);
                DrawStat(105, 159, 13, "Satiety     ", body.GetAttribute("satiety"), 100, 0.25f);

                //drawStat(105, 159, 13, "Carry Weight: " + String.valueOf(stats.getCarryWeight()), Colors.white);

                DrawAvailableActions(gameSpace.PlayerController);

                //fTerminal.write(105, 73, "Dark: " + String.valueOf(darkness), Colors.white);
            } catch (Exception ex) {
                Logger.Write("GameView.UpdateView(): " + ex.Message);
            }
        }

        private void DrawAvailableActions(PlayerController playerCtl)
        {
            IList<IAction> actions = playerCtl.AvailableActions;
            if (actions != null) {
                int top = 50;
                int idx = 0;
                foreach (var action in actions) {
                    char key = (char)('A' + idx++);
                    DrawText(105, top, "[" + key + "] " + action.Name, Colors.White);
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
            try {
                PlayerController playerController = gameSpace.PlayerController;
                Human player = playerController.Player;
                IMap map = player.Map;
                fMapRect = playerController.Viewport;

                int px = player.PosX;
                int py = player.PosY;

                var lightRect = playerController.LightRect;
                int lrX = lightRect.Left;
                int lrY = lightRect.Top;
                float[,] lightMap = playerController.LightMap;

                int gvX = MRLData.GV_BOUNDS.Left - fMapRect.Left;
                int gvY = MRLData.GV_BOUNDS.Top - fMapRect.Top;

                for (int ay = fMapRect.Top; ay <= fMapRect.Bottom; ay++) {
                    for (int ax = fMapRect.Left; ax <= fMapRect.Right; ax++) {
                        int sx = gvX + ax;
                        int sy = gvY + ay;
                        float light = lightMap[ay - lrY, ax - lrX];
                        float darkness = 1.0f - (gameSpace.LightBrightness + light);

                        DrawTile(map, px, py, ax, ay, sx, sy, darkness);
                    }
                }

                DrawEntities(map, gvX, gvY);
            } catch (Exception ex) {
                Logger.Write("GameView.DrawGameField(): " + ex.Message);
                Logger.Write("GameView.DrawGameField(): " + ex.StackTrace.ToString());
            }
        }

        private void DrawEntities(IMap map, int gvX, int gvY)
        {
            int bgc = Colors.Black;

            var creatures = ((Layer)map).Creatures;
            for (int i = 0; i < creatures.Count; i++) {
                var creature = creatures[i];
                int ax = creature.PosX;
                int ay = creature.PosY;

                BaseTile tile = map.GetTile(ax, ay);
                // faster than checking area first
                if (tile != null && tile.HasState(BaseTile.TS_SEEN)) {
                    char tileChar = creature.Appearance;
                    int fgc = creature.AppearanceColor;

                    int sx = gvX + ax;
                    int sy = gvY + ay;
                    fTerminal.Write(sx, sy, tileChar, fgc, bgc);
                }
            }
        }

        private void DrawTile(IMap map, int px, int py, int ax, int ay, int sx, int sy, float darkness)
        {
            char tileChar;
            int fgc;
            int bgc;

            if (fMouseClick.Equals(ax, ay) && ax != px && ay != py) {
                tileChar = '*';
                fgc = Colors.White;
                bgc = Colors.Black;
            } else {
                BaseTile tile = map.GetTile(ax, ay);
                if (tile != null && tile.HasState(BaseTile.TS_VISITED)) {
                    bool seen = tile.HasState(BaseTile.TS_SEEN);

                    Creature creature = null;
                    /*if (seen) {
                        creature = (Creature)map.FindCreature(ax, ay);
                    }*/

                    if (creature != null) {
                        tileChar = creature.Appearance;
                        fgc = creature.AppearanceColor;
                        bgc = Colors.Black;
                    } else {
                        byte pf_status = tile.PathStatus;
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
                        } else {
                            tileChar = ' ';
                            fgc = Colors.White;
                            bgc = Colors.Black;
                        }

                        if (!seen) {
                            fgc = GfxHelper.Darker(fgc, darkness);
                        }
                    }
                } else {
                    tileChar = ' ';
                    fgc = Colors.White;
                    bgc = Colors.Black;
                }
            }

            fTerminal.Write(sx, sy, tileChar, fgc, bgc);
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            Keys code = e.Key;

            PlayerController playerController = GameSpace.PlayerController;
            int newX = playerController.Player.PosX;
            int newY = playerController.Player.PosY;

            switch (code) {
                case Keys.GK_ESCAPE:
                    playerController.ClearPath();
                    break;

                case Keys.GK_TAB:
                    MainView.View = ViewType.vtMinimap;
                    break;

                case Keys.GK_LEFT:
                case Keys.GK_NUMPAD4:
                    newX--;
                    break;
                case Keys.GK_UP:
                case Keys.GK_NUMPAD8:
                    newY--;
                    break;
                case Keys.GK_RIGHT:
                case Keys.GK_NUMPAD6:
                    newX++;
                    break;
                case Keys.GK_DOWN:
                case Keys.GK_NUMPAD2:
                    newY++;
                    break;

                case Keys.GK_NUMPAD7:
                    newX--;
                    newY--;
                    break;
                case Keys.GK_NUMPAD9:
                    newX++;
                    newY--;
                    break;
                case Keys.GK_NUMPAD1:
                    newX--;
                    newY++;
                    break;
                case Keys.GK_NUMPAD3:
                    newX++;
                    newY++;
                    break;
            }

            playerController.MoveTo(newX, newY);
        }

        public override void KeyTyped(KeyPressEventArgs e)
        {
            PlayerController playerCtl = GameSpace.PlayerController;

            switch (char.ToLower(e.Key)) {
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

                case 'x':
                    var sl = new Streetlight(playerCtl.Player.Space, playerCtl.Player.Map, playerCtl.Player.PosX, playerCtl.Player.PosY);
                    playerCtl.Player.Map.Features.Add(sl);
                    sl.Render();
                    break;

                case 'q':
                    fTerminal.System.Quit();
                    break;

                case ' ':
                    break;

                case 'p':
                    {
                        Realm newRealm = GameSpace.InitNewRealm(null);
                        
                        ExtRect area = ExtRect.Create(playerCtl.Player.PosX - 5, playerCtl.Player.PosY - 5,
                                                      playerCtl.Player.PosX + 5, playerCtl.Player.PosY + 5);
                        IMap map = GameSpace.CurrentRealm.PlainMap;
                        ExtPoint pt = map.SearchFreeLocation(area);
                        Portal portal = new Portal(GameSpace, map, pt.X, pt.Y, GameSpace.CurrentRealm, newRealm, pt.X, pt.Y);
                        map.Features.Add(portal);
                        portal.Render();
                    }
                    break;

                default:
                    IList<IAction> actions = playerCtl.AvailableActions;
                    if (actions != null) {
                        int idx = 0;
                        foreach (var action in actions) {
                            char key = (char)('A' + idx++);
                            if (e.Key == key) {
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

                if (e.Button == MouseButton.mbLeft) {
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

        public override void Tick()
        {
            GameSpace.DoTurn();
        }
    }
}
