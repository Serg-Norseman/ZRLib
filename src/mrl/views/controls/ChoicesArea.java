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
package mrl.views.controls;

import java.awt.Color;
import java.util.ArrayList;
import java.util.List;
import jzrlib.core.Rect;
import jzrlib.jterm.JTerminal;
import mrl.views.UIPainter;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class ChoicesArea
{
    private static class Choice
    {
        public char Key;
        public String Title;
        public String ItemText;
        
        public Choice(char key, String title)
        {
            this.Key = key;
            this.Title = title;
            this.ItemText = "[" + key + "] " + title;
        }
    }

    protected final JTerminal fTerminal;
    protected final List<Choice> fChoices;
    protected final int fTop;
    protected Rect fArea;
    protected Color fColor;
    
    public ChoicesArea(JTerminal terminal, int top, Color color)
    {
        this.fTerminal = terminal;
        this.fChoices = new ArrayList<>();
        this.fTop = top;
        this.fArea = new Rect();
        this.fColor = color;
    }
    
    public void addChoice(char key, String title)
    {
        this.fChoices.add(new Choice(key, title));
        this.adjust();
    }
    
    private void adjust()
    {
        int mxw = 0;
        for (Choice choice : this.fChoices) {
            String item = choice.ItemText;
            if (mxw < item.length()) {
                mxw = item.length();
            }
        }
        
        int chW = mxw + 8;
        int chH = this.fChoices.size() + (this.fChoices.size() - 1) + 8;
        int chX = (this.fTerminal.getTermWidth() - chW) / 2;
        int chY = this.fTop + (this.fTerminal.getTermHeight() - this.fTop - chH) / 2;
        
        this.fArea.setBounds(chX, chY, chX + chW - 1, chY + chH - 1);
    }
    
    public void draw()
    {
        this.fTerminal.setTextForeground(this.fColor);
        UIPainter.drawBox(fTerminal, this.fArea.Left, this.fArea.Top, this.fArea.Right, this.fArea.Bottom, false);

        int yy = this.fArea.Top + 4;
        int xx = this.fArea.Left + 4;

        for (Choice choice : this.fChoices) {
            String item = choice.ItemText;
            this.fTerminal.write(xx, yy, item);
            yy += 2;
        }        
    }
}
