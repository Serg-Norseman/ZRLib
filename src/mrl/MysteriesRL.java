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
package mrl;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.awt.event.MouseMotionListener;
import javax.swing.JFrame;
import javax.swing.Timer;
import jzrlib.jterm.JTerminal;
import mrl.core.GlobalData;
import mrl.core.Locale;
import mrl.views.MainView;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public final class MysteriesRL extends JFrame
{
    private final JTerminal fTerminal;

    private MainView fMainView;
    private Timer fTimer;
    private Locale fLocale;

    public MysteriesRL()
    {
        super();

        this.setResizable(false);
        this.fTerminal = new JTerminal(160, 80);
        this.fTerminal.setFocusable(true);
        this.fTerminal.setFocusTraversalKeysEnabled(false);
        this.add(fTerminal);
        this.pack();
        this.setLocationRelativeTo(null);
        this.setTitle(GlobalData.MRL_NAME);

        this.fLocale = new Locale();
        
        this.initViews();
        this.initTerminal();
        this.initTimer();
    }

    private void initViews()
    {
        this.fMainView = new MainView(this, null, this.fTerminal);
    }

    private void initTimer()
    {
        this.fTimer = new Timer(100, new ActionListener()
        {
            @Override
            public void actionPerformed(ActionEvent e)
            {
                fMainView.tick();
                repaint();

                if (JTerminal.DEBUG_PAINT) {
                    setTitle(String.valueOf(fTerminal.CharsPainted));
                }
            }
        });

        this.fTimer.setRepeats(true);
        this.fTimer.start();
    }

    private void initTerminal()
    {
        this.fTerminal.addKeyListener(new KeyListener()
        {
            @Override
            public void keyPressed(KeyEvent e)
            {
                fMainView.keyPressed(e);
            }

            @Override
            public void keyReleased(KeyEvent e)
            {
                //System.out.println("test");
            }

            @Override
            public void keyTyped(KeyEvent e)
            {
                fMainView.keyTyped(e);
            }
        });

        this.fTerminal.addMouseMotionListener(new MouseMotionListener()
        {
            @Override
            public void mouseDragged(MouseEvent e)
            {
            }

            @Override
            public void mouseMoved(MouseEvent e)
            {
                fMainView.mouseMoved(e);
            }
        });

        this.fTerminal.addMouseListener(new MouseListener()
        {
            @Override
            public void mouseClicked(MouseEvent e)
            {
                fMainView.mouseClicked(e);
            }

            @Override
            public void mousePressed(MouseEvent e)
            {
            }

            @Override
            public void mouseReleased(MouseEvent e)
            {
            }

            @Override
            public void mouseEntered(MouseEvent e)
            {
            }

            @Override
            public void mouseExited(MouseEvent e)
            {
            }
        });
    }

    public static void main(String[] args)
    {
        MysteriesRL app = new MysteriesRL();
        app.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        app.setVisible(true);
    }
}
