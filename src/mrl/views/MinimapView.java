/*
 *  "MysteriesRL", Java Roguelike game.
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
package mrl.views;

import java.awt.Color;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.java.MathPoint;
import jzrlib.java.MathUtils;
import jzrlib.jterm.JTerminal;
import jzrlib.map.BaseTile;
import jzrlib.map.IMap;
import jzrlib.map.PathSearch;
import mrl.core.GlobalData;
import mrl.core.Locale;
import mrl.core.RS;
import mrl.creatures.Human;
import mrl.game.Game;
import mrl.maps.Minimap;
import mrl.maps.TileID;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class MinimapView extends SubView
{
    private static final Rect MapArea = new Rect(1, 1, 157, 75);
    private static final int XRad = 78;
    private static final int YRad = 37;

    private final Rect fMapRect = new Rect();

    private Point fCenter;
    private String fLocationName = "---";
    private int fTick;

    private boolean fShowPrivateHouse = false;
    private Point fPrivateHousePt = null;
    private Point fPrivateHousePtRest = null;
        
    public MinimapView(BaseView ownerView, JTerminal terminal)
    {
        super(ownerView, terminal);
    }

    @Override
    public final Game getGameSpace()
    {
        return ((MainView) this.fOwnerView).getGameSpace();
    }

    @Override
    protected void updateView()
    {
        this.fTerminal.clear();
        this.fTerminal.setTextBackground(Color.black);
        this.fTerminal.setTextForeground(Color.white);
        UIPainter.drawBox(fTerminal, 0, 0, 159, 79, false);

        Human player = this.getGameSpace().getPlayerController().getPlayer();
        IMap map = this.getGameSpace().fMinimap;
        
        int px = player.getPosX() / Minimap.SCALE;
        int py = player.getPosY() / Minimap.SCALE;

        int cx = this.fCenter.X;
        int cy = this.fCenter.Y;
        fMapRect.setBounds(cx - XRad, cy - YRad, cx + XRad, cy + YRad);

        if (fMapRect.contains(fPrivateHousePt)) {
            fPrivateHousePtRest = fPrivateHousePt;
        } else {
            MathPoint pf = MathUtils.intersectLine2Rect(new MathPoint(cx, cy), new MathPoint(fPrivateHousePt.X, fPrivateHousePt.Y), fMapRect);
            fPrivateHousePtRest = new Point((int) pf.x, (int) pf.y);
        }

        for (int ay = fMapRect.Top; ay <= fMapRect.Bottom; ay++) {
            for (int ax = fMapRect.Left; ax <= fMapRect.Right; ax++) {
                int sx = MapArea.Left + (ax - fMapRect.Left);
                int sy = MapArea.Top + (ay - fMapRect.Top);

                this.drawTile(map, px, py, ax, ay, sx, sy);
            }
        }

        UIPainter.writeCenter(this.fTerminal, 1, 158, 78, Locale.format(RS.rs_Location, this.fLocationName));
    }

    private void drawTile(IMap map, int px, int py, int ax, int ay, int sx, int sy)
    {
        char tileChar = ' ';
        Color fg = Color.white;

        if (ax == px && ay == py) {
            tileChar = '@';
            fg = Color.yellow;
        } else if (this.fShowPrivateHouse && this.fPrivateHousePtRest.equals(ax, ay)) {
            int ex = this.fTick % 2;
            if (ex == 0) {
                tileChar = 'H';
                fg = Color.yellow;
            }
        } else {
            BaseTile tile = map.getTile(ax, ay);
            if (tile != null) {
                byte pf_status = tile.pf_status;
                int id;
                
                switch (pf_status) {
                    case PathSearch.tps_Start:
                        id = TileID.tid_PathStart.Value;
                        break;
                    case PathSearch.tps_Finish:
                        id = TileID.tid_PathFinish.Value;
                        break;
                    case PathSearch.tps_Path:
                        id = TileID.tid_Path.Value;
                        break;
                    default:
                        id = (tile.Foreground != 0) ? tile.Foreground : tile.Background;
                        break;
                }
                
                TileID tileId = TileID.forValue(id);
                if (tileId != null) {
                    tileChar = tileId.Sym;
                    fg = tileId.SymColor;
                }
                
                /*if (!tile.State.contains(TileStates.tsSeen)) {
                    fg = ColorUtil.darker(fg, 0.25f);
                }*/
            }
        }
        
        this.fTerminal.write(sx, sy, tileChar, fg);
    }
    
    @Override
    public void tick()
    {
        if (this.fShowPrivateHouse) {
            this.fTick++;
            this.updateView();
        }
    }

    @Override
    public void keyPressed(KeyEvent e)
    {
        int code = e.getKeyCode();
        switch (code) {
            case GlobalData.KEY_LEFT:
                this.fCenter.X--;
                break;
            case GlobalData.KEY_UP:
                this.fCenter.Y--;
                break;
            case GlobalData.KEY_RIGHT:
                this.fCenter.X++;
                break;
            case GlobalData.KEY_DOWN:
                this.fCenter.Y++;
                break;
                
            case GlobalData.KEY_TAB:
            case GlobalData.KEY_ESC:
                this.getMainView().setView(ViewType.vtGame);
                break;

            default:
                break;
        }
    }

    @Override
    public void keyTyped(KeyEvent e)
    {
        switch (e.getKeyChar()) {
            case 'p':
                this.fShowPrivateHouse = !this.fShowPrivateHouse;
                break;

            default:
                break;
        }
    }

    @Override
    public void mouseClicked(MouseEvent e)
    {
        Point termPt = this.getTerminalPoint(e.getX(), e.getY());
        if (MapArea.contains(termPt)) {
            Point lpt = this.getLocalePoint(termPt);
            this.fLocationName = this.getGameSpace().getLocationName(lpt.X * Minimap.SCALE, lpt.Y * Minimap.SCALE);
        }
    }

    @Override
    public void mouseMoved(MouseEvent e)
    {
    }

    public Point getLocalePoint(Point terminalPoint)
    {
        int mpx = (terminalPoint.X - MapArea.Left) + fMapRect.Left;
        int mpy = (terminalPoint.Y - MapArea.Top) + fMapRect.Top;
        return new Point(mpx, mpy);
    }
    
    @Override
    public final void show()
    {
        Human player = this.getGameSpace().getPlayerController().getPlayer();
        
        Point ppt = player.getLocation();
        Rect privRt = player.getApartment().getArea().clone();
        Point phPt = privRt.getCenter();
        this.fPrivateHousePt = new Point(phPt.X / Minimap.SCALE, phPt.Y / Minimap.SCALE);

        this.fCenter = new Point(ppt.X / Minimap.SCALE, ppt.Y / Minimap.SCALE);
        
        this.fTick = 0;
    }
}
