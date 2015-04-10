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
import java.util.List;
import jzrlib.core.Point;
import jzrlib.core.Rect;
import jzrlib.core.action.IAction;
import jzrlib.external.ColorUtil;
import jzrlib.jterm.JTerminal;
import jzrlib.map.BaseTile;
import jzrlib.map.IMap;
import jzrlib.map.PathSearch;
import jzrlib.map.TileStates;
import mrl.core.GlobalData;
import mrl.creatures.Creature;
import mrl.creatures.Human;
import mrl.creatures.NPCStats;
import mrl.creatures.body.HumanBody;
import mrl.game.Game;
import mrl.game.Message;
import mrl.game.PlayerController;
import mrl.maps.TileID;
import mrl.maps.city.City;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class GameView extends SubView
{
    private Rect fMapRect = new Rect();
    private Point fMouseClick = new Point();
    private int fTick;

    public GameView(BaseView ownerView, JTerminal terminal)
    {
        super(ownerView, terminal);
    }

    @Override
    public final Game getGameSpace()
    {
        return ((MainView) this.fOwnerView).getGameSpace();
    }

    public Point getLocalePoint(Point terminalPoint)
    {
        int mpx = (terminalPoint.X - GlobalData.GV_BOUNDS.Left) + fMapRect.Left;
        int mpy = (terminalPoint.Y - GlobalData.GV_BOUNDS.Top) + fMapRect.Top;
        return new Point(mpx, mpy);
    }
    
    @Override
    protected void updateView()
    {
        this.fTerminal.clear();
        this.fTerminal.setTextBackground(Color.black);
        this.fTerminal.setTextForeground(Color.white);
        UIPainter.drawBox(fTerminal, 0, 0, 102, 70, false);
        UIPainter.drawBox(fTerminal, 0, 71, 159, 79, false);

        Game gameSpace = this.getGameSpace();
        City city = gameSpace.getCity();
        
        this.drawGameField(gameSpace);
        this.drawMessages(gameSpace);

        this.fTerminal.setTextForeground(Color.white);
        
        this.drawText(105, 1, "Time: " + gameSpace.getTime().toString(true, true), Color.white);
        this.drawText(105, 3, "City name: " + city.getName(), Color.white);

        Human player = gameSpace.getPlayerController().getPlayer();
        NPCStats stats = player.getStats();
        HumanBody body = (HumanBody) player.getBody();

        this.drawStat(105, 159,  5, "Player name ", player.getName());
        this.drawStat(105, 159,  7, "XP          ", stats.getCurXP(), stats.getNextLevelXP(), 0);
        this.drawStat(105, 159,  9, "HP          ", player.getHP(), player.getHPMax(), 0.25f);
        this.drawStat(105, 159, 11, "Stamina     ", body.getAttribute("stamina"), 100, 0.25f);
        this.drawStat(105, 159, 13, "Hunger      ", body.getAttribute("hunger"), 100, 0.25f);

        //this.drawStat(105, 159, 13, "Carry Weight: " + String.valueOf(stats.getCarryWeight()), Color.white);

        this.drawAvailableActions(gameSpace.getPlayerController());

        //this.fTerminal.write(105, 73, "Dark: " + String.valueOf(darkness), Color.white);
    }

    private void drawAvailableActions(PlayerController playerCtl)
    {
        List<IAction> actions = playerCtl.AvailableActions;
        if (actions != null) {
            int top = 50;
            int idx = 0;
            for (IAction action : actions) {
                char key = (char) ('A' + idx++);
                this.drawText(105, top, "[" + key + "] " + action.getName(), Color.white);
                top += 2;
            }
        }
    }
    
    private void drawMessages(Game gameSpace)
    {
        List<Message> messages = gameSpace.getMessages();
        if (messages.size() < 1) {
            return;
        }
        
        int top = 72;
        int hb = messages.size() - 1;
        int lb = Math.max(0, hb - 6);
        
        for (int i = lb; i <= hb; i++) {
            Message msg = messages.get(i);
            int yy = top + (i - lb);
            this.fTerminal.write(1, yy, msg.text, msg.color);
        }
    }
    
    private void drawGameField(Game gameSpace)
    {
        float darkness = 1 - gameSpace.getLightBrightness();
        PlayerController playerController = gameSpace.getPlayerController();
        Human player = playerController.getPlayer();
        IMap map = player.getMap();
        
        int px = player.getPosX();
        int py = player.getPosY();
        fMapRect = playerController.getViewport();

        for (int ay = fMapRect.Top; ay <= fMapRect.Bottom; ay++) {
            for (int ax = fMapRect.Left; ax <= fMapRect.Right; ax++) {
                int sx = GlobalData.GV_BOUNDS.Left + (ax - fMapRect.Left);
                int sy = GlobalData.GV_BOUNDS.Top + (ay - fMapRect.Top);

                this.drawTile(map, px, py, ax, ay, sx, sy, darkness);
            }
        }
    }
    
    private void drawTile(IMap map, int px, int py, int ax, int ay, int sx, int sy, float darkness)
    {
        char tileChar = ' ';
        Color fgc = Color.white;
        Color bgc = Color.black;

        if (this.fMouseClick != null && this.fMouseClick.equals(ax, ay) && ax != px && ay != py) {
            tileChar = '*';
            fgc = Color.white;
        } else {
            BaseTile tile = map.getTile(ax, ay);
            if (tile != null && tile.hasState(TileStates.TS_VISITED)) {
                boolean seen = tile.hasState(TileStates.TS_SEEN);
                
                Creature creature = null;
                if (seen) {
                    creature = (Creature) map.findCreature(ax, ay);
                }
                
                if (creature != null) {
                    tileChar = creature.getAppearance();
                    fgc = creature.getAppearanceColor();
                } else {
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
                        fgc = tileId.SymColor;
                        bgc = tileId.BackColor;
                    }

                    if (!seen) {
                        fgc = ColorUtil.darker(fgc, darkness);
                    }
                }
            }
        }

        this.fTerminal.write(sx, sy, tileChar, fgc, bgc);
    }
    
    @Override
    public void keyPressed(KeyEvent e)
    {
        int code = e.getKeyCode();
        //System.out.println(String.valueOf(code));

        PlayerController playerController = this.getGameSpace().getPlayerController();
        int newX = playerController.getPlayer().getPosX();
        int newY = playerController.getPlayer().getPosY();

        switch (code) {
            case GlobalData.KEY_ESC:
                playerController.clearPath();
                break;

            case GlobalData.KEY_TAB:
                this.getMainView().setView(ViewType.vtMinimap);
                break;

            case GlobalData.KEY_LEFT:
                newX--;
                break;
            case GlobalData.KEY_UP:
                newY--;
                break;
            case GlobalData.KEY_RIGHT:
                newX++;
                break;
            case GlobalData.KEY_DOWN:
                newY++;
                break;

            default:
                break;
        }

        playerController.moveTo(newX, newY);
    }

    @Override
    public void keyTyped(KeyEvent e)
    {
        PlayerController playerCtl = this.getGameSpace().getPlayerController();
        
        switch (e.getKeyChar()) {
            case 'a':
                playerCtl.attack();
                break;

            case 'c':
                this.getMainView().setView(ViewType.vtSelf);
                break;

            case 'f':
                playerCtl.CircularFOV = !playerCtl.CircularFOV;
                break;

            case 'h':
                this.getMainView().setView(ViewType.vtHelp);
                break;

            case 'k':
                playerCtl.switchLock();
                break;

            case 'l':
                this.getGameSpace().look(-1, -1);
                break;

            case 't':
                if (this.fMouseClick != null) {
                    playerCtl.moveTo(this.fMouseClick.X, this.fMouseClick.Y);
                }
                break;

            case 'w':
                this.doWalk();
                break;

            case ' ':
                break;

            default:
                List<IAction> actions = playerCtl.AvailableActions;
                if (actions != null) {
                    int idx = 0;
                    for (IAction action : actions) {
                        char key = (char) ('A' + idx++);
                        if (e.getKeyChar() == key) {
                            action.execute(playerCtl.getPlayer());
                        }
                    }
                }
                break;
        }
    }

    public void doWalk()
    {
        this.getGameSpace().getPlayerController().walk(this.fMouseClick.clone());
    }

    @Override
    public void mouseClicked(MouseEvent e)
    {
        Point termPt = this.getTerminalPoint(e.getX(), e.getY());
        if (GlobalData.GV_BOUNDS.contains(termPt)) {
            Point lpt = this.getLocalePoint(termPt);
            
            if (e.getButton() == MouseEvent.BUTTON1) {
                if (this.getGameSpace().checkTarget(lpt.X, lpt.Y)) {
                    this.fMouseClick = lpt;
                } else {
                    this.fMouseClick = null;
                }
            } else {
                this.getGameSpace().look(lpt.X, lpt.Y);
            }
        }
    }

    @Override
    public void mouseMoved(MouseEvent e)
    {
    }

    @Override
    public void tick()
    {
        this.fTick++;
        int ex = this.fTick % 20;
        if (ex == 0) {
            this.getGameSpace().updateWater();
        }
        
        this.getGameSpace().doTurn();
        this.updateView();
    }
    
    @Override
    public final void show()
    {
        this.fTick = 0;
    }
}
