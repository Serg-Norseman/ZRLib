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

import jzrlib.jterm.JTerminal;

/**
 *
 * @author Serg V. Zhdanovskih
 */
public class UIPainter
{
    private static final char[] SINGLE_BOX = new char[] {(char) 218, (char) 191, (char) 192, (char) 217, (char) 196, (char) 179};
    private static final char[] DOUBLE_BOX = new char[] {(char) 201, (char) 187, (char) 200, (char) 188, (char) 205, (char) 186};
    
    public static void drawBox(JTerminal term, int x1, int y1, int x2, int y2, boolean single, String title)
    {
        drawBox(term, x1, y1, x2, y2, single);
        title = " " + title + " ";
        writeCenter(term, x1, x2, y1, title);
    }
    
    public static void drawBox(JTerminal term, int x1, int y1, int x2, int y2, boolean single)
    {
        char[] charSet;
        if (single) {
            charSet = SINGLE_BOX;
        } else {
            charSet = DOUBLE_BOX;
        }
        
        for (int xx = x1 + 1; xx <= x2 - 1; xx++) {
            term.write(xx, y1, charSet[4]);
            term.write(xx, y2, charSet[4]);
        }

        for (int yy = y1 + 1; yy <= y2 - 1; yy++) {
            term.write(x1, yy, charSet[5]);
            term.write(x2, yy, charSet[5]);
        }

        term.write(x1, y1, charSet[0]);
        term.write(x2, y1, charSet[1]);
        term.write(x1, y2, charSet[2]);
        term.write(x2, y2, charSet[3]);
    }

    public static void writeCenter(JTerminal term, int x1, int x2, int y, String text)
    {
        int xx = x1 + (x2 - x1 - text.length()) / 2;
        term.write(xx, y, text);
    }
    
    public static void drawProgress(JTerminal term, int x1, int x2, int y, float complete, float size)
    {
        int len = (x2 - x1) + 1;
        float percent = Math.min(1.0f, (complete / size));
        int bound = x1 + (int) (len * percent) - 1;
        for (int x = x1; x <= x2; x++) {
            char chr = (x <= bound) ? (char) 219 : (char) 176;
            term.write(x, y, chr);
        }
    }
}
