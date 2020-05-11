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
using PrimevalRL.Creatures;
using PrimevalRL.Game;
using PrimevalRL.Maps;
using ZRLib.Core;
using ZRLib.Engine;
using ZRLib.Map;

namespace PrimevalRL.Views
{
    public sealed class MinimapView : SubView
    {
        private static readonly ExtRect MapArea = ExtRect.Create(1, 1, 157, 75);
        private const int XRad = 78;
        private const int YRad = 37;

        private ExtPoint fCenter;
        private string fLocationName = "---";
        private ExtRect fMapRect;
        private int fTick;

        private bool fShowPrivateHouse = false;
        private ExtPoint fPrivateHousePt = ExtPoint.Empty;
        private ExtPoint fPrivateHousePtRest = ExtPoint.Empty;

        public MinimapView(BaseView ownerView, Terminal terminal)
            : base(ownerView, terminal)
        {
        }

        internal override void UpdateView()
        {
            fTerminal.Clear();
            fTerminal.TextBackground = Colors.Black;
            fTerminal.TextForeground = Colors.White;
            fTerminal.DrawBox(0, 0, 159, 79, false);

            Human player = GameSpace.PlayerController.Player;
            IMap map = GameSpace.CurrentRealm.fMinimap;

            int px = player.PosX / Minimap.SCALE;
            int py = player.PosY / Minimap.SCALE;

            int cx = fCenter.X;
            int cy = fCenter.Y;
            fMapRect.SetBounds(cx - XRad, cy - YRad, cx + XRad, cy + YRad);

            if (fMapRect.Contains(fPrivateHousePt)) {
                fPrivateHousePtRest = fPrivateHousePt;
            } else {
                ExtPointF pf = AuxUtils.IntersectLine2Rect(new ExtPointF(cx, cy), new ExtPointF(fPrivateHousePt.X, fPrivateHousePt.Y), fMapRect);
                fPrivateHousePtRest = new ExtPoint((int)pf.X, (int)pf.Y);
            }

            for (int ay = fMapRect.Top; ay <= fMapRect.Bottom; ay++) {
                for (int ax = fMapRect.Left; ax <= fMapRect.Right; ax++) {
                    int sx = MapArea.Left + (ax - fMapRect.Left);
                    int sy = MapArea.Top + (ay - fMapRect.Top);

                    DrawTile(map, px, py, ax, ay, sx, sy);
                }
            }

            fTerminal.WriteCenter(1, 158, 78, Locale.Format(RS.Rs_Location, fLocationName));
        }

        private void DrawTile(IMap map, int px, int py, int ax, int ay, int sx, int sy)
        {
            char tileChar = ' ';
            int fg = Colors.White;

            if (ax == px && ay == py) {
                tileChar = '@';
                fg = Colors.Yellow;
            } else if (fShowPrivateHouse && fPrivateHousePtRest.Equals(ax, ay)) {
                int ex = fTick % 2;
                if (ex == 0) {
                    tileChar = 'H';
                    fg = Colors.Yellow;
                }
            } else {
                BaseTile tile = map.GetTile(ax, ay);
                if (tile != null) {
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
                        var tileRec = MRLData.Tiles[id];
                        tileChar = tileRec.Sym;
                        fg = tileRec.SymColor;
                    }

                    /*if (!tile.State.contains(TileStates.tsSeen)) {
                        fg = ColorUtil.darker(fg, 0.25f);
                    }*/
                }
            }

            fTerminal.Write(sx, sy, tileChar, fg);
        }

        public override void Tick()
        {
            if (fShowPrivateHouse) {
                fTick++;
            }
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            Keys code = e.Key;
            switch (code) {
                case Keys.GK_LEFT:
                    fCenter.X--;
                    break;
                case Keys.GK_UP:
                    fCenter.Y--;
                    break;
                case Keys.GK_RIGHT:
                    fCenter.X++;
                    break;
                case Keys.GK_DOWN:
                    fCenter.Y++;
                    break;

                case Keys.GK_TAB:
                case Keys.GK_ESCAPE:
                    MainView.View = ViewType.vtGame;
                    break;
            }
        }

        public override void KeyTyped(KeyPressEventArgs e)
        {
            switch (e.Key) {
                case 'p':
                    fShowPrivateHouse = !fShowPrivateHouse;
                    break;
            }
        }

        public override void MouseClicked(MouseEventArgs e)
        {
            ExtPoint termPt = fTerminal.GetTerminalPoint(e.X, e.Y);
            if (MapArea.Contains(termPt)) {
                ExtPoint lpt = GetLocalePoint(termPt);
                fLocationName = GameSpace.GetLocationName(lpt.X * Minimap.SCALE, lpt.Y * Minimap.SCALE);
            }
        }

        public ExtPoint GetLocalePoint(ExtPoint terminalPoint)
        {
            int mpx = (terminalPoint.X - MapArea.Left) + fMapRect.Left;
            int mpy = (terminalPoint.Y - MapArea.Top) + fMapRect.Top;
            return new ExtPoint(mpx, mpy);
        }

        public override void Show()
        {
            Human player = GameSpace.PlayerController.Player;

            ExtPoint ppt = player.Location;
            fCenter = new ExtPoint(ppt.X / Minimap.SCALE, ppt.Y / Minimap.SCALE);
            fTick = 0;

            if (player.Apartment != null) {
                ExtRect privRt = player.Apartment.Area;
                ExtPoint phPt = privRt.GetCenter();
                fPrivateHousePt = new ExtPoint(phPt.X / Minimap.SCALE, phPt.Y / Minimap.SCALE);
            }
        }
    }
}
