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
import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import jzrlib.jterm.JTerminal;
import jzrlib.utils.Logger;
import mrl.core.GlobalData;
import mrl.core.IProgressController;
import mrl.core.Locale;
import mrl.core.RS;
import mrl.game.Game;
import mrl.views.controls.ChoicesArea;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class StartupView extends SubView implements IProgressController
{
    private Color SV_COL = new Color(0, 128, 128); //new Color(255, 0, 255);
    private String[] fTitle;
    private final ChoicesArea fChoicesArea;
    private boolean fMenuMode;
    
    public StartupView(BaseView ownerView, JTerminal terminal)
    {
        super(ownerView, terminal);
        this.parseTitle();
        
        this.fChoicesArea = new ChoicesArea(terminal, 40, Color.green.darker());
        this.fChoicesArea.addChoice('N', Locale.getStr(RS.rs_NewGame));
        this.fChoicesArea.addChoice('Q', Locale.getStr(RS.rs_Quit));
        
        this.fMenuMode = true;
    }

    private void parseTitle()
    {
        InputStream is = StartupView.class.getResourceAsStream("/resources/startup.txt");
        DataInputStream dis = new DataInputStream(is);
        BufferedReader br = new BufferedReader(new InputStreamReader(dis));

        this.fTitle = new String[36];
        
        try {
            int i = 0;
            String strLine;
            while ((strLine = br.readLine()) != null) {
                fTitle[i] = strLine;
                i++;
            }
        } catch (IOException ex) {
            Logger.write("StartupView.parseTitle(): " + ex.getMessage());
        }
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
        
        this.fTerminal.setTextForeground(SV_COL);
        UIPainter.drawBox(fTerminal, 0, 0, 159, 79, false);
        
        for (int i = 0; i < this.fTitle.length; i++) {
            String line = this.fTitle[i];
            if (line != null) {
                for (int x = 0; x < line.length(); x++) {
                    char chr = line.charAt(x);
                    if (chr != '.') {
                        chr = (char) 177;
                    } else {
                        chr = ' ';
                    }
                    this.fTerminal.write(1 + x, 1 + i, chr);
                }
            }
        }

        int top = this.fTitle.length;
        UIPainter.writeCenter(fTerminal, 1, 158, top + 1, GlobalData.MRL_VER);
        UIPainter.writeCenter(fTerminal, 1, 158, top + 3, GlobalData.MRL_COPYRIGHT);
        
        if (this.fMenuMode) {
            this.fChoicesArea.draw();
        }
        
        this.drawProgress();
    }

    @Override
    public void keyPressed(KeyEvent e)
    {
    }

    @Override
    public void keyTyped(KeyEvent e)
    {
        switch (e.getKeyChar()) {
            case 'q':
            case 'Q':
                System.exit(0);
                break;

            case 'n':
            case 'N':
                this.fMenuMode = false;
                this.getGameSpace().initNew(this);
                this.getMainView().setView(ViewType.vtPlayerChoice);
                break;

            default:
                break;
        }
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
    
    @Override
    public void tick()
    {
        this.updateView();
    }
    
    private void drawProgress()
    {
        if (this.fGenLabel != null) {
            this.fTerminal.setTextForeground(Color.white);
            this.fTerminal.write(20, 70, this.fGenLabel);
            UIPainter.drawProgress(this.fTerminal, 20, 159 - 20, 71, fGenCompleted, fGenStages);
        }
    }
    
    private int fGenStages = 50;
    private int fGenCompleted = 5;
    private String fGenLabel = null;
    
    @Override
    public void complete(int stage)
    {
        this.fGenCompleted++;

        this.updateView();
        this.getMainView().repaintImmediately();
    }

    @Override
    public void setStage(String label, int size)
    {
        this.fGenLabel = label;
        this.fGenStages = size;
        this.fGenCompleted = 0;

        this.updateView();
        this.getMainView().repaintImmediately();
    }
}
