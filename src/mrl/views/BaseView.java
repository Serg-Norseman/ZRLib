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
import jzrlib.jterm.JTerminal;
import jzrlib.utils.AuxUtils;
import mrl.game.Game;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public abstract class BaseView
{
    protected final BaseView fOwnerView;
    protected final JTerminal fTerminal;

    public BaseView(BaseView ownerView, JTerminal terminal)
    {
        this.fOwnerView = ownerView;
        this.fTerminal = terminal;
    }
    
    public final Point getTerminalPoint(int mx, int my)
    {
        int tx = mx / this.fTerminal.getCharWidth();
        int ty = my / this.fTerminal.getCharHeight();
        return new Point(tx, ty);
    }
    
    // <editor-fold defaultstate="collapsed" desc="Drawing routines">

    public final void drawText(int x, int y, String string)
    {
        this.fTerminal.write(x, y, string);
    }
    
    public final void drawText(int x, int y, String string, Color foreground)
    {
        this.fTerminal.write(x, y, string, foreground);
    }
    
    public final void drawStat(int x1, int x2, int y, String name, String value)
    {
        name = name + "[";
        this.fTerminal.write(x1, y, name);
        this.fTerminal.write(x2, y, "]");
        
        int xx = x1 + name.length();
        int xval = ((x2 - xx) - value.length()) / 2;
        this.fTerminal.write(xx + xval, y, value);
    }
    
    public final void drawStat(int x1, int x2, int y, String name, float value, float max, float redLevel)
    {
        name = name + "[";
        this.fTerminal.write(x1, y, name);

        float factor = (value / max);
        int percent = (int) (factor * 100.0f);
        String val = AuxUtils.adjustNum(percent, 3) + "%]";
        this.fTerminal.write(x2 - val.length() + 1, y, val);
        
        Color curfg;
        if (factor < redLevel) {
            curfg = Color.red;
        } else {
            curfg = this.fTerminal.getTextForeground();
        }
        
        int xx1 = x1 + name.length() + 1;
        int xx2 = x2 - val.length() - 1;
        int len = (int) ((xx2 - xx1) * factor);
        if (len > 0) {
            for (int i = xx1; i <= xx1 + len; i++) {
                this.fTerminal.write(i, y, (char) 177, curfg);
            }
        }
    }
    
    // </editor-fold>

    protected abstract void updateView();

    public abstract Game getGameSpace();
    
    public abstract void keyPressed(KeyEvent e);
    public abstract void keyTyped(KeyEvent e);

    public abstract void mouseClicked(MouseEvent e);
    public abstract void mouseMoved(MouseEvent e);
    
    public abstract void tick();
    public abstract void show();
}
