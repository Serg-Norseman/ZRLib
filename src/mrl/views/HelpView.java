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
import jzrlib.jterm.JTerminal;
import mrl.core.GlobalData;
import mrl.game.Game;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class HelpView extends SubView
{
    public HelpView(BaseView ownerView, JTerminal terminal)
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
        
        this.fTerminal.setTextForeground(Color.lightGray);
        this.fTerminal.write(2, 2, "Keys: ");
    }
    
    @Override
    public void tick()
    {
    }

    @Override
    public void keyPressed(KeyEvent e)
    {
        switch (e.getKeyCode()) {
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
    }

    @Override
    public void mouseClicked(MouseEvent e)
    {
    }

    @Override
    public void mouseMoved(MouseEvent e)
    {
    }
    
    @Override
    public final void show()
    {
    }
}
