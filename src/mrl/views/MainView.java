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

import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;
import javax.swing.JFrame;
import jzrlib.jterm.JTerminal;
import mrl.game.Game;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class MainView extends BaseView
{
    private final JFrame fFrame;
    private final Game fGameSpace;
    private BaseView fSubView;
    private ViewType fCurView;
    
    private StartupView fStartupView;
    private GameView fGameView;
    private MinimapView fMinimapView;
    private HelpView fHelpView;
    private PlayerChoiceView fPlayerChoiceView;
    private SelfView fSelfView;
    
    public MainView(JFrame frame, BaseView ownerView, JTerminal terminal)
    {
        super(ownerView, terminal);
        
        this.fFrame = frame;
        this.fGameSpace = new Game(this);
        
        this.initViews();
        
        this.setView(ViewType.vtStartup);
    }

    @Override
    public final Game getGameSpace()
    {
        return this.fGameSpace;
    }

    private void initViews()
    {
        this.fStartupView = new StartupView(this, this.fTerminal);
        this.fGameView = new GameView(this, this.fTerminal);
        this.fMinimapView = new MinimapView(this, this.fTerminal);
        this.fHelpView = new HelpView(this, this.fTerminal);
        this.fPlayerChoiceView = new PlayerChoiceView(this, this.fTerminal);
        this.fSelfView = new SelfView(this, this.fTerminal);
    }

    public final void setView(ViewType type)
    {
        //ViewType prevView = this.fCurView;
        
        this.fCurView = type;

        switch (type) {
            case vtStartup:
                this.fSubView = this.fStartupView;
                break;
            
            case vtGame:
                this.fSubView = this.fGameView;
                break;

            case vtMinimap:
                this.fSubView = this.fMinimapView;
                break;
                
            case vtHelp:
                this.fSubView = this.fHelpView;
                break;
                
            case vtPlayerChoice:
                this.fSubView = this.fPlayerChoiceView;
                break;
                
            case vtSelf:
                this.fSubView = this.fSelfView;
                break;
        }
        
        this.fSubView.show();
        this.updateView();
    }

    @Override
    protected void updateView()
    {
        this.fSubView.updateView();
    }
    
    @Override
    public void tick()
    {
        this.fSubView.tick();
    }

    @Override
    public void keyPressed(KeyEvent e)
    {
        this.fSubView.keyPressed(e);
        this.updateView();
    }

    @Override
    public void keyTyped(KeyEvent e)
    {
        this.fSubView.keyTyped(e);
        this.updateView();
    }

    @Override
    public void mouseClicked(MouseEvent e)
    {
        this.fSubView.mouseClicked(e);
        this.updateView();
    }

    @Override
    public void mouseMoved(MouseEvent e)
    {
        this.fSubView.mouseMoved(e);
    }
    
    @Override
    public final void show()
    {
        // dummy
    }
    
    public final void setTitle(String title)
    {
        this.fFrame.setTitle(title);
    }
    
    public final void repaintImmediately()
    {
        this.fTerminal.paintImmediately(0, 0, this.fTerminal.getWidth(), this.fTerminal.getHeight());
    }
}
